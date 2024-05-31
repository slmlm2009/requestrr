using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public interface IMusicUserInterface
    {
        Task WarnNoMusicFoundAsync(string musicName);
    }
}
