using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            return source.SortSet(Comparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
        {
            return source
                .Changes
                .Scan(
                    new[] { SortedSetChangedNotification<TSource>.Reset.WithComparer(comparer) },
                    (currentTargetNotification, sourceNotification) =>
                    {
                        var newRet = currentTargetNotification[^1]
                            .Sort(sourceNotification)
                            .ToArray();

                        return newRet.Length > 0 ? newRet :
                            currentTargetNotification.Length > 0
                                ? new[] { currentTargetNotification[0] }
                                : currentTargetNotification;
                    })
                .SelectMany(x => x)
                .DistinctUntilChanged()
                .ToReactiveCollection();
        }
    }
}
