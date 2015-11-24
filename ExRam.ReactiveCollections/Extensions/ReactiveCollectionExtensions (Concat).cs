// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region Node
        [ContractClass(typeof(NodeContracts<>))]
        private abstract class Node<T>
        {
            protected Node(ImmutableList<T> list, int maxIndex)
            {
                this.List = list;
                this.MaxIndex = maxIndex;
            }

            public abstract Node<T> ReplaceNode(ImmutableList<T> newList, int index, out int? replacementOffset, out ImmutableList<T> oldList);

            public int MaxIndex { get; }
            public ImmutableList<T> List { get; }
        }

        [ContractClassFor(typeof(Node<>))]
        private abstract class NodeContracts<T> : Node<T>
        {
            protected NodeContracts(ImmutableList<T> list, int maxIndex) : base(list, maxIndex)
            {
            }

            public override Node<T> ReplaceNode(ImmutableList<T> newList, int index, out int? replacementOffset, out ImmutableList<T> oldList)
            {
                Contract.Ensures(Contract.Result<Node<T>>() != null);

                replacementOffset = default(int);
                oldList = default(ImmutableList<T>);

                return default(Node<T>);
            }
        }

        private sealed class InnerNode<T> : Node<T>
        {
            private readonly Node<T> _left;
            private readonly Node<T> _right;

            public InnerNode(Node<T> left, Node<T> right) : base(
                left.List != null && right.List != null
                    ? left.List.AddRange(right.List)
                    : null,
                Math.Max(left.MaxIndex, right.MaxIndex))
            {
                Contract.Requires(left != null);
                Contract.Requires(right != null);

                this._left = left;
                this._right = right;
            }

            public override Node<T> ReplaceNode(ImmutableList<T> newList, int index, out int? replacementOffset, out ImmutableList<T> oldList)
            {
                if (this.MaxIndex < index)
                    throw new InvalidOperationException();

                if (this._left.MaxIndex >= index)
                    return new InnerNode<T>(this._left.ReplaceNode(newList, index, out replacementOffset, out oldList), this._right);

                var newNode = new InnerNode<T>(this._left, this._right.ReplaceNode(newList, index, out replacementOffset, out oldList));
                replacementOffset += this._left.List?.Count;

                return newNode;
            }
        }

        private sealed class TerminalNode<T> : Node<T>
        {
            public TerminalNode(ImmutableList<T> list, int maxIndex) : base(list, maxIndex)
            {
            }

            public override Node<T> ReplaceNode(ImmutableList<T> newList, int index, out int? replacementOffset, out ImmutableList<T> oldList)
            {
                oldList = this.List;
                replacementOffset = 0;

                return new TerminalNode<T>(newList, index);
            }
        }
        #endregion

        #region IndexedNotification
        private struct IndexedNotification<T>
        {
            public int Index { get; }
            public ListChangedNotification<T> Notification { get; }

            public IndexedNotification(int index, ListChangedNotification<T> notification)
            {
                this.Index = index;
                this.Notification = notification;
            }
        }
        #endregion

        #region ConcatListReactiveCollection
        private sealed class ConcatListReactiveCollection<T> : IReactiveCollection<ListChangedNotification<T>>
        {
            public ConcatListReactiveCollection(
                IObservable<ListChangedNotification<T>>[] sources,
                IEqualityComparer<T> equalityComparer)
            {
                Contract.Requires(sources != null);
                Contract.Requires(equalityComparer != null);

                this.Changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var rootNode = this.GetTree(0, sources.Length - 1);

                        return sources
                            .Select((observable, i) => observable
                                .Select(notification => new IndexedNotification<T>(i, notification)))
                            .Merge()
                            .Select(tuple =>
                            {
                                lock(syncRoot)
                                {
                                    int? offset;
                                    ImmutableList<T> oldList;

                                    rootNode = rootNode.ReplaceNode(tuple.Notification.Current, tuple.Index, out offset, out oldList);

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
                            .Where(x => x != null);
                    })
                    .ReplayFresh(1)
                    .RefCount()
                    .Normalize();
            }

            public IObservable<ListChangedNotification<T>> Changes { get; }

            private Node<T> GetTree(int min, int max)
            {
                if (min == max)
                    return new TerminalNode<T>(null, min);

                var half = (max - min + 1) / 2;
               
                return new InnerNode<T>(this.GetTree(min, (min + half - 1)), this.GetTree(min + half, max));
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
            Contract.Requires(source1 != null);
            Contract.Requires(source2 != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<T>>>() != null);

            return source1.Concat(source2, EqualityComparer<T>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<T>> Concat<T>(this IReactiveCollection<ListChangedNotification<T>> source1, IReactiveCollection<ListChangedNotification<T>> source2, IEqualityComparer<T> equalityComparer)
        {
            Contract.Requires(source1 != null);
            Contract.Requires(source2 != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<T>>>() != null);

            return new ConcatListReactiveCollection<T>(new[] { source1.Changes, source2.Changes } , equalityComparer);
        }
    }
}
