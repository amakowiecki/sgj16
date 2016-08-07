using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public delegate void PowerUpEffect(Player player);
    public class PowerUpModel
    {
        public Texture2D Texture;
        public PowerUpEffect Effect;
        public SoundEffect Sound;

        public PowerUpModel(Texture2D texture, PowerUpEffect effect, SoundEffect sound)
        {
            Texture = texture;
            Effect = effect;
            Sound = sound;
        }
    }
    public class PowerUp : IDisplayable
    {
        PowerUpModel model;
        public Rectangle rectangle;
        public PowerUp(PowerUpModel model)
        {
            this.model = model;
            this.rectangle = new Rectangle(0, 0, model.Texture.Width, model.Texture.Height);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(model.Texture, rectangle.Location.ToVector2(), Color.White);
        }

        public void Update()
        {
            return;
        }

        public void Take(Player player, Map map)
        {
            model.Effect.Invoke(player);
            model.Sound.Play();
            map.PowerUps.Remove(this);
        }
    }


    public class PowerUpManager
    {
        //---Konfiguracja powerUpów
        public const int SpeedModifier = 10;
        public const int Heal = 25;
        public const int SpeedUpTime = 5 * 1000; //milisekundy
        public const int DmgUpTime = 5 * 1000;
        public const int InvurnerabilityTime = 3 * 1000;

        public const int PowerUpNumberLimit = 10;
        public const int PowerUpSpawnMin = 60; //w klatkach
        public const int PowerUpSpawnMax = 300; //jw
        public static Random RNG = new Random();
        public static List<PowerUpModel> PowerUpModels = new List<PowerUpModel>();
        public static List<EffectArgs> AwaitingEffects = new List<EffectArgs>();
        public static Map map;

        private static int frameCount = 0;
        private static int nextPowerUpFrame = PowerUpSpawnMin;

        public static void Load(ContentManager content)
        {
            //heal
            PowerUpModel model = new PowerUpModel(content.Load<Texture2D>("heal"), HealEffect, content.Load<SoundEffect>("pizzaeating"));
            PowerUpModels.Add(model);

            //speedUp
            model = new PowerUpModel(content.Load<Texture2D>("speed"), SpeedUp, content.Load<SoundEffect>("SpeedUp"));
            PowerUpModels.Add(model);

            //dmgUp
            model = new PowerUpModel(content.Load<Texture2D>("gumy"), DmgUp, content.Load<SoundEffect>("dmgUp"));
            PowerUpModels.Add(model);

            //coneDmg
            model = new PowerUpModel(content.Load<Texture2D>("tama"), DmgCone, content.Load<SoundEffect>("coneDmg"));
            PowerUpModels.Add(model);

            //invulnerability
            model = new PowerUpModel(content.Load<Texture2D>("star"), MakeInvulnerable, content.Load<SoundEffect>("Invulnerable"));
            PowerUpModels.Add(model);

        }

        private static void findEmptySpace(PowerUp powerUp)
        {
            bool positionOK = false;
            Rectangle positionRect;
            while (!positionOK)
            {
                positionOK = true;
                positionRect = new Rectangle(new Point(RNG.Next(0, Config.WINDOW_WIDTH), RNG.Next(0, Config.WINDOW_HEIGHT)), powerUp.rectangle.Size);
                foreach (var wall in map.Walls)
                {
                    if (wall.Intersects(positionRect))
                    {
                        positionOK = false;
                        break;
                    }
                }
                if (positionOK)
                {
                    foreach (var pu in map.PowerUps)
                    {
                        if (pu.rectangle.Intersects(positionRect))
                        {
                            positionOK = false;
                            break;
                        }
                    }
                }

                if (!positionOK)
                {
                    continue;
                }
                else
                {
                    powerUp.rectangle = positionRect;
                    return;
                }
            }
        }

        public static void Update(GameTime gametime)
        {
            spawnPowerUps();

            for (int i = AwaitingEffects.Count - 1; i >= 0; i--)
            {
                var effect = AwaitingEffects[i];
                if (effect.CheckEffect(gametime))
                {
                    AwaitingEffects.Remove(effect);
                }
            }
        }

        private static void spawnPowerUps()
        {
            frameCount++;
            if ((map.PowerUps.Count >= PowerUpNumberLimit)
                || frameCount < nextPowerUpFrame)
            {
                return;
            }

            var newPowerUpModel = PowerUpModels[RNG.Next(PowerUpModels.Count)];
            var newPowerUp = new PowerUp(newPowerUpModel);
            findEmptySpace(newPowerUp);
            map.PowerUps.Add(newPowerUp);
            nextPowerUpFrame = RNG.Next(PowerUpSpawnMin, PowerUpSpawnMax);
            frameCount = 0;
        }

        //---Efekty powerUpów---

        public static void HealEffect(Player player)
        {
            player.Heal(Heal);
        }

        public static void SpeedUp(Player player)
        {
            player.CurrentSpeed += SpeedModifier;
            AwaitingEffects.Add(new EffectArgs(SpeedDown, SpeedUpTime, player));
        }

        public static void SpeedDown(Player player)
        {
            player.CurrentSpeed -= SpeedModifier;
        }

        public static void DmgUp(Player player)
        {
            player.missileModelType = MissileModelType.Strong;
            AwaitingEffects.Add(new EffectArgs(DmgRegular, DmgUpTime, player));
        }

        public static void DmgCone(Player player)
        {
            player.missileModelType = MissileModelType.Cone;
            AwaitingEffects.Add(new EffectArgs(DmgRegular, DmgUpTime, player));
        }

        public static void DmgRegular(Player player)
        {
            player.missileModelType = MissileModelType.Basic;
        }

        public static void MakeInvulnerable(Player player)
        {
            player.IsInvurnelable = true;
            AwaitingEffects.Add(new EffectArgs(MakeVulnerable, InvurnerabilityTime, player));
        }

        public static void MakeVulnerable(Player player)
        {
            player.IsInvurnelable = false;
        }

    }

    public class EffectArgs
    {
        PowerUpEffect effect;
        int secondsToInvoke;
        Player player;

        public EffectArgs(PowerUpEffect effect, int seconds, Player player)
        {
            this.effect = effect;
            this.secondsToInvoke = seconds;
            this.player = player;
        }

        public bool CheckEffect(GameTime gameTime)
        {
            secondsToInvoke -= gameTime.ElapsedGameTime.Milliseconds;
            if (secondsToInvoke <= 0)
            {
                this.effect.Invoke(player);
                return true;
            }
            return false;
        }
    }
}
