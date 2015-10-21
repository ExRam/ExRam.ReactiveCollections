namespace ExRam.ReactiveCollections
{
    public interface IIndexedCollectionChangedNotification<out T> : ICollectionChangedNotification<T>
    {
        int? Index { get; }
    }
}