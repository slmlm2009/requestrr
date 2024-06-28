using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public interface IMusicRequester
    {
        Task<MusicRequestResult> RequestMusicAsync(MusicRequest request, Music music);
    }


    public class MusicRequestResult
    {
        public bool WasDenied { get; set; }
    }
}
