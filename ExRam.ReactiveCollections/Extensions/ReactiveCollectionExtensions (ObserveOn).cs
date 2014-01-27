using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region ObserveOnReactiveCollection
        private sealed class ObserveOnReactiveCollection<TNotification, TSource> : IReactiveCollection<TNotification, TSource>
            where TNotification : ICollectionChangedNotification<TSource>
        {
            private readonly IObservable<TNotification> _changes;

            public ObserveOnReactiveCollection(IReactiveCollection<TNotification, TSource> source, SynchronizationContext syncContext)
            {
                Contract.Requires(source != null);

                this._changes = source.Changes
                    .ObserveOn(syncContext);
            }

            public IObservable<TNotification> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        public static IReactiveCollection<TNotification, TSource> ObserveOn<TNotification, TSource>(this IReactiveCollection<TNotification, TSource> source, SynchronizationContext syncContext) where TNotification : ICollectionChangedNotification<TSource>
        {
            Contract.Requires(source != null);

            return new ObserveOnReactiveCollection<TNotification, TSource>(source, syncContext);
        }
    }
}
