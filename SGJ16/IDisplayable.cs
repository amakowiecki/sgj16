using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public interface IDisplayable
    {
       // void Initialize(Game1 game);

        void Update();

        void Draw(SpriteBatch batch);
    }
}
