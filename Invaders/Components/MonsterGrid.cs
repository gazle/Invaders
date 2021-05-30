using Invaders.GameStates;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Invaders.Components
{
    class MonsterGrid : Component, IEnumerable<Monster>
    {
        public event Action<Monster> MonsterExploding;
        public event Action<string, Monster> ShotDropped;
        public PlayingState PlayingState;
        public int Count => Monsters.Count;
        const int separationX = 16;
        const int separationY = 8;
        readonly (int Score, int Rate)[] rateTable = new[] { (3000, 0x07), (2000, 0x08), (1000, 0x0B), (200, 0x10), (-1, 0x30) };
        readonly int[] plungerCols = new[] { 0, 6, 0, 0, 0, 3, 10, 0, 5, 2, 0, 0, 10, 8, 1, 7 };
        readonly int[] squiggleCols = new[] { 10, 0, 5, 2, 0, 0, 10, 8, 1, 7, 1, 10, 3, 6, 9 };
        readonly LinkedList<Monster> Monsters;
        LinkedListNode<Monster> currentMonster;
        public int CurrentXDirection;
        public int CurrentYDirection;
        int ShotFrequency => rateTable.First(o => o.Score < PlayingState.Score).Rate;
        int plungerIndex;
        int squiggleIndex;
        int shotCounter;
        int dnnCounter;
        int soundCounter;

        public MonsterGrid(PlayingState playingState)
        {
            PlayingState = playingState;
            Monsters = new LinkedList<Monster>();
        }

        public void Initialize()
        {
            Monsters.Clear();
            int startX = MainGame.LeftSide + 18;
            // First get the rackHeight element for the level we're on
            int startY = PlayingState.Level == 1 ? Game.RackHeights[0] : Game.RackHeights[(PlayingState.Level - 2) % 8 + 1];
            // Convert it to the Y coordinate for the bottom row of the grid
            startY = MainGame.BaseY - separationY * startY;
            for (int row = 0; row < 2; row++)
                for (int col = 0; col < 11; col++)
                    Monsters.AddLast(new Monster("bot", new Vector2(startX + col * separationX, startY - row * separationY * 2), row, col, 10, Color.Yellow, this));
            for (int row = 2; row < 4; row++)
                for (int col = 0; col < 11; col++)
                    Monsters.AddLast(new Monster("mid", new Vector2(startX + col * separationX + 1, startY - row * separationY * 2), row, col, 20, Color.Blue, this));
            for (int row = 4; row < 5; row++)
                for (int col = 0; col < 11; col++)
                    Monsters.AddLast(new Monster("top", new Vector2(startX + col * separationX + 2, startY - row * separationY * 2), row, col, 30, Color.Red, this));
            // Receive Removing events from each Monster
            foreach (Monster m in Monsters)
            {
                m.Exploding += Exploding;
                m.Removing += RemoveMonster;
            }
            currentMonster = null;
            CurrentXDirection = 2;
            CurrentYDirection = 0;
            plungerIndex = 0;
            squiggleIndex = 0;
            shotCounter = 0;
            soundCounter = 50;
        }

        void Exploding(Sprite monster, Sprite bullet)
        {
            // Bubble it
            MonsterExploding?.Invoke((Monster)monster);
        }

        void RemoveMonster(Component m)
        {
            // If we are removing the currentMonster then move currentMonster to the next one so that the node is valid
            if (m == currentMonster?.Value)
                currentMonster = currentMonster.Next;
            Monsters.Remove((Monster)m);
        }

        void TryDropPlunger()
        {
            int col = plungerCols[plungerIndex];
            plungerIndex = (plungerIndex + 1) % plungerCols.Length;
            Monster m = Monsters.FirstOrDefault(o => o.Col == col);
            if (m != null)
                ShotDropped?.Invoke("plunger", m);
        }

        void TryDropSquiggle()
        {
            int col = squiggleCols[squiggleIndex];
            squiggleIndex = (squiggleIndex + 1) % squiggleCols.Length;
            Monster m = Monsters.FirstOrDefault(o => o.Col == col);
            if (m != null)
                ShotDropped?.Invoke("squiggle", m);
        }

        void TryDropScrew()
        {
            int x = (int)PlayingState.Player.Position.X + (PlayingState.Player.Width - 1) / 2;
            Monster m = Monsters.FirstOrDefault(o => (uint)(x - o.Position.X + 2) < o.Width + 4);       // Give Monster 2 pixels berth either side
            if (m != null)
            {
                m = Monsters.First(o => o.Col == m.Col);    // Ensure the bottom one of the column
                ShotDropped?.Invoke("screw", m);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (PlayingState.Player.ExplodeCounter != -1 || Monsters.Count == 0)
                return;
            shotCounter++;
            if (PlayingState.Shots.Select(o => o.Age).DefaultIfEmpty(1000).Min() >= ShotFrequency)
                switch (shotCounter % 3)
                {
                    case 0: if (!PlayingState.Shots.Any(o => o.Name == "plunger")) TryDropPlunger(); break;
                    case 1: if (!PlayingState.Shots.Any(o => o.Name == "squiggle")) TryDropSquiggle(); break;
                    case 2: if (!PlayingState.Shots.Any(o => o.Name == "screw")) TryDropScrew(); break;
                }
            if (PlayingState.Explosions.Any(o => o.Name == "explosion"))
                return;
            if (soundCounter-- == 0)
            {
                Game.SoundDict["dnn" + dnnCounter.ToString()].Play();
                soundCounter = Count + 3;
                dnnCounter = (dnnCounter + 1) % 4;
            }

            // When starting a new sheet move check if we need to descend and reverse direction
            if (currentMonster == null)
            {
                if (CurrentXDirection < 0 && Monsters.Any((o) => o.Position.X <= MainGame.LeftSide))
                {
                    CurrentXDirection = Monsters.Count == 1 ? 3 : 2;
                    CurrentYDirection = separationY;
                }
                else
                if (CurrentXDirection > 0 && Monsters.Any((o) => o.Position.X > MainGame.RightSide))
                {
                    CurrentXDirection = -2;
                    CurrentYDirection = separationY;
                }
                else
                    CurrentYDirection = 0;
                currentMonster = Monsters.First;
            }
            // move the current invader only, could be null when sheet cleared
            currentMonster?.Value.UpdateRoutine(gameTime);
            // Iterate the currentMonster, will be null when no invaders left, Next is null after the last one
            currentMonster = currentMonster?.Next;
        }

        protected override void Draw(GameTime gameTime)
        {
            Monsters.ForEachWithRemoval(o => o.DrawRoutine(gameTime));
        }

        public IEnumerator<Monster> GetEnumerator()
        {
            return Monsters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Monsters.GetEnumerator();
        }
    }
}
