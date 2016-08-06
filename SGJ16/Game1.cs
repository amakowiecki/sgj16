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

        Aim p1Aim;
        MissileModel missileModel = new MissileModel { MaxDistance = 500 , Speed = 5.0f };

        List<Missile> missiles = new List<Missile>();

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

            p1Input = new PlayerInput();
            p2Input = new PlayerInput();

            p1Aim = new Aim();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Aim.Initialize(Config.MIN_AIM_ANGLE, Config.MAX_AIM_ANGLE, Config.AIM_STEP, Config.DISTANCE);

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


            player1 = new Player(false);
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

            missileModel.Texture = Content.Load<Texture2D>("pocisk");

            player1.playerTexture = Content.Load<Texture2D>("idle");

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
            if (IsKeyPressed(p1Input, GameKey.MoveLeft))
            {                
                player1.Move(Direction.Left);
            }
            else if (IsKeyPressed(p1Input, GameKey.MoveRight))
            {
                player1.Move(Direction.Right);
            }
            else if (IsKeyDown(p1Input, GameKey.Jump))
            {
                player1.Jump();
            }
            else if(player1.CurrentState != State.InAir)
            {
                player1.CurrentState = State.Standing;
            }
            player1.Update();


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

            spriteBatch.Draw(p1Aim.Texture, DisplayCenter + p1Aim.GetRelativePosition() 
                - p1Aim.Texture.GetHalfSize(), Color.White);
            foreach (Missile missile in missiles)
            {
                missile.Draw(spriteBatch);
            }
            //spriteBatch.Draw(sample, DisplaySize.ToVector2() / 2 - new Vector2(sample.Width, sample.Height), Color.White * 0.5f);

            player1.Draw(spriteBatch);

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
            missiles.Add(new Missile(missileModel, DisplayCenter, 
                Config.MISSILE_FORCE * StaticMethods.NormalVectorInDirection(aim.Angle)));
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
    }
}
