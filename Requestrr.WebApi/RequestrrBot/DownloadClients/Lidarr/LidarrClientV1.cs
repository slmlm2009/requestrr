using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Requestrr.WebApi.Extensions;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Radarr;
using Requestrr.WebApi.RequestrrBot.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr.LidarrClient;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr
{
    public class LidarrClientV1 : IMusicSearcher
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
            try
            {
                HttpResponseMessage response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/rootfolder");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<JSONRootPath>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error while getting Lidarr root paths: " + ex.Message);
            }

            throw new Exception("An error occurred while getting Lidarr root paths");
        }


        public static async Task<IList<JSONProfile>> GetProfiles(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            try
            {
                HttpResponseMessage response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/qualityprofile");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<JSONProfile>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error while getting Lidarr profiles: " + ex.Message);
            }

            throw new Exception("An error occurred while getting Lidarr profiles");
        }

        public static async Task<IList<JSONTag>> GetTags(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            try
            {
                HttpResponseMessage response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/tag");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<JSONTag>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error while getting Lidarr tags: " + ex.Message);
            }

            throw new Exception("An error occurred while getting Lidarr tags");
        }


        /// <summary>
        /// Handle 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Task<HttpResponseMessage> HttpGetAsync(string url)
        {
            return HttpGetAsync(_httpClientFactory.CreateClient(), _lidarrSettings, url);
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

        public Task<IReadOnlyList<Music.Music>> SearchMusicAsyc(MusicRequest request, string musicName)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Handles the fetching of a single query based on Music DB Id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Music.Music> SearchMusicForArtistIdAsync(MusicRequest request, string artistId)
        {
            try
            {
                JSONMusic foundMusicJson = await FindExistingArtistByMusicDbIdAsync(artistId);

                if (foundMusicJson == null)
                {
                    HttpResponseMessage response = await HttpGetAsync($"{BaseURL}/artist/lookup?term=lidarr:{artistId}");
                    await response.ThrowIfNotSuccessfulAsync("LidarrMusicLookup failed", x => x.error);

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    foundMusicJson = JsonConvert.DeserializeObject<JSONMusic>(jsonResponse);
                }

                return foundMusicJson != null ? ConvertToMusic(foundMusicJson) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while searching for music by Id \"{artistId}\" with Lidarr: {ex.Message}");
            }

            throw new Exception("An error occurred while searching for music by Id with Lidarr");
        }



        /// <summary>
        /// Handles the fetching of a 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IReadOnlyList<Music.Music>> SearchMusicForArtistAsync(MusicRequest request, string artistName)
        {
            try
            {
                string searchTerm = Uri.EscapeDataString(artistName.ToLower().Trim());
                HttpResponseMessage response = await HttpGetAsync($"{BaseURL}/artist/lookup?term={searchTerm}");
                await response.ThrowIfNotSuccessfulAsync("LidarrMusicLookup failed", x => x.error);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                List<JSONMusic> jsonMusic = JsonConvert.DeserializeObject<List<JSONMusic>>(jsonResponse);

                //TODO: Correct this, searching should handle both artist and albums
                return jsonMusic.Where(x => x != null).Select(x => ConvertToMusic(x)).ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for music with Lidarr: " + ex.Message);
            }

            throw new Exception("An error occurred while searching for music with Lidarr");
        }



        private async Task<JSONMusic> FindExistingArtistByMusicDbIdAsync(string artistId)
        {
            try
            {
                HttpResponseMessage response = await HttpGetAsync($"{BaseURL}/artist?mbId={artistId}");
                await response.ThrowIfNotSuccessfulAsync("Could not search music by Id", x => x.error);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JSONMusic[] jsonMusicList = JsonConvert.DeserializeObject<List<JSONMusic>>(jsonResponse).ToArray();

                if (jsonMusicList.Any())
                    return jsonMusicList.First();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred finding existing music by Id \"{artistId}\" with Lidarr: {ex.Message}");
            }

            return null;
        }



        //private Music.Music ConvertToMusic(JSONMusicSearch jsonMusic)
        //{
        //    string downloadClientId = jsonMusic.Id.ToString();

        //    return new Music.Music
        //    {
        //        DownloadClientId = downloadClientId,
        //        ArtistId = jsonMusic.Artist.ArtistMetadataId.ToString(),
        //        ArtistName = jsonMusic.Artist.ArtistName,
        //        Overview = jsonMusic.Artist.Overview,

        //        Available = !string.IsNullOrWhiteSpace(jsonMusic.Artist.Folder),
        //        Quality = string.Empty,
        //        Requested = (!string.IsNullOrWhiteSpace(downloadClientId)),

        //        PlexUrl = string.Empty,
        //        EmbyUrl = string.Empty,
        //        PosterPath = GetPosterImageUrl(jsonMusic.Artist.Images)
        //        //ReleaseDate = jsonMusic..Empty
        //    };
        //}


        private Music.Music ConvertToMusic(JSONMusic jsonMusic)
        {
            string downloadClientId = jsonMusic.Id.ToString();

            return new Music.Music
            {
                DownloadClientId = downloadClientId,
                ArtistId = jsonMusic.ArtistMetadataId.ToString(),
                ArtistName = jsonMusic.ArtistName,
                Overview = jsonMusic.Overview,

                Available = !string.IsNullOrWhiteSpace(jsonMusic.Folder),
                Quality = string.Empty,
                Requested = (!string.IsNullOrWhiteSpace(downloadClientId)),

                PlexUrl = string.Empty,
                EmbyUrl = string.Empty,
                PosterPath = GetPosterImageUrl(jsonMusic.Images)
                //ReleaseDate = jsonMusic..Empty
            };
        }


        private string GetPosterImageUrl(List<JSONImage> images)
        {
            JSONImage posterImage = images.Where(x => x.CoverType.Equals("poster", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (posterImage != null)
            {
                if (!string.IsNullOrWhiteSpace(posterImage.RemoteUrl))
                    return posterImage.RemoteUrl;

                return posterImage.Url;
            }
            return string.Empty;
        }



        public class JSONLink
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class JSONImage
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("coverType")]
            public string CoverType { get; set; }

            [JsonProperty("extension")]
            public string Extension { get; set; }

            [JsonProperty("remoteUrl")]
            public string RemoteUrl { get; set; }
        }

        public class JSONRating
        {
            [JsonProperty("votes")]
            public int Votes { get; set; }

            [JsonProperty("value")]
            public float Value { get; set; }
        }

        public class JSONStatistics
        {
            [JsonProperty("albumCount")]
            public int AlbumCount { get; set; }

            [JsonProperty("trackFileCount")]
            public int TrackFileCount { get; set; }

            [JsonProperty("trackCount")]
            public int TrackCount { get; set; }

            [JsonProperty("totalTrackCount")]
            public int TotalTrackCount { get; set; }

            [JsonProperty("sizeOnDisk")]
            public int SizeOnDisk { get; set; }

            [JsonProperty("percentOfTracks")]
            public int PercentOfTracks { get; set; }
        }


        public class JSONAlbum
        {

        }


        //private class JSONMusicSearch
        //{
        //    [JsonProperty("id")]
        //    public int Id { get; set; }

        //    [JsonProperty("foreignId")]
        //    public string ForeignId { get; set; }

        //    [JsonProperty("artist")]
        //    public JSONMusic? Artist { get; set; }
        //}


        private class JSONMusic
        {
            [JsonProperty("id")]
            public int? Id { get; set; }

            [JsonProperty("artistMetadataId")]
            public int? ArtistMetadataId { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("ended")]
            public bool Ended { get; set; }

            [JsonProperty("artistName")]
            public string ArtistName { get; set; }

            [JsonProperty("foreignArtistId")]
            public Guid ForeignArtistId { get; set; }

            [JsonProperty("tadbId")]
            public int TadbId { get; set; }

            [JsonProperty("discogsId")]
            public int DiscogsId { get; set; }

            [JsonProperty("overview")]
            public string Overview { get; set; }

            [JsonProperty("artistType")]
            public string ArtistType { get; set; }

            [JsonProperty("disambiguation")]
            public string Disambiguation { get; set; }

            [JsonProperty("links")]
            public List<JSONLink> Links { get; set; }

            [JsonProperty("images")]
            public List<JSONImage> Images { get; set; }

            [JsonProperty("qualityProfileId")]
            public int QualityProfileId { get; set; }

            [JsonProperty("metadataProfileId")]
            public int MetadataProfileId { get; set; }

            [JsonProperty("monitored")]
            public bool Monitored { get; set; }

            [JsonProperty("monitorNewItems")]
            public string MonitorNewItems { get; set; }

            [JsonProperty("folder")]
            public string Folder { get; set; }

            [JsonProperty("genres")]
            public List<string> Genres { get; set; }

            [JsonProperty("tags")]
            public List<int> Tags { get; set; }

            [JsonProperty("added")]
            public DateTime Added { get; set; }

            [JsonProperty("ratings")]
            public JSONRating Ratings { get; set; }

            [JsonProperty("statistics")]
            public JSONStatistics Statistics { get; set; }
        }
    }
}
