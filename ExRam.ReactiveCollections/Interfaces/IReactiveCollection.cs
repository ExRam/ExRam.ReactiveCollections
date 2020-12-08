using System;

namespace ExRam.ReactiveCollections
{
    public interface IReactiveCollection<out TNotification>
        where TNotification : ICollectionChangedNotification
    {
        IObservable<TNotification> Changes
        {
            get;
        }
    }
}
