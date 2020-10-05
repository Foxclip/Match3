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
    /// Vector2, но с целыми координатами.
    /// </summary>
    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static bool operator ==(Vector2Int one, Vector2Int another)
        {
            return (one.x == another.x) && (one.y == another.y);
        }

        public static bool operator !=(Vector2Int one, Vector2Int another)
        {
            return !(one == another);
        }

        public static Vector2Int operator +(Vector2Int one, Vector2Int another)
        {
            return new Vector2Int(one.x + another.x, one.y + another.y);
        }
    }

    /// <summary>
    /// Объект на игровом поле.
    /// </summary>
    public class GameBoardObject
    {
        /// <summary>
        /// Позиция объекта на игровом поле.
        /// </summary>
        public Vector2Int pos;

        /// <summary>
        /// Спрайт объекта.
        /// </summary>
        public Texture2D sprite;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="pos">Начальная клетка игрового поля.</param>
        public GameBoardObject(Vector2Int pos)
        {
            this.pos = pos;
        }

        /// <summary>
        /// Рисует объект на экране.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Позиция спрайта на экране
            Vector2 screenPos = pos.ToVector2() * Game1.cellSize + Game1.gameBoardOffset;
            // Масштабирование спрайта
            float spriteScale = Game1.cellSize / sprite.Width * Game1.spriteDownScale;
            // Отрисовка спрайта
            spriteBatch.Draw(sprite, screenPos, null, Color.White, 0f, new Vector2(0, 0), spriteScale, SpriteEffects.None, 0f);
        }
    }

    /// <summary>
    /// Квадрат.
    /// </summary>
    public class SquareObject : GameBoardObject
    {
        public SquareObject(Vector2Int pos) : base(pos)
        {
            sprite = Game1.squareSprite;
        }
    }

    /// <summary>
    /// Круг.
    /// </summary>
    public class CircleObject : GameBoardObject
    {
        public CircleObject(Vector2Int pos) : base(pos)
        {
            sprite = Game1.circleSprite;
        }
    }

    /// <summary>
    /// Треугольник.
    /// </summary>
    public class TriangleObject : GameBoardObject
    {
        public TriangleObject(Vector2Int pos) : base(pos)
        {
            sprite = Game1.triangleSprite;
        }
    }

    /// <summary>
    /// Шестиугольник.
    /// </summary>
    public class HexagonObject : GameBoardObject
    {
        public HexagonObject(Vector2Int pos) : base(pos)
        {
            sprite = Game1.hexagonSprite;
        }
    }

    /// <summary>
    /// Ромб.
    /// </summary>
    public class DiamondObject : GameBoardObject
    {
        public DiamondObject(Vector2Int pos) : base(pos)
        {
            sprite = Game1.diamondSprite;
        }
    }
}
