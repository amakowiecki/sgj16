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
        public const int DefaultPlayerHeight = Config.PLAYER_HEIGHT;
        public const int DefaultPlayerWidth = Config.PLAYER_WIDTH;
        public const int DefaultSpeed = 10;
        public const int DefaultJumpSpeed = 15;
        public const float DefaultHP = 100;
        public const int HeadSize = 20;
        public const int BodyHeight = 40;
        public const int LegHeight = DefaultPlayerHeight - HeadSize - BodyHeight;
        public const int LegWidth = Config.PLAYER_WIDTH;
        public const int MaxFramesInAir = 15;
        public static Vector2 DefaultGunOrigin = new Vector2(16, 7);
        public static Vector2 DefaultGunPosition = new Vector2(5, 5 + HeadSize);

        private static int textureChangeRate = 25;
        private static short IdleTexturesNumber = 2;
        private static short WalkingTexturesNumber = 3;

        public string Name;
        public bool IsLeft;
        public int PlayerHeight;
        public int PlayerWidth;
        public Map Map
        {
            get { return map; }
            set
            {
                if (!value.Players.Contains(this))
                {
                    map = value;
                    value.Players.Add(this);
                }
            }
        }
        public Vector2 CurrentPosition;
        public Dictionary<HitBox, Rectangle> BoundingBoxes;
        public Texture2D playerTexture;
        public State CurrentState;
        public bool IsFalling;
        public float MaxHp;
        public float CurrentHp;
        public int CurrentSpeed;
        public int CurrentJumpSpeed;
        public Direction CurrentDirection;
        public Rectangle rect
        {
            get
            { return new Rectangle(CurrentPosition.ToPoint(), new Point(PlayerWidth, PlayerHeight)); }
        }

        public static Dictionary<HitBox, float> DamageModifiers;

        public Gun Gun;
        public HpBar HpBar;
        public Aim Aim;
        public PlayerInput Input;
        
        public Vector2 AbsoluteMissileOrigin
        {
            get
            {
                if (CurrentDirection == Direction.Right)
                {
                    return CurrentPosition + Gun.Position + Gun.Origin;
                }
                else
                {
                    return CurrentPosition + new Vector2(PlayerWidth - Gun.Position.X
                        - Gun.Origin.X, Gun.Position.Y + Gun.Origin.Y);
                }
            }
        }

        private Map map;
        private short currentFrameNumber; //0 <= x < frameChangeRate 
        private short currentTextureNumber; //0 <= x < texturesNumber
        private int framesInAir;

        static Player()
        {
            DamageModifiers = new Dictionary<HitBox, float>();
        }

        public Player(bool isLeft)
        {
            IsLeft = isLeft;
            currentFrameNumber = 0;
            CurrentDirection = isLeft ? Direction.Right : Direction.Left;
            currentTextureNumber = 0;
            CurrentState = State.Standing;
            CurrentHp = MaxHp = DefaultHP;
            CurrentSpeed = DefaultSpeed;
            CurrentJumpSpeed = DefaultJumpSpeed;
            PlayerHeight = DefaultPlayerHeight;
            PlayerWidth = DefaultPlayerWidth;
            setInitialPosition(isLeft);

            IsFalling = false;
            framesInAir = 0;

            BoundingBoxes = new Dictionary<HitBox, Rectangle>();
            BoundingBoxes.Add(HitBox.Body,
                new Rectangle((int) CurrentPosition.X, (int) CurrentPosition.Y + HeadSize, PlayerWidth, BodyHeight));
            BoundingBoxes.Add(HitBox.Head,
                new Rectangle((int) CurrentPosition.X + (PlayerWidth - HeadSize) / 2,
                BoundingBoxes[HitBox.Body].Top, HeadSize, HeadSize));
            BoundingBoxes.Add(HitBox.Legs,
                new Rectangle((int)CurrentPosition.X + (PlayerWidth - LegWidth) / 2, BoundingBoxes[HitBox.Body].Bottom, 
                LegWidth, LegHeight));

            Gun = new Gun(this);
            Gun.Position = DefaultGunPosition;
            Gun.Origin = DefaultGunOrigin;

            Aim = new Aim(this);
            Input = new PlayerInput();
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

            Gun.Draw(batch);
        }
        
        public void Update()
        {
            if ((currentFrameNumber >= textureChangeRate && CurrentState == State.Standing)
                || (currentFrameNumber * 3 >= textureChangeRate && CurrentState == State.Walking))
            {
                currentFrameNumber = 0;
                updateTextureNumber();
            }
            if (CurrentState == State.InAir)
            {
                updateTextureNumber();
                flyLikeAFuckingBird();
            }
            currentFrameNumber++;

            HpBar.Update();
        }

        public bool Move(Direction direction)
        {
            if (CurrentState != State.InAir)
            {
                CurrentState = State.Walking;
            }

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
            
            if (isStandingOnAnything())
            {
                CurrentState = State.InAir;
                IsFalling = true;
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
            if (CurrentState != State.InAir)
            {
                CurrentState = State.InAir;
                CurrentJumpSpeed = DefaultJumpSpeed;
            }
        }

        public bool CheckDamage(float basicDamage, Circle circle)
        {
            float placeholder = 0;
            return CheckDamage(basicDamage, circle, out placeholder);
        }

        /// <summary>
        /// Sprawdź kolizję, zadaj obrażenia. Zwraca czy został 
        /// trafiony, result przechowuje ile obrażeń otrzymano.
        /// </summary>
        public bool CheckDamage(float basicDamage, Circle circle, out float result)
        {
            foreach (var boxInfo in BoundingBoxes)
            {
                if (StaticMethods.CheckCollision(circle, boxInfo.Value))
                {
                    if (DamageModifiers.ContainsKey(boxInfo.Key))
                    {
                        float dmg = DamageModifiers[boxInfo.Key] * basicDamage;
                        if (CurrentHp > dmg)
                        {
                            CurrentHp -= dmg;
                            result = dmg;
                            return true;
                        }
                        else
                        {
                            result = CurrentHp;
                            CurrentHp = 0;
                            return true;
                        }
                    }
                }
            }
            result = 0;
            return false;
        }

        /// <summary>
        /// Przywróć HP. Zwraca faktyczną ilość zregenerowanego HP.
        /// </summary>
        public float Heal(float hpToHeal)
        {
            if (CurrentHp + hpToHeal > MaxHp)
            {
                float result = MaxHp - CurrentHp;
                CurrentHp = MaxHp;
                return result;
            }
            else
            {
                CurrentHp += hpToHeal;
                return hpToHeal;
            }
        }

        /// <summary>
        /// Czy stopy znajdują się na wierzchu czegokolwiek innego i czy nie jesteśmy w powietrzu.
        /// </summary>
        /// <returns></returns>
        private bool isStandingOnAnything()
        {
            if (CurrentState == State.InAir) { return false; }
            int playerBottom = (int)CurrentPosition.Y + PlayerHeight;
            int minFeetX = BoundingBoxes[HitBox.Legs].X;
            int maxFeetX = minFeetX + BoundingBoxes[HitBox.Legs].Width;
            return !Map.Walls.Any(wall => { return wall.Top == 
                playerBottom && minFeetX <= wall.Right && wall.Left <= maxFeetX; });
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
            Rectangle inflatedRect;
            if (IsFalling)
            {
                inflatedRect = new Rectangle(rect.X, rect.Y - CurrentJumpSpeed, rect.Width, rect.Height);
            }
            else
            {
                if (rect.Height == 0)
                {
                    return int.MaxValue;
                }
                inflatedRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + CurrentJumpSpeed);
            }

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

        private bool flyLikeAFuckingBird()
        {
            //CurrentJumpSpeed = (CurrentJumpSpeed/(((framesInAir)/2)+1))+5;
            CurrentJumpSpeed = CurrentJumpSpeed * 2 / (int) Math.Ceiling(Math.Log(framesInAir + 2)) + 1;
            // CurrentJumpSpeed -= 10 * ((framesInAir / 60) + 1);
            if (framesInAir >= MaxFramesInAir)
            {
                IsFalling = true;
            }
            int distance = int.MaxValue;
            foreach (var wall in Map.Walls)
            {
                int d = checkVerticalCollision(wall);
                if (d < distance)
                {
                    distance = d;
                }
            }

            if (IsFalling)
            {
                if (framesInAir <= 0)
                {
                    framesInAir = 0;
                    CurrentJumpSpeed = DefaultJumpSpeed;
                }
                else
                {
                    framesInAir--;
                }
                CurrentPosition.Y += distance == int.MaxValue ? CurrentJumpSpeed : distance;
                if (CurrentPosition.Y + PlayerHeight > Config.WINDOW_HEIGHT)
                {
                    CurrentPosition.Y = Config.PLAYER_HEIGHT - PlayerHeight;
                    IsFalling = false;
                    CurrentState = State.Standing;
                    currentTextureNumber = 0;
                }
            }
            else
            {
                framesInAir++;
                CurrentPosition.Y -= distance == int.MaxValue ? CurrentJumpSpeed : distance;
            }

            changePosition();
            if (distance != int.MaxValue)
            {
                if (IsFalling)
                {
                    IsFalling = false;
                    CurrentState = State.Standing;
                }
                else IsFalling = true;
                framesInAir = 0;
                currentTextureNumber = 0;
                return false;
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

        private void setInitialPosition(bool isLeft)
        {
            CurrentPosition = new Vector2(
                isLeft ? (float)Config.PLAYER_POSITION_X : Config.WINDOW_WIDTH - PlayerWidth - Config.PLAYER_POSITION_X,
                (float)Config.WINDOW_HEIGHT - PlayerHeight - Config.GROUND_LEVEL);
        }

        private void changePosition()
        {
            BoundingBoxes[HitBox.Body] =
                new Rectangle((int) CurrentPosition.X, (int) CurrentPosition.Y + HeadSize, PlayerWidth, BodyHeight);
            BoundingBoxes[HitBox.Head] =
                new Rectangle((int) CurrentPosition.X + (PlayerWidth - HeadSize) / 2, (int) CurrentPosition.Y, HeadSize, HeadSize);
            BoundingBoxes[HitBox.Legs] =
                new Rectangle((int) CurrentPosition.X + (PlayerWidth - LegWidth) / 2, (int) CurrentPosition.Y + HeadSize + BodyHeight, LegWidth, LegHeight);
        }

    }
}
