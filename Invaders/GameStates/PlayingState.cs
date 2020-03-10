using Invaders.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Invaders.GameStates
{
    class PlayingState : GameState
    {
        public Spawner Spawner;
        public MonsterGrid Grid;
        public Player Player;
        public LinkedList<Bullet> Bullets;
        public LinkedList<Explosion> Explosions;
        public LinkedList<Shot> Shots;
        public LinkedList<Barricade> Barricades;
        public LinkedList<Saucer> Saucers;
        public LinkedList<StringSprite> Messages;
        public Sprite Line;
        public int LivesRemaining;
        public int Level;
        int endSheetCountdown;
        int bulletsFired;
        readonly int[] saucerScores = new[] { 100, 100, 50, 50, 100, 150, 100, 100, 50, 300, 100, 100, 100, 50, 150 };

        int score;
        public int Score
        {
            get { return score; }
            set
            {
                if (score < 1500 && value >= 1500)
                    LivesRemaining++;
                score = value;
            }
        }
        public int HighScore;

        public PlayingState()
        {
            HighScore = 1200;
            Spawner = new Spawner(this);
            Grid = new MonsterGrid(this);
            Player = new Player(this);
            Bullets = new LinkedList<Bullet>();
            Explosions = new LinkedList<Explosion>();
            Shots = new LinkedList<Shot>();
            Barricades = new LinkedList<Barricade>();
            Saucers = new LinkedList<Saucer>();
            Messages = new LinkedList<StringSprite>();
            Line = new Sprite("line", 0, new Vector2(0, MainGame.LineY), this);
            Player.FirePressed += FireBullet;
            Grid.MonsterExploding += CreateMonsterExplosion;
            Grid.ShotDropped += DropShot;
            Spawner.SpawnedTick += SpawnSaucer;
            for (int i = 0; i < 4; i++)
                Barricades.AddLast(new Barricade("barricade", new Vector2(32 + i * 45, MainGame.BarricadeY), this));
        }

        void Initialize()
        {
            Bullets.Clear();
            Explosions.Clear();
            Shots.Clear();
            Saucers.Clear();
            Level = 0;
            Score = 0;
            LivesRemaining = 3;
            Color[] lineData = Game.TextureData[Game.TextureDict["line"]];
            for (int i = 0; i < lineData.Length; i++)
                lineData[i] = Color.LightGreen;
            Game.TextureDict["line"].SetData(lineData);
            NewSheet();
        }

        void NewSheet()
        {
            Level++;
            Grid.Initialize();
            Player.Initialize();
            Spawner.Initialize();
            bulletsFired = 0;
            foreach (Barricade b in Barricades)
                b.Initialize();
            endSheetCountdown = 100;
        }

        #region Component Event Handlers
        void FireBullet(Player player)
        {
            if (Explosions.Count(o => o.Name == "bulletexplosion" || o.Name == "explosion") + Bullets.Count >= MainGame.MaxBullets || Grid.Count == 0)
                return;
            bulletsFired++;
            Bullet b = new Bullet(Player.Position + new Vector2(6, 0), this);
            Bullets.AddLast(b);
            b.Exploding += CreateBulletExplosion;
            Game.SoundDict["laser"].Play();
        }

        void DropShot(string animKey, Monster monster)
        {
            Texture2D t = Game.AnimationDict[animKey][0];
            // Position the shot centered and underneath the Sprite
            Shot shot = new Shot(animKey, monster.Position + new Vector2((monster.Width - t.Width) / 2, monster.Height + 4), this);
            shot.Exploding += CreateShotExplosionDamage;
            Shots.AddLast(shot);
        }

        void CreateMonsterExplosion(Monster monster)
        {
            // Sprite is the object we're exploding so we can calculate the relative position
            Score += monster.Score;
            Texture2D t = Game.TextureDict["explosion"];
            Explosions.AddLast(new Explosion("explosion", monster.Position + new Vector2((monster.Width - t.Width) / 2, (monster.Height - t.Height) / 2), 16, monster.Tint, this));
            Game.SoundDict["splat"].Play();
        }

        void CreateBulletExplosion(Sprite bullet, Sprite sprite)
        {
            Explosion exp = new Explosion("bulletexplosion", bullet.Position + new Vector2(-3, -2), 16, this);
            Explosions.AddLast(exp);
            if (sprite is Barricade)
                exp.PlotMask(sprite);
        }

        void CreateShotExplosionDamage(Sprite shot, Sprite sprite)
        {
            // Create Explosion and damage the Barricade and baseline
            Explosion exp = new Explosion("shotexplosion", shot.Position + new Vector2(-2, 0), 16, this);
            Explosions.AddLast(exp);
            if (sprite.Name == "line")
                exp.PlotMask(sprite);
            if (sprite is Barricade)
                exp.PlotMask(sprite);
        }

        void SpawnSaucer()
        {
            int dir = bulletsFired % 2 == 0 ? 1 : -1;
            Saucer saucer = new Saucer(dir, this);
            saucer.Removing += SaucerRemoved;
            saucer.Exploding += CreateSaucerExplosion;
            Saucers.AddLast(saucer);
            Game.SoundDict["saucer"].Play();
        }

        void SaucerRemoved(Component saucer)
        {
            Game.SoundDict["saucer"].Stop();
        }

        void CreateSaucerExplosion(Sprite s, Sprite b)
        {
            Texture2D t = Game.TextureDict["saucerexplosion"];
            Explosion exp = new Explosion("saucerexplosion", s.Position + new Vector2((s.Width - t.Width) / 2, (s.Height - t.Height) / 2), 30, s.Tint, this);
            exp.Removing += AddSaucerScore;     // When the Saucer explosion is done show its score
            Explosions.AddLast(exp);
            Game.SoundDict["saucersplat"].Play();
        }

        void AddSaucerScore(Component e)
        {
            Explosion exp = (Explosion)e;
            int s = saucerScores[bulletsFired % 15];
            Score += s;
            string message = s.ToString();
            Vector2 box = Game.Font.MeasureString(message);
            Messages.AddLast(new StringSprite(exp.Position + new Vector2((exp.Width - box.X) / 2, (exp.Height - box.Y) / 2), message, 60, this));
        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            Grid.UpdateRoutine(gameTime);
            Player.UpdateRoutine(gameTime);
            Bullets.ForEachWithRemoval(o => o.UpdateRoutine(gameTime));
            Explosions.ForEachWithRemoval(o => o.UpdateRoutine(gameTime));
            Shots.ForEachWithRemoval(o => o.UpdateRoutine(gameTime));
            Barricades.ForEachWithRemoval(o => o.UpdateRoutine(gameTime));
            Saucers.ForEachWithRemoval(o => o.UpdateRoutine(gameTime));
            Messages.ForEachWithRemoval(o => o.UpdateRoutine(gameTime));
            Spawner.UpdateRoutine(gameTime);

            if (Player.ExplodeCounter == 0)
            {
                if (LivesRemaining <= 0)
                {
                    GameStateManager.PushState(Game.GameOverState);        // Next Draw will be in the GameOverState
                    return;
                }
                Player.Initialize();
            }
            if (Grid.Count == 0 && endSheetCountdown-- <= 0 && Player.ExplodeCounter == -1)
                NewSheet();
            if (Game.CurrentKeyboardState.IsKeyDown(Keys.R))
                GameStateManager.PopState();
        }

        protected override void Draw(GameTime gameTime)
        {
            Shots.ForEachWithRemoval(o => o.DrawRoutine(gameTime));
            Grid.DrawRoutine(gameTime);
            Player.DrawRoutine(gameTime);
            Bullets.ForEachWithRemoval(o => o.DrawRoutine(gameTime));
            Explosions.ForEachWithRemoval(o => o.DrawRoutine(gameTime));
            Barricades.ForEachWithRemoval(o => o.DrawRoutine(gameTime));
            Saucers.ForEachWithRemoval(o => o.DrawRoutine(gameTime));
            Messages.ForEachWithRemoval(o => o.DrawRoutine(gameTime));
            Line.DrawRoutine(gameTime);

            for (int i = 1; i <= LivesRemaining; i++)
                Game.SpriteBatch.Draw(Game.AnimationDict["base"][0], new Vector2(i * 16 + 4, MainGame.LivesY), Color.White);
            Game.SpriteBatch.DrawString(Game.Font, "SCORE", new Vector2(48, 8), Color.White);
            Game.SpriteBatch.DrawStringRightJustified(Score.ToString(), new Vector2(76, 24), Color.White);
            Game.SpriteBatch.DrawString(Game.Font, "HI-SCORE", new Vector2(144, 8), Color.White);
            Game.SpriteBatch.DrawStringRightJustified(HighScore.ToString(), new Vector2(176, 24), Color.White);
            Game.SpriteBatch.DrawString(Game.Font, LivesRemaining.ToString(), new Vector2(8, 237), Color.White);
        }

        public override void OnEntering(GameState oldState)
        {
            Initialize();
        }

        public override void OnObscuring(GameState newState)
        {
            if (newState is GameOverState)
                Game.SoundDict["saucer"].Stop();
            else
                Game.SoundDict["saucer"].Pause();
        }

        public override void OnRevealed(GameState oldState)
        {
            Game.SoundDict["saucer"].Resume();
        }

        public override void OnLeft(GameState newState)
        {
            Game.SoundDict["saucer"].Stop();
        }
    }
}
