namespace OpenTrackLogger.Secrets
{
    using OpenTrackLogger.Secrets.Common;

    public partial class Secrets
    {
        public static SecretsData Data { get; private set; }

        //public void Foo()
        //{
        //    using (var streamReader = File.OpenText("")) {
        //        var serializer = new JsonSerializer();
        //        var secretsDAta = serializer.Deserialize(streamReader, typeof(SecretsData));
        //    }
        //}
    }

    //public class SecretsData
    //{
    //    public string ClientId { get; set; }
    //}
}
