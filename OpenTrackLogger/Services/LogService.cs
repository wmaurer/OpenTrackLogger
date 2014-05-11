namespace OpenTrackLogger.Services
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Storage;

    public class LogService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static StorageFile _file;

        private static async Task<StorageFile> GetLogFile()
        {
            if (_file != null) return _file;
            
            _file = await ApplicationData.Current.LocalFolder.CreateFileAsync(string.Format("Log{0}.txt", DateTime.UtcNow.ToString("yyyyMMddHHmmss")), CreationCollisionOption.GenerateUniqueName);

            return _file;
        }

        public async Task Log(string message)
        {
            await _semaphore.WaitAsync();

            try {
                var messageToWrite = string.Format("{0}Z: {1}\n", DateTime.UtcNow.ToString("s"), message);
                var file = await GetLogFile();
                using (var stream = await file.OpenStreamForWriteAsync()) {
                    stream.Seek(0, SeekOrigin.End);
                    var chars = messageToWrite.ToCharArray();
                    var bytes = new byte[chars.Length * sizeof(char)];
                    Buffer.BlockCopy(chars, 0, bytes, 0, bytes.Length);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
            finally {
                _semaphore.Release();
            }
        }
    }
}
