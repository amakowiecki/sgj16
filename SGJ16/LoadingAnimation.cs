using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class LoadingAnimation
    {
        public static int TotalTextures = 100;
        public static int Width = 800;
        public static int Height = 600;

        public List<Texture2D> Textures;
        int currentTexture;
        public Vector2 Position;
        int counter = 0;
        public int Speed = 3;

        public LoadingAnimation(Vector2 position)
        {
            Textures = new List<Texture2D>(TotalTextures);
            Position = position;
        }

        public void Load(ContentManager content )
        {
            for (int i = 0; i < TotalTextures; i++)
            {
                Textures.Add(content.Load<Texture2D>(string.Format("loadingGif/frame_{0}_delay-0.05s", i)));
            }
        }

        public void Update()
        {
            counter++;
            if (counter >= Speed)
            {
                counter = 0;
                currentTexture++;
                if (currentTexture >= TotalTextures)
                {
                    currentTexture = 0;
                }
            }
        }

        public void Draw(SpriteBatch sBatch)
        {
            sBatch.Draw(Textures[currentTexture], new Rectangle(Position.ToPoint(), new Point(Width, Height - 50)), Color.White);
        }
    }
}
