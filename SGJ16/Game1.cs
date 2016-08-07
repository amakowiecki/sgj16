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
        Ended,
        PreEnding,
        Loading,
        Start
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

        float pulseCounter = 0;
        int maxPulse = 30;
        bool increasePulse = false;
        float pauseTintMaxValue = 0.85f;

        Player winner = null;
        Player loser = null;

        AnimationManager animationManager;

        string title;
        LoadingAnimation loadingAnimation;
        Rectangle loadingProgressBar;
        double loadingProgress = 0;
        Random rng = new Random();

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

            animationManager = new AnimationManager();
            Missile.AnimationManager = animationManager;
            loadingAnimation = new LoadingAnimation(new Vector2((Config.WINDOW_WIDTH - LoadingAnimation.Width) / 2, -16));
            loadingProgressBar = new Rectangle(new Point((Config.WINDOW_WIDTH - LoadingAnimation.Width) / 2, Config.WINDOW_HEIGHT - 72),
                new Point(LoadingAnimation.Width, 48));

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
            gameState = GameState.Loading;

            title = "Wojownicze Żółwie Rambo";

            Aim.Initialize(Config.MIN_AIM_ANGLE, Config.MAX_AIM_ANGLE, Config.AIM_STEP, Config.DISTANCE);
            PowerUpManager.animationManager = animationManager;
            PowerUp.animationManager = animationManager;
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
                    Damage = Config.DEFAULT_MISSILE_DMG
                });
            Map.missiles.Models.Add(MissileModelType.Strong,
                new MissileModel
                {
                    MaxDistance = 640,
                    Speed = 8.0f,
                    Radius = 6,
                    InitialDistance = 50,
                    Damage = 2 * Config.DEFAULT_MISSILE_DMG
                });
            Map.missiles.Models.Add(MissileModelType.Cone,
                new MissileModel
                {
                    MaxDistance = 640,
                    Speed = 11.0f,
                    Radius = 4,
                    InitialDistance = 50,
                    Damage = Config.DEFAULT_MISSILE_DMG / 2
                });

            player2.Input.SetKey(GameKey.MoveLeft, Keys.Left);
            player2.Input.SetKey(GameKey.MoveRight, Keys.Right);
            player2.Input.SetKey(GameKey.LookUp, Keys.Up);
            player2.Input.SetKey(GameKey.LookDown, Keys.Down);
            player2.Input.SetKey(GameKey.Jump, Keys.Enter);
            player2.Input.SetKey(GameKey.Shot, Keys.Delete);
            player2.Input.SetKey(GameKey.Pause, Keys.Space);
            player2.Input.SetKey(GameKey.Quit, Keys.Escape);
            player2.Input.SetKey(GameKey.Skip, Keys.OemComma);

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

            loadingAnimation.Load(Content);
            
            animationManager.AddModel(AnimationType.SmallExplosion, Content.Load<Texture2D>("explodesmall"), 8);
            animationManager.AddModel(AnimationType.SmallSparkle, Content.Load<Texture2D>("sparkle"), 10);
            //animationManager.AddSimpleAnimation(AnimationType.Test, Vector2.Zero, 10);
            //animationManager.AddLoopAnimation(AnimationType.SmallSparkle, new Vector2(0, 0), 3);

            Map.missiles.Models[MissileModelType.Basic].Texture = Content.Load<Texture2D>("pocisk");
            Map.missiles.Models[MissileModelType.Strong].Texture = Content.Load<Texture2D>("superMissile");
            Map.missiles.Models[MissileModelType.Cone].Texture = Content.Load<Texture2D>("pocisk");

            HpBar.Textures[HpRangeType.Normal] = Content.Load<Texture2D>("hpbargreen");
            HpBar.Textures[HpRangeType.Low] = Content.Load<Texture2D>("hpbaryellow");
            HpBar.Textures[HpRangeType.VeryLow] = Content.Load<Texture2D>("hpbarred");
            HpBar.BackTexture = Content.Load<Texture2D>("hpbarback");
            p1HpBar.Position = HpBarPosition;
            p2HpBar.Position = new Vector2(Config.WINDOW_WIDTH - HpBarPosition.X
                - HpBar.BackTexture.Width, HpBarPosition.Y);

            player1.playerTexture = Content.Load<Texture2D>("player1-2");
            player2.playerTexture = Content.Load<Texture2D>("player2-2");
            var gunTexture = Content.Load<Texture2D>("weapon");
            player1.Gun.Texture = gunTexture;
            player2.Gun.Texture = gunTexture;
            var aimTexture = Content.Load<Texture2D>("aim");
            player1.Aim.Texture = aimTexture;
            player2.Aim.Texture = aimTexture;

            Map.MapTexture = Content.Load<Texture2D>("background1st");
            Map.SetPlatforms();

            SoundManager.Load(Content);
            MusicManager.Load(Content);
            PowerUpManager.map = Map;
            PowerUpManager.Load(Content);

            MusicManager.PlayMusicMenu();
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

            switch (gameState)
            {
                case GameState.Normal:
                    {
                        updateNormalState(gameTime);
                        break;
                    }
                case GameState.Paused:
                    {
                        updatePauseState();
                        break;
                    }
                case GameState.Ended:
                    {
                        updateEndState();
                        break;
                    }
                case GameState.PreEnding:
                    {
                        updatePreEndState();
                        break;
                    }
                case GameState.Loading:
                    {
                        updateLoadingState();
                        break;
                    }
                case GameState.Start:
                    {
                        updateMenu();
                        break;
                    }
                default:
                    break;
            }

            MusicManager.Update(gameTime);
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
            spriteBatch.Begin();

            if (gameState == GameState.Loading)
            {
                drawLoadingScreen();
            }
            else if (gameState == GameState.Start)
            {
                drawMenu();
            }
            else
            {
                //GraphicsDevice.Clear(new Color(96, 96, 96));
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
                animationManager.Draw(spriteBatch);
                foreach (Player p in Map.Players)
                {
                    DrawHpBar(p);
                }

                

                switch (gameState)
                {
                    case GameState.Paused:
                        {
                            drawPauseScreen();
                            break;
                        }
                    case GameState.Ended:
                        {
                            drawEndScreen();
                            break;
                        }
                    case GameState.PreEnding:
                        {
                            drawPreEndScreen();
                            break;
                        }
                    default:
                        break;
                }
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

        private Vector2 RotatePoint(Vector2 point, Vector2 origin, float angle)
        {
            point -= origin;
            float c = (float) Math.Cos(angle);
            float s = (float) Math.Sin(angle);
            float xnew = point.X * c - point.Y * s;
            float ynew = point.X * s + point.Y * c;
            return new Vector2(xnew + origin.X, ynew + origin.Y);
        }

        private void CreateMissile(Player player)
        {
            if (player.missileModelType == MissileModelType.Cone)
            {
                Vector2 center = player.AbsoluteMissileOrigin;
                Vector2 middle = Config.MISSILE_FORCE * player.Aim.GetMissileVelocity();
                float angle = MathHelper.Pi / 18;
                Map.missiles.InitializeMissile(player.missileModelType, center, middle);
                for (int i = 1; i < 3; i++)
                {
                    float tempAng = i * angle;
                    Vector2 velocity = RotatePoint(center + middle, center, tempAng) - center;
                    Map.missiles.InitializeMissile(player.missileModelType, center, velocity);
                    velocity = RotatePoint(center + middle, center, -tempAng) - center;
                    Map.missiles.InitializeMissile(player.missileModelType, center, velocity);
                }
            }
            else
            {
                Map.missiles.InitializeMissile(player.missileModelType, player.AbsoluteMissileOrigin,
                    Config.MISSILE_FORCE * player.Aim.GetMissileVelocity());
            }
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
                if (player.missileModelType == MissileModelType.Strong)
                {
                    SoundManager.PlaySuperShot();
                }
                else
                {
                    SoundManager.PlayShot();
                }
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
                spriteBatch.Draw(texture, bar.Position, new Rectangle(0, 0, (int) width, texture.Height), Color.White);
                spriteBatch.DrawOutlinedString(defaultFont, player.Name, bar.Position
                    + new Vector2(player.IsLeft ? 0 : texture.Width - defaultFont.MeasureString(player.Name).X,
                    16 + texture.Height), Color.White, 2.0f, Color.Black, 1.0f);
            }
        }

        private void drawPreEndScreen()
        {
            tintScreen(Color.Black * pauseTintMaxValue * pulseCounter);
        }

        private void drawEndScreen()
        {
            tintScreen(Color.Black * pauseTintMaxValue);
            string text = "Koniec gry";
            float min = 0.3f;
            float opacity = min + (float) pulseCounter / maxPulse * (1 - min);
            spriteBatch.DrawOutlinedString(defaultFontXl, text, new Vector2(
                StaticMethods.CenterTextX(defaultFontXl, text), 256), Color.White, 2.0f, Color.Black, opacity);
            text = "Gracz \'" + winner.Name + "\' wins.";
            spriteBatch.DrawOutlinedString(defaultFont, text, new Vector2(
                StaticMethods.CenterTextX(defaultFont, text), 360), Color.White, 2.0f, Color.Black, opacity);
            text = "Wciśnij \'Space\', aby continue.";
            spriteBatch.DrawOutlinedString(defaultFont, text, new Vector2(
                StaticMethods.CenterTextX(defaultFont, text), 400), Color.White, 2.0f, Color.Black, opacity);
        }

        private void drawLoadingScreen()
        {
            GraphicsDevice.Clear(Color.White);
            loadingAnimation.Draw(spriteBatch);
            Texture2D temp = new Texture2D(GraphicsDevice, 1, 1);
            temp.SetData(new Color[] { new Color(128, 128, 128) });
            spriteBatch.Draw(temp, new Rectangle(loadingProgressBar.X, loadingProgressBar.Y,
                (int) loadingProgress, loadingProgressBar.Height), Color.White);
            float min = 0.3f;
            float opacity = min + (float) pulseCounter / maxPulse * (1 - min);
            spriteBatch.DrawString(defaultFont, "Loading",
                new Vector2(StaticMethods.CenterTextX(defaultFont, "Loading"), 532),
                Color.Black * opacity);
        }

        private void drawMenu()
        {
            GraphicsDevice.Clear(Color.DarkCyan);
            spriteBatch.DrawOutlinedString(defaultFontXl, "Wojownicze",
                new Vector2(StaticMethods.CenterTextX(defaultFontXl, "Wojownicze"), 216), Color.White, 2.0f,
                Color.Black, 1.0f);
            spriteBatch.DrawOutlinedString(defaultFontXl, "Żółwie Rambo",
                new Vector2(StaticMethods.CenterTextX(defaultFontXl, "Żółwie Rambo"), 300), Color.White, 2.0f,
                Color.Black, 1.0f);
            string text = "Wciśnij \'Spacja\', aby rozpocząć.";
            spriteBatch.DrawOutlinedString(defaultFont, text, new Vector2(
                StaticMethods.CenterTextX(defaultFont, text), 424), Color.White, 2.0f, Color.Black, 1.0f);
        }

        private void drawPauseScreen()
        {
            tintScreen(Color.Black * pauseTintMaxValue);
            string text = "Pauza";
            float min = 0.3f;
            float opacity = min + (float) pulseCounter / maxPulse * (1 - min);
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

        private void updateEndState()
        {
            foreach (Player player in Map.Players)
            {
                if (IsKeyPressed(player.Input, GameKey.Quit))
                {
                    Exit();
                }
                if (IsKeyDown(player.Input, GameKey.Pause))
                {
                    restartGame();
                    break;
                }
            }
            updatePulseCounter();
        }

        private void updateMenu()
        {
            foreach (Player player in Map.Players)
            {
                if (IsKeyPressed(player.Input, GameKey.Quit))
                {
                    Exit();
                }
                if (IsKeyDown(player.Input, GameKey.Pause))
                {
                    gameState = GameState.Normal;
                    MusicManager.Play();
                    break;
                }
            }
            updatePulseCounter();
        }

        private void updatePauseState()
        {
            foreach (Player player in Map.Players)
            {
                if (IsKeyPressed(player.Input, GameKey.Quit))
                {
                    Exit();
                }
                if (IsKeyDown(player.Input, GameKey.Pause))
                {
                    pauseGame();
                    break;
                }
            }
            updatePulseCounter();
        }

        private void updatePreEndState()
        {
            foreach (Player player in Map.Players)
            {
                if (IsKeyPressed(player.Input, GameKey.Quit))
                {
                    Exit();
                }
            }

            if (loser.HpBar.VisibleHpPercentage <= 0)
            {
                if (loser.Opacity > 0)
                {
                    loser.Opacity -= Config.PLAYER_FADING_SPEED;
                    if (pulseCounter <= pauseTintMaxValue)
                    {
                        pulseCounter += Config.PLAYER_FADING_SPEED;
                    }
                    else
                    {
                        pulseCounter = pauseTintMaxValue;
                    }
                }
                else
                {
                    gameState = GameState.Ended;
                    pulseCounter = pauseTintMaxValue;
                }
            }
            else
            {
                loser.HpBar.Update();
                loser.Opacity = 1.0f;
            }

            animationManager.Update();
        }

        private void updateLoadingState()
        {
            if (keyboard.IsKeyDown(Keys.Back))
            {
                gameState = GameState.Normal;
                return;
            }

            loadingAnimation.Update();
            loadingProgress += (rng.Next(40) - rng.Next(20) + rng.Next(10) + rng.Next(10)) / 20;
            if (loadingProgress > loadingProgressBar.Width / 3)
            {
                loadingProgress += rng.Next(20) - rng.Next(10) + rng.Next(10);
            }
            if (loadingProgress >= loadingProgressBar.Width)
            {
                gameState = GameState.Start;
            }
            updatePulseCounter();
        }

        private void updateNormalState(GameTime gameTime)
        {
            foreach (Player player in Map.Players)
            {
                if (IsKeyPressed(player.Input, GameKey.Quit))
                {
                    Exit();
                }
                if (IsKeyDown(player.Input, GameKey.Pause))
                {
                    pauseGame();
                    break;
                }
            }

            if (checkWinLoseConditions())
            {
                foreach (Missile missile in Map.missiles)
                {
                    missile.Dispose();
                }
                gameState = GameState.PreEnding;
                pulseCounter = 0;
            }
            else
            {
                foreach (Missile missile in Map.missiles)
                {
                    missile.Update();
                }

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

                foreach (var item in displayableItems)
                {
                    item.Update();
                }

                UpdatePlayer(player1);
                UpdatePlayer(player2);
                animationManager.Update();
                PowerUpManager.Update(gameTime);
            }
        }

        private void restartGame()
        {
            player1.CurrentHp = Player.DefaultHP;
            player2.CurrentHp = Player.DefaultHP;

            player1.SetInitialPosition(true);
            player2.SetInitialPosition(false);

            player1.missileModelType = MissileModelType.Basic;
            player2.missileModelType = MissileModelType.Basic;

            player1.CurrentSpeed = Player.DefaultSpeed;
            player2.CurrentSpeed = Player.DefaultSpeed;

            player1.Opacity = 1.0f;
            player2.Opacity = 1.0f;

            player1.Aim.Angle = 0;
            player2.Aim.Angle = 0;

            Map.PowerUps.Clear();
            animationManager.Animations.Clear();

            winner = null;
            loser = null;

            gameState = GameState.Normal;
            pulseCounter = 0;
        }
    }
}
