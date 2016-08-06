using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class MissileModel
    {
        public Texture2D Texture { get; set; }
        public float MaxDistance { get; set; }
        public float Speed { get; set; }
        public float Radius { get; set; }
    }

    public class Missile : IDisplayable
    {
        public MissileModel Model;

        private float time;
        private Vector2 initialPositon;
        private Vector2 initialVelocity;

        public Missile(MissileModel model, Vector2 initialPositon, Vector2 initialVelocity)
        {
            Model = model;
            this.initialPositon = initialPositon;
            this.initialVelocity = initialVelocity;
        }

        public Vector2 Position
        {
            get
            {
                return initialPositon + initialVelocity * time;
                // + 0.0001f * new Vector2(0, Config.GRAV_FORCE * StaticMethods.Sqr(time) / 2);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            Vector2 halfSize = Model.Texture.GetHalfSize();
            batch.Draw(Model.Texture, Position - halfSize, null, Color.White, 
                (float)Math.Atan2(initialVelocity.Y, initialVelocity.X), 
                halfSize, 1.0f, SpriteEffects.None, 1.0f);
        }

        public void Update()
        {
            time += Model.Speed;
            if (time > Model.MaxDistance)
            {
                time = Model.MaxDistance;
            }
        }
    }
}
