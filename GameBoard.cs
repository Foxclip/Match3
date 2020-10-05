using System;
using System.Collections.Generic;
using System.Text;

namespace Match3
{
    /// <summary>
    /// Игровое поле.
    /// </summary>
    public class GameBoard
    {
        /// <summary>
        /// Список объектов на игровом поле.
        /// </summary>
        public readonly List<GameBoardObject> objectList = new List<GameBoardObject>();

        /// <summary>
        /// Генератор случайных чисел.
        /// </summary>
        public readonly Random random = new Random();

        public GameBoard()
        {
            // Создание объектов на игровом поле
            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    int randomNumber = random.Next(5);
                    GameBoardObject newGameBoardObject = null;
                    switch(randomNumber)
                    {
                        case 0: newGameBoardObject = new SquareObject(pos);   break;
                        case 1: newGameBoardObject = new CircleObject(pos);   break;
                        case 2: newGameBoardObject = new TriangleObject(pos); break;
                        case 3: newGameBoardObject = new HexagonObject(pos);  break;
                        case 4: newGameBoardObject = new DiamondObject(pos);  break;
                    }
                    objectList.Add(newGameBoardObject);
                }
            }
        }
    }
}
