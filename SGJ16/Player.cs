using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Player
    {
        public ICollection<Rectangle> BoundingBoxes;

        public bool CheckCollision(Circle circle)
        {
            return BoundingBoxes.Any(b => StaticMethods.CheckCollision(circle, b));
        }
    }
}
