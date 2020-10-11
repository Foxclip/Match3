using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public class LineBonus : GameBoardObject
    {
        /// <summary>
        /// Метка бонуса.
        /// </summary>
        public Texture2D lineSprite;

        /// <summary>
        /// Направление, вертикальное или нет.
        /// </summary>
        public bool vertical;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="baseSprite">Спрайт базового объекта.</param>
        /// <param name="vertical">Направление бонуса.</param>
        /// <param name="pos">Клетка игрового поля.</param>
        /// <param name="spritePos">Позиция спрайта.</param>
        public LineBonus(GameBoardObject baseObject, bool vertical, Vector2Int pos, Vector2 spritePos) : base(pos, spritePos)
        {
            sprite = baseObject.sprite;
            spriteScale = baseObject.spriteScale;
            this.vertical = vertical;
            lineSprite = Game1.lineBonusSprite;
        }

        /// <summary>
        /// Рисует объект на экране.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Рисуем базовый спрайт
            base.Draw(spriteBatch);

            // Позиция спрайта на экране
            Vector2 spriteScreenPos = WorldToScreen(spriteWorldPos);
            // Центр метки бонуса
            Vector2 lineBonusOffset = new Vector2(lineSprite.Width / 2, lineSprite.Height / 2);
            // Отрисовка метки бонуса
            float rotation = vertical ? (float)Math.PI / 2 : 0f;
            spriteBatch.Draw(lineSprite, spriteScreenPos, null, Color.White, rotation, lineBonusOffset, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
