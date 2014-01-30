using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Threading;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<TNotification, TSource> ObserveOn<TNotification, TSource>(this IReactiveCollection<TNotification, TSource> source, SynchronizationContext syncContext) where TNotification : ICollectionChangedNotification<TSource>
        {
            Contract.Requires(source != null);

            return source.Changes
                .ObserveOn(syncContext)
                .ToReactiveCollection<TNotification, TSource>();
        }
    }
}
