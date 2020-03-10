using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Invaders.GameStates
{
    class GameOverState : GameState
    {
        GameState oldState;

        protected override void Update(GameTime gameTime)
        {
            if (Game.CurrentKeyboardState.IsKeyDown(Keys.Enter))
            {
                GameStateManager.PopState();       // Pop this state
                GameStateManager.PopState();       // Pop the PlayingState underneath
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            oldState.DrawGameStateRoutine(gameTime);
            Game.SpriteBatch.DrawStringCentered("GAME OVER", 48, Color.White);
        }

        public override void OnEntering(GameState oldState)
        {
            this.oldState = oldState;
            Game.PlayingState.HighScore = Game.PlayingState.Score > Game.PlayingState.HighScore ? Game.PlayingState.Score : Game.PlayingState.HighScore;
        }
    }
}
