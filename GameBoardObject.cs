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
    public class GameBoardObject : GenericObject, IBoundingBox, IDrawable
    {
        public enum GameBoardObjectType
        {
            None,
            Square,
            Circle,
            Triangle,
            Hexagon,
            Diamond
        }
        /// <summary>
        /// Тип объекта.
        /// </summary>
        public GameBoardObjectType objectType;

        /// <summary>
        /// Позиция объекта на игровом поле.
        /// </summary>
        public Vector2Int worldPos;

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
        public static Vector2 WorldToScreen(Vector2 vector)
        {
            return vector * Game1.cellSize + Game1.gameBoardOffset;
        }

        /// <summary>
        /// Конвертирует координаты на экране в мировые координаты.
        /// </summary>
        public static Vector2 ScreenToWorld(int x, int y)
        {
            Vector2 vector = new Vector2(x, y);
            return (vector - Game1.gameBoardOffset) / Game1.cellSize;
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
            objectType = GameBoardObjectType.Square;
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
            objectType = GameBoardObjectType.Circle;
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
            objectType = GameBoardObjectType.Triangle;
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
            objectType = GameBoardObjectType.Hexagon;
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
            objectType = GameBoardObjectType.Diamond;
        }
    }
}
