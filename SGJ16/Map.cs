using Microsoft.Xna.Framework;
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

        public Map()
        {
            Players = new List<Player>();
            Walls = new List<Rectangle>();

            SetBoundingWalls();
        }

        public void SetBoundingWalls()
        {
            Walls.Add(new Rectangle(0, 0, 1, Config.WINDOW_HEIGHT));
            Walls.Add(new Rectangle(Config.WINDOW_WIDTH, 0, 1, Config.WINDOW_HEIGHT));
        }
    }
}
