using System;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr
{
    public class LidarrSettingsProvider
    {

        //TODO: Correct the Provider
        public LidarrSettings Provider()
        {
            dynamic settings = SettingsFile.Read();
            throw new NotImplementedException("Not implements");

            return new LidarrSettings
            {
                Hostname = "",
                Port = 8686,
                ApiKey = "",
                BaseUrl = "",
                SearchNewRequests = false,
                MonitorNewRequests = false,
                UseSSL = false,
                Version = "1"
            };
        }
    }
}
