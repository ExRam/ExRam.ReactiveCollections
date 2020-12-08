using System;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<TNotification> WithChanges<TNotification>(this IReactiveCollection<TNotification> source, Func<IObservable<TNotification>, IObservable<TNotification>> changesTransformation) where TNotification : ICollectionChangedNotification
        {
            return changesTransformation(source.Changes)
                .ToReactiveCollection();
        }
    }
}
