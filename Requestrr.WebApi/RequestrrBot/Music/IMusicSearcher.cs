using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public interface IMusicSearcher
    {
        Task<IReadOnlyList<MusicArtist>> SearchMusicForArtistAsync(MusicRequest request, string artistName);
        Task<MusicArtist> SearchMusicForArtistIdAsync(MusicRequest request, string artistId);

        //Task<MusicDetails> GetMusicArtistDetails(MusicRequest request, string artistId);
    }

    //public class MusicDetails
    //{
    //    public string 
    //}
}
