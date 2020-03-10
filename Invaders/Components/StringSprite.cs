using Invaders.GameStates;
using Microsoft.Xna.Framework;

namespace Invaders.Components
{
    // Used to briefly display the Saucer score
    class StringSprite : Component
    {
        public string Message;
        public Vector2 Position;
        int toLive;

        public StringSprite(Vector2 position, string message, int lifeTime, PlayingState playingState) : base(playingState)
        {
            Position = position;
            Message = message;
            toLive = lifeTime;
        }

        protected override void Update(GameTime gameTime)
        {
            if (toLive-- <= 0)
                OnRemove();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.DrawString(Game.Font, Message, Position, Color.White);
            base.Draw(gameTime);
        }
    }
}
