// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region MulticastConnectableObservable
        internal sealed class MulticastConnectableObservable<T> : IConnectableObservable<T>
        {
            private readonly IObservable<T> _source;
            private readonly object _syncRoot = new();
            private readonly Func<ISubject<T>> _subjectFactory;

            private ISubject<T> _currentSubject;
            private IDisposable _currentSubscription;

            public MulticastConnectableObservable(IObservable<T> source, Func<ISubject<T>> subjectFactory)
            {
                _source = source;
                _subjectFactory = subjectFactory;
            }

            public IDisposable Connect()
            {
                lock (_syncRoot)
                {
                    if (_currentSubscription != null)
                        throw new InvalidOperationException();

                    _currentSubject ??= _subjectFactory();

                    var sourceSubscription = _currentSubscription = _source.Subscribe(_currentSubject);

                    return new CompositeDisposable(
                        sourceSubscription,
                        Disposable.Create(() =>
                        {
                            lock (_syncRoot)
                            {
                                if (ReferenceEquals(_currentSubscription, sourceSubscription))
                                {
                                    _currentSubscription = null;

                                    (_currentSubject as IDisposable)?.Dispose();
                                    _currentSubject = null;
                                }
                            }
                        }));
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                lock (_syncRoot)
                {
                    _currentSubject ??= _subjectFactory();

                    return _currentSubject.Subscribe(observer);
                }
            }
        }
        #endregion

        internal static IConnectableObservable<T> Multicast<T>(this IObservable<T> source, Func<ISubject<T>> subjectFactory)
        {
            return new MulticastConnectableObservable<T>(source, subjectFactory);
        }

        internal static IConnectableObservable<T> ReplayFresh<T>(this IObservable<T> observable, int bufferSize)
        {
            return observable
                .Multicast(() => new ReplaySubject<T>(bufferSize));
        }
    }
}
