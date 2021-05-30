using Invaders.GameStates;
using Microsoft.Xna.Framework;
using System;

namespace Invaders.Components
{
    class Spawner : Component
    {
        public event Action SpawnedTick;
        PlayingState playingState;
        int framesToSaucer;

        public Spawner(PlayingState playingState)
        {
            this.playingState = playingState;
        }

        public void Initialize()
        {
            framesToSaucer = MainGame.SaucerFrequency;
        }

        protected override void Update(GameTime gameTime)
        {
            if (playingState.Grid.Count >= MainGame.MinSaucerSpawn && framesToSaucer-- == 0)
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
