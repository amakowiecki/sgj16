using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using SGJ16.Common;

namespace SGJ16
{
    public enum State
    {
        Standing,
        Walking,
        InAir
    }

    public class Player : IDisplayable
    {
        public const int DefaultPlayerHeight = 128;
        public const int DefaultPlayerWidth = 65;
        public const int DefaultSpeed = 10;
        public const int DefaultJumpSpeed = 15;
        public const float DefaultHP = 100;
        public const int HeadSize = 20;
        public const int BodyHeight = 40;
        public const int LegHeight = DefaultPlayerHeight - HeadSize - BodyHeight;

        private static int textureChangeRate = 5;
        private static short IdleTexturesNumber = 1;
        private static short WalkingTexturesNumber = 3;

        public int PlayerHeight;
        public int PlayerWidth;
        public Map Map;
        public Vector2 CurrentPosition;
        public Dictionary<HitBox, Rectangle> BoundingBoxes;
        public Texture2D playerTexture;
        public State CurrentState;
        public float CurrentHP;
        public int CurrentSpeed;
        public int CurrentJumpSpeed;
        public Direction CurrentDirection;
        private short currentFrameNumber; //0 <= x < frameChangeRate 
        private short currentTextureNumber; //0 <= x < texturesNumber

        public Player(bool isLeft)
        {
            currentFrameNumber = 0;
            CurrentDirection = isLeft ? Direction.Left : Direction.Right;
            currentTextureNumber = 0;
            CurrentState = State.Standing;
            CurrentHP = DefaultHP;
            CurrentSpeed = DefaultSpeed;
            PlayerHeight = DefaultPlayerHeight;
            PlayerWidth = DefaultPlayerWidth;
            CurrentPosition = new Vector2((float) Config.WINDOW_WIDTH / 2, (float) Config.WINDOW_HEIGHT - PlayerHeight);

            BoundingBoxes = new Dictionary<HitBox, Rectangle>();
            BoundingBoxes.Add(HitBox.Body,
                new Rectangle((int) CurrentPosition.X, (int) CurrentPosition.Y, PlayerWidth, PlayerHeight));
        }

        public bool CheckCollision(Circle circle)
        {
            //return BoundingBoxes.Any(b => StaticMethods.CheckCollision(circle, b));
            return false;
        }

        /// <summary>
        /// Zwraca odległość od obiektu z którym skolidowalibyśmy w następnym ruchu
        /// lub -1 jeśli nie ma żadnych kolizji
        /// </summary>
        /// <param name="rect">Obiekt do sprawdzenia</param>
        /// <returns>Odległość pozostała lub -1 dla braku kolizji</returns>
        public int CheckHorizontalCollision(Rectangle rect)
        {
            var inflatedRect = new Rectangle(rect.X - CurrentSpeed, rect.Y,
                rect.Width + 2 * CurrentSpeed, rect.Height);             //?? potencjalnie ujemna pozycja 

            if (BoundingBoxes[HitBox.Body].Intersects(inflatedRect))
            {
                var distance1 = Math.Abs(BoundingBoxes[HitBox.Body].X + PlayerWidth - rect.X);
                var distance2 = Math.Abs(BoundingBoxes[HitBox.Body].X - rect.X + rect.Width);
                return distance1 < distance2 ? distance1 : distance2;
            }
            return -1;
        }

        public void Draw(SpriteBatch batch)
        {            
            if (CurrentDirection == Direction.Left)
            {
                batch.Draw(playerTexture, CurrentPosition, getCurrentTextureBox(), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
            }
            else
            {
                batch.Draw(playerTexture, CurrentPosition, getCurrentTextureBox(), Color.White);
            }            
        }

        public void Update()
        {
            currentFrameNumber++;
            if (currentFrameNumber >= textureChangeRate)
            {
                currentFrameNumber = 0;
                updateTextureNumber();
            }
        }

        public bool Move(Direction direction)
        {
            CurrentDirection = direction;
            int distance = -1;
            foreach (var wall in Map.Walls)
            {
                int d = CheckHorizontalCollision(wall);
                if (d != -1 && d < distance)
                {
                    distance = d;
                }
            }
            if (distance == -1)
            {
                distance = CurrentSpeed;
            }
            if (distance == 0)
            {
                return false;
            }
            switch (direction)
            {
                case Direction.Left:
                    CurrentPosition.X -= distance;
                    break;
                case Direction.Right:
                    CurrentPosition.X += distance;
                    break;
                default:
                    break;
            }
            changePosition();
            return true;
        }

        private Rectangle getCurrentTextureBox()
        {
            Rectangle result;
            switch (CurrentState)
            {
                case State.Standing:
                    result = new Rectangle(currentTextureNumber * PlayerWidth, 0, PlayerWidth, PlayerHeight);
                    break;
                case State.Walking:
                    result = new Rectangle((IdleTexturesNumber * PlayerWidth) + currentTextureNumber * PlayerWidth, 0, PlayerWidth, PlayerHeight);
                    break;
                case State.InAir:
                    result = new Rectangle((IdleTexturesNumber * PlayerWidth) + (WalkingTexturesNumber * PlayerWidth)+ currentTextureNumber * PlayerWidth, 0, PlayerWidth, PlayerHeight);
                    break;
                default:
                    result = new Rectangle(currentTextureNumber * PlayerWidth, 0, PlayerWidth, PlayerHeight);
                    break;
            }

            return result;
        }

        private void updateTextureNumber()
        {
            currentTextureNumber++;
            switch (CurrentState)
            {
                case State.Standing:
                    if (currentTextureNumber >= IdleTexturesNumber)
                    {
                        currentTextureNumber = 0;
                    }
                    break;
                case State.Walking:
                    if (currentTextureNumber >= WalkingTexturesNumber)
                    {
                        currentTextureNumber = 0;
                    }
                    break;
                case State.InAir:
                    break;
                default:
                    break;
            }
        }

        private void changePosition()
        {
            BoundingBoxes[HitBox.Body] =
                new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y + HeadSize, PlayerWidth, BodyHeight);

        }

    }
}
