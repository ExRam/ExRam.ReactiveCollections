namespace ExRam.ReactiveCollections
{
    public interface IReactiveCollectionSource<out TNotification>
         where TNotification : ICollectionChangedNotification
    {
        IReactiveCollection<TNotification> ReactiveCollection
        {
            get;
        }
    }
}
