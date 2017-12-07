// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Reactive.Disposables;
using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region MulticastConnectableObservable
        internal sealed class MulticastConnectableObservable<T> : IConnectableObservable<T>
        {
            private readonly IObservable<T> _source;
            private readonly object _syncRoot = new object();
            private readonly Func<ISubject<T>> _subjectFactory;

            private ISubject<T> _currentSubject;
            private IDisposable _currentSubscription;

            public MulticastConnectableObservable([NotNull] IObservable<T> source, [NotNull] Func<ISubject<T>> subjectFactory)
            {
                this._source = source;
                this._subjectFactory = subjectFactory;
            }

            public IDisposable Connect()
            {
                lock (this._syncRoot)
                {
                    if (this._currentSubscription != null)
                        throw new InvalidOperationException();

                    if (this._currentSubject == null)
                        this._currentSubject = this._subjectFactory();

                    var sourceSubscription = this._currentSubscription = this._source.Subscribe(this._currentSubject);

                    return new CompositeDisposable(
                        sourceSubscription,
                        Disposable.Create(() =>
                        {
                            lock (this._syncRoot)
                            {
                                if (object.ReferenceEquals(this._currentSubscription, sourceSubscription))
                                {
                                    this._currentSubscription = null;

                                    (this._currentSubject as IDisposable)?.Dispose();
                                    this._currentSubject = null;
                                }
                            }
                        }));
                }
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                lock (this._syncRoot)
                {
                    if (this._currentSubject == null)
                        this._currentSubject = this._subjectFactory();

                    return this._currentSubject.Subscribe(observer);
                }
            }
        }
        #endregion

        [NotNull]
        internal static IConnectableObservable<T> Multicast<T>([NotNull] this IObservable<T> source, [NotNull] Func<ISubject<T>> subjectFactory)
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
