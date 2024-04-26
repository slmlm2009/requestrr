
namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr
{
    public class LidarrSettings
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        //public RadarrCategory[] Categories { get; set; } = Array.Empty<RadarrCategory>();
        public bool SearchNewRequests { get; set; }
        public bool MonitorNewRequests { get; set; }
        public bool UseSSL { get; set; }
        public string Version { get; set; }
    }
}
