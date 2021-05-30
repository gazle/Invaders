using Invaders.GameStates;
using Microsoft.Xna.Framework;

namespace Invaders.Components
{
    class Explosion : Sprite
    {
        int toLive;

        public Explosion(string animKey, Vector2 position, int lifeTime) : this(animKey, position, lifeTime, Color.White)
        {
        }

        public Explosion(string animKey, Vector2 position, int lifeTime, Color color) : base(animKey, 0, position, color)
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
