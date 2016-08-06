using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Map
    {
        public List<Player> Players { get; set; }
        public List<Rectangle> Walls { get; set; }
        public List<Rectangle> Platforms { get; set; }  //tymczasowe

        public Map()
        {
            Players = new List<Player>();
            Walls = new List<Rectangle>();
            Platforms = new List<Rectangle>();

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
            var newPlatform = new Rectangle( Config.WINDOW_WIDTH / 3, Config.WINDOW_HEIGHT -  128, 128, 32);
            Walls.Add(newPlatform);
            Platforms.Add(newPlatform);
        }
    }
}
