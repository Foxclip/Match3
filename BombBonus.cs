using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public class BombBonus : GameBoardObject
    {
        /// <summary>
        /// Метка бонуса.
        /// </summary>
        public Texture2D bombSprite;

        /// <summary>
        /// Масштаб спрайта бомбы.
        /// </summary>
        public float bombSpriteScale = 0.3f;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="baseObject">Базовый объект.</param>
        /// <param name="pos">Клетка игрового поля.</param>
        /// <param name="spritePos">Позиция спрайта.</param>
        public BombBonus(GameBoardObject baseObject, Vector2Int pos, Vector2 spritePos) : base(pos, spritePos)
        {
            sprite = baseObject.sprite;
            spriteScale = baseObject.spriteScale;
            objectType = baseObject.objectType;
            bombSprite = Game1.bombSprite;
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
            float finalBombSpriteScale = bombSpriteScale * spriteAnimatedScale;
            // Центр метки бонуса
            Vector2 bombBonusOffset = new Vector2(bombSprite.Width / 2, bombSprite.Height / 2);
            // Отрисовка метки бонуса
            spriteBatch.Draw(bombSprite, spriteScreenPos, null, Color.White, 0.0f, bombBonusOffset, finalBombSpriteScale, SpriteEffects.None, 0f);
        }
    }
}
