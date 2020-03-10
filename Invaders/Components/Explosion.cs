using Invaders.GameStates;
using Microsoft.Xna.Framework;

namespace Invaders.Components
{
    class Explosion : Sprite
    {
        int toLive;

        public Explosion(string animKey, Vector2 position, int lifeTime, PlayingState playingState) : this(animKey, position, lifeTime, Color.White, playingState)
        {
        }

        public Explosion(string animKey, Vector2 position, int lifeTime, Color color, PlayingState playingState) : base(animKey, 0, position, color, playingState)
        {
            toLive = lifeTime;
        }

        protected override void Update(GameTime gametime)
        {
            if (toLive-- <= 0)
                OnRemove();
            base.Update(gametime);
        }
    }
}
