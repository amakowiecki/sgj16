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
        public float InitialDistance { get; set; }
        public float MaxDistance { get; set; }
        public float Speed { get; set; }
        public float Radius { get; set; }
        public float Damage { get; set; }
    }

    public enum MissileModelType
    {
        Empty,
        Basic
    }

    public class Missile : IDisplayable
    {
        private Missiles missiles;
        internal int collectionIdx;

        public MissileModelType ModelType;
        internal float time;
        internal Vector2 initialPositon;
        internal Vector2 initialVelocity;

        /// <summary>
        /// Nie ruszać. Nie wywoływać.
        /// </summary>
        internal Missile(Missiles collection, int index)
        {
            missiles = collection;
            collectionIdx = index;
        }

        public void Initialize(MissileModelType modelType, Vector2 initialPositon, Vector2 velocity)
        {
            ModelType = modelType;
            time = missiles.GetMissileModel(ModelType).InitialDistance;
            this.initialPositon = initialPositon;
            this.initialVelocity = velocity;
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
            MissileModel model = missiles.GetMissileModel(ModelType);
            if (model != null)
            {
                Vector2 halfSize = model.Texture.GetHalfSize();
                batch.Draw(model.Texture, Position - halfSize, null, Color.White,
                    (float)Math.Atan2(initialVelocity.Y, initialVelocity.X),
                    halfSize, 1.0f, SpriteEffects.None, 1.0f);
            }
        }

        public void Update()
        {
            MissileModel model = missiles.GetMissileModel(ModelType);
            if (model != null)
            {
                time += model.Speed;
                if (time > model.MaxDistance)
                {
                    time = model.MaxDistance;
                    this.Dispose();
                    return;
                }
                Map map = Map;
                Circle circle = this.ToCircle();
                foreach (Rectangle wall in map.Walls)
                {
                    float temp = StaticMethods.SqDistanceToRectangle(circle.Center, wall);

                    if (StaticMethods.CheckCollision(circle, wall))
                    {
                        this.Dispose();
                        return;
                    }
                }
                float damage = Model.Damage;
                foreach (Player player in map.Players)
                {
                    if (player.CheckDamage(damage, circle))
                    {
                        this.Dispose();
                        return;
                    }
                }
            }
        }

        public void Dispose()
        {
            missiles.DisposeMissile(this);
        }

        public Map Map { get { return missiles.Map; } }

        public MissileModel Model { get { return missiles.Models[ModelType]; } }

        private Circle ToCircle()
        {
            return new Circle(this.Position, Model.Radius);
        }
    }    
}
