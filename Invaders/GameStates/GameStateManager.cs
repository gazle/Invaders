using Invaders.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Invaders.GameStates
{
    static class GameStateManager
    {
        static readonly Stack<GameState> statesStack = new Stack<GameState>();
        static public GameState CurrentState => statesStack.Peek();

        static public void PushState(GameState newState)
        {
            GameState oldState = statesStack.Count > 0 ? statesStack.Peek() : null;
            oldState?.OnObscuring(newState);
            newState.OnEntering(oldState);
            statesStack.Push(newState);
        }

        static public void PopState()
        {
            GameState oldState = statesStack.Pop();
            GameState newState = statesStack.Count > 0 ? statesStack.Peek() : null;
            oldState.OnLeft(newState);
            newState?.OnRevealed(oldState);
        }

        static public void UpdateGameState(GameTime gameTime)
        {
            statesStack.Peek().UpdateGameStateRoutine(gameTime);
        }

        static public void DrawGameState(GameTime gameTime)
        {
            statesStack.Peek().DrawGameStateRoutine(gameTime);
        }
    }

    abstract class GameState
    {
        public MainGame Game;
        public Routine UpdateGameStateRoutine;
        public Routine DrawGameStateRoutine;

        public GameState()
        {
            Game = MainGame.Current;
            UpdateGameStateRoutine = Update;
            DrawGameStateRoutine = Draw;
        }

        protected abstract void Update(GameTime gameTime);
        protected abstract void Draw(GameTime gameTime);

        /// <summary>
        /// We are about to be pushed onto the StateStack
        /// </summary>
        /// <param name="oldState">The state we're obscuring</param>
        public virtual void OnEntering(GameState oldState) { }

        /// <summary>
        /// We have been popped off the StateStack
        /// </summary>
        /// <param name="newState"></param>
        public virtual void OnLeft(GameState newState) { }

        /// <summary>
        /// A new GameState is about to be pushed on top of us
        /// </summary>
        /// <param name="newState"></param>
        public virtual void OnObscuring(GameState newState) { }

        /// <summary>
        /// The GamesState above us has just been popped off the StateStack
        /// </summary>
        /// <param name="oldState"></param>
        public virtual void OnRevealed(GameState oldState) { }
    }
}
