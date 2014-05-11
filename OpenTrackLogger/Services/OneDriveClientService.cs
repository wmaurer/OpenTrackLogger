namespace OpenTrackLogger.Services
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Live;

    using OpenTrackLogger.Secrets;

    public interface IOneDriveClientService
    {
        Task<LiveConnectClient> GetClient();
    }

    public class OneDriveClientService : IOneDriveClientService
    {
        private LiveConnectClient _client;

        public async Task<LiveConnectClient> GetClient()
        {
            if (_client != null) return _client;

            var scopes = new[] { "wl.signin", "wl.skydrive_update" };
            try {
                var loginResult = await new LiveAuthClient(Secrets.Data.ClientId).InitializeAsync(scopes);
                if (loginResult.Status != LiveConnectSessionStatus.Connected) {
                    loginResult = await new LiveAuthClient(Secrets.Data.ClientId).LoginAsync(scopes);
                    if (loginResult.Status != LiveConnectSessionStatus.Connected) {
                        throw new Exception("Failed to Connect");
                    }
                }
                _client = new LiveConnectClient(loginResult.Session) { BackgroundTransferPreferences = BackgroundTransferPreferences.AllowBattery };
            }
            catch (Exception ex) {
                var m = ex.Message;
                throw;
            }

            return _client;
        }
    }
}
