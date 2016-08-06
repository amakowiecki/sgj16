using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Aim
    {
        public Texture2D Texture;
        public Player Player;
        public float Angle;

        public static float Distance;
        public static float MinAngle;
        public static float MaxAngle;
        public static float AngleStep;

        public Aim(Player player)
        {
            Player = player;
        }

        public static void Initialize(float minAngle, float maxAngle, float angleStep, float distance)
        {
            Distance = distance;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
            AngleStep = angleStep;
        }

        public Vector2 GetRelativePosition()
        {
            Vector2 vec = Distance * StaticMethods.NormalVectorInDirection(Angle);
            if (Player.CurrentDirection == Direction.Left) { vec.X = -vec.X; }
            return vec;
        }

        public Vector2 GetMissileVelocity()
        {
            Vector2 vec = StaticMethods.NormalVectorInDirection(Angle);
            if (Player.CurrentDirection == Direction.Left) { vec.X = -vec.X; }
            return vec;
        }

        public Vector2 Position
        {
            get
            {
                return Player.AbsoluteMissileOrigin + GetRelativePosition();
            }
        }

        public void IncreaseAngle()
        {
            Angle += AngleStep;
            if (Angle > MaxAngle)
            {
                Angle = MaxAngle;
            }
        }

        public void DecreaseAngle()
        {
            Angle -= AngleStep;
            if (Angle < MinAngle)
            {
                Angle = MinAngle;
            }
        }
    }
}
