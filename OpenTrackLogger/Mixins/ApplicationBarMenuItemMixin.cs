namespace OpenTrackLogger.Mixins
{
    using System;
    using System.Reactive.Linq;

    using Microsoft.Phone.Shell;

    public static class ApplicationBarMenuItemMixin
    {
        public static ApplicationBarMenuItemEvents Events(this ApplicationBarMenuItem applicationBarMenuItem)
        {
            return new ApplicationBarMenuItemEvents(applicationBarMenuItem);
        }
    }

    public class ApplicationBarMenuItemEvents
    {
        private readonly ApplicationBarMenuItem _applicationBarMenuItem;

        public ApplicationBarMenuItemEvents(ApplicationBarMenuItem applicationBarMenuItem)
        {
            _applicationBarMenuItem = applicationBarMenuItem;
        }

        public IObservable<object> Click
        {
            get { return Observable.FromEventPattern<EventHandler, EventArgs>(h => _applicationBarMenuItem.Click += h, h => _applicationBarMenuItem.Click -= h).Select(x => x.EventArgs); }
        }
    }
}
