// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region ReactiveReadOnlyObservableCollection
        private sealed class ReactiveReadOnlyObservableCollection<T> : IList<T>, IReadOnlyList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
        {
            #region Events
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
            #endregion

            private readonly INotifyPropertyChanged _propertyChanged;
            private readonly INotifyCollectionChanged _collectionChanged;

            private ImmutableList<T> _currentList = ImmutableList<T>.Empty;

            public ReactiveReadOnlyObservableCollection(IObservable<ListChangedNotification<T>> source)
            {
                Contract.Requires(source != null);

                var eventArgs = source
                    .Select(notification =>
                    {
                        this._currentList = notification.Current;

                        switch (notification.Action)
                        {
                            case (NotifyCollectionChangedAction.Add):
                            {
                                return new EventArgs[]
                                {
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)notification.NewItems, notification.Index.Value),
                                    new PropertyChangedEventArgs("Count"),
                                    new PropertyChangedEventArgs("Item[]")
                                };
                            }

                            case (NotifyCollectionChangedAction.Remove):
                            {
                                return new EventArgs[]
                                {

                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)notification.OldItems, notification.Index.Value),
                                    new PropertyChangedEventArgs("Count"),
                                    new PropertyChangedEventArgs("Item[]")
                                };
                            }

                            case (NotifyCollectionChangedAction.Replace):
                            {
                                return new EventArgs[]
                                {
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)notification.NewItems, (IList)notification.OldItems, notification.Index.Value),
                                    new PropertyChangedEventArgs("Item[]")
                                };
                            }

                            default:
                            {
                                var list = new List<EventArgs>();

                                if (this._currentList.Count > 0)
                                {
                                    this._currentList = ImmutableList<T>.Empty;

                                    list.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                    list.Add(new PropertyChangedEventArgs("Count"));
                                    list.Add(new PropertyChangedEventArgs("Item[]"));
                                }

                                if (notification.Current.Count > 0)
                                {
                                    this._currentList = notification.Current;

                                    list.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)notification.Current, 0));
                                    list.Add(new PropertyChangedEventArgs("Count"));
                                    list.Add(new PropertyChangedEventArgs("Item[]"));
                                }

                                return list.ToArray();
                            }
                        }
                    })
                    .Skip(1)
                    .SelectMany(x => x)
                    .Publish()
                    .RefCount();

                this._collectionChanged = eventArgs
                    .OfType<NotifyCollectionChangedEventArgs>()
                    .ToNotifyCollectionChangedEventPattern(this);

                this._propertyChanged = eventArgs
                    .OfType<PropertyChangedEventArgs>()
                    .ToNotifyPropertyChangedEventPattern(this);
            }

            void ICollection<T>.Add(T item)
            {
                throw new NotSupportedException();
            }

            void ICollection<T>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<T>.Contains(T item)
            {
                return this._currentList.Contains(item);
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                this._currentList.CopyTo(array, arrayIndex);
            }

            int ICollection<T>.Count
            {
                get 
                {
                    return this._currentList.Count;
                }
            }

            bool ICollection<T>.IsReadOnly
            {
                get 
                {
                    return true;
                }
            }

            bool ICollection<T>.Remove(T item)
            {
                throw new NotSupportedException();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this._currentList.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this._currentList.GetEnumerator();
            }

            int IList<T>.IndexOf(T item)
            {
                return this._currentList.IndexOf(item);
            }

            void IList<T>.Insert(int index, T item)
            {
                throw new NotSupportedException();
            }

            void IList<T>.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            T IList<T>.this[int index]
            {
                get
                {
                    return this._currentList[index];
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    return this._currentList[index];
                }
            }

            int IReadOnlyCollection<T>.Count
            {
                get
                {
                    return this._currentList.Count;
                }
            }

            int IList.Add(object value)
            {
                throw new NotSupportedException();
            }

            void IList.Clear()
            {
                throw new NotSupportedException();
            }

            bool IList.Contains(object value)
            {
                return ((IList)this._currentList).Contains(value);
            }

            int IList.IndexOf(object value)
            {
                return ((IList)this._currentList).IndexOf(value);
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            bool IList.IsFixedSize
            {
                get 
                {
                    return false;
                }
            }

            bool IList.IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException();
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            object IList.this[int index]
            {
                get
                {
                    return this._currentList[index];
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)this._currentList).CopyTo(array, index);
            }

            int ICollection.Count
            {
                get
                {
                    return this._currentList.Count;
                }
            }

            bool ICollection.IsSynchronized
            {
                get 
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }
        }
        #endregion

        public static ICollection<T> ToObservableCollection<T>(this IReactiveCollection<ListChangedNotification<T>, T> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<ICollection<T>>() != null);

            return new ReactiveReadOnlyObservableCollection<T>(source.Changes);
        }
    }
}
