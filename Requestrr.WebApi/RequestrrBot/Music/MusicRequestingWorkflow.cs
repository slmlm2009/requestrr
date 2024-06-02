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
            var music = await SearchMusicForArtistListAsync(artistName);
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
    }
}
