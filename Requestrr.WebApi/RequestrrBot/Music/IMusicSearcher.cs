using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public interface IMusicSearcher
    {
        Task<IReadOnlyList<Music>> SearchMusicAsync(MusicRequest request, string query);


        Task<Music> SearchMusicAsync(MusicRequest request, Guid guid);
    }
}
