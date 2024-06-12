using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public class MusicRequestingWorkflow
    {
        private readonly int _categoryId;
        private readonly MusicUserRequester _user;
        private readonly IMusicSearcher _musicSearcher;
        //private readonly IMusicRequester
        private readonly IMusicUserInterface _userInterface;
        //private readonly IMusicNotificationWorkflow _notificationWorkflow;


        public MusicRequestingWorkflow(
            MusicUserRequester user,
            int categoryId,
            IMusicSearcher searcher,
            IMusicUserInterface userInterface
        )
        {
            _categoryId = categoryId;
            _user = user;
            _musicSearcher = searcher;
            _userInterface = userInterface;
        }


        public async Task SearchMusicForArtistAsync(string artistName)
        {
            IReadOnlyList<Music> musicList = await SearchMusicForArtistListAsync(artistName);

            if (musicList.Any())
            {
                if (musicList.Count > 1)
                {
                    await _userInterface.ShowMusicSelection(new MusicRequest(_user, _categoryId), musicList);
                }
                else if (musicList.Count == 1)
                {
                    Music music = musicList.Single();
                    await HandleMusicSelectionAsync(music);
                }
            }
        }

        public async Task<IReadOnlyList<Music>> SearchMusicForArtistListAsync(string artistName)
        {
            IReadOnlyList<Music> music = Array.Empty<Music>();

            artistName = artistName.Replace(".", " ");
            music = await _musicSearcher.SearchMusicForArtistAsync(new MusicRequest(_user, _categoryId), artistName);

            if (!music.Any())
                await _userInterface.WarnNoMusicFoundAsync(artistName);

            return music;
        }


        private async Task HandleMusicSelectionAsync(Music music)
        {
            if (CanBeRequested(music))
            {
                await _userInterface.DisplayMusicDetailsAsync(new MusicRequest(_user, _categoryId), music);
            }
            else
            {
                if (music.Available)
                {
                    await _userInterface.WarnMusicAlreadyAvailableAsync(music);
                }
                else
                {
                    //await _notificationWorkflow.NotifyForExistingRequestAsync();
                }
            }
        }


        private static bool CanBeRequested(Music music)
        {
            return !music.Available && !music.Requested;
        }
    }
}
