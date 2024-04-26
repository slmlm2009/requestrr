using Microsoft.Extensions.Logging;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Radarr;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr
{
    public class LidarrClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LidarrClient> _logger;
        private readonly LidarrSettingsProvider _settingsProvider;


        public LidarrClient(IHttpClientFactory httpClientFactory, ILogger<LidarrClient> logger, LidarrSettingsProvider settingsProvider)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _settingsProvider = settingsProvider;
        }


        /// <summary>
        /// Used to test a connection to Lidarr
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Task TestConnectionAsync(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            return LidarrClientV1.TestConnectionAsync(httpClient, logger, settings);
        }



        /// <summary>
        /// Returns all paths setup in Lidarr where media is stored
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <returns>Returns a list of JSONRootPath objects</returns>
        public static Task<IList<JSONRootPath>> GetRootPaths(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            return LidarrClientV1.GetRootPaths(httpClient, logger, settings);
        }



        /// <summary>
        /// Returns all profiles setup in Lidarr
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <returns>Returns a list of JSONProfile objects</returns>
        public static Task<IList<JSONProfile>> GetProfiles(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            return LidarrClientV1.GetProfiles(httpClient, logger, settings);
        }



        /// <summary>
        /// Returns all tags setup in Lidarr
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// <returns>Returns a list of JSONTag objects</returns>
        public static Task<IList<JSONTag>> GetTags(HttpClient httpClient, ILogger<LidarrClient> logger, LidarrSettings settings)
        {
            return LidarrClientV1.GetTags(httpClient, logger, settings);
        }



        //-----------------------------


        //public Task<Movie> SearchMovieAsync(MovieRequest request, int theMovieDbId)
        //{
        //    return CreateInstance<IMovieSearcher>().SearchMovieAsync(request, theMovieDbId);
        //}

        //public Task<IReadOnlyList<Movie>> SearchMovieAsync(MovieRequest request, string movieName)
        //{
        //    return CreateInstance<IMovieSearcher>().SearchMovieAsync(request, movieName);
        //}

        //public Task<MovieDetails> GetMovieDetails(MovieRequest request, string theMovieDbId)
        //{
        //    return CreateInstance<IMovieSearcher>().GetMovieDetails(request, theMovieDbId);
        //}

        //public Task<Dictionary<int, Movie>> SearchAvailableMoviesAsync(HashSet<int> theMovieDbIds, System.Threading.CancellationToken token)
        //{
        //    return CreateInstance<IMovieSearcher>().SearchAvailableMoviesAsync(theMovieDbIds, token);
        //}

        //public Task<MovieRequestResult> RequestMovieAsync(MovieRequest request, Movie movie)
        //{
        //    return CreateInstance<IMovieRequester>().RequestMovieAsync(request, movie);
        //}

        //private T CreateInstance<T>() where T : class
        //{
        //    if (_settingsProvider.Provide().Version == "2")
        //    {
        //        return new RadarrClientV2(_httpClientFactory, _logger, _settingsProvider) as T;
        //    }
        //    else
        //    {
        //        return new RadarrClientV3(_httpClientFactory, _logger, _settingsProvider) as T;
        //    }
        //}

        //-----------------------------


        public class JSONRootPath
        {
            public string path { get; set; }
            public int id { get; set; }
        }

        public class JSONProfile
        {
            public string name { get; set; }
            public int id { get; set; }
        }

        public class JSONTag
        {
            public string label { get; set; }
            public int id { get; set; }
        }
    }
}
