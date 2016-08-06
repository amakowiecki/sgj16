using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGJ16
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Point DisplaySize { get { return new Point(Config.WINDOW_WIDTH, Config.WINDOW_HEIGHT); } }

        KeyboardInput keyboard;

        List<IDisplayable> displayableItems;

        //PlayerInput p2Input;
        Player player1;
        Player player2;
        Map Map;
        Texture2D testPlatformTexture;

        HpBar p1HpBar;
        HpBar p2HpBar;
        public static Vector2 HpBarPosition = new Vector2(36, 36);

        Missiles missiles;

        public Vector2 DisplayCenter
        {
            get { return DisplaySize.ToVector2() / 2; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = DisplaySize.X;
            graphics.PreferredBackBufferHeight = DisplaySize.Y;
            //graphics.IsFullScreen = true;

            IsMouseVisible = true;

            keyboard = new KeyboardInput();

            player1 = new Player(true);
            p1HpBar = new HpBar(player1);

            player2 = new Player(false);
            p2HpBar = new HpBar(player2);
            
            Map = new Map();
            missiles = new Missiles(Map, 60);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Aim.Initialize(Config.MIN_AIM_ANGLE, Config.MAX_AIM_ANGLE, Config.AIM_STEP, Config.DISTANCE);

            HpBar.Ranges = new List<HpRange>
            {
                new HpRange { UpperBound = 1.0f, Type = HpRangeType.Normal },
                new HpRange { UpperBound = 0.5f, Type = HpRangeType.Low },
                new HpRange { UpperBound = 0.2f, Type = HpRangeType.VeryLow }
            };

            missiles.Models.Add(MissileModelType.Basic,
                new MissileModel
                {
                    MaxDistance = 640,
                    Speed = 11.0f,
                    Radius = 4,
                    InitialDistance = 20,
                    Damage = 10
                });

            player1.Input.SetKey(GameKey.MoveLeft, Keys.Left);
            player1.Input.SetKey(GameKey.MoveRight, Keys.Right);
            player1.Input.SetKey(GameKey.LookUp, Keys.Up);
            player1.Input.SetKey(GameKey.LookDown, Keys.Down);
            player1.Input.SetKey(GameKey.Jump, Keys.RightShift);
            player1.Input.SetKey(GameKey.Shot, Keys.Enter);
            player1.Input.SetKey(GameKey.Pause, Keys.Space);
            player1.Input.SetKey(GameKey.Quit, Keys.Escape);

            player2.Input.SetKey(GameKey.MoveLeft, Keys.A);
            player2.Input.SetKey(GameKey.MoveRight, Keys.D);
            player2.Input.SetKey(GameKey.LookUp, Keys.W);
            player2.Input.SetKey(GameKey.LookDown, Keys.S);
            player2.Input.SetKey(GameKey.Jump, Keys.LeftShift);
            player2.Input.SetKey(GameKey.Shot, Keys.Z);
            player2.Input.SetKey(GameKey.Pause, Keys.Space);
            player2.Input.SetKey(GameKey.Quit, Keys.Escape);

            Player.DamageModifiers.Add(HitBox.Legs, 0.50f);
            Player.DamageModifiers.Add(HitBox.Body, 1.00f);
            Player.DamageModifiers.Add(HitBox.Head, 4.17f);

            player1.Map = Map;
            player2.Map = Map;
            displayableItems = new List<IDisplayable>();
            
            //kolejność ma znaczenie
            displayableItems.Add(Map);
            displayableItems.Add(player1);
            displayableItems.Add(player2);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            

            missiles.Models[MissileModelType.Basic].Texture = Content.Load<Texture2D>("pocisk");

            HpBar.Textures[HpRangeType.Normal] = Content.Load<Texture2D>("hpbargreen");
            HpBar.Textures[HpRangeType.Low] = Content.Load<Texture2D>("hpbaryellow");
            HpBar.Textures[HpRangeType.VeryLow] = Content.Load<Texture2D>("hpbarred");
            HpBar.BackTexture = Content.Load<Texture2D>("hpbarback");
            p1HpBar.Position = HpBarPosition;
            p2HpBar.Position = new Vector2(Config.WINDOW_WIDTH - HpBarPosition.X 
                - HpBar.BackTexture.Width, HpBarPosition.Y);

            player1.playerTexture = Content.Load<Texture2D>("idle");
            player2.playerTexture = Content.Load<Texture2D>("player2");
            var gunTexture = Content.Load<Texture2D>("weapon");
            player1.Gun.Texture = gunTexture;
            player2.Gun.Texture = gunTexture;
            var aimTexture = Content.Load<Texture2D>("aim");
            player1.Aim.Texture = aimTexture;
            player2.Aim.Texture = aimTexture;

            Map.MapTexture = Content.Load<Texture2D>("background1st");
            testPlatformTexture = Content.Load<Texture2D>("testPlatform");
            Map.SetPlatforms();

            PowerUpManager.map = Map;
            PowerUpManager.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {     
            keyboard.Update();

            foreach (Missile missile in missiles)
            {
                missile.Update();
            }

            foreach (var item in displayableItems)
            {
                item.Update();
            }

            UpdatePlayer(player1);
            UpdatePlayer(player2);

            PowerUpManager.SpawnPowerUps();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(96, 96, 96));

            spriteBatch.Begin();

            foreach (var item in displayableItems)
            {
                item.Draw(spriteBatch);
            }
            foreach (var powerUp in Map.PowerUps)
            {
                powerUp.Draw(spriteBatch);
            }
            foreach (Missile missile in missiles)
            {
                missile.Draw(spriteBatch);
            }

            Aim aim = player1.Aim;
            spriteBatch.Draw(aim.Texture, aim.Position - aim.Texture.GetHalfSize(), Color.White);
            aim = player2.Aim;
            spriteBatch.Draw(aim.Texture, aim.Position - aim.Texture.GetHalfSize(), Color.White);

            DrawHpBar(p1HpBar);
            DrawHpBar(p2HpBar);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool IsKeyPressed(PlayerInput playerInput, GameKey key)
        {
            return keyboard.IsKeyPressed(playerInput.GetKey(key));
        }

        private bool IsKeyUp(PlayerInput playerInput, GameKey key)
        {
            return keyboard.IsKeyUp(playerInput.GetKey(key));
        }

        private bool IsKeyDown(PlayerInput playerInput, GameKey key)
        {
            return keyboard.IsKeyDown(playerInput.GetKey(key));
        }

        private void CreateMissile(Aim aim)
        {
            missiles.InitializeMissile(MissileModelType.Basic, aim.Player.AbsoluteMissileOrigin, 
                Config.MISSILE_FORCE * aim.GetMissileVelocity());
        }

        private void UpdateAim(Aim aim, PlayerInput playerInput)
        {
            bool upPressed = IsKeyPressed(playerInput, GameKey.LookUp);
            bool downPressed = IsKeyPressed(playerInput, GameKey.LookDown);
            if (upPressed && !downPressed)
            {
                aim.DecreaseAngle();
            }
            else if (downPressed && !upPressed)
            {
                aim.IncreaseAngle();
            }
            if (IsKeyDown(playerInput, GameKey.Shot))
            {
                CreateMissile(aim);
            }
        }

        private void DrawHpBar(HpBar bar)
        {
            spriteBatch.Draw(HpBar.BackTexture, bar.Position, Color.White);
            Texture2D texture = bar.Texture;
            if (texture != null)
            {
                float width = bar.VisibleHpPercentage * texture.Width;                
                spriteBatch.Draw(texture, bar.Position, new Rectangle(0, 0, (int)width, texture.Height), Color.White);
            }
        }

        private void UpdatePlayer(Player player)
        {
            PlayerInput pInput = player.Input;
            UpdateAim(player.Aim, pInput);

            if (IsKeyPressed(pInput, GameKey.Quit))
            {
                Exit();
            }


            if (IsKeyPressed(pInput, GameKey.MoveLeft))
            {
                player.Move(Direction.Left);
            }
            else if (IsKeyPressed(pInput, GameKey.MoveRight))
            {
                player.Move(Direction.Right);
            }
            else if (player.CurrentState != State.InAir)
            {
                player.CurrentState = State.Standing;
            }
            if (IsKeyDown(pInput, GameKey.Jump))
            {
                player.Jump();
            }

            //player.Update();
        }
    }
}
