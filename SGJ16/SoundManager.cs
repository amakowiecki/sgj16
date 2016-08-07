using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class SoundManager
    {
        static List<SoundEffect> deathSounds;
        static List<SoundEffect> jumpSounds;
        static List<SoundEffect> landSounds;
        static List<SoundEffect> shotSounds;
        static List<SoundEffect> superShotSounds;
        static List<SoundEffect> hurtSounds;

        static SoundEffect pojebalo;

        static Random RNG;

        public static void Load(ContentManager content)
        {
            RNG = new Random();
            deathSounds = new List<SoundEffect>();
            deathSounds.Add(content.Load<SoundEffect>("death1"));
            deathSounds.Add(content.Load<SoundEffect>("death2"));

            jumpSounds = new List<SoundEffect>();
            jumpSounds.Add(content.Load<SoundEffect>("jump1"));
            jumpSounds.Add(content.Load<SoundEffect>("jump2"));

            landSounds = new List<SoundEffect>();
            landSounds.Add(content.Load<SoundEffect>("jumpland1"));
            landSounds.Add(content.Load<SoundEffect>("jumpland2"));
            landSounds.Add(content.Load<SoundEffect>("jumpland3"));

            shotSounds = new List<SoundEffect>();
            shotSounds.Add(content.Load<SoundEffect>("shot1"));
            shotSounds.Add(content.Load<SoundEffect>("shot2"));

            superShotSounds = new List<SoundEffect>();
            superShotSounds.Add(content.Load<SoundEffect>("supershot"));

            hurtSounds = new List<SoundEffect>();
            hurtSounds.Add(content.Load<SoundEffect>("ouch1"));
            hurtSounds.Add(content.Load<SoundEffect>("ouch2"));
            //hurtSounds.Add(content.Load<SoundEffect>("tykurwo"));
            hurtSounds.Add(content.Load<SoundEffect>("hurt (1)"));
            hurtSounds.Add(content.Load<SoundEffect>("hurt (2)"));
            hurtSounds.Add(content.Load<SoundEffect>("hurt (3)"));

            pojebalo = content.Load<SoundEffect>("pojebalocie");

        }

        private static void playRandomSound(List<SoundEffect> soundList, float volume = 1.0f)
        {
            int i = RNG.Next(soundList.Count);
            soundList[i].Play(volume, 0.0f, 0.0f);
        }

        public static void PlayDeath()
        {
            playRandomSound(deathSounds, 1);
        }

        public static void PlayJump()
        {
            playRandomSound(jumpSounds, 1);
        }

        public static void PlayLand()
        {
            playRandomSound(landSounds, 1);
        }

        public static void PlayShot()
        {
            playRandomSound(shotSounds, 0.7f);
        }

        public static void PlaySuperShot()
        {
            playRandomSound(superShotSounds, 1.0f);
        }

        public static void PlayHurt()
        {
            if (RNG.Next(20)==4)
            {
                pojebalo.Play();
            }
            else
            {
                playRandomSound(hurtSounds, 1.0f);
            }
        }

    }
}
