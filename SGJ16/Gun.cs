using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Gun : IDisplayable
    {
        public Texture2D Texture;
        public Rectangle Rectangle;
        public Player Player;

        public Gun(Player player)
        {
            Player = player;
        }

        public void Update()
        {
            if (Player.CurrentDirection == Direction.Right)
            {
                Rectangle.X = Player.BoundingBoxes[HitBox.Body].X + 5;
                Rectangle.Y = Player.BoundingBoxes[HitBox.Body].Y + 5;
            }
            else
            {
                Rectangle.X = Player.BoundingBoxes[HitBox.Body].Right - Texture.Width - 5;
                Rectangle.Y = Player.BoundingBoxes[HitBox.Body].Y + 5;
            }

        }

        public void Draw(SpriteBatch batch)
        {
            if (Player.CurrentDirection == Direction.Left)
            {
                batch.Draw(Texture, Rectangle.Location.ToVector2(), new Rectangle(0, 0, Texture.Width, Texture.Height), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            }
            else
            {
                batch.Draw(Texture, Rectangle.Location.ToVector2(), Color.White);
            }
            
        }
    }
}
