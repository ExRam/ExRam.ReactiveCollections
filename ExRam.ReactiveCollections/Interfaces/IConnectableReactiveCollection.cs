using System;

namespace ExRam.ReactiveCollections
{
    public interface IConnectableReactiveCollection<out TNotification> : IReactiveCollection<TNotification>
        where TNotification : ICollectionChangedNotification
    {
        IDisposable Connect();
    }
}
