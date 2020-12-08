using System;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region AsReactiveCollectionImpl
        private sealed class AsReactiveCollectionImpl<TNotification, T> : IReactiveCollection<TNotification>
            where TNotification : ICollectionChangedNotification<T>
        {
            private readonly IReactiveCollection<TNotification> _reactiveCollection;

            public AsReactiveCollectionImpl(IReactiveCollection<TNotification> reactiveCollection)
            {
                _reactiveCollection = reactiveCollection;
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes => _reactiveCollection.Changes.AsObservable();
        }
        #endregion

        public static IReactiveCollection<TNotification> AsReactiveCollection<TNotification, T>(this IReactiveCollection<TNotification> reactiveCollection)
            where TNotification : ICollectionChangedNotification<T>
        {
            return new AsReactiveCollectionImpl<TNotification, T>(reactiveCollection);
        }
    }
}
