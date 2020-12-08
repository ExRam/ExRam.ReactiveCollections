using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<TNotification> ObserveOn<TNotification, TSource>(this IReactiveCollection<TNotification> source, SynchronizationContext syncContext) where TNotification : ICollectionChangedNotification<TSource>
        {
            return source.Changes
                .ObserveOn(syncContext)
                .ToReactiveCollection();
        }

        public static IReactiveCollection<TNotification> ObserveOn<TNotification, TSource>(this IReactiveCollection<TNotification> source, IScheduler scheduler) where TNotification : ICollectionChangedNotification<TSource>
        {
            return source.Changes
                .ObserveOn(scheduler)
                .ToReactiveCollection();
        }
    }
}
