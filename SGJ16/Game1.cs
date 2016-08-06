using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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

        PlayerInput p1Input;
        PlayerInput p2Input;
        Player player1;
        Map Map;
        Texture2D testPlatformTexture;

        Aim p1Aim;

        HpBar p1HpBar;
        HpBar p2HpBar;
        public static Vector2 HpBarPosition = new Vector2(36, 36);

        Missiles missiles = new Missiles(60);

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

            player1 = new Player(false);
            p1Input = new PlayerInput();
            p1Aim = new Aim(player1);
            p1HpBar = new HpBar(player1);

            p2Input = new PlayerInput();
            p2HpBar = new HpBar(player1);
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
                    Speed = 12.0f,
                    Radius = 4,
                    InitialDistance = 20
                });

            p1Input.SetKey(GameKey.MoveLeft, Keys.Left);
            p1Input.SetKey(GameKey.MoveRight, Keys.Right);
            p1Input.SetKey(GameKey.LookUp, Keys.Up);
            p1Input.SetKey(GameKey.LookDown, Keys.Down);
            p1Input.SetKey(GameKey.Jump, Keys.Z);
            p1Input.SetKey(GameKey.Shot, Keys.X);
            p1Input.SetKey(GameKey.Pause, Keys.Space);
            p1Input.SetKey(GameKey.Quit, Keys.Escape);

            p2Input.SetKey(GameKey.MoveLeft, Keys.A);
            p2Input.SetKey(GameKey.MoveRight, Keys.D);
            p2Input.SetKey(GameKey.LookUp, Keys.W);
            p2Input.SetKey(GameKey.LookDown, Keys.S);
            p2Input.SetKey(GameKey.Jump, Keys.Q);
            p2Input.SetKey(GameKey.Shot, Keys.E);
            p2Input.SetKey(GameKey.Pause, Keys.Space);
            p2Input.SetKey(GameKey.Quit, Keys.Escape);

            Map = new Map();
            player1.Map = Map;
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
            
            p1Aim.Texture = Content.Load<Texture2D>("aim");

            missiles.Models[MissileModelType.Basic].Texture = Content.Load<Texture2D>("pocisk");

            HpBar.Textures[HpRangeType.Normal] = Content.Load<Texture2D>("hpbargreen");
            HpBar.Textures[HpRangeType.Low] = Content.Load<Texture2D>("hpbaryellow");
            HpBar.Textures[HpRangeType.VeryLow] = Content.Load<Texture2D>("hpbarred");
            HpBar.BackTexture = Content.Load<Texture2D>("hpbarback");
            p1HpBar.Position = HpBarPosition;
            p2HpBar.Position = new Vector2(Config.WINDOW_WIDTH - HpBarPosition.X 
                - HpBar.BackTexture.Width, HpBarPosition.Y);

            player1.playerTexture = Content.Load<Texture2D>("idle");
            player1.Gun.Texture = Content.Load<Texture2D>("weapon");

            testPlatformTexture = Content.Load<Texture2D>("testPlatform");
            Map.SetPlatforms();

            // TODO: use this.Content to load your game content here
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
            if (IsKeyPressed(p1Input, GameKey.Quit) || IsKeyPressed(p2Input, GameKey.Quit))
            {
                Exit();
            }

            keyboard.Update();
            UpdateAim(p1Aim, p1Input);

            foreach (Missile missile in missiles)
            {
                missile.Update();
            }

            if (IsKeyDown(p1Input, GameKey.Jump))
            {
                player1.Jump();
            }

            if (IsKeyPressed(p1Input, GameKey.MoveLeft))
            {                
                player1.Move(Direction.Left);
            }
            else if (IsKeyPressed(p1Input, GameKey.MoveRight))
            {
                player1.Move(Direction.Right);
            }          
            else if(player1.CurrentState != State.InAir)
            {
                player1.CurrentState = State.Standing;
            }
            player1.Update();
            
            player1.CurrentHp -= 1;
            if (player1.CurrentHp < 0)
                player1.CurrentHp = 0;

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

            player1.Draw(spriteBatch);
            foreach (Missile missile in missiles)
            {
                missile.Draw(spriteBatch);
            }
            foreach (var platform in Map.Platforms)
            {//tymczasowe,platformy będą na mapie
                spriteBatch.Draw(testPlatformTexture, platform.Location.ToVector2(), Color.White);
            }
            spriteBatch.Draw(p1Aim.Texture, p1Aim.Position - p1Aim.Texture.GetHalfSize(), Color.White);

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
    }
}
