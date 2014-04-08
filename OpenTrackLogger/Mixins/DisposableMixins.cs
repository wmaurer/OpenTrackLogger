namespace OpenTrackLogger.Mixins
{
    using System;
    using System.Reactive.Disposables;

    public static class DisposableMixins
    {
        public static IDisposable DisposeAndReturnEmpty(this IDisposable disposable)
        {
            disposable.Dispose();
            return Disposable.Empty;
        }
    }
}
