// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region ReactiveReadOnlyObservableCollection
        private sealed class ReactiveReadOnlyObservableCollection<T> : Collection<T>, IList<T>, IReadOnlyList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged
            {
                add
                {
                    this._propertyChanged.PropertyChanged += value;
                }

                remove
                {
                    this._propertyChanged.PropertyChanged -= value;
                }
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    this._collectionChanged.CollectionChanged += value;
                }

                remove
                {
                    this._collectionChanged.CollectionChanged -= value;
                }
            }

            private readonly INotifyPropertyChanged _propertyChanged;
            private readonly INotifyCollectionChanged _collectionChanged;

            public ReactiveReadOnlyObservableCollection(IObservable<ListChangedNotification<T>> source) : base(new List<T>())
            {
                Contract.Requires(source != null);

                var eventArgs = Observable
                    .Create<EventArgs>((obs) => source
                        .Subscribe(notification =>
                        {
                            switch (notification.Action)
                            {
                                case (NotifyCollectionChangedAction.Add):
                                {
                                    // ReSharper disable PossibleInvalidOperationException
                                    var index = notification.Index.Value;
                                    // ReSharper restore PossibleInvalidOperationException

                                    for (var i = 0; i < notification.NewItems.Count; i++)
                                    {
                                        this.Items.Insert((i + index), notification.NewItems[i]);
                                    }

                                    obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)notification.NewItems, index));
                                    obs.OnNext(new PropertyChangedEventArgs("Count"));
                                    obs.OnNext(new PropertyChangedEventArgs("Item[]"));

                                    break;
                                }

                                case (NotifyCollectionChangedAction.Remove):
                                {
                                    // ReSharper disable PossibleInvalidOperationException
                                    var index = notification.Index.Value;
                                    // ReSharper restore PossibleInvalidOperationException

                                    for (var i = 0; i < notification.OldItems.Count; i++)
                                    {
                                        this.Items.RemoveAt(index);
                                    }

                                    obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)notification.OldItems, index));
                                    obs.OnNext(new PropertyChangedEventArgs("Count"));
                                    obs.OnNext(new PropertyChangedEventArgs("Item[]"));

                                    break;
                                }

                                case (NotifyCollectionChangedAction.Replace):
                                {
                                    // ReSharper disable PossibleInvalidOperationException
                                    var index = notification.Index.Value;
                                    // ReSharper restore PossibleInvalidOperationException

                                    for (var i = 0; i < notification.OldItems.Count; i++)
                                    {
                                        this.Items[i + index] = notification.NewItems[i];
                                    }

                                    obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)notification.NewItems, (IList)notification.OldItems, index));
                                    obs.OnNext(new PropertyChangedEventArgs("Item[]"));

                                    break;
                                }

                                default:
                                {
                                    if (this.Count > 0)
                                    {
                                        this.Items.Clear();
                                        obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                        obs.OnNext(new PropertyChangedEventArgs("Count"));
                                        obs.OnNext(new PropertyChangedEventArgs("Item[]"));
                                    }

                                    if (notification.Current.Count > 0)
                                    {
                                        foreach (var item in notification.Current)
                                        {
                                            this.Items.Add(item);
                                        }

                                        obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)notification.Current, 0));
                                        obs.OnNext(new PropertyChangedEventArgs("Count"));
                                        obs.OnNext(new PropertyChangedEventArgs("Item[]"));
                                    }

                                    break;
                                }
                            }
                        }))
                    .Publish()
                    .RefCount();

                this._collectionChanged = eventArgs
                    .OfType<NotifyCollectionChangedEventArgs>()
                    .ToNotifyCollectionChangedEventPattern(this);

                this._propertyChanged = eventArgs
                    .OfType<PropertyChangedEventArgs>()
                    .ToNotifyPropertyChangedEventPattern(this);
            }

            protected override void ClearItems()
            {
                throw new NotSupportedException();
            }

            protected override void InsertItem(int index, T item)
            {
                throw new NotSupportedException();
            }

            protected override void RemoveItem(int index)
            {
                throw new NotSupportedException();
            }

            protected override void SetItem(int index, T item)
            {
                throw new NotSupportedException();
            }
        }
        #endregion

        public static Collection<T> ToObservableCollection<T>(this IReactiveCollection<ListChangedNotification<T>, T> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<Collection<T>>() != null);

            return new ReactiveReadOnlyObservableCollection<T>(source.Changes);
        }
    }
}
