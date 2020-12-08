using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region NotifyCollectionChangedEventPatternSource
        private sealed class NotifyCollectionChangedEventPatternSource : EventPatternSourceBase<object, NotifyCollectionChangedEventArgs>, INotifyCollectionChanged
        {
            public NotifyCollectionChangedEventPatternSource(IObservable<EventPattern<object, NotifyCollectionChangedEventArgs>> source)
                : base(source, (invokeAction, eventPattern) => invokeAction(eventPattern.Sender, eventPattern.EventArgs))
            {
            }

            public event NotifyCollectionChangedEventHandler? CollectionChanged
            {
                add
                {
                    if (value is null)
                        throw new ArgumentNullException();

                    Add(value, (o, e) => value?.Invoke(o, e));
                }
                remove
                {
                    if (value is null)
                        throw new ArgumentNullException();

                    Remove(value);
                }
            }
        }
        #endregion

        #region NotifyPropertyChangedEventPatternSource
        private sealed class NotifyPropertyChangedEventPatternSource : EventPatternSourceBase<object, PropertyChangedEventArgs>, INotifyPropertyChanged
        {
            public NotifyPropertyChangedEventPatternSource(IObservable<EventPattern<object, PropertyChangedEventArgs>> source)
                : base(source, (invokeAction, eventPattern) => invokeAction(eventPattern.Sender, eventPattern.EventArgs))
            {
            }

            public event PropertyChangedEventHandler? PropertyChanged
            {
                add
                {
                    if (value is null)
                        throw new ArgumentNullException();

                    Add(value, (o, e) => value?.Invoke(o, e));
                }

                remove
                {
                    if (value is null)
                        throw new ArgumentNullException();

                    Remove(value);
                }
            }
        }
        #endregion

        public static INotifyCollectionChanged ToNotifyCollectionChangedEventPattern(this IObservable<NotifyCollectionChangedEventArgs> source, object sender)
        {
            return new NotifyCollectionChangedEventPatternSource(source.Select(x => new EventPattern<NotifyCollectionChangedEventArgs>(sender, x)));
        }

        public static INotifyPropertyChanged ToNotifyPropertyChangedEventPattern(this IObservable<PropertyChangedEventArgs> source, object sender)
        {
            return new NotifyPropertyChangedEventPatternSource(source.Select(x => new EventPattern<PropertyChangedEventArgs>(sender, x)));
        }
    }
}
