using Invaders.GameStates;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Invaders.Components
{
    class Bullet : Sprite
    {
        Sprite sprite;

        public Bullet(Vector2 position, PlayingState playingState)
        : base("bullet", 0, position, playingState)
        {
        }

        protected override void Update(GameTime gametime)
        {
            Position.Y -= 4;
            base.Update(gametime);
            if ((sprite = ((PlayingState)GameState).Barricades.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                OnExplode(sprite);
                OnRemove();
            }
            else
            if ((sprite = ((PlayingState)GameState).Shots.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                OnExplode(sprite);
                OnRemove();
            }
            else
            if ((sprite = ((PlayingState)GameState).Grid.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                (sprite as Monster).Explode(this);
                OnRemove();
            }
            else
            if ((sprite = ((PlayingState)GameState).Saucers.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                (sprite as Saucer).Explode(this);
                OnRemove();
            }
            if (Position.Y <= MainGame.CeilingY)
                OnRemove();
        }
    }
}
