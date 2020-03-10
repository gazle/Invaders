using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Invaders
{
    class Animation
    {
        public Texture2D[] Textures;
        public int Speed;
        public Texture2D CurrentTexture => Textures[currentFrame];
        int currentFrame;
        int count;
        readonly int maxCount;

        public Animation(Texture2D[] textures, int speed)
        {
            Textures = textures;
            Speed = speed;
            maxCount = textures.Length << 16;
            currentFrame = 0;
        }

        public void StepAnimation()
        {
            count = (count + Speed) % maxCount;
            currentFrame = count >> 16;
        }

        public void DrawCurrentTexture(SpriteBatch spriteBatch, Vector2 position, Color tint)
        {
            spriteBatch.Draw(Textures[currentFrame], position, tint);
        }
    }
}
