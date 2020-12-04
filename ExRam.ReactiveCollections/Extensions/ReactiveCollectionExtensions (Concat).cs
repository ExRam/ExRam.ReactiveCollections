// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region ConcatListReactiveCollection
        private sealed class ConcatListReactiveCollection<T> : IReactiveCollection<ListChangedNotification<T>>
        {
            #region Node
            private abstract class Node
            {
                protected Node(ImmutableList<T>? list, int maxIndex)
                {
                    List = list;
                    MaxIndex = maxIndex;
                }

                public abstract Node ReplaceNode(ImmutableList<T> newList, int index, out int? replacementOffset, out ImmutableList<T>? oldList);

                public int MaxIndex { get; }
                public ImmutableList<T>? List { get; }
            }

            private sealed class InnerNode : Node
            {
                private readonly Node _left;
                private readonly Node _right;

                public InnerNode(Node left, Node right) : base(
                    left.List != null && right.List != null
                        ? left.List.AddRange(right.List)
                        : null,
                    Math.Max(left.MaxIndex, right.MaxIndex))
                {
                    _left = left;
                    _right = right;
                }

                public override Node ReplaceNode(ImmutableList<T> newList, int index, out int? replacementOffset, out ImmutableList<T>? oldList)
                {
                    if (MaxIndex < index)
                        throw new InvalidOperationException();

                    if (_left.MaxIndex >= index)
                        return new InnerNode(_left.ReplaceNode(newList, index, out replacementOffset, out oldList), _right);

                    var newNode = new InnerNode(_left, _right.ReplaceNode(newList, index, out replacementOffset, out oldList));
                    replacementOffset += _left.List?.Count;

                    return newNode;
                }
            }

            private sealed class TerminalNode : Node
            {
                public TerminalNode(ImmutableList<T>? list, int maxIndex) : base(list, maxIndex)
                {
                }

                public override Node ReplaceNode(ImmutableList<T> newList, int index, out int? replacementOffset, out ImmutableList<T>? oldList)
                {
                    oldList = List;
                    replacementOffset = 0;

                    return new TerminalNode(newList, index);
                }
            }
            #endregion

            #region IndexedNotification
            private readonly struct IndexedNotification
            {
                public int Index { get; }
                public ListChangedNotification<T> Notification { get; }

                public IndexedNotification(int index, ListChangedNotification<T> notification)
                {
                    Index = index;
                    Notification = notification;
                }
            }
            #endregion

            public ConcatListReactiveCollection(
                IObservable<ListChangedNotification<T>>[] sources,
                IEqualityComparer<T> equalityComparer)
            {
                Changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var rootNode = GetTree(0, sources.Length - 1);

                        return sources
                            .Select((observable, i) => observable
                                .Select(notification => new IndexedNotification(i, notification)))
                            .Merge()
                            .Select(tuple =>
                            {
                                lock(syncRoot)
                                {
                                    rootNode = rootNode.ReplaceNode(tuple.Notification.Current, tuple.Index, out var offset, out var oldList);

                                    if (rootNode.List != null)
                                    {
                                        switch (tuple.Notification.Action)
                                        {
                                            case NotifyCollectionChangedAction.Add:
                                                return new ListChangedNotification<T>(rootNode.List, tuple.Notification.Action, ImmutableList<T>.Empty, tuple.Notification.NewItems, tuple.Notification.Index + offset);
                                            case NotifyCollectionChangedAction.Move:
                                            case NotifyCollectionChangedAction.Replace:
                                                return new ListChangedNotification<T>(rootNode.List, tuple.Notification.Action, tuple.Notification.OldItems, tuple.Notification.NewItems, tuple.Notification.Index + offset);
                                            case NotifyCollectionChangedAction.Remove:
                                                return new ListChangedNotification<T>(rootNode.List, tuple.Notification.Action, tuple.Notification.OldItems, ImmutableList<T>.Empty, tuple.Notification.Index + offset);
                                            case NotifyCollectionChangedAction.Reset:
                                            {
                                                if (oldList == null)
                                                    return new ListChangedNotification<T>(rootNode.List, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);

                                                if (oldList.Count > 0)
                                                {
                                                    return tuple.Notification.Current.Count > 0
                                                        ? new ListChangedNotification<T>(rootNode.List, NotifyCollectionChangedAction.Replace, oldList, tuple.Notification.Current, offset)
                                                        : new ListChangedNotification<T>(rootNode.List, NotifyCollectionChangedAction.Remove, oldList, ImmutableList<T>.Empty, offset);
                                                }

                                                return tuple.Notification.Current.Count > 0
                                                    ? new ListChangedNotification<T>(rootNode.List, NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, tuple.Notification.Current, offset)
                                                    : null;
                                            }
                                            default:
                                                throw new InvalidOperationException();
                                        }
                                    }

                                    return null;
                                }
                            })
                            .Where(x => x != null)
                            .Select(x => x!);
                    })
                    .ReplayFresh(1)
                    .RefCount()
                    .Normalize();
            }

            public IObservable<ListChangedNotification<T>> Changes { get; }

            private Node GetTree(int min, int max)
            {
                if (min == max)
                    return new TerminalNode(null, min);

                var half = (max - min + 1) / 2;
               
                return new InnerNode(GetTree(min, min + half - 1), GetTree(min + half, max));
            } 
        }
        #endregion

        public static IReactiveCollection<ListChangedNotification<T>> Concat<T>(this IEnumerable<IReactiveCollection<ListChangedNotification<T>>> collections)
        {
            return collections.Concat(EqualityComparer<T>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<T>> Concat<T>(this IEnumerable<IReactiveCollection<ListChangedNotification<T>>> collections, IEqualityComparer<T> equalityComparer)
        {
            var sourcesArray = collections
                .Select(x => x.Changes)
                .ToArray();

            if (sourcesArray.Length == 0)
            {
                return Observable
                    .Return(new ListChangedNotification<T>(ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null))
                    .ToReactiveCollection();
            }

            return sourcesArray.Length == 1
                ? sourcesArray[0].ToReactiveCollection() 
                : new ConcatListReactiveCollection<T>(sourcesArray, equalityComparer);
        }

        public static IReactiveCollection<ListChangedNotification<T>> Concat<T>(this IReactiveCollection<ListChangedNotification<T>> source1, IReactiveCollection<ListChangedNotification<T>> source2)
        {
            return source1.Concat(source2, EqualityComparer<T>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<T>> Concat<T>(this IReactiveCollection<ListChangedNotification<T>> source1, IReactiveCollection<ListChangedNotification<T>> source2, IEqualityComparer<T> equalityComparer)
        {
            return new ConcatListReactiveCollection<T>(new[] { source1.Changes, source2.Changes } , equalityComparer);
        }
    }
}
