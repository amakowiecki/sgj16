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
    public delegate void PowerUpEffect(Player player, PowerUp powerUp);
    public class PowerUpModel
    {
        public Texture2D Texture;
        public PowerUpEffect Effect;
        public SoundEffect Sound; 
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

        public void Take(Player player)
        {
            model.Effect.Invoke(player, this);
            model.Sound.Play();
        }
    }


    public static class PowerUpManager
    {
        public const int PowerUpNumberLimit = 10;
        public const int PowerUpSpawnMin = 60; //w klatkach
        public const int PowerUpSpawnMax = 600; //jw
        public static Random RNG = new Random();
        public static List<PowerUpModel> PowerUpModels = new List<PowerUpModel>();
        public static Map map;

        private static int frameCount = 0;
        private static int nextPowerUpFrame = PowerUpSpawnMin;

        public static void Load(ContentManager content)
        {
            //heal
            PowerUpModel model = new PowerUpModel();
            model.Texture = content.Load<Texture2D>("heal");
            model.Effect = HealEffect;
            model.Sound = content.Load<SoundEffect>("pizzaeating");
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

        public static void SpawnPowerUps()
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

        public static void HealEffect(Player player, PowerUp powerUp)
        {
            player.Heal(Config.BASIC_HEAL);
            map.PowerUps.Remove(powerUp);
        }
    }
}
