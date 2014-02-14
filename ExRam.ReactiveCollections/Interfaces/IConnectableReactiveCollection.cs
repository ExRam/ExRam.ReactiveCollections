using System;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    [ContractClass(typeof(ConnectableReactiveCollectionContracts<,>))]
    public interface IConnectableReactiveCollection<out TNotification, T> : IReactiveCollection<TNotification, T>
        where TNotification : ICollectionChangedNotification<T>
    {
        IDisposable Connect();
    }

    [ContractClassFor(typeof(IConnectableReactiveCollection<,>))]
    public abstract class ConnectableReactiveCollectionContracts<TNotification, T> : IConnectableReactiveCollection<TNotification, T>
         where TNotification : ICollectionChangedNotification<T>
    {
        public abstract IObservable<TNotification> Changes
        {
            get;
        }

        public IDisposable Connect()
        {
            Contract.Ensures(Contract.Result<IDisposable>() != null);

            return default(IDisposable);
        }
    }
}
