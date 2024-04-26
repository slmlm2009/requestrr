using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Radarr;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr.LidarrClient;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr
{
    public class LidarrClientV1
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LidarrClient> _logger;
        private LidarrSettingsProvider _lidarrSettingProvider;
        private LidarrSettings _lidarrSettings => _lidarrSettingProvider.Provider();

        private string BaseURL => GetBaseURL(_lidarrSettings);


        public LidarrClientV1(IHttpClientFactory httpClientFactory, ILogger<LidarrClient> logger, LidarrSettingsProvider lidarrSettingsProvider)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _lidarrSettingProvider = lidarrSettingsProvider;
        }



        /// <summary>
        /// Used to test if Lidarr service can be found
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task TestConnectionAsync(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.BaseUrl) && !settings.BaseUrl.StartsWith("/"))
            {
                throw new Exception("Invalid base URL, must start with /");
            }

            var testSuccessful = false;

            try
            {
                var response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/config/host");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Invalid api key");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Incorrect api version");
                }

                try
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JObject.Parse(responseString);

                    if (!jsonResponse.urlBase.ToString().Equals(settings.BaseUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception("Base url does not match what is set in Lidarr");
                    }
                }
                catch
                {
                    throw new Exception("Base url does not match what is set in Lidarr");
                }

                testSuccessful = true;
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Error while testing Lidarr connection: " + ex.Message);
                throw new Exception("Invalid host and/or port");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error while testing Lidarr connection: " + ex.Message);

                if (ex.GetType() == typeof(Exception))
                {
                    throw;
                }
                else
                {
                    throw new Exception("Invalid host and/or port");
                }
            }

            if (!testSuccessful)
            {
                throw new Exception("Invalid host and/or port");
            }
        }


        public static async Task<IList<JSONRootPath>> GetRootPaths(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            throw new NotImplementedException();

        }


        public static async Task<IList<JSONProfile>> GetProfiles(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            throw new NotImplementedException();
        }

        public static async Task<IList<JSONTag>> GetTags(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Makes a connection to Lidarr and returns a response from API
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="url">Full URL to the API</param>
        /// <returns>Returns the HttpReponseMessage from the API</returns>
        private static async Task<HttpResponseMessage> HttpGetAsync(HttpClient client, LidarrSettings settings, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-Api-Key", settings.ApiKey);

            using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5)))
            {
                return await client.SendAsync(request, cts.Token);
            }
        }


        /// <summary>
        /// Gets Base URL for Lidarr server
        /// </summary>
        /// <param name="settings">Lidarr Settings</param>
        /// <returns>Returns a string of the URL</returns>
        private static string GetBaseURL(LidarrSettings settings)
        {
            var protocol = settings.UseSSL ? "https" : "http";

            return $"{protocol}://{settings.Hostname}:{settings.Port}{settings.BaseUrl}/api/v{settings.Version}";
        }
    }
}
