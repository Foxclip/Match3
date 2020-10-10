using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Match3
{
    /// <summary>
    /// Объект на игровом поле.
    /// </summary>
    public class GameBoardObject : IBoundingBox, IDrawable
    {
        /// <summary>
        /// Позиция объекта на игровом поле.
        /// </summary>
        public Vector2Int worldPos;

        /// <summary>
        /// Спрайт объекта.
        /// </summary>
        public Texture2D sprite;

        /// <summary>
        /// Позиция спрайта. Если отличается от позиции объекта, спрайт начинает перемещаться в позицию объекта.
        /// </summary>
        public Vector2 spriteWorldPos;

        /// <summary>
        /// Масштабирование спрайта.
        /// </summary>
        public float spriteScale;

        /// <summary>
        /// Множитель мвсштаба управляемый анимацией.
        /// </summary>
        public float spriteAnimatedScale = 1.0f;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="pos">Клетка игрового поля.</param>
        public GameBoardObject(Vector2Int pos, Vector2 spritePos)
        {
            worldPos = pos;
            spriteWorldPos = spritePos;
        }

        /// <summary>
        /// Конвертирует координаты на игровом поле в экранные координаты.
        /// </summary>
        public Vector2 WorldToScreen(Vector2 vector)
        {
            return vector * Game1.cellSize + Game1.gameBoardOffset;
        }

        /// <summary>
        /// Конвертирует координаты на экране в мировые координаты.
        /// </summary>
        public Vector2 ScreenToWorld(int x, int y)
        {
            Vector2 vector = new Vector2(x, y);
            return (vector - Game1.gameBoardOffset) / Game1.cellSize;
        }

        /// <summary>
        /// Рисует объект на экране.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Позиция спрайта на экране
            Vector2 spriteScreenPos = WorldToScreen(spriteWorldPos);
            // Центр спрайта
            Vector2 spriteOffset = new Vector2(sprite.Width / 2, sprite.Height / 2);
            // Масштабирование спрайта
            float finalSpriteScale = Game1.cellSize / sprite.Width * spriteScale * spriteAnimatedScale * Game1.globalSpriteScale;
            // Отрисовка спрайта
            spriteBatch.Draw(sprite, spriteScreenPos, null, Color.White, 0f, spriteOffset, finalSpriteScale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Возвращает bounding box объекта в экранных координатах.
        /// </summary>
        public Rectangle GetScreenBoundingBox()
        {
            Vector2 spriteScreenPos = WorldToScreen(spriteWorldPos);
            float finalSpriteScale = Game1.cellSize / sprite.Width;
            int spriteScreenSize = (int)(sprite.Width * finalSpriteScale);
            int x = (int)(spriteScreenPos.X - spriteScreenSize / 2);
            int y = (int)(spriteScreenPos.Y - spriteScreenSize / 2);
            Rectangle boundingBox = new Rectangle(x, y, spriteScreenSize, spriteScreenSize);
            return boundingBox;
        }

    }

    /// <summary>
    /// Квадрат.
    /// </summary>
    public class SquareObject : GameBoardObject
    {
        public SquareObject(Vector2Int pos, Vector2 spritePos) : base(pos, spritePos)
        {
            sprite = Game1.squareSprite;
            spriteScale = 0.8f;
        }
    }

    /// <summary>
    /// Круг.
    /// </summary>
    public class CircleObject : GameBoardObject
    {
        public CircleObject(Vector2Int pos, Vector2 spritePos) : base(pos, spritePos)
        {
            sprite = Game1.circleSprite;
            spriteScale = 0.9f;
        }
    }

    /// <summary>
    /// Треугольник.
    /// </summary>
    public class TriangleObject : GameBoardObject
    {
        public TriangleObject(Vector2Int pos, Vector2 spritePos) : base(pos, spritePos)
        {
            sprite = Game1.triangleSprite;
            spriteScale = 0.8f;
        }
    }

    /// <summary>
    /// Шестиугольник.
    /// </summary>
    public class HexagonObject : GameBoardObject
    {
        public HexagonObject(Vector2Int pos, Vector2 spritePos) : base(pos, spritePos)
        {
            sprite = Game1.hexagonSprite;
            spriteScale = 0.9f;
        }
    }

    /// <summary>
    /// Ромб.
    /// </summary>
    public class DiamondObject : GameBoardObject
    {
        public DiamondObject(Vector2Int pos, Vector2 spritePos) : base(pos, spritePos)
        {
            sprite = Game1.diamondSprite;
            spriteScale = 0.9f;
        }
    }
}
