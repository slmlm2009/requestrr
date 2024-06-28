using System.Collections.Generic;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public interface IMusicUserInterface
    {
        Task ShowMusicSelection(MusicRequest request, IReadOnlyList<Music> music);
        Task WarnNoMusicFoundAsync(string musicName);

        Task DisplayMusicDetailsAsync(MusicRequest request, Music music);
        Task DisplayRequestDeniedAsync(Music music);
        Task DisplayRequestSuccessAsync(Music music);

        Task WarnMusicAlreadyAvailableAsync(Music music);
    }
}
