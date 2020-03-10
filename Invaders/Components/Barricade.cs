using Invaders.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Invaders.Components
{
    class Barricade : Sprite
    {
        readonly Texture2D[] textures;

        public Barricade(string animKey, Vector2 position, PlayingState playingState)
        : base(animKey, 0, position, playingState)
        {
            // Give each Barricade its own Textures, not the shared array from AnimDict
            textures = new[] { new Texture2D(Game.GraphicsDevice, 22, 16) };
            // Overwrite the Animation that base creates
            Animation = new Animation(textures, 0);
        }

        ~Barricade()
        {
            textures[0].Dispose();
        }

        public void Initialize()
        {
            // Restore the barricade
            Texture2D t = Game.TextureDict["barricade"];
            textures[0].SetData(Game.TextureData[t]);
        }

        protected override Color[] GetPixelData()
        {
            // Get from our local copy
            Color[] data = new Color[Width * Height];
            textures[0].GetData(data);
            return data;
        }
    }
}
