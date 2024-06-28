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
        private readonly IMusicRequester _requester;
        private readonly IMusicUserInterface _userInterface;
        //private readonly IMusicNotificationWorkflow _notificationWorkflow;


        public MusicRequestingWorkflow(
            MusicUserRequester user,
            int categoryId,
            IMusicSearcher searcher,
            IMusicRequester requester,
            IMusicUserInterface userInterface
        )
        {
            _categoryId = categoryId;
            _user = user;
            _musicSearcher = searcher;
            _requester = requester;
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


        public async Task HandleMusicArtistSelectionAsync(string musicId)
        {
            await HandleMusicSelectionAsync(await _musicSearcher.SearchMusicForArtistIdAsync(new MusicRequest(_user, _categoryId), musicId));
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


        /// <summary>
        /// Handles the request for an artist
        /// </summary>
        /// <param name="artistId"></param>
        /// <returns></returns>
        public async Task RequestMusicArtistAsync(string artistId)
        {
            Music music = await _musicSearcher.SearchMusicForArtistIdAsync(new MusicRequest(_user, _categoryId), artistId);
            MusicRequestResult result = await _requester.RequestMusicAsync(new MusicRequest(_user, _categoryId), music);

            if (result.WasDenied)
            {
                await _userInterface.DisplayRequestDeniedAsync(music);
            }
            else
            {
                await _userInterface.DisplayRequestSuccessAsync(music);
                //await _not
            }
        }



        private static bool CanBeRequested(Music music)
        {
            return !music.Available && !music.Requested;
        }
    }
}
