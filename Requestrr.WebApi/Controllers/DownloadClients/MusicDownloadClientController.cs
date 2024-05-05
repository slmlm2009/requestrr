using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Requestrr.WebApi.config;
using Requestrr.WebApi.Controllers.DownloadClients.Lidarr;
using Requestrr.WebApi.RequestrrBot.DownloadClients;
using Requestrr.WebApi.RequestrrBot.Music;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Requestrr.WebApi.Controllers.DownloadClients
{
    [ApiController]
    [Authorize]
    [Route("/api/music")]
    public class MusicDownloadClientController : ControllerBase
    {
        private readonly MusicSettings _musicSettings;
        private readonly DownloadClientsSettings _downloadClientsSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public MusicDownloadClientController(
            IHttpClientFactory httpClientFactory,
            MusicSettingsProvider musicSettingsProvider,
            DownloadClientsSettingsProvider downloadClientsSettingsProvider )
        {
            _httpClientFactory = httpClientFactory;
            _musicSettings = musicSettingsProvider.Provide();
            _downloadClientsSettings = downloadClientsSettingsProvider.Provide();
        }


        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(new MusicSettingsModel
            {
                Client = _musicSettings.Client,
                Lidarr = new LidarrSettingsModel
                {
                    Hostname = _downloadClientsSettings.Lidarr.Hostname,
                    BaseUrl = _downloadClientsSettings.Lidarr.BaseUrl,
                    Port = _downloadClientsSettings.Lidarr.Port,
                    ApiKey = _downloadClientsSettings.Lidarr.ApiKey,
                    Categories = _downloadClientsSettings.Lidarr.Categories.Select(x => new LidarrSettingsCategory
                    {
                        Id = x.Id,
                        Name = x.Name,
                        MinimumAvailability = x.MinimumAvailability,
                        ProfileId = x.ProfileId,
                        RootFolder = x.RootFolder,
                        Tags = x.Tags
                    }).ToArray(),
                    UseSSL = _downloadClientsSettings.Lidarr.UseSSL,
                    SearchNewRequests = _downloadClientsSettings.Lidarr.SearchNewRequests,
                    MonitorNewRequests = _downloadClientsSettings.Lidarr.MonitorNewRequests,
                    Version = _downloadClientsSettings.Lidarr.Version
                }
            });
        }


        [HttpPost("disable")]
        public async Task<IActionResult> SaveAsync()
        {
            _musicSettings.Client = DownloadClient.Disabled;
            DownloadClientsSettingsRepository.SetDisabledClient(_musicSettings);
            return Ok(new { ok = true });
        }
    }
}
