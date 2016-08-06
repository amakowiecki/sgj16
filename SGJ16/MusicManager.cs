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

        public static void Load(ContentManager content)
        {
            songs = new List<Song>();
            var newSong = content.Load<Song>("muzykawalki");
            songs.Add(newSong);
        }

        public static void Play()
        {
            currentSongIdx = 0;
            playNext();
        }

        public static void Update(GameTime gametime) //na razie nie wywoływane bo jeden utwór
        {
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
           
            MediaPlayer.IsRepeating = true; //bo jedna
            currentSongIdx++;
            if (currentSongIdx >= songs.Count)
            {
                currentSongIdx = 0;
            }
        }
    }
}
