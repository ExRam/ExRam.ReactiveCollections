// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;

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
                Contract.Requires(source != null);
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    base.Add(value, (o, e) => value(o, e));
                }

                remove
                {
                    base.Remove(value);
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
                Contract.Requires(source != null);
            }

            public event PropertyChangedEventHandler PropertyChanged
            {
                add
                {
                    base.Add(value, (o, e) => value(o, e));
                }

                remove
                {
                    base.Remove(value);
                }
            }
        }
        #endregion

        public static INotifyCollectionChanged ToNotifyCollectionChangedEventPattern(this IObservable<NotifyCollectionChangedEventArgs> source, object sender)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<INotifyCollectionChanged>() != null);

            return new NotifyCollectionChangedEventPatternSource(source.Select(x => new EventPattern<NotifyCollectionChangedEventArgs>(sender, x)));
        }

        public static INotifyPropertyChanged ToNotifyPropertyChangedEventPattern(this IObservable<PropertyChangedEventArgs> source, object sender)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<INotifyPropertyChanged>() != null);

            return new NotifyPropertyChangedEventPatternSource(source.Select(x => new EventPattern<PropertyChangedEventArgs>(sender, x)));
        }
    }
}
