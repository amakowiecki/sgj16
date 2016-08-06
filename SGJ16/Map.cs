using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Map :IDisplayable
    {
        public List<Player> Players { get; set; }
        public List<Rectangle> Walls { get; set; }
        public Texture2D MapTexture { get; set; }
        public List<PowerUp> PowerUps { get; set; }

        public Map()
        {
            Players = new List<Player>();
            Walls = new List<Rectangle>();
            PowerUps = new List<PowerUp>();

            SetBoundingWalls();
        }

        public void SetBoundingWalls()
        {
            Walls.Add(new Rectangle(0, 0, 1, Config.WINDOW_HEIGHT));
            Walls.Add(new Rectangle(Config.WINDOW_WIDTH, 0, 1, Config.WINDOW_HEIGHT));
            Walls.Add(new Rectangle(0, 0, Config.WINDOW_WIDTH, 1));
            Walls.Add(new Rectangle(0, Config.WINDOW_HEIGHT, Config.WINDOW_WIDTH, 1));
        }

        public void SetPlatforms()
        {
            var newPlatform = new Rectangle(254, 358, 174, 38); //platforma z DeLoreanem
            Walls.Add(newPlatform);
            newPlatform = new Rectangle(579, 535, 69, 106);  //platforma na środku
            Walls.Add(newPlatform);
            newPlatform = new Rectangle(717, 498, 175, 6); //trzepak
            Walls.Add(newPlatform);
            newPlatform = new Rectangle(813, 353, 170, 26); //przy drzewie
            Walls.Add(newPlatform);
            newPlatform = new Rectangle(0, 512, 151, 129);
            Walls.Add(newPlatform);
            newPlatform = new Rectangle(1091, 503, 189, 139);
            Walls.Add(newPlatform);
        }

        public void Update()
        {
            return;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(MapTexture, new Vector2(0, 0), Color.White);
        }
    }
}
