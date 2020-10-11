using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public abstract class GenericObject
    {
        /// <summary>
        /// Спрайт объекта.
        /// </summary>
        public Texture2D sprite;

        /// <summary>
        /// Позиция спрайта.
        /// </summary>
        public Vector2 spriteWorldPos;

        /// <summary>
        /// Масштабирование спрайта.
        /// </summary>
        public float spriteScale = 1.0f;

        /// <summary>
        /// Множитель мвсштаба управляемый анимацией.
        /// </summary>
        public float spriteAnimatedScale = 1.0f;

        /// <summary>
        /// Рисует объект на экране.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Позиция спрайта на экране
            Vector2 spriteScreenPos = GameBoardObject.WorldToScreen(spriteWorldPos);
            // Центр спрайта
            Vector2 spriteOffset = new Vector2(sprite.Width / 2, sprite.Height / 2);
            // Масштабирование спрайта
            float finalSpriteScale = Game1.cellSize / sprite.Width * spriteScale * spriteAnimatedScale * Game1.globalSpriteScale;
            // Отрисовка спрайта
            spriteBatch.Draw(sprite, spriteScreenPos, null, Color.White, 0f, spriteOffset, finalSpriteScale, SpriteEffects.None, 0f);
        }
    }
}
