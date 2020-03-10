using Invaders.GameStates;
using Microsoft.Xna.Framework;
using System;

namespace Invaders.Components
{
    class Spawner : Component
    {
        public event Action SpawnedTick;
        int framesToSaucer;

        public Spawner(PlayingState playingState) : base(playingState)
        {
        }

        public void Initialize()
        {
            framesToSaucer = MainGame.SaucerFrequency;
        }

        protected override void Update(GameTime gameTime)
        {
            if (((PlayingState)GameState).Grid.Count >= MainGame.MinSaucerSpawn && framesToSaucer-- == 0)
            {
                framesToSaucer = MainGame.SaucerFrequency;
                OnSpawnedTick();
            }
        }

        protected virtual void OnSpawnedTick()
        {
            SpawnedTick?.Invoke();
        }
    }
}
