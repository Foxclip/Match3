using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

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

        public static implicit operator Vector2(Vector2Int v2i)
        {
            return new Vector2(v2i.x, v2i.y);
        }

        public static explicit operator Vector2Int(Vector2 v2)
        {
            return new Vector2Int((int)v2.X, (int)v2.Y);
        }
    }
}
