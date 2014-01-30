using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<TNotification, TSource> ToWeakReactiveCollection<TNotification, TSource>(this IReactiveCollection<TNotification, TSource> source) where TNotification : ICollectionChangedNotification<TSource>
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<TNotification, TSource>>() != null);

            return source.Changes
                .ToWeakObservable()
                .ToReactiveCollection<TNotification, TSource>();
        }
    }
}
