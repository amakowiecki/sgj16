using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class Gun
    {
        public Texture2D Texture;
        public Player Player;
        public Vector2 Origin;
        public Vector2 Position;

        public Gun(Player player)
        {
            Player = player;
        }

        public void Draw(SpriteBatch batch, float opacity)
        {
            Aim aim = Player.Aim;
            if (Player.CurrentDirection == Direction.Right)
            {
                batch.Draw(Texture, Player.CurrentPosition + Position + Origin, null, Color.White * opacity,
                    aim.Angle, Origin, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                batch.Draw(Texture, Player.CurrentPosition +
                    new Vector2(Player.PlayerWidth - Position.X - Origin.X, Position.Y + Origin.Y),
                    null, Color.White * opacity, -aim.Angle, new Vector2(Texture.Width - Origin.X, Origin.Y),
                    1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            }
        }
    }
}
