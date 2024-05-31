using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Music
{
    public class MusicNotificationWorkflow : IMusicNotificationWorkflow
    {
        //private readonly MusicNotificationsRepository _notificationsRepository;
        private readonly IMusicUserInterface _userInterface;
        private readonly IMusicSearcher _musicSearcher;
        private readonly bool _automaticNotificationForNewRequests;


        public MusicNotificationWorkflow(
            //MusicNotificationsRepository musicNotificationsRepository,
            IMusicUserInterface userInterface,
            IMusicSearcher musicSearcher,
            bool automaticNotificationForNewRequests
        )
        {
            //_notificationsRepository = musicNotificationsRepository;
            _userInterface = userInterface;
            _musicSearcher = musicSearcher;
            _automaticNotificationForNewRequests = automaticNotificationForNewRequests;
        }


        //TODO: Add and setup funtions
    }
}
