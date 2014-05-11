namespace OpenTrackLogger.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Attached property inteded to make the target ItemsControl a region
    /// where controls can be remotely injected. For example a
    /// region "Master" is defined by an ItemsControl or subclass with an attached
    /// property
    /// 
    /// <ItemsControl Region.Region="Master"/>
    ///
    /// and a control
    /// 
    /// <RegionContentControl Region="Master">....</RegionContentControl>
    ///
    /// can provide content for this control from any other view. The content
    /// is injected on load of the client and removed on unload of the client.
    /// </summary>
    public static class Region
    {
        public static readonly DependencyProperty RegionProperty =
            DependencyProperty.RegisterAttached("Region", typeof(string), typeof(Region), new PropertyMetadata(string.Empty, RegionUpdated));

        public static void SetRegion(ItemsControl dp, string region)
        {
            dp.SetValue(RegionProperty, region);
        }

        public static string GetRegion(ItemsControl dp)
        {
            return (string)dp.GetValue(RegionProperty);
        }

        public static IDisposable GetRegionSubscription(DependencyObject obj)
        {
            return (IDisposable)obj.GetValue(RegionSubscriptionProperty);
        }

        public static void SetRegionSubscription(DependencyObject obj, IDisposable value)
        {
            obj.SetValue(RegionSubscriptionProperty, value);
        }

        // Using a DependencyProperty as the backing store for RegionSubscription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionSubscriptionProperty =
            DependencyProperty.RegisterAttached("RegionSubscription", typeof(IDisposable), typeof(Region), new PropertyMetadata(Disposable.Empty));


        private static void RegionUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;

            var itemsControl = d as ItemsControl;
            if (d == null) {
                throw new Exception(d.GetType() + " is not an ItemsControl");
            }

            GetRegionSubscription(itemsControl).Dispose();

            var s = ReactiveUI.MessageBus.Current.Listen<RegionMessage>()
                .Subscribe(message => HandleRegionMessage(itemsControl, message));

            SetRegionSubscription(itemsControl, s);
        }

        private static void HandleRegionMessage(ItemsControl control, RegionMessage obj)
        {
            if (obj.Region != GetRegion(control)) {
                return;
            }

            if (obj.Action == RegionMessage.RegionAction.Add) {
                foreach (var item in obj.Controls) {
                    control.Items.Add(item);
                }
            }
            else {
                foreach (var item in obj.Controls) {
                    control.Items.Remove(item);
                }
            }
        }
    }

    public class RegionMessage
    {
        public enum RegionAction
        {
            Add,
            Remove,
        }

        public string Region { get; set; }
        public List<object> Controls { get; set; }
        public RegionAction Action { get; set; }
    }

    public class RegionContentControl : ItemsControl
    {
        public string Region
        {
            get { return (string)GetValue(RegionProperty); }
            set { SetValue(RegionProperty, value); }
        }

        public static readonly DependencyProperty RegionProperty =
            DependencyProperty.Register("Region", typeof(string), typeof(RegionContentControl), new PropertyMetadata(string.Empty));

        public RegionContentControl()
        {
            this.Events().Loaded.Subscribe(OnLoaded);
            this.Events().Unloaded.Subscribe(OnUnloaded);
        }

        private void OnUnloaded(RoutedEventArgs obj)
        {
            ReactiveUI.MessageBus.Current.SendMessage(new RegionMessage
            {
                Action = RegionMessage.RegionAction.Remove,
                Controls = _hiddenChildren,
                Region = Region
            });
        }

        List<object> _hiddenChildren;

        private void OnLoaded(RoutedEventArgs obj)
        {
            if (_hiddenChildren == null) {
                _hiddenChildren = new List<object>();
                foreach (var item in Items) {
                    _hiddenChildren.Add(item);
                }
                Items.Clear();
            }

            ReactiveUI.MessageBus.Current.SendMessage(new RegionMessage
            {
                Action = RegionMessage.RegionAction.Add,
                Controls = _hiddenChildren,
                Region = Region
            });
        }
    }
}
