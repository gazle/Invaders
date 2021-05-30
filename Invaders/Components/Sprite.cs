using Invaders.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Invaders.Components
{
    class Sprite : Component
    {
        public event Action<Sprite, Sprite> Exploding;

        public Vector2 Position;
        public Vector2 Velocity;

        protected Animation Animation;
        public Color Tint;
        public int Width => Animation.CurrentTexture.Width;
        public int Height => Animation.CurrentTexture.Height;
        public int Left => (int)Position.X;
        public int Right => (int)Position.X + Width;
        public int Top => (int)Position.Y;
        public int Bottom => (int)Position.Y + Height;

        public Sprite(string animKey, int animSpeed) : this(animKey, animSpeed, Vector2.Zero)
        {
        }

        public Sprite(string animKey, int animSpeed, Vector2 position) : this(animKey, animSpeed, position, Color.White)
        {
        }

        public Sprite(string animKey, int animSpeed, Vector2 position, Color tint)
        {
            Name = animKey;
            Texture2D[] textures = Game.AnimationDict[animKey];
            Animation = new Animation(textures, animSpeed);
            Tint = tint;
            Position = position;
        }

        protected virtual Color[] GetPixelData()
        {
            // Base implementation gets the data from the main dictionary
            return Game.TextureData[Animation.CurrentTexture];
        }

        protected virtual void SetPixelData(Color[] data)
        {
            // Rewrite new pixeldata into the current texture
            Animation.CurrentTexture.SetData(data);
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="sprite2">Second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public bool IntersectPixels(Sprite sprite2)
        {
            if ((uint)(Position.X - sprite2.Position.X + Width - 1) >= Width + sprite2.Width - 1 ||
                    (uint)(Position.Y - sprite2.Position.Y + Height - 1) >= Height + sprite2.Height - 1)
                return false;
            Color[] dataA = GetPixelData();
            Color[] dataB = sprite2.GetPixelData();
            // Find the bounds of the rectangle intersection
            int top = Math.Max(Top, sprite2.Top);
            int bottom = Math.Min(Bottom, sprite2.Bottom);
            int left = Math.Max(Left, sprite2.Left);
            int right = Math.Min(Right, sprite2.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - Left) + (y - Top) * Width];
                    Color colorB = dataB[(x - sprite2.Left) + (y - sprite2.Top) * sprite2.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Draws the non-transparent pixels from sprite1 as transparent into sprite2
        /// </summary>
        /// <param name="sprite2">Second sprite</param>
        /// <returns>True if there was a non-transparent pixel collision</returns>
        public bool PlotMask(Sprite sprite2)
        {
            if ((uint)(Position.X - sprite2.Position.X + Width - 1) >= Width + sprite2.Width - 1 ||
                    (uint)(Position.Y - sprite2.Position.Y + Height - 1) >= Height + sprite2.Height - 1)
                return false;
            Color[] dataA = GetPixelData();
            Color[] dataB = sprite2.GetPixelData();
            // Find the bounds of the rectangle intersection
            int top = Math.Max(Top, sprite2.Top);
            int bottom = Math.Min(Bottom, sprite2.Bottom);
            int left = Math.Max(Left, sprite2.Left);
            int right = Math.Min(Right, sprite2.Right);
            bool collision = false;

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Plot colorA into rectangleB
                    Color colorA = dataA[(x - Left) + (y - Top) * Width];
                    Color colorB = dataB[(x - sprite2.Left) + (y - sprite2.Top) * sprite2.Width];
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        collision = true;
                        dataB[(x - sprite2.Left) + (y - sprite2.Top) * sprite2.Width] = Color.Transparent;
                    }
                }
            }
            if (collision)
                sprite2.SetPixelData(dataB);

            return collision;
        }

        protected override void Update(GameTime gametime)
        {
            Position += Velocity;
            Animation.StepAnimation();
        }

        protected override void Draw(GameTime gameTime)
        {
            Animation.DrawCurrentTexture(Game.SpriteBatch, Position, Tint);
        }

        /// <param name="sprite">The Sprite we're exploding with.</param>
        protected virtual void OnExplode(Sprite sprite)
        {
            Exploding?.Invoke(this, sprite);
        }
    }
}
