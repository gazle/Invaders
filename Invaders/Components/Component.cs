using Invaders.GameStates;
using Microsoft.Xna.Framework;
using System;

namespace Invaders.Components
{
    delegate void Routine(GameTime gameTime);

    /// <summary>
    /// Encapulates a Game object with Update and Draw routines.
    /// The UpdateRoutine and DrawRoutine delegates can be rerouted to change the behaviour.
    /// </summary>
    class Component
    {
        public string Name;
        public MainGame Game;
        public GameState GameState;
        public bool IsRemoved;
        public Routine UpdateRoutine;
        public Routine DrawRoutine;
        public event Action<Component> Removing;

        public Component(GameState gameState)
        {
            Game = MainGame.Current;
            GameState =  gameState;
            UpdateRoutine = Update;
            DrawRoutine = Draw;
        }

        protected virtual void Update(GameTime gameTime) { }
        protected virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// Ensures this <see cref="Component"/> will be removed by the engine after Update returns.
        /// </summary>
        protected virtual void OnRemove()
        {
            IsRemoved = true;
            Removing?.Invoke(this);
        }
    }
}
