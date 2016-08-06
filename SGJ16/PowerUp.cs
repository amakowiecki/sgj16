﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
            PowerUpModels.Add(model);
        }

        private static void findEmptySpace(PowerUp powerUp)
        {
            bool positionOK = true;
            while (true)
            {
                Rectangle positionRect = new Rectangle(new Point(RNG.Next(0, Config.WINDOW_WIDTH), RNG.Next(0, Config.WINDOW_HEIGHT)), powerUp.rectangle.Size);
                foreach (var wall in map.Walls)
                {
                    if (wall.Intersects(positionRect))
                    {
                        positionOK = false;
                        break;
                    }
                }
                if (!positionOK)
                {
                    continue;
                }
                powerUp.rectangle = positionRect;
                return;
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
    }
}