
namespace ExRam.ReactiveCollections
{
    public interface IReactiveCollectionSource<out TNotification, T>
         where TNotification : ICollectionChangedNotification<T>
    {
        IReactiveCollection<TNotification, T> ReactiveCollection
        {
            get;
        }
    }
}
