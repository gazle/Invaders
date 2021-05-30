using Invaders.GameStates;
using Microsoft.Xna.Framework;

namespace Invaders.Components
{
    class Monster : Sprite, IExplodable
    {
        readonly MonsterGrid grid;
        public readonly int Row;
        public readonly int Col;
        public readonly int Score;

        public Monster(string animKey, Vector2 position, int row, int col, int score, Color color, MonsterGrid parentGrid)
            : base(animKey, 0x10000, position, color)
        {
            grid = parentGrid;
            Row = row;
            Col = col;
            Score = score;
            UpdateRoutine = UpdatePre;
            DrawRoutine = g => { };         // Not drawn before first update
        }

        void UpdatePre(GameTime gameTime)
        {
            UpdateRoutine = Update;
            DrawRoutine = Draw;
        }

        protected override void Update(GameTime gameTime)
        {
            Position.X += grid.CurrentXDirection;
            Position.Y += grid.CurrentYDirection;
            base.Update(gameTime);
            if (Position.Y == MainGame.BaseY)
                // Landed
                grid.PlayingState.Player.Explode(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Draw invader's mask over the barricade
            foreach (Barricade b in grid.PlayingState.Barricades)
                PlotMask(b);
            base.Draw(gameTime);
        }

        public void Explode(Sprite sprite)
        {
            OnExplode(sprite);      // Grid subscribes to Exploding and bubbles it
            OnRemove();             // Grid subscribes to the Removing event
        }
    }
}
