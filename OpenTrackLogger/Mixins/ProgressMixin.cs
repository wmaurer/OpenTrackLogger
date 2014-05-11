namespace OpenTrackLogger.Mixins
{
    using System;
    using System.Reactive.Linq;

    public static class ProgressMixin
    {
        public static ProgressEvents<T> Events<T>(this Progress<T> progress)
        {
            return new ProgressEvents<T>(progress);
        }
    }

    public class ProgressEvents<T>
    {
        private readonly Progress<T> _progress;

        public ProgressEvents(Progress<T> progress)
        {
            _progress = progress;
        }

        public IObservable<T> ProgressChanged
        {
            get { return Observable.FromEventPattern<EventHandler<T>, T>(h => _progress.ProgressChanged += h, h => _progress.ProgressChanged -= h).Select(x => x.EventArgs); }
        }
    }
}
