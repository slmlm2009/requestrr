using DSharpPlus;
using DSharpPlus.Entities;
using Requestrr.WebApi.RequestrrBot.Locale;
using Requestrr.WebApi.RequestrrBot.Music;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;


namespace Requestrr.WebApi.RequestrrBot.ChatClients.Discord
{
    public class DiscordMusicUserInterface : IMusicUserInterface
    {
        private readonly DiscordInteraction _interationContext;
        private readonly IMusicSearcher _musicSearcher;

        public DiscordMusicUserInterface(
            DiscordInteraction interationContext,
            IMusicSearcher musicSearcher)
        {
            _interationContext = interationContext;
            _musicSearcher = musicSearcher;
        }


        public async Task ShowMusicSelection(MusicRequest request, IReadOnlyList<Music.Music> music)
        {
            List<DiscordSelectComponentOption> options = music.Take(15).Select(x => new DiscordSelectComponentOption(GetFormattedMusicArtistName(x), $"{request.CategoryId}/{x.ArtistId}")).ToList();
            DiscordSelectComponent select = new DiscordSelectComponent($"MuRS/A/{_interationContext.User.Id}/{request.CategoryId}", LimitStringSize("Select option"), options);
            ///FIX STRING

            await _interationContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddComponents(select).WithContent("Music xxxxx"));
            ///FIX STRING
        }



        public async Task DisplayMusicDetailsAsync(MusicRequest request, Music.Music music)
        {
            string message = "Example Message";
            ///FIX STRING

            DiscordButtonComponent requestButton = new DiscordButtonComponent(ButtonStyle.Primary, $"MuRS/{_interationContext.User.Id}/{request.CategoryId}/{music.ArtistId}", "Request Button");
            ///FIX STRING

            var builder = (await AddPreviousDropdownsAsync(music, new DiscordWebhookBuilder().AddEmbed(await GenerateMusicDetailsAsync(music, _musicSearcher)))).AddComponents(requestButton).WithContent(message);
            await _interationContext.EditOriginalResponseAsync(builder);
        }


        public async Task WarnMusicAlreadyAvailableAsync(Music.Music music)
        {
            var requestButton = new DiscordButtonComponent(ButtonStyle.Primary, $"???/{_interationContext.User.Id}/{music.ArtistId}", "Music Avalible", true);
            ///FIX STRING

            var builder = (await AddPreviousDropdownsAsync(music, new DiscordWebhookBuilder().AddEmbed(await GenerateMusicDetailsAsync(music, _musicSearcher)))).AddComponents(requestButton).WithContent("Music Already Available");
            ///FIX STRING

            await _interationContext.EditOriginalResponseAsync(builder);
        }


        public async Task WarnNoMusicFoundAsync(string musicName)
        {
            await _interationContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.DiscordCommandMovieNotFound)); //.ReplaceTokens(????)
        }



        public static async Task<DiscordEmbed> GenerateMusicDetailsAsync(Music.Music music, IMusicSearcher musicSearcher = null)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle($"{music.ArtistId}")
                .WithTimestamp(DateTime.Now)
                .WithUrl($"https://musicbrainz.org/artist/{music.ArtistId}")
                .WithFooter("Powered by Requestrr");

            if (!string.IsNullOrWhiteSpace(music.Overview))
                embedBuilder.WithDescription(music.Overview.Substring(0, Math.Min(music.Overview.Length, 255)) + "(...)");

            if (!string.IsNullOrWhiteSpace(music.PosterPath) && music.PosterPath.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                embedBuilder.WithImageUrl(music.PosterPath);

            if (!string.IsNullOrWhiteSpace(music.Quality))
                embedBuilder.AddField($"__{"xxxx"}__", $"{music.Quality}", true);

            if (musicSearcher != null)
            {
                //try
                //{
                //    var details = await musicSearcher.GetMusicDetails
                //}
            }

            if (!string.IsNullOrWhiteSpace(music.PlexUrl))
                embedBuilder.AddField($"__Plex__", $"[{"xxx"}]({music.PlexUrl})", true);
            ///FIX STRING

            if (!string.IsNullOrWhiteSpace(music.EmbyUrl))
                embedBuilder.AddField($"__Emby__", $"[{"XXX"}]({music.EmbyUrl})", true);
            ///FIX STRING

            return embedBuilder.Build();
        }



        private string GetFormattedMusicArtistName(Music.Music music)
        {
            return LimitStringSize(music.ArtistName);
        }


        private string LimitStringSize(string value, int limit = 100)
        {
            return value.Count() > limit ? value[..(limit - 3)] + "..." : value;
        }


        private async Task<DiscordWebhookBuilder> AddPreviousDropdownsAsync(Music.Music music, DiscordWebhookBuilder builder)
        {
            DiscordSelectComponent previousMusicSelector = (DiscordSelectComponent)(await _interationContext.GetOriginalResponseAsync()).Components.FirstOrDefault(x => x.Components.OfType<DiscordSelectComponent>().Any())?.Components?.Single();
            if (previousMusicSelector == null)
            {
                DiscordSelectComponent musicSelector = new DiscordSelectComponent(previousMusicSelector.CustomId, GetFormattedMusicArtistName(music), previousMusicSelector.Options);
                builder.AddComponents(musicSelector);
            }

            return builder;
        }
    }
}
