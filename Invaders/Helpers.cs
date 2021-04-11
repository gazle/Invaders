using Invaders.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Invaders
{
    static class Helpers
    {
        public static void DrawStringCentered(this SpriteBatch spriteBatch, string message, float y, Color colour)
        {
            Vector2 measure = MainGame.Current.Font.MeasureString(message);
            spriteBatch.DrawString(MainGame.Current.Font, message, new Vector2((MainGame.Width - measure.X) / 2, y), colour);
        }

        public static void DrawStringRightJustified(this SpriteBatch spriteBatch, string message, Vector2 position, Color colour)
        {
            Vector2 measure = MainGame.Current.Font.MeasureString(message);
            spriteBatch.DrawString(MainGame.Current.Font, message, new Vector2(position.X - measure.X, position.Y), colour);
        }

        /// <summary>
        /// Perform <see cref="Action"/> on elements where IsRemoved == false.
        /// After the Action elements with IsRemoved == true are removed mid-iteration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEachWithRemoval<T>(this LinkedList<T> list, Action<T> action) where T : Component
        {
            list.ForEachWithRemoval(action, () => true);
        }

        /// <summary>
        /// Perform <see cref="Action"/> on elements where IsRemoved == false, while a conditon is met.
        /// After the Action elements with IsRemoved == true are removed mid-iteration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public static void ForEachWithRemoval<T>(this LinkedList<T> list, Action<T> action, Func<bool> condition) where T : Component
        {
            LinkedListNode<T> node = list.First, next;
            while (node != null && condition())
            {
                next = node.Next;
                if (!node.Value.IsRemoved)
                {
                    action(node.Value);
                    if (node.Value.IsRemoved)
                        list.Remove(node);
                }
                else
                {
                    list.Remove(node);
                }
                node = next;
            }
        }
    }
}
