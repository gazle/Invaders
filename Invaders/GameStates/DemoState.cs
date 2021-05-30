using Invaders.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Invaders.GameStates
{
    class DemoState : GameState
    {
        readonly Sprite monster;
        Vector2 offset;
        Vector2 yPosition;

        public DemoState()
        {
            monster = new Sprite("top", 0x10000);
            offset = new Vector2(6, 3);
            yPosition = new Vector2(120, 64);
        }

        void Initialize()
        {
            monster.Position = new Vector2(MainGame.Width + 100, 67);
            monster.Velocity.X = -1;
        }

        protected override void Update(GameTime gameTime)
        {
            if (monster.Position.X < 126)
                monster.Velocity.X = 1;
            monster.UpdateRoutine(gameTime);
            // TODO: Add your update logic here
            if (!Game.PreviousKeyBoardState.IsKeyDown(Keys.Enter) && Game.CurrentKeyboardState.IsKeyDown(Keys.Enter))
                GameStateManager.PushState(Game.PlayingState);
        }

        protected override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Draw(Game.TextureDict["saucer"], new Vector2(76, 124), Color.White);
            Game.SpriteBatch.Draw(Game.TextureDict["top1"], new Vector2(80, 139), Color.White);
            Game.SpriteBatch.Draw(Game.TextureDict["mid1"], new Vector2(79, 155), Color.White);
            Game.SpriteBatch.Draw(Game.TextureDict["bot1"], new Vector2(78, 171), Color.White);

            Game.SpriteBatch.DrawStringCentered("PLA", 64, Color.White);
            Game.SpriteBatch.DrawStringCentered("SPACE INVADERS", 80, Color.White);
            Game.SpriteBatch.DrawStringCentered("*SCORE ADVANCE TABLE*", 104, Color.White);
            Game.SpriteBatch.DrawString(Game.Font, "=? Mystery", new Vector2(96, 120), Color.White);
            Game.SpriteBatch.DrawString(Game.Font, "=30 POINTS", new Vector2(96, 136), Color.White);
            Game.SpriteBatch.DrawString(Game.Font, "=20 POINTS", new Vector2(96, 152), Color.White);
            Game.SpriteBatch.DrawString(Game.Font, "=10 POINTS", new Vector2(96, 168), Color.White);

            monster.DrawRoutine(gameTime);
            if (monster.Velocity.X > 0)
                Game.SpriteBatch.DrawString(Game.Font, "Y", yPosition, Color.White);
            else
                Game.SpriteBatch.DrawString(Game.Font, "Y", monster.Position - offset, Color.White);
        }

        public override void OnEntering(GameState oldState)
        {
            Initialize();
        }

        public override void OnRevealed(GameState oldState)
        {
            if (oldState is PlayingState)
                Initialize();
        }
    }
}
