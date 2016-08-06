using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public struct Circle
    {
        public Vector2 Center;
        public float Radius;

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }

    public static partial class StaticMethods
    {
        public static int Sqr(int value) { return value * value; }
        public static float Sqr(float value) { return value * value; }

        public static float SqDistanceToRectangle(Vector2 point, Rectangle rectangle)
        {
            float x = point.X, y = point.Y;

            float xRes = (x < rectangle.Left) ? rectangle.Left - x
                : (x > rectangle.Right) ? x - rectangle.Right : 0;
            float yRes = (y < rectangle.Top) ? rectangle.Top - y
                : (y > rectangle.Bottom) ? y - rectangle.Bottom : 0;

            return Sqr(xRes) + Sqr(yRes);
        }

        public static bool CheckCollision(Circle circle, Rectangle rectangle)
        {
            return SqDistanceToRectangle(circle.Center, rectangle) <= Sqr(circle.Radius);
        }

        public static float CalculateThrow(float initialVelocity, float time)
        {
            return initialVelocity * time - Config.GRAV_FORCE * StaticMethods.Sqr(time) / 2;
        }

        public static float AngleBetween(Vector2 point1, Vector2 point2)
        {
            return (float)Math.Atan2(point1.Y - point2.Y, point2.X - point1.X);
        }

        public static Vector2 NormalVectorInDirection(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static Vector2 GetHalfSize(this Texture2D texture)
        {
            return new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}
