using Invaders.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Invaders.Components
{
    class Player : Sprite
    {
        public event Action<Player> FirePressed;
        public int ExplodeCounter;
        readonly Animation aliveAnimation;
        readonly Animation deadAnimation;

        public Player(PlayingState playingState)
            : base("base", 0, playingState)
        {
            aliveAnimation = Animation;
            deadAnimation = new Animation(Game.AnimationDict["dead"], 0x4000);
        }

        public void Initialize()
        {
            Position = new Vector2(MainGame.PlayerLeftSide, MainGame.BaseY);
            ExplodeCounter = -1;        // Healthy
            Animation = aliveAnimation;
            UpdateRoutine = Update;
        }

        public void Explode(Sprite sprite)
        {
            if (UpdateRoutine == UpdateExploding)
                return;
            ExplodeCounter = MainGame.PlayerExplodeTime;
            if (sprite != null)
                ((PlayingState)GameState).LivesRemaining--;     // Landed
            else
                ((PlayingState)GameState).LivesRemaining = 0;
            Animation = deadAnimation;
            UpdateRoutine = UpdateExploding;
            Game.SoundDict["baseexp"].Play();
        }

        void UpdateExploding(GameTime gameTime)
        {
            ExplodeCounter--;
            base.Update(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Position.X > MainGame.PlayerLeftSide && Game.CurrentKeyboardState.IsKeyDown(Keys.Z))
                Position.X -= 1;
            if (Position.X < MainGame.PlayerRightSide && Game.CurrentKeyboardState.IsKeyDown(Keys.X))
                Position.X += 1;
            if (Game.CurrentKeyboardState.IsKeyDown(Keys.RightShift))
                FirePressed?.Invoke(this);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
