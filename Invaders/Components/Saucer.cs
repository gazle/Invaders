using Invaders.GameStates;
using Microsoft.Xna.Framework;

namespace Invaders.Components
{
    class Saucer : Sprite, IExplodable
    {
        public Saucer(int dir, PlayingState playingState) : base("saucer", 0, playingState)
        {
            Position = new Vector2(dir > 0 ? 0 : MainGame.Width - Width, MainGame.SaucerY);
            Velocity = new Vector2(MainGame.SaucerSpeed * dir, 0);
        }

        protected override void Update(GameTime gametime)
        {
            base.Update(gametime);
            if (Position.X < 0 || Position.X > MainGame.Width - Width)
                OnRemove();
        }

        public void Explode(Sprite sprite)
        {
            OnExplode(sprite);
            OnRemove();
        }
    }
}
