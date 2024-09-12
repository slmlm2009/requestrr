using Requestrr.WebApi.RequestrrBot.Movies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace Requestrr.WebApi.RequestrrBot.Notifications.Music
{
    public class MusicNotificationsRepository
    {
        private class Notification
        {
            public string UserId { get; }
            public string MusicArtistId { get; }
            public Notification(string userId, string musicArtistId)
            {
                UserId = userId;
                MusicArtistId = musicArtistId;
            }

            public override bool Equals(object obj)
            {
                return obj is Notification notification &&
                    UserId == notification.UserId &&
                    MusicArtistId == notification.MusicArtistId;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(UserId, MusicArtistId);
            }
        }


        private HashSet<Notification> _notifications = new HashSet<Notification>();
        private object _lock = new object();

        public MusicNotificationsRepository()
        {
            var musicNotifications = NotificationsFile.Read();
            //if (musicNotifications.Music == null)
            //    musicNotifications.Music = new NotificationsFile.NotificationClass.MusicNotification();

            //if (musicNotifications.Music.Artist == null)
            //    musicNotifications.Music.Artist = Array.Empty<NotificationsFile.NotificationClass.MusicArtistNotification>();


            foreach (var notification in musicNotifications.Music)
            {
                foreach (var musicId in notification.MusicArtistId)
                {
                    _notifications.Add(new Notification(notification.UserId.ToString(), (string)musicId));
                }
            }
        }



        public void AddNotification(string userId, string musicArtistId)
        {
            lock (_lock)
            {
                if (_notifications.Add(new Notification(userId, musicArtistId)))
                {
                    NotificationsFile.WriteMusicArtist(_notifications.GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.Select(y => y.MusicArtistId).ToArray()));
                }
            }
        }



        public void RemoveNotification(string userId, string musicArtistId)
        {
            lock (_lock)
            {
                if (_notifications.Remove(new Notification(userId, musicArtistId)))
                {
                    NotificationsFile.WriteMusicArtist(_notifications.GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.Select(y => y.MusicArtistId).ToArray()));
                }
            }
        }


        public Dictionary<string, HashSet<string>> GetAllMusicNotifications()
        {
            lock (_lock)
            {
                return _notifications
                    .GroupBy(x => x.MusicArtistId)
                    .ToDictionary(x => x.Key, x => new HashSet<string>(x.Select(y => y.UserId)));
            }
        }


        public bool HasNotification(string userId, string musicArtistId)
        {
            var hasRequest = false;

            lock (_lock)
            {
                hasRequest = _notifications.Contains(new Notification(userId, musicArtistId));
            }

            return hasRequest;
        }
    }
}