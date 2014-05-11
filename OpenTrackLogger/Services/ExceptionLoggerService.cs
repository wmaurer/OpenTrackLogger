namespace OpenTrackLogger.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Windows.Storage;

    public static class ExceptionLoggerService
    {
        public static async Task LogException(Exception e)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(string.Format("Exception{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")), CreationCollisionOption.GenerateUniqueName);
            using (var stream = await file.OpenStreamForWriteAsync()) {
                var chars = e.ToString().ToCharArray();
                var bytes = new byte[chars.Length * sizeof(char)];
                Buffer.BlockCopy(chars, 0, bytes, 0, bytes.Length);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
