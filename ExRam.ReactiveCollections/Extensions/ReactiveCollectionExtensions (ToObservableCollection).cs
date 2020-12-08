using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region ReactiveReadOnlyObservableCollection
        private sealed class ReactiveReadOnlyObservableCollection<T> : IList<T>, IReadOnlyList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
        {
            #region Events
            public event PropertyChangedEventHandler? PropertyChanged
            {
                add => _propertyChanged.PropertyChanged += value;

                remove => _propertyChanged.PropertyChanged -= value;
            }

            public event NotifyCollectionChangedEventHandler? CollectionChanged
            {
                add => _collectionChanged.CollectionChanged += value;

                remove => _collectionChanged.CollectionChanged -= value;
            }
            #endregion

            private readonly INotifyPropertyChanged _propertyChanged;
            private readonly INotifyCollectionChanged _collectionChanged;

            private IReadOnlyCollection<T> _currentList = ImmutableList<T>.Empty;

            public ReactiveReadOnlyObservableCollection(IObservable<IIndexedCollectionChangedNotification<T>> source)
            {
                var eventArgs = source
                    .Do(notification => _currentList = notification.Current)
                    .Skip(1)
                    .SelectMany(notification => Observable
                        .Create<EventArgs>(obs =>
                        {
                            switch (notification.Action)
                            {
                                case NotifyCollectionChangedAction.Add:
                                {
                                    obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)notification.NewItems, notification.Index!.Value));
                                    obs.OnNext(new PropertyChangedEventArgs("Count"));
                                    obs.OnNext(new PropertyChangedEventArgs("Item[]"));

                                    break;
                                }

                                case NotifyCollectionChangedAction.Remove:
                                {

                                    obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)notification.OldItems, notification.Index!.Value));
                                    obs.OnNext(new PropertyChangedEventArgs("Count"));
                                    obs.OnNext(new PropertyChangedEventArgs("Item[]"));

                                    break;
                                }

                                case NotifyCollectionChangedAction.Replace:
                                {
                                    obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)notification.NewItems, (IList)notification.OldItems, notification.Index!.Value));
                                    obs.OnNext(new PropertyChangedEventArgs("Item[]"));

                                    break;
                                }

                                default:
                                {
                                    if (_currentList.Count > 0)
                                    {
                                        _currentList = ImmutableList<T>.Empty;

                                        obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                        obs.OnNext(new PropertyChangedEventArgs("Count"));
                                        obs.OnNext(new PropertyChangedEventArgs("Item[]"));
                                    }

                                    if (notification.Current.Count > 0)
                                    {
                                        _currentList = notification.Current;

                                        obs.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)notification.Current, 0));
                                        obs.OnNext(new PropertyChangedEventArgs("Count"));
                                        obs.OnNext(new PropertyChangedEventArgs("Item[]"));
                                    }

                                    break;
                                }
                            }

                            return Disposable.Empty;
                        }))
                    .Publish()
                    .RefCount();

                _collectionChanged = eventArgs
                    .OfType<NotifyCollectionChangedEventArgs>()
                    .ToNotifyCollectionChangedEventPattern(this);

                _propertyChanged = eventArgs
                    .OfType<PropertyChangedEventArgs>()
                    .ToNotifyPropertyChangedEventPattern(this);
            }

            void ICollection<T>.Add(T item) => throw new NotSupportedException();

            public void Clear() => throw new NotSupportedException();

            bool ICollection<T>.Contains(T item)
            {
                return !(_currentList is ICollection<T> list)
                    ? throw new InvalidOperationException()
                    : list.Contains(item);
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                if (!(_currentList is ICollection<T> list))
                    throw new InvalidOperationException();

                list.CopyTo(array, arrayIndex);
            }

            public int Count => _currentList.Count;

            public bool IsReadOnly => true;

            bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => _currentList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _currentList.GetEnumerator();

            int IList<T>.IndexOf(T item)
            {
                return _currentList is IList<T> list
                    ? list.IndexOf(item)
                    : throw new InvalidOperationException();
            }

            void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

            void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

            T IList<T>.this[int index]
            {
                get => _currentList is IReadOnlyList<T> list
                    ? list[index]
                    : throw new InvalidOperationException();
                set => throw new NotSupportedException();
            }

            T IReadOnlyList<T>.this[int index] => ((IList<T>)this)[index];

            int IList.Add(object? value) => throw new NotSupportedException();

            bool IList.Contains(object? value) => ((IList)_currentList).Contains(value);

            int IList.IndexOf(object? value) => ((IList)_currentList).IndexOf(value);

            void IList.Insert(int index, object? value) => throw new NotSupportedException();

            bool IList.IsFixedSize => false;

            void IList.Remove(object? value) => throw new NotSupportedException();

            void IList.RemoveAt(int index) => throw new NotSupportedException();

            object? IList.this[int index]
            {
                get => ((IList<T>)this)[index];
                set => throw new NotSupportedException();
            }

            void ICollection.CopyTo(Array array, int index) => ((ICollection)_currentList).CopyTo(array, index);

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => this;
        }
        #endregion

        public static ICollection<T> ToObservableCollection<T>(this IReactiveCollection<IIndexedCollectionChangedNotification<T>> source)
        {
            return new ReactiveReadOnlyObservableCollection<T>(source.Changes);
        }
    }
}
