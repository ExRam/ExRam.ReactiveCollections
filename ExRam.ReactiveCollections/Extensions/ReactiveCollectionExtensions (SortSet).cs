using System.Collections.Generic;

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
                .Transform(
                    SortedSetChangedNotification<TSource>.Reset.WithComparer(comparer),
                    (source, target) => target.Sort(source));
        }
    }
}
