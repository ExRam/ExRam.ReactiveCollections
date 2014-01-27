using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region ReactiveReadOnlyObservableCollection
        private sealed class ReactiveReadOnlyObservableCollection<T> : ReadOnlyObservableCollection<T>
        {
            // ReSharper disable NotAccessedField.Local
            private readonly IDisposable _subscription;
            // ReSharper restore NotAccessedField.Local

            private readonly IObservable<ListChangedNotification<T>> _source;

            public ReactiveReadOnlyObservableCollection(IObservable<ListChangedNotification<T>> source)
                : base(new ObservableCollection<T>())
            {
                Contract.Requires(source != null);

                this._source = source;

                this._subscription = source
                    .ToWeakObservable()
                    .Subscribe((notification) =>
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
                                        base.Items.Insert((i + index), notification.NewItems[i]);
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
                                        base.Items.RemoveAt(i + index);
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
                                        base.Items[i + index] = notification.NewItems[i];
                                    }

                                    break;
                                }

                            default:
                                {
                                    this.Items.Clear();

                                    foreach (var item in notification.Current)
                                    {
                                        this.Items.Add(item);
                                    }

                                    break;
                                }
                        }
                    });
            }

            public IObservable<ListChangedNotification<T>> Source
            {
                get
                {
                    return this._source;
                }
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
