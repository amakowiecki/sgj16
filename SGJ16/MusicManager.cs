using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public static class MusicManager
    {
        static List<Song> songs;
        static int currentSongIdx;
        static int timePlaying;
        static int currentSongDuration;
        static Song menuSong;
        static bool menu;

        public static void Load(ContentManager content)
        {
            menuSong = content.Load<Song>("menumusic");
            songs = new List<Song>();
            //var newSong = content.Load<Song>("muzykawalki");
            var newSong = content.Load<Song>("sabrepulse/1");
            songs.Add(newSong);
            newSong = content.Load<Song>("sabrepulse/2");
            songs.Add(newSong);
            newSong = content.Load<Song>("sabrepulse/3");
            songs.Add(newSong);
            newSong = content.Load<Song>("sabrepulse/4");
            songs.Add(newSong);
            newSong = content.Load<Song>("sabrepulse/5");
            songs.Add(newSong);
        }

        public static void PlayMusicMenu()
        {
            MediaPlayer.IsRepeating = true;
            menu = true;
            MediaPlayer.Play(menuSong);
        }

        public static void Play()
        {
            MediaPlayer.Stop();
            menu = false;
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Volume = 0.3f;
            currentSongIdx = 0;
            playNext();
        }

        public static void Update(GameTime gametime) 
        {
            if (menu)
            {
                return;
            }
            timePlaying += (int) gametime.ElapsedGameTime.TotalSeconds;

            if (currentSongDuration <= timePlaying)
            {
                playNext();
            }
        }

        private static void playNext()
        {
            timePlaying = 0;
            currentSongDuration = (int)songs[currentSongIdx].Duration.TotalSeconds;
            MediaPlayer.Play(songs[currentSongIdx]);
           
           // MediaPlayer.IsRepeating = true; 
            currentSongIdx++;
            if (currentSongIdx >= songs.Count)
            {
                currentSongIdx = 0;
            }
        }
    }
}
