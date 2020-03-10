using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Invaders.GameStates
{
    class PausedState : GameState
    {
        GameState oldState;

        void UpdateWaitForP(GameTime gameTime)
        {
            if (Game.CurrentKeyboardState.IsKeyDown(Keys.P))
            {
                GameStateManager.PopState();
                UpdateGameStateRoutine = Update;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (!Game.CurrentKeyboardState.IsKeyDown(Keys.P))
                UpdateGameStateRoutine = UpdateWaitForP;
        }

        protected override void Draw(GameTime gameTime)
        {
            oldState.DrawGameStateRoutine(gameTime);
            Game.SpriteBatch.DrawStringCentered("PAUSE", 100, Color.White);
        }

        public override void OnEntering(GameState oldState)
        {
            this.oldState = oldState;
        }
    }
}
