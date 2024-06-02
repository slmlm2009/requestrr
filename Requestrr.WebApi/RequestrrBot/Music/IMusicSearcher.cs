using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public interface IMusicSearcher
    {
        Task<IReadOnlyList<Music>> SearchMusicForArtistAsync(MusicRequest request, string artistName);


        Task<Music> SearchMusicForArtistIdAsync(MusicRequest request, string artistId);
    }
}
