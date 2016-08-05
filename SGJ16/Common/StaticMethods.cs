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
        public static int Sqr(int value) { return value * value; }

        public static int SqDistanceToRectangle(Point point, Rectangle rectangle)
        {
            int x = point.X, y = point.Y;

            int xRes = (x < rectangle.X) ? rectangle.X - x
                : (x > rectangle.X) ? x - rectangle.X : 0;
            int yRes = (y < rectangle.Y) ? rectangle.Y - y
                : (y > rectangle.Y) ? y - rectangle.Y : 0;

            return Sqr(xRes) + Sqr(yRes);
        }

        public static bool CheckCollision(Circle circle, Rectangle rectangle)
        {
            return SqDistanceToRectangle(circle.Center, rectangle) <= Sqr(circle.Radius);
        }
    }
}
