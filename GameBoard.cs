using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ComboList = System.Collections.Generic.List<System.Collections.Generic.List<Match3.GameBoardObject>>;


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

        /// <summary>
        /// Выбранный объект.
        /// </summary>
        public GameBoardObject SelectedObject { get; private set; } = null;

        public GameBoard()
        {
            // Создание объектов на игровом поле
            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    GameBoardObject randomObject = CreateRandomElement(pos, pos);
                    objectList.Add(randomObject);
                }
            }
        }

        /// <summary>
        /// Создает случайный элемент.
        /// </summary>
        /// <param name="pos">Позиция объекта на игровом поле.</param>
        public GameBoardObject CreateRandomElement(Vector2Int pos, Vector2 spritePos)
        {
            int randomNumber = random.Next(5);
            GameBoardObject newGameBoardObject = null;
            switch(randomNumber)
            {
                case 0: newGameBoardObject = new SquareObject(pos, spritePos);   break;
                case 1: newGameBoardObject = new CircleObject(pos, spritePos);   break;
                case 2: newGameBoardObject = new TriangleObject(pos, spritePos); break;
                case 3: newGameBoardObject = new HexagonObject(pos, spritePos);  break;
                case 4: newGameBoardObject = new DiamondObject(pos, spritePos);  break;
            }
            return newGameBoardObject;
        }

        /// <summary>
        /// Возвращает объект, находящийся в клетке игрового поля или null если там ничего нет.
        /// </summary>
        /// <param name="pos">Клетка игрового поля.</param>
        public GameBoardObject GetObjectAtPosition(Vector2Int pos)
        {
            List<GameBoardObject> foundObjects = objectList.FindAll(obj => obj.worldPos == pos);
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
        /// Меняет местами позиции объектов.
        /// </summary>
        public void SwapObjectPositions(GameBoardObject object1, GameBoardObject object2)
        {
            Debug.WriteLine($"Swapping {object1} and {object2}");
            Vector2Int object1Pos = object1.worldPos;
            Vector2Int object2Pos = object2.worldPos;
            object1.worldPos = object2Pos;
            object2.worldPos = object1Pos;
        }

        /// <summary>
        /// Выбор объекта.
        /// </summary>
        public void SelectObject(GameBoardObject gameBoardObject)
        {
            if(SelectedObject != null)
            {
                SelectedObject.pulseAnimationActive = false;
            }
            SelectedObject = gameBoardObject;
            SelectedObject.pulseAnimationActive = true;
        }

        /// <summary>
        /// Убрать выделение объекта.
        /// </summary>
        public void ClearSelection()
        {
            SelectedObject.pulseAnimationActive = false;
            SelectedObject = null;
        }

        /// <summary>
        /// Возвращает объект, находящийся в клетке игрового поля или null если там ничего нет.
        /// </summary>
        public GameBoardObject GetObjectAtPosition(int x, int y)
        {
            return GetObjectAtPosition(new Vector2Int(x, y));
        }

        /// <summary>
        /// /// Возвращает список комбинаций элементов, стоящих в ряд.
        /// </summary>
        /// <returns>Список комбинаций, где каждая комбинация это список объектов, входящих в комбинацию.</returns>
        public ComboList GetComboList()
        {
            return CheckCombo(false).Concat(CheckCombo(true)).ToList();
        }

        /// <summary>
        /// Возвращает список комбинаций (в определенном направлении) элементов, стоящих в ряд.
        /// </summary>
        /// <param name="vertical">Вертикальное направление, иначе горизонтальное.</param>
        /// <returns>Список комбинаций в определенном направлении, где каждая комбинация это список объектов, входящих в комбинацию.</returns>
        private ComboList CheckCombo(bool vertical)
        {
            string directionString = vertical ? "Вертикальное" : "Горизонтальное";

            // Определение комбо
            ComboList allComboList = new ComboList();
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

            return allComboList;
        }

        public void DeleteCombos(ComboList comboList)
        {
            // Удаляем объекты
            List<GameBoardObject> objectsToDelete = comboList.SelectMany(tempList => tempList).ToList();
            objectList.RemoveAll(obj => objectsToDelete.Contains(obj));
            for(int x = 0; x < 8; x++)
            {
                // Сдвигаем элементы сверху
                int objectsUnder = 0;
                for(int y = 7; y >= 0; y--)
                {
                    GameBoardObject gameBoardObject = GetObjectAtPosition(x, y);
                    if(gameBoardObject != null)
                    {
                        Vector2Int newPos = new Vector2Int(gameBoardObject.worldPos.x, 7 - objectsUnder);
                        if(gameBoardObject.worldPos != newPos)
                        {
                            Debug.WriteLine($"Moving {gameBoardObject.worldPos} to {newPos}");
                            gameBoardObject.worldPos = newPos;
                        }
                        objectsUnder++;
                    }
                }
                // Добавляем новые элементы
                int newElementCount = 8 - objectsUnder;
                for(int new_i = 0; new_i < newElementCount; new_i++)
                {
                    Vector2Int pos = new Vector2Int(x, new_i);
                    Vector2 spritePos = pos - new Vector2(0, newElementCount);
                    GameBoardObject randomObject = CreateRandomElement(pos, spritePos);
                    objectList.Add(randomObject);
                }
            }
        }
    }
}
