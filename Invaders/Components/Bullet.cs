using Invaders.GameStates;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Invaders.Components
{
    class Bullet : Sprite
    {
        Sprite sprite;
        PlayingState playingState;

        public Bullet(Vector2 position, PlayingState playingState) : base("bullet", 0, position)
        {
            this.playingState = playingState;
        }

        protected override void Update(GameTime gametime)
        {
            Position.Y -= 4;
            base.Update(gametime);
            if ((sprite = playingState.Barricades.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                // Let the PlayingState subscribe to Exploding and handle explosion creation, sound, scoring etc.
                OnExplode(sprite);
                OnRemove();
            }
            else
            if ((sprite = playingState.Shots.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                OnExplode(sprite);
                OnRemove();
            }
            else
            if ((sprite = playingState.Grid.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                // Here we explode the Monster and remove the bullet
                (sprite as Monster).Explode(this);
                OnRemove();
            }
            else
            if ((sprite = playingState.Saucers.FirstOrDefault(o => IntersectPixels(o))) != null)
            {
                (sprite as Saucer).Explode(this);
                OnRemove();
            }
            if (Position.Y <= MainGame.CeilingY)
                OnRemove();
        }
    }
}
