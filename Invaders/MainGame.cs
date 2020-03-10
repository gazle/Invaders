using Invaders.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Invaders
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class MainGame : Game
    {
        public static MainGame Current;
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public DemoState DemoState;
        public PausedState PausedState;
        public PlayingState PlayingState;
        public GameOverState GameOverState;
        public Dictionary<string, Texture2D> TextureDict;
        public Dictionary<string, Texture2D[]> AnimationDict;
        public Dictionary<Texture2D, Color[]> TextureData;
        public Dictionary<string, SoundEffectInstance> SoundDict;
        public SpriteFont Font;
        public KeyboardState CurrentKeyboardState;
        public KeyboardState PreviousKeyBoardState;
        public GameState PreviousGameState;
        Texture2D backgroundImage;
        readonly double[] times = new double[1];
        int timesIndex = 0;
        public const int Scale = 4;
        public const int Width = 896 / Scale;
        public const int Height = 1040 / Scale;
        public const int LeftSide = 8;
        public const int RightSide = 202;
        public const int PlayerLeftSide = 19;
        public const int PlayerRightSide = 187;
        public const int CeilingY = 32;
        public const int SaucerY = CeilingY + 8;
        public const int BaseY = 216;
        public const int BarricadeY = BaseY - 24;
        public const int LivesY = 240;
        public const int MaxBullets = 1;
        public const int LineY = 239;
        public const int PlayerExplodeTime = 100;
        public const int SaucerFrequency = 500;
        public const int SaucerSpeed = 1;
        public const int MinSaucerSpawn = 8;
        public readonly int[] RackHeights = new[] { 11, 8, 6, 5, 5, 5, 4, 4, 4 };

        public MainGame()
        {
            Current = this;
            Graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = Width * Scale, PreferredBackBufferHeight = Height * Scale };
            Content.RootDirectory = "Content";
            TextureDict = new Dictionary<string, Texture2D>();
            AnimationDict = new Dictionary<string, Texture2D[]>();
            TextureData = new Dictionary<Texture2D, Color[]>();
            SoundDict = new Dictionary<string, SoundEffectInstance>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization logic here
            DemoState = new DemoState();
            PausedState = new PausedState();
            PlayingState = new PlayingState();
            GameOverState = new GameOverState();
            GameStateManager.PushState(DemoState);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            void addToTextureDict(string key)
            {
                Texture2D t = Content.Load<Texture2D>(@"Sprites\" + key);
                TextureDict.Add(key, t);
                Color[] d = new Color[t.Width * t.Height];
                t.GetData(d);
                TextureData.Add(t, d);
            }

            void addToAnimDict(string animKey, params string[] names)
                => AnimationDict.Add(animKey, names.Select(o => TextureDict[o]).ToArray());

            foreach (string key in new[]
                { "top0", "top1", "mid0", "mid1", "bot0", "bot1", "explosion", "bulletexplosion",
                  "shotexplosion", "saucerexplosion", "squiggle0", "squiggle1", "squiggle2", "squiggle3",
                  "plunger0", "plunger1", "plunger2", "plunger3", "screw0", "screw1", "screw2",
                  "dead0", "dead1", "base", "bullet", "barricade", "saucer"})
                addToTextureDict(key);

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            addToAnimDict("top", "top0", "top1");
            addToAnimDict("mid", "mid0", "mid1");
            addToAnimDict("bot", "bot0", "bot1");
            addToAnimDict("explosion", "explosion");
            addToAnimDict("bulletexplosion", "bulletexplosion");
            addToAnimDict("shotexplosion", "shotexplosion");
            addToAnimDict("saucerexplosion", "saucerexplosion");
            addToAnimDict("squiggle", "squiggle0", "squiggle1", "squiggle2", "squiggle3");
            addToAnimDict("plunger", "plunger0", "plunger1", "plunger2", "plunger3");
            addToAnimDict("screw", "screw0", "screw1", "screw2", "screw1");
            addToAnimDict("dead", "dead0", "dead1");
            addToAnimDict("base", "base");
            addToAnimDict("bullet", "bullet");
            addToAnimDict("barricade", "barricade");
            addToAnimDict("saucer", "saucer");
            Texture2D line = new Texture2D(GraphicsDevice, Width, 1);
            TextureDict.Add("line", line);
            TextureData.Add(line, new Color[Width]);
            AnimationDict.Add("line", new Texture2D[] { line });
            using (FileStream stream = new FileStream(@"Content\background.jpg", FileMode.Open))
            {
                backgroundImage = Texture2D.FromStream(GraphicsDevice, stream);
            }
            Font = Content.Load<SpriteFont>("Tahoma");

            SoundDict.Add("laser", Content.Load<SoundEffect>(@"Sounds\laser").CreateInstance());
            SoundDict.Add("splat", Content.Load<SoundEffect>(@"Sounds\splat").CreateInstance());
            SoundDict.Add("baseexp", Content.Load<SoundEffect>(@"Sounds\baseexp").CreateInstance());
            SoundDict.Add("saucer", Content.Load<SoundEffect>(@"Sounds\saucer").CreateInstance());
            SoundDict["saucer"].IsLooped = true;
            SoundDict.Add("saucersplat", Content.Load<SoundEffect>(@"Sounds\saucersplat").CreateInstance());
            SoundDict.Add("dnn0", Content.Load<SoundEffect>(@"Sounds\dnn0").CreateInstance());
            SoundDict.Add("dnn1", Content.Load<SoundEffect>(@"Sounds\dnn1").CreateInstance());
            SoundDict.Add("dnn2", Content.Load<SoundEffect>(@"Sounds\dnn2").CreateInstance());
            SoundDict.Add("dnn3", Content.Load<SoundEffect>(@"Sounds\dnn3").CreateInstance());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            PreviousKeyBoardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            if (CurrentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();
            // Global pause, LeftShift P steps frame by frame
            if (CurrentKeyboardState.IsKeyDown(Keys.P) && GameStateManager.CurrentState != PausedState
                && (!PreviousKeyBoardState.IsKeyDown(Keys.P) && !CurrentKeyboardState.IsKeyDown(Keys.LeftShift)
                || CurrentKeyboardState.IsKeyDown(Keys.LeftShift) && PreviousGameState != PausedState))
                    GameStateManager.PushState(PausedState);
            PreviousGameState = GameStateManager.CurrentState;
            GameStateManager.UpdateGameState(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// Displays an fps counter.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(backgroundImage, new Vector2((Width * Scale - backgroundImage.Width) / 2, (Height * Scale - backgroundImage.Height) / 2), Color.White);
            SpriteBatch.End();
            // TODO: Add your drawing code here
            Matrix matrix = Matrix.CreateScale(Scale, Scale, 0);
            SpriteBatch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointClamp);
            GameStateManager.DrawGameState(gameTime);
            SpriteBatch.End();
            timesIndex = (timesIndex + 1) % times.Length;
            double t = gameTime.TotalGameTime.TotalSeconds;
            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, $"fps: { times.Length / (t - times[timesIndex])}", Vector2.Zero, Color.White);
            times[timesIndex] = t;
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
