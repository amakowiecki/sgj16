using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class AnimationModel
    {
        public Texture2D Texture;
        public int TotalFrames;
        public Point FrameSize
        {
            get { return new Point(Texture.Width / TotalFrames, Texture.Height); }
        }
        public Rectangle GetFrameRectangle(int frame)
        {
            Point size = FrameSize;
            return new Rectangle(new Point(frame * size.X, 0), size);
        }
    }

    public class Animation
    {
        public AnimationModel Model;
        public int CurrentFrame;
        public int Speed;
        public int Repeats;
        public bool Loop;
        public Vector2 Position;

        private int currentRepeat;
        private int counter;
        
        public Animation(AnimationModel model, int speed, Vector2 position)
        : this(model, speed, 1, false, position) { }

        public Animation(AnimationModel model, int speed, bool loop, Vector2 position)
        : this(model, speed, 0, loop, position) { }

        public Animation(AnimationModel model, int speed, int repeats, Vector2 position)
            : this(model, speed, repeats, false, position) { }

        public Animation(AnimationModel model, int speed, int repeats, bool loop, Vector2 position)
        {
            Model = model;
            Speed = speed;
            Repeats = repeats;
            Loop = loop;
            CurrentFrame = currentRepeat = counter = 0;
            Position = position;
        }

        public void Update()
        {
            ++counter;
            if (counter >= Speed)
            {
                counter = 0;
                ++CurrentFrame;
            }
            if (CurrentFrame == Model.TotalFrames)
            {
                if (Loop || currentRepeat < Repeats)
                {
                    ++currentRepeat;
                    CurrentFrame = 0;
                }
            }
        }

        public bool IsCompleted
        {
            get { return !Loop && currentRepeat == Repeats; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsCompleted)
            {
                spriteBatch.Draw(Model.Texture, Position, Model.GetFrameRectangle(CurrentFrame), Color.White);
            }
        }
    }

    public enum AnimationType
    {
        Test
    }

    public class AnimationManager
    {
        public Dictionary<AnimationType, AnimationModel> Models;
        public List<Animation> Animations;

        public AnimationManager()
        {
            Models = new Dictionary<AnimationType, AnimationModel>();
            Animations = new List<Animation>();
        }

        public void Update()
        {
            int i = Animations.Count - 1;
            while (i >= 0)
            {
                Animations[i].Update();
                if (Animations[i].IsCompleted)
                {
                    Animations.RemoveAt(i);
                }
                --i;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Animation anim in Animations)
            {
                anim.Draw(spriteBatch);
            }
        }

        public void AddAnimation(AnimationType type, Vector2 position, int speed, int repeats, bool loop)
        {
            if (Models.ContainsKey(type))
            {
                Animations.Add(new Animation(Models[type], speed, repeats, loop, position));
            }
        }

        public void AddLoopAnimation(AnimationType type, Vector2 position, int speed)
        {
            AddAnimation(type, position, speed, 0, true);
        }

        public void AddRepeatableAnimation(AnimationType type, Vector2 position, int speed, int repeats)
        {
            AddAnimation(type, position, speed, repeats, false);
        }

        public void AddSimpleAnimation(AnimationType type, Vector2 position, int speed)
        {
            AddAnimation(type, position, speed, 1, false);
        }

        public void AddModel(AnimationType type, Texture2D texture, int totalFrames)
        {
            if (!Models.ContainsKey(type))
            {
                Models.Add(type, new AnimationModel { Texture = texture, TotalFrames = totalFrames });
            }
        }
    }
}
