using DSharpPlus.Entities;
using Requestrr.WebApi.RequestrrBot.Locale;
using Requestrr.WebApi.RequestrrBot.Music;
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


        public async Task WarnNoMusicFoundAsync(string musicName)
        {
            await _interationContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.DiscordCommandMovieNotFound)); //.ReplaceTokens(????)
        }
    }
}
