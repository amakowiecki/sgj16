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
        public const int MaxFramesInAir = 30;

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
        public bool IsFalling;
        public float CurrentHP;
        public int CurrentSpeed;
        public int CurrentJumpSpeed;
        public Direction CurrentDirection;
        private short currentFrameNumber; //0 <= x < frameChangeRate 
        private short currentTextureNumber; //0 <= x < texturesNumber
        private int framesInAir;

        public Player(bool isLeft)
        {
            currentFrameNumber = 0;
            CurrentDirection = isLeft ? Direction.Left : Direction.Right;
            currentTextureNumber = 0;
            CurrentState = State.Standing;
            CurrentHP = DefaultHP;
            CurrentSpeed = DefaultSpeed;
            CurrentJumpSpeed = DefaultJumpSpeed;
            PlayerHeight = DefaultPlayerHeight;
            PlayerWidth = DefaultPlayerWidth;
            CurrentPosition = new Vector2((float) Config.WINDOW_WIDTH / 2, (float) Config.WINDOW_HEIGHT - PlayerHeight);
            IsFalling = false;
            framesInAir = 0;

            BoundingBoxes = new Dictionary<HitBox, Rectangle>();
            BoundingBoxes.Add(HitBox.Body,
                new Rectangle((int) CurrentPosition.X, (int) CurrentPosition.Y + HeadSize, PlayerWidth, BodyHeight));
            BoundingBoxes.Add(HitBox.Head,
                new Rectangle((int) CurrentPosition.X + (PlayerWidth - HeadSize) / 2,
                BoundingBoxes[HitBox.Body].Top, HeadSize, HeadSize));
            BoundingBoxes.Add(HitBox.Legs,
                new Rectangle(BoundingBoxes[HitBox.Body].Left, BoundingBoxes[HitBox.Body].Bottom, PlayerWidth, LegHeight));
        }

        public bool CheckCollision(Circle circle)
        {
            //return BoundingBoxes.Any(b => StaticMethods.CheckCollision(circle, b));
            return false;
        }

        /// <summary>
        /// Zwraca odległość od obiektu z którym skolidowalibyśmy w następnym ruchu
        /// lub int.MaxValue jeśli nie ma żadnych kolizji
        /// </summary>
        /// <param name="rect">Obiekt do sprawdzenia</param>
        /// <returns>Odległość pozostała lub int.MaxValue dla braku kolizji</returns>
        private int checkHorizontalCollision(Rectangle rect)
        {
            Rectangle inflatedRect;
            switch (CurrentDirection)
            {
                case Direction.Left:
                    inflatedRect = new Rectangle(rect.X, rect.Y,
                rect.Width + CurrentSpeed, rect.Height);
                    break;
                case Direction.Right:
                    inflatedRect = new Rectangle(rect.X - CurrentSpeed, rect.Y,
                rect.Width, rect.Height);
                    break;
                default:
                    inflatedRect = new Rectangle(rect.X, rect.Y,
                rect.Width, rect.Height);
                    break;
            }
            int distance = 0;
            int smallestDistance = int.MaxValue;
            foreach (var hitBox in BoundingBoxes.Values)
            {
                if (hitBox.Intersects(inflatedRect))
                {
                    switch (CurrentDirection)
                    {
                        case Direction.Left:
                            distance = hitBox.Left - rect.Right;
                            break;
                        case Direction.Right:
                            distance = rect.Left - hitBox.Right;
                            break;
                        default:
                            distance = 0;
                            break;
                    }
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                    }
                }
            }
            return smallestDistance;
        }

        /// <summary>
        /// Zwraca odległość do przeszkody lub int.MaxValue dla braku kolizji
        /// </summary>
        /// <returns></returns>
        private int checkVerticalCollision(Rectangle rect)
        {
            Rectangle inflatedRect = new Rectangle(rect.X, rect.Y + CurrentJumpSpeed, rect.Width, rect.Height + CurrentJumpSpeed);
            int smallestDistance = int.MaxValue;
            int distance = 0;
            foreach (var hitBox in BoundingBoxes.Values)
            {
                if (hitBox.Intersects(inflatedRect))
                {
                    if (IsFalling)
                    {
                        distance = rect.Top - hitBox.Bottom;
                    }
                    else
                    {
                        distance = hitBox.Top - rect.Bottom;
                    }
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                    }
                }
            }
            return smallestDistance;
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
            if (CurrentState == State.InAir)
            {
                flyLikeAFuckingBird();
            }
        }

        public bool Move(Direction direction)
        {
            CurrentState = State.Walking;
            CurrentDirection = direction;
            int distance = int.MaxValue;
            foreach (var wall in Map.Walls)
            {
                int d = checkHorizontalCollision(wall);
                if (d < distance)
                {
                    distance = d;
                }
            }
            if (distance == int.MaxValue)
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

        public void Jump()
        {
            CurrentState = State.InAir;
        }

        private bool flyLikeAFuckingBird()
        {
            framesInAir++;
            if (framesInAir >= MaxFramesInAir)
            {
                IsFalling = true;
                framesInAir = 0;
            }
            int distance = int.MaxValue;
            foreach (var wall in Map.Walls)
            {
                distance = checkVerticalCollision(wall);
            }
            if (distance == 0)
            {
                CurrentState = State.Standing;
                return false;
            }

            if (IsFalling)
            {
                CurrentPosition.Y += distance == int.MaxValue ? CurrentJumpSpeed : distance;
            }
            else
            {
                CurrentPosition.Y -= distance == int.MaxValue ? CurrentJumpSpeed : distance;
            }
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
                    result = new Rectangle((IdleTexturesNumber * PlayerWidth) + (WalkingTexturesNumber * PlayerWidth) + currentTextureNumber * PlayerWidth, 0, PlayerWidth, PlayerHeight);
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
                    currentTextureNumber = 0;
                    break;
                default:
                    break;
            }
        }

        private void changePosition()
        {
            BoundingBoxes[HitBox.Body] =
                new Rectangle((int) CurrentPosition.X, (int) CurrentPosition.Y + HeadSize, PlayerWidth, BodyHeight);

        }

    }
}
