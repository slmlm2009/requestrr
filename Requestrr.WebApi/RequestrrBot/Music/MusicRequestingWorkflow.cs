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


        public async Task SearchMusicAsync(string query)
        {
            var music = await SearchMusicsAsync(query);
        }

        public async Task<IReadOnlyList<Music>> SearchMusicsAsync(string query)
        {
            IReadOnlyList<Music> music = Array.Empty<Music>();

            query = query.Replace(".", " ");
            music = await _musicSearcher.SearchMusicAsync(new MusicRequest(_user, _categoryId), query);

            if (!music.Any())
                await _userInterface.WarnNoMusicFoundAsync(query);

            return music;
        }
    }
}
