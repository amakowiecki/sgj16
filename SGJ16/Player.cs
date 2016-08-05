using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public struct Circle
    {
        public Point Center;
        public int Radius;
    }

    public static partial class StaticMethods
    {
        public static bool CheckCollision(Circle circle, Rectangle rectangle)
        {
            return false;
        }
    }

    public class Player
    {
        public ICollection<Rectangle> BoundingBoxes;

        public bool CheckCollision(Circle circle)
        {
            return false;
        }
    }
}
