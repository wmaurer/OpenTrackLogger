namespace OpenTrackLogger
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

    using Microsoft.Phone.Shell;

    public class BackgroundHost
    {
        public IObservable<Unit> IsRunningInBackground { get; set; }
        public IObservable<Unit> IsResuming { get; set; }

        public BackgroundHost()
        {
            IsRunningInBackground = PhoneApplicationService.Current.Events().RunningInBackground.Select(x => Unit.Default);
            IsResuming = PhoneApplicationService.Current.Events().Activated.Select(x => Unit.Default);
        }
    }
}
