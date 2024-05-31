using Requestrr.WebApi.RequestrrBot.Movies;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public class DisabledMusicNotificationWorkflow : IMusicNotificationWorkflow
    {
        private readonly IMusicUserInterface _userInterface;

        public DisabledMusicNotificationWorkflow(IMusicUserInterface userInterface)
        {
            _userInterface = userInterface;
        }
    }
}
