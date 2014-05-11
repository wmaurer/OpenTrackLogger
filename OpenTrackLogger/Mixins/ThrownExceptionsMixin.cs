namespace OpenTrackLogger.Mixins
{
    using System;

    using OpenTrackLogger.Services;

    using ReactiveUI;

    public static class ThrownExceptionsMixin
    {
        public static IDisposable LogException(this IObservable<Exception> thrownExceptions, string message = null)
        {
            return thrownExceptions
                .Subscribe(async x => {
                    await ExceptionLoggerService.LogException(message == null ? x : new Exception(message, x));
                    //await RxApp.DependencyResolver.GetService<LogService>().Log(new Exception(message, x).ToString());
                });
        }
    }
}
