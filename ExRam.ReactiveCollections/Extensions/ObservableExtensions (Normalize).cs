using ExRam.ReactiveCollections;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        internal static IObservable<TNotification> Normalize<TNotification>(this IObservable<TNotification> observable)
            where TNotification : ICollectionChangedNotification
        {
            return observable
                .Scan(
                    (isFirst: true, notification: default(TNotification)), 
                    (state, notification) => (false, state.isFirst ? (TNotification)notification.ToResetNotification() : notification))
                .Select(x => x.notification!);
        }
    }
}
