using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGJ16
{
    public enum GameState
    {
        Normal,
        Paused,
        Ended
    }

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
        SpriteFont defaultFont;
        SpriteFont defaultFontXl;

        List<IDisplayable> displayableItems;

        Player player1;
        Player player2;
        Map Map;

        HpBar p1HpBar;
        HpBar p2HpBar;
        public static Vector2 HpBarPosition = new Vector2(36, 36);

        GameState gameState;

        int pulseCounter = 0;
        int maxPulse = 30;
        bool increasePulse = false;

        Player winner = null;
        Player loser = null;

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
            Map.missiles = new Missiles(Map, 60);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameState = GameState.Normal;
            Aim.Initialize(Config.MIN_AIM_ANGLE, Config.MAX_AIM_ANGLE, Config.AIM_STEP, Config.DISTANCE);

            HpBar.Ranges = new List<HpRange>
            {
                new HpRange { UpperBound = 1.0f, Type = HpRangeType.Normal },
                new HpRange { UpperBound = 0.5f, Type = HpRangeType.Low },
                new HpRange { UpperBound = 0.2f, Type = HpRangeType.VeryLow }
            };

            Map.missiles.Models.Add(MissileModelType.Basic,
                new MissileModel
                {
                    MaxDistance = 640,
                    Speed = 11.0f,
                    Radius = 4,
                    InitialDistance = 50,
                    Damage = 10
                });
            Map.missiles.Models.Add(MissileModelType.Strong,
                new MissileModel
                {
                    MaxDistance = 640,
                    Speed = 8.0f,
                    Radius = 6,
                    InitialDistance = 50,
                    Damage = 20
                });

            player2.Input.SetKey(GameKey.MoveLeft, Keys.Left);
            player2.Input.SetKey(GameKey.MoveRight, Keys.Right);
            player2.Input.SetKey(GameKey.LookUp, Keys.Up);
            player2.Input.SetKey(GameKey.LookDown, Keys.Down);
            player2.Input.SetKey(GameKey.Jump, Keys.Enter);
            player2.Input.SetKey(GameKey.Shot, Keys.Delete);
            player2.Input.SetKey(GameKey.Pause, Keys.Space);
            player2.Input.SetKey(GameKey.Quit, Keys.Escape);

            player1.Input.SetKey(GameKey.MoveLeft, Keys.A);
            player1.Input.SetKey(GameKey.MoveRight, Keys.D);
            player1.Input.SetKey(GameKey.LookUp, Keys.W);
            player1.Input.SetKey(GameKey.LookDown, Keys.S);
            player1.Input.SetKey(GameKey.Jump, Keys.G);
            player1.Input.SetKey(GameKey.Shot, Keys.F);
            player1.Input.SetKey(GameKey.Pause, Keys.Space);
            player1.Input.SetKey(GameKey.Quit, Keys.Escape);

            Player.DamageModifiers.Add(HitBox.Legs, 0.50f);
            Player.DamageModifiers.Add(HitBox.Body, 1.00f);
            Player.DamageModifiers.Add(HitBox.Head, 4.17f);

            player1.Name = "Player 1";
            player2.Name = "Gracz 2";
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

            defaultFont = Content.Load<SpriteFont>("DefaultFont");
            defaultFontXl = Content.Load<SpriteFont>("DefaultFontXL");

            Map.missiles.Models[MissileModelType.Basic].Texture = Content.Load<Texture2D>("pocisk");
            Map.missiles.Models[MissileModelType.Strong].Texture = Content.Load<Texture2D>("superMissile");

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
            Map.SetPlatforms();

            MusicManager.Load(Content);
            PowerUpManager.map = Map;
            PowerUpManager.Load(Content);


            MusicManager.Play();
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

            foreach (Player player in Map.Players)
                if (gameState != GameState.Ended)
                {
                    if (gameState != GameState.Paused)
                    {
                        if (checkWinLoseConditions())
                        {
                            gameState = GameState.Ended;
                        }
                        else
                        {
                            foreach (Missile missile in Map.missiles)
                            {
                                missile.Update();
                            }

                            foreach (var item in displayableItems)
                            {
                                item.Update();
                            }

                            UpdatePlayer(player1);
                            UpdatePlayer(player2);

                            if (Map.PowerUps.Count > 0)
                            {
                                for (var i = Map.PowerUps.Count - 1; i >= 0; i--)
                                {
                                    var powerUp = Map.PowerUps[i];
                                    if (powerUp.rectangle.Intersects(player1.rect))
                                    {
                                        powerUp.Take(player1, Map);
                                    }
                                    else if (powerUp.rectangle.Intersects(player2.rect))
                                    {
                                        powerUp.Take(player2, Map);
                                    }
                                }
                            }
                            //PowerUpManager.SpawnPowerUps();

                            PowerUpManager.Update(gameTime);
                            MusicManager.Update(gameTime);
                            base.Update(gameTime);
                        }
                    }
                    else
                    {
                        updatePulseCounter();
                    }
                }
                else
                {
                    updatePulseCounter();
                }

            base.Update(gameTime);
        }

        private void updatePulseCounter()
        {
            if (increasePulse)
            {
                ++pulseCounter;
                if (pulseCounter >= maxPulse)
                {
                    increasePulse = false;
                }
            }
            else
            {
                --pulseCounter;
                if (pulseCounter < 0)
                {
                    increasePulse = true;
                }
            }
        }

        private bool checkWinLoseConditions()
        {
            if (player1.CurrentHp <= 0)
            {
                winner = player2;
                loser = player1;
                return true;
            }
            if (player2.CurrentHp <= 0)
            {
                winner = player1;
                loser = player2;
                return true;
            }
            return false;
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
            foreach (Missile missile in Map.missiles)
            {
                missile.Draw(spriteBatch);
            }

            //Aim aim = player1.Aim;
            //spriteBatch.Draw(aim.Texture, aim.Position - aim.Texture.GetHalfSize(), Color.White);
            //aim = player2.Aim;
            //spriteBatch.Draw(aim.Texture, aim.Position - aim.Texture.GetHalfSize(), Color.White);

            foreach (Player p in Map.Players)
            {
                DrawHpBar(p);
            }

            if (gameState == GameState.Ended)
            {
                drawEndScreen();
            }
            else if (gameState == GameState.Paused)
            {
                drawPauseScreen();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void tintScreen(Color color)
        {
            Texture2D tintTexture = new Texture2D(GraphicsDevice, 1, 1);
            tintTexture.SetData(new Color[] { Color.White });
            spriteBatch.Draw(tintTexture, new Rectangle(0, 0, Config.WINDOW_WIDTH, Config.WINDOW_HEIGHT), color);
        }

        private void pauseGame()
        {
            gameState = gameState == GameState.Paused
                ? GameState.Normal : GameState.Paused;
            pulseCounter = 0;
            increasePulse = true;
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

        private void CreateMissile(Player player)
        {
            Map.missiles.InitializeMissile(player.missileModelType, player.AbsoluteMissileOrigin,
                Config.MISSILE_FORCE * player.Aim.GetMissileVelocity());
        }

        private void UpdateAim(Player player)
        {
            bool upPressed = IsKeyPressed(player.Input, GameKey.LookUp);
        bool downPressed = IsKeyPressed(player.Input, GameKey.LookDown);
            if (upPressed && !downPressed)
            {
                player.Aim.DecreaseAngle();
            }
            else if (downPressed && !upPressed)
            {
                player.Aim.IncreaseAngle();
            }
            if (IsKeyDown(player.Input, GameKey.Shot))
            {
                CreateMissile(player);
            }
        }

        private void DrawHpBar(Player player)
{
    HpBar bar = player.HpBar;
    spriteBatch.Draw(HpBar.BackTexture, bar.Position, Color.White);
    Texture2D texture = bar.Texture;
    if (texture != null)
    {
        float width = bar.VisibleHpPercentage * texture.Width;
        spriteBatch.Draw(texture, bar.Position, new Rectangle(0, 0, (int)width, texture.Height), Color.White);
        spriteBatch.DrawOutlinedString(defaultFont, player.Name, bar.Position
            + new Vector2(player.IsLeft ? 0 : texture.Width - defaultFont.MeasureString(player.Name).X,
            16 + texture.Height), Color.White, 2.0f, Color.Black, 1.0f);
    }
}

private void drawEndScreen()
{
    tintScreen(Color.Black * 0.85f);
    string text = "Koniec gry";
    float min = 0.3f;
    float opacity = min + (float)pulseCounter / maxPulse * (1 - min);
    spriteBatch.DrawOutlinedString(defaultFontXl, text, new Vector2(
        StaticMethods.CenterTextX(defaultFontXl, text), 256), Color.White, 2.0f, Color.Black, opacity);
    text = "Gracz \'" + winner.Name + "\' wins.";
    spriteBatch.DrawOutlinedString(defaultFont, text, new Vector2(
        StaticMethods.CenterTextX(defaultFont, text), 360), Color.White, 2.0f, Color.Black, opacity);
    text = "Wciśnij \'Space\', aby continue.";
    spriteBatch.DrawOutlinedString(defaultFont, text, new Vector2(
        StaticMethods.CenterTextX(defaultFont, text), 400), Color.White, 2.0f, Color.Black, opacity);
}

private void drawPauseScreen()
{
    tintScreen(Color.Black * 0.85f);
    string text = "Pauza";
    float min = 0.3f;
    float opacity = min + (float)pulseCounter / maxPulse * (1 - min);
    spriteBatch.DrawOutlinedString(defaultFontXl, text, new Vector2(
        StaticMethods.CenterTextX(defaultFontXl, text), 280), Color.White, 2.0f, Color.Black, opacity);
    text = "Press \'Spacja\' to continue.";
    spriteBatch.DrawOutlinedString(defaultFont, text, new Vector2(
        StaticMethods.CenterTextX(defaultFont, text), 390), Color.White, 2.0f, Color.Black, opacity);
}

private void UpdatePlayer(Player player)
{
    PlayerInput pInput = player.Input;
    UpdateAim(player);

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
