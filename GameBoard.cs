using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        /// <summary>
        /// Возвращает объект, находящийся в клетке игрового поля или null если там ничего нет.
        /// </summary>
        /// <param name="pos">Клетка игрового поля.</param>
        /// <returns></returns>
        public GameBoardObject GetObjectAtPosition(Vector2Int pos)
        {
            List<GameBoardObject> foundObjects = objectList.FindAll(obj => obj.pos == pos);
            if(foundObjects.Count > 1)
            {
                throw new InvalidOperationException($"В клетке {pos} найдено объектов: {foundObjects.Count}");
            }
            else if(foundObjects.Count == 1)
            {
                return foundObjects[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает объект, находящийся в клетке игрового поля или null если там ничего нет.
        /// </summary>
        public GameBoardObject GetObjectAtPosition(int x, int y)
        {
            return GetObjectAtPosition(new Vector2Int(x, y));
        }

        /// <summary>
        /// Проверяет, есть ли на доске несколько элементов, стоящих в ряд.
        /// </summary>
        /// <param name="vertical">Вертикальное направление, иначе горизонтальное.</param>
        public bool CheckCombo(bool vertical)
        {
            string directionString = vertical ? "Вертикальное" : "Горизонтальное";

            // Определение комбо
            List<List<GameBoardObject>> allComboList = new List<List<GameBoardObject>>();
            List<GameBoardObject> tempComboList = new List<GameBoardObject>();
            for(int i = 0; i < 8; i++)
            {
                tempComboList.Clear();
                Type comboType = null;
                for(int j = 0; j < 8; j++)
                {
                    GameBoardObject obj = vertical ? GetObjectAtPosition(i, j) : GetObjectAtPosition(j, i);
                    if(obj is null)
                    {
                        comboType = null;
                        continue;
                    }
                    if(obj.GetType() == comboType)
                    {
                        tempComboList.Add(obj);
                    }
                    else
                    {
                        if(tempComboList.Count >= 3)
                        {
                            allComboList.Add(new List<GameBoardObject>(tempComboList));
                            Debug.WriteLine($"{directionString} комбо {comboType} x{tempComboList.Count}");
                        }
                        tempComboList.Clear();
                        tempComboList.Add(obj);
                        comboType = obj.GetType();
                    }
                }
                if(tempComboList.Count >= 3)
                {
                    allComboList.Add(new List<GameBoardObject>(tempComboList));
                    Debug.WriteLine($"{directionString} комбо {comboType} x{tempComboList.Count}");
                }
            }

            // Удаление объектов
            List<GameBoardObject> objectsToDelete = allComboList.SelectMany(tempList => tempList).ToList();
            objectList.RemoveAll(obj => objectsToDelete.Contains(obj));

            // Сдвигаем элементы сверху
            for(int x = 0; x < 8; x++)
            {
                int objectsUnder = 0;
                for(int y = 7; y >= 0; y--)
                {
                    GameBoardObject gameBoardObject = GetObjectAtPosition(x, y);
                    if(gameBoardObject != null)
                    {
                        Vector2Int newPos = new Vector2Int(gameBoardObject.pos.x, 7 - objectsUnder);
                        Debug.WriteLine($"Moving {gameBoardObject.pos} to {newPos}");
                        gameBoardObject.pos = newPos;
                        objectsUnder++;
                    }
                }
            }

            return allComboList.Count > 0;
        }
    }
}
