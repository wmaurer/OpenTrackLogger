namespace OpenTrackLogger.Secrets.Reader
{
    using System.IO;

    using Newtonsoft.Json;

    using OpenTrackLogger.Secrets.Common;

    public static class SecretsReader
    {
        public static SecretsData Read(string filepath)
        {
            return JsonConvert.DeserializeObject<SecretsData>(File.ReadAllText(filepath));
        }
    }
}
