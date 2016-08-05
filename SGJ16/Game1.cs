﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SGJ16
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Point DisplaySize { get { return new Point(800, 480); } }

        KeyboardInput keyboard;

        PlayerInput p1Input;
        PlayerInput p2Input;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = DisplaySize.X;
            graphics.PreferredBackBufferHeight = DisplaySize.Y;

            IsMouseVisible = true;

            keyboard = new KeyboardInput();

            p1Input = new PlayerInput();
            p2Input = new PlayerInput();
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (IsKeyPressed(p1Input, GameKey.Shot))
            {
                GraphicsDevice.Clear(Color.DarkRed);
            }
            else if (IsKeyPressed(p2Input, GameKey.Shot))
            {
                GraphicsDevice.Clear(Color.DarkGreen);
            }
            else
            {
                GraphicsDevice.Clear(Color.DarkSalmon);
            }

            // TODO: Add your drawing code here

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
    }
}
