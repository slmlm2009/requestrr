using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Requestrr.WebApi.Controllers.DownloadClients.Lidarr
{
    [ApiController]
    [Authorize]
    [Route("/api/music/lidarr")]
    public class LidarrClientController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LidarrClient> _logger;

        public LidarrClientController(IHttpClientFactory httpClientFactory, ILogger<LidarrClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }


        [HttpPost("test")]
        public async Task<IActionResult> TestLidarrSettings([FromBody] TestLidarrSettingsModel model)
        {
            try
            {
                await LidarrClient.TestConnectionAsync(_httpClientFactory.CreateClient(), _logger, ConvertToLidarrSettings(model));
                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private static LidarrSettings ConvertToLidarrSettings(TestLidarrSettingsModel model)
        {
            return new LidarrSettings
            {
                ApiKey = model.ApiKey,
                Hostname = model.Hostname,
                BaseUrl = model.BaseUrl,
                Port = model.Port,
                UseSSL = model.UseSSL,
                Version = model.Version
            };
        }
    }
}
