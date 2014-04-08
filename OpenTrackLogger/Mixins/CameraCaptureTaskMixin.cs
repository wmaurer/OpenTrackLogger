namespace OpenTrackLogger.Mixins
{
    using System;
    using System.Reactive.Linq;

    using Microsoft.Phone.Tasks;

    public static class CameraCaptureTaskMixin
    {
        public static CameraCaptureTaskEvents Events(this CameraCaptureTask cameraCaptureTask)
        {
            return new CameraCaptureTaskEvents(cameraCaptureTask);
        }
    }

    public class CameraCaptureTaskEvents
    {
        private readonly CameraCaptureTask _cameraCaptureTask;

        public CameraCaptureTaskEvents(CameraCaptureTask cameraCaptureTask)
        {
            _cameraCaptureTask = cameraCaptureTask;
        }

        public IObservable<PhotoResult> PhotoCaptureCompleted
        {
            get { return Observable.FromEventPattern<EventHandler<PhotoResult>, PhotoResult>(h => _cameraCaptureTask.Completed += h, h => _cameraCaptureTask.Completed -= h).Select(x => x.EventArgs); }
        }
    }
}
