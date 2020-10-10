using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public class Button : IBoundingBox, IDrawable
    {
        /// <summary>
        /// Спрайт кнопки.
        /// </summary>
        public Texture2D sprite;

        /// <summary>
        /// Позиция центра кнопки в экранных координатах.
        /// </summary>
        public Vector2 screenPos;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="sprite">Спрайт кнопки.</param>
        /// <param name="screenPos">Позиция кнопки в экранных координатах.</param>
        public Button(Texture2D sprite, Vector2 screenPos)
        {
            this.sprite = sprite;
            this.screenPos = screenPos;
        }

        /// <summary>
        /// Возвращает bounding box кнопки в экранных координатах.
        /// </summary>
        public Rectangle GetScreenBoundingBox()
        {
            int x = (int)(screenPos.X - sprite.Width / 2);
            int y = (int)(screenPos.Y - sprite.Height / 2);
            Rectangle boundingBox = new Rectangle(x, y, sprite.Width, sprite.Height);
            return boundingBox;
        }

        /// <summary>
        /// Отрисовка кнопки на экране.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spriteOffset = new Vector2(sprite.Width / 2, sprite.Height / 2);
            spriteBatch.Draw(sprite, screenPos, null, Color.White, 0f, spriteOffset, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
