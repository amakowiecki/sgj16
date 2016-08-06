using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Config
    {
        public const int WINDOW_WIDTH = 640;
        public const int WINDOW_HEIGHT = 640;
        public const float GRAV_FORCE = 9.81f;

        public const float MIN_AIM_ANGLE = -MathHelper.Pi * 5 / 12;
        public const float MAX_AIM_ANGLE = MathHelper.Pi * 5 / 12;
        public const float AIM_STEP = MathHelper.Pi / 36;
        public const float DISTANCE = 150;

        public const float MISSILE_FORCE = 1.5f;
    }
}
