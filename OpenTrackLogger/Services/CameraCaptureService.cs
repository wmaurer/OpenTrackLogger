namespace OpenTrackLogger.Services
{
    using System;

    using Microsoft.Phone.Tasks;

    using OpenTrackLogger.Mixins;

    public interface ICameraService
    {
        void CapturePhoto();
        IObservable<PhotoResult> PhotoCaptureCompleted { get; }
    }

    public class CameraService : ICameraService
    {
        private readonly CameraCaptureTask _cameraCaptureTask;

        public CameraService()
        {
            _cameraCaptureTask = new CameraCaptureTask();
            PhotoCaptureCompleted = _cameraCaptureTask.Events().PhotoCaptureCompleted;
        }

        public void CapturePhoto()
        {
            _cameraCaptureTask.Show();
        }

        public IObservable<PhotoResult> PhotoCaptureCompleted { get; private set; }
    }
}
