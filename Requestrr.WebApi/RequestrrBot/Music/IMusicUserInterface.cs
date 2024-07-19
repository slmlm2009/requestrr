using System.Collections.Generic;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public interface IMusicUserInterface
    {
        Task ShowMusicSelection(MusicRequest request, IReadOnlyList<MusicArtist> music);
        Task WarnNoMusicFoundAsync(string musicName);

        Task DisplayMusicDetailsAsync(MusicRequest request, MusicArtist music);
        Task DisplayRequestDeniedAsync(MusicArtist music);
        Task DisplayRequestSuccessAsync(MusicArtist music);

        Task WarnMusicAlreadyAvailableAsync(MusicArtist music);
    }
}
