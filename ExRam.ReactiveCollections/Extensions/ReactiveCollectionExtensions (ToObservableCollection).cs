using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region ReactiveReadOnlyObservableCollection
        private sealed class ReactiveReadOnlyObservableCollection<T> : ReadOnlyObservableCollection<T>
        {
            #region ConnectableObservableImpl
            private sealed class ConnectableObservableImpl : IConnectableObservable<NotifyCollectionChangedEventArgs>
            {
                private readonly IObservable<ListChangedNotification<T>> _source;
                private readonly ReactiveReadOnlyObservableCollection<T> _collection;

                public ConnectableObservableImpl(ReactiveReadOnlyObservableCollection<T> collection, IObservable<ListChangedNotification<T>> source)
                {
                    Contract.Requires(collection != null);
                    Contract.Requires(source != null);

                    this._source = source;
                    this._collection = collection;
                }

                public IDisposable Connect()
                {
                    return this._source.Subscribe((notification) =>
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
                                    this._collection.Items.Insert((i + index), notification.NewItems[i]);
                                }

                                break;
                            }

                            case (NotifyCollectionChangedAction.Remove):
                            {
                                // ReSharper disable PossibleInvalidOperationException
                                var index = notification.Index.Value;
                                // ReSharper restore PossibleInvalidOperationException

                                for (var i = 0; i < notification.OldItems.Count; i++)
                                {
                                    this._collection.Items.RemoveAt(index);
                                }

                                break;
                            }

                            case (NotifyCollectionChangedAction.Replace):
                            {
                                // ReSharper disable PossibleInvalidOperationException
                                var index = notification.Index.Value;
                                // ReSharper restore PossibleInvalidOperationException

                                for (var i = 0; i < notification.OldItems.Count; i++)
                                {
                                    this._collection.Items[i + index] = notification.NewItems[i];
                                }

                                break;
                            }

                            default:
                            {
                                if (this._collection.Count > 0)
                                    this._collection.Items.Clear();

                                foreach (var item in notification.Current)
                                {
                                    this._collection.Items.Add(item);
                                }

                                break;
                            }
                        }
                    });
                }

                public IDisposable Subscribe(IObserver<NotifyCollectionChangedEventArgs> observer)
                {
                    return this._collection._baseCollectionChangedEvents
                        .Subscribe(observer);
                }
            }
            #endregion

            protected override event NotifyCollectionChangedEventHandler CollectionChanged
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

            private readonly INotifyCollectionChanged _collectionChanged;
            private readonly IObservable<NotifyCollectionChangedEventArgs> _baseCollectionChangedEvents;

            public ReactiveReadOnlyObservableCollection(IObservable<ListChangedNotification<T>> source) : base(new ObservableCollection<T>())
            {
                Contract.Requires(source != null);

                this._collectionChanged = new ConnectableObservableImpl(this, source)
                    .RefCount()
                    .ToNotifyCollectionChangedEventPattern(this);

                this._baseCollectionChangedEvents = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>((eh) => base.CollectionChanged += eh, (eh) => base.CollectionChanged -= eh)
                    .Select(x => x.EventArgs);
            }
        }
        #endregion

        public static ReadOnlyObservableCollection<T> ToObservableCollection<T>(this IReactiveCollection<ListChangedNotification<T>, T> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<INotifyCollectionChanged>() != null);

            return new ReactiveReadOnlyObservableCollection<T>(source.Changes);
        }
    }
}
