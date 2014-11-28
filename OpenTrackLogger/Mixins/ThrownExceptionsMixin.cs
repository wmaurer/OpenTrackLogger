namespace OpenTrackLogger.Mixins
{
    using System;
    using System.Reactive.Linq;

    using ReactiveUI;

    public static class ThrownExceptionsMixin
    {
        public static IObservable<T> Log2<T, TObj>(this IObservable<T> This, TObj klass, string message = null, Func<T, string> stringifier = null) where TObj : IEnableLogger
        {
            return This.Log(klass, message, stringifier);
        }

        //public static IDisposable LogException(this IObservable<Exception> thrownExceptions, string message = null)
        //{
        //    return thrownExceptions
        //        .Subscribe(async x => {
        //            await ExceptionLoggerService.LogException(message == null ? x : new Exception(message, x));
        //            //await RxApp.DependencyResolver.GetService<LogService>().Log(new Exception(message, x).ToString());
        //        });
        //}
    }
}
