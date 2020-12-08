using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            return source.Sort(Comparer<TSource>.Default, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
        {
            return source.Sort(comparer, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IEqualityComparer<TSource> equalityComparer)
        {
            return source.Sort(Comparer<TSource>.Default, equalityComparer);
        }

        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
        {
            return source
                .Transform(
                    SortedListChangedNotification<TSource>.Reset.WithComparer(comparer),
                    (source, target) => target.Sort(source, equalityComparer));
        }
    }
}
