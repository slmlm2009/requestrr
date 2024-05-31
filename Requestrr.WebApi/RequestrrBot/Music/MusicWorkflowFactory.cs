using DSharpPlus.Entities;
using Requestrr.WebApi.RequestrrBot.ChatClients.Discord;
using Requestrr.WebApi.RequestrrBot.DownloadClients;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr;
using Requestrr.WebApi.RequestrrBot.Movies;
using Requestrr.WebApi.RequestrrBot.Notifications;
using System;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public class MusicWorkflowFactory
    {
        private readonly DiscordSettingsProvider _settingsProvider;
        //private readonly MusicNotificationsRepository _notificationsRepository;
        private LidarrClient _lidarrClient;


        public MusicWorkflowFactory(
            DiscordSettingsProvider settingsProvider,
            LidarrClient lidarrClient
        )
        {
            _settingsProvider = settingsProvider;
            _lidarrClient = lidarrClient;
        }


        public MusicRequestingWorkflow CreateRequestingWorkflow(DiscordInteraction interation, int categoryId)
        {
            DiscordSettings settings = _settingsProvider.Provide();
            return new MusicRequestingWorkflow(
                new MusicUserRequester(
                    interation.User.Id.ToString(),
                    interation.User.Username
                    ),
                categoryId,
                GetMusicClient<IMusicSearcher>(settings),
                new DiscordMusicUserInterface(interation, GetMusicClient<IMusicSearcher>(settings))
                //CreateMusicNotificationWorkflow(interation, settings)
                );
        }



        private IMusicNotificationWorkflow CreateMusicNotificationWorkflow(DiscordInteraction interaction, DiscordSettings settings)
        {
            DiscordMusicUserInterface userInterface = new DiscordMusicUserInterface(interaction, GetMusicClient<IMusicSearcher>(settings));
            IMusicNotificationWorkflow musicNotificationWorkflow = new DisabledMusicNotificationWorkflow(userInterface);

            if (settings.NotificationMode != NotificationMode.Disabled)
                musicNotificationWorkflow = new MusicNotificationWorkflow(userInterface, GetMusicClient<IMusicSearcher>(settings), settings.AutomaticallyNotifyRequesters);

            return musicNotificationWorkflow;
        }



        private T GetMusicClient<T>(DiscordSettings settings) where T : class
        {
            if (settings.MusicDownloadClient == DownloadClient.Lidarr)
            {
                return _lidarrClient as T;
            }

            throw new Exception($"Invalid configured music download client {settings.MusicDownloadClient}");
        }
    }
}
