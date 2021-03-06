﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ComboList = System.Collections.Generic.List<System.Collections.Generic.List<Match3.GameBoardObject>>;
using CrossList = System.Collections.Generic.List<(System.Collections.Generic.List<Match3.GameBoardObject>, System.Collections.Generic.List<Match3.GameBoardObject>)>;


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
        public List<GameBoardObject> objectList = new List<GameBoardObject>();

        /// <summary>
        /// Список разрушителей.
        /// </summary>
        public List<Destroyer> destroyerList = new List<Destroyer>();

        /// <summary>
        /// Генератор случайных чисел.
        /// </summary>
        public readonly Random random = new Random();

        /// <summary>
        /// Фазы игры.
        /// </summary>
        public enum GamePhase
        {
            /// <summary>
            /// Главное меню.
            /// </summary>
            MainMenu,
            /// <summary>
            /// Обычное состояние, игрок может совершать действия.
            /// </summary>
            Normal,
            /// <summary>
            /// Перестановка элементов.
            /// </summary>
            ElementSwap,
            /// <summary>
            /// Перестановка элементов обратно.
            /// </summary>
            SwapBack,
            /// <summary>
            /// Удаление комбинаций.
            /// </summary>
            ComboDeletion,
            /// <summary>
            /// Сдвиг элементов после удаления комбинаций.
            /// </summary>
            ElementSlide,
            /// <summary>
            /// Бонус активирован.
            /// </summary>
            Bonus,
            /// <summary>
            /// Игра завершена.
            /// </summary>
            GameOver,
        }

        /// <summary>
        /// Количество очков.
        /// </summary>
        public int score = 0;

        /// <summary>
        /// Текущая фаза игры.
        /// </summary>
        public GamePhase currentGamePhase = GamePhase.MainMenu;

        /// <summary>
        /// Выбранный объект.
        /// </summary>
        public GameBoardObject SelectedObject { get; private set; } = null;

        /// <summary>
        /// Список активных анимаций.
        /// </summary>
        public List<Animation> activeAnimations = new List<Animation>();

        /// <summary>
        /// Анимация пульсации выбранного объекта.
        /// </summary>
        public PulseAnimation selectedObjectPulseAnimation = null;

        /// <summary>
        /// Список исчезающих объектов.
        /// </summary>
        public List<GameBoardObject> implodingObjects = new List<GameBoardObject>();

        // Объекты, меняемые местами
        public GameBoardObject objectSwap1;
        public GameBoardObject objectSwap2;

        /// <summary>
        /// Оставшееся время в секундах.
        /// </summary>
        public double timeRemaining = 60.0;

        /// <summary>
        /// Конструктор.
        /// </summary>
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

            // Удаление комбинаций образовавшихся после случайной генерации объектов
            ComboList comboList = GetComboList();
            int iteration = 1;
            while(comboList.Count > 0)
            {
                Debug.WriteLine($"Iteration: {iteration}");
                GameBoardObject newObject = null;
                int objectsChanged = 0;
                foreach(List<GameBoardObject> combination in comboList)
                {
                    GameBoardObject middleObject = combination[combination.Count / 2];
                    switch(combination[0].GetType().Name)
                    {
                        case nameof(SquareObject):   newObject = new CircleObject(middleObject.worldPos, middleObject.worldPos);   break;
                        case nameof(CircleObject):   newObject = new TriangleObject(middleObject.worldPos, middleObject.worldPos); break;
                        case nameof(TriangleObject): newObject = new HexagonObject(middleObject.worldPos, middleObject.worldPos);  break;
                        case nameof(HexagonObject):  newObject = new DiamondObject(middleObject.worldPos, middleObject.worldPos);  break;
                        case nameof(DiamondObject):  newObject = new SquareObject(middleObject.worldPos, middleObject.worldPos);   break;
                    }
                    objectList.Remove(middleObject);
                    objectList.Add(newObject);
                    objectsChanged++;
                }
                Debug.WriteLine($"Objects changed: {objectsChanged}");
                comboList = GetComboList();
                iteration++;
            }

            //TurnIntoSquare(new Vector2Int(5, 2));
            //TurnIntoBomb(new Vector2Int(5, 2));
            //TurnIntoSquare(new Vector2Int(4, 2));

            //TurnIntoSquare(new Vector2Int(2, 3));
            //TurnIntoSquare(new Vector2Int(3, 3));
            //TurnIntoSquare(new Vector2Int(4, 3));
            //TurnIntoSquare(new Vector2Int(6, 3));

            //TurnIntoSquare(new Vector2Int(4, 4));
            //TurnIntoSquare(new Vector2Int(5, 4));

        }

        /// <summary>
        /// Превращает объект в заданной позиции в квадрат.
        /// </summary>
        public void TurnIntoSquare(Vector2Int pos)
        {
            GameBoardObject obj = GetObjectAtPosition(pos);
            SquareObject squareObject = new SquareObject(obj.worldPos, obj.worldPos);
            objectList.Remove(obj);
            objectList.Add(squareObject);
        }

        /// <summary>
        /// Превращает объект в заданной позиции в круг.
        /// </summary>
        public void TurnIntoCircle(Vector2Int pos)
        {
            GameBoardObject obj = GetObjectAtPosition(pos);
            CircleObject circleObject = new CircleObject(obj.worldPos, obj.worldPos);
            objectList.Remove(obj);
            objectList.Add(circleObject);
        }

        /// <summary>
        /// Превращает объект в заданной позиции в бомбу.
        /// </summary>
        public void TurnIntoBomb(Vector2Int pos)
        {
            GameBoardObject obj = GetObjectAtPosition(pos);
            BombBonus bomb = new BombBonus(obj, obj.worldPos, obj.worldPos);
            objectList.Remove(obj);
            objectList.Add(bomb);
        }

        /// <summary>
        /// Превращает объект в заданной позиции в Line.
        /// </summary>
        public void TurnIntoLine(Vector2Int pos)
        {
            GameBoardObject obj = GetObjectAtPosition(pos);
            LineBonus bomb = new LineBonus(obj, true, obj.worldPos, obj.worldPos);
            objectList.Remove(obj);
            objectList.Add(bomb);
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
        /// Обновляет состояние игрового поля.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Пытаемся изменить состояние игры
            ChangeState();

            // Действие разрушителей
            List<Destroyer> destroyerListCopy = new List<Destroyer>(destroyerList);
            foreach(Destroyer destroyer in destroyerListCopy)
            {
                foreach(GameBoardObject gameBoardObject in objectList)
                {
                    Rectangle boundingBox = gameBoardObject.GetScreenBoundingBox();
                    bool boundingBoxHit = boundingBox.Contains(GameBoardObject.WorldToScreen(destroyer.spriteWorldPos));
                    bool alreadyImploding = implodingObjects.Contains(gameBoardObject);
                    if(boundingBoxHit && !alreadyImploding)
                    {
                        implodingObjects.Add(gameBoardObject);
                        ScaleAnimation implodeAnimation = new ScaleAnimation(
                            gameBoardObject,
                            beginScale: 1.0,
                            endScale: 0.0,
                            delay: 0.0,
                            blocking: true,
                            finishedCallback: _ => objectList.Remove(gameBoardObject)
                        );
                        activeAnimations.Add(implodeAnimation);
                        score++;
                        // Если это LineBonus
                        if(gameBoardObject.GetType() == typeof(LineBonus))
                        {
                            TriggerLineBonus((LineBonus)gameBoardObject);
                        }
                        // Если это BombBonus
                        if(gameBoardObject.GetType() == typeof(BombBonus))
                        {
                            TriggerBombBonus((BombBonus)gameBoardObject);
                        }
                    }
                }
            }

            // Обновляем состояние анимаций
            activeAnimations.ForEach(animation => animation.Update(gameTime));
            // Удаляем завершившиеся анимации
            List<Animation> animationsToDelete = activeAnimations.FindAll(animation => !animation.active);
            animationsToDelete.ForEach(animation => animation.OnDelete());
            activeAnimations = activeAnimations.Except(animationsToDelete).ToList();

            // Синхронизируем список implodingObjects с основным списком
            implodingObjects = implodingObjects.Intersect(objectList).ToList();

            // Уменьшаем остаток времени
            if(currentGamePhase != GamePhase.MainMenu && currentGamePhase != GamePhase.GameOver)
            {
                timeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        /// <summary>
        /// Возвращает объект, находящийся в клетке игрового поля или null если там ничего нет.
        /// </summary>
        /// <param name="pos">Клетка игрового поля.</param>
        public GameBoardObject GetObjectAtPosition(Vector2Int pos)
        {
            string foundObjectsString = "";
            List<GameBoardObject> foundObjects = objectList.FindAll(obj => obj.worldPos == pos && !implodingObjects.Contains(obj));
            foreach(GameBoardObject obj in foundObjects)
            {
                foundObjectsString += obj.ToString() + ", ";
            }
            if(foundObjects.Count > 1)
            {
                throw new InvalidOperationException($"В клетке {pos} найдено объектов: {foundObjects.Count} {foundObjectsString}");
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

        public void ObjectClick(GameBoardObject clickedObject)
        {
            Debug.WriteLine($"CLICKED ON {clickedObject.GetType()}");
            // Если нет выбранного объекта
            if(SelectedObject is null)
            {
                SelectObject(clickedObject);
            }
            // Если выбирается тот же объект
            else if(clickedObject == SelectedObject)
            {
                ClearSelection();
            }
            // Если выбирается другой объект
            else
            {
                // Если соседний объект
                if((clickedObject.worldPos - SelectedObject.worldPos).Magnitude == 1.0)
                {
                    // Запоминаем какие объекты меняем, чтобы потом поменять их обратно
                    objectSwap1 = clickedObject;
                    objectSwap2 = SelectedObject;
                    // Меняем местами их позиции
                    SwapObjectPositions(clickedObject, SelectedObject);

                    ClearSelection();
                    currentGamePhase = GamePhase.ElementSwap;
                }
                // Если не соседний объект
                else
                {
                    ClearSelection();
                }
            }
        }

        /// <summary>
        /// Меняет местами позиции объектов.
        /// </summary>
        public void SwapObjectPositions(GameBoardObject object1, GameBoardObject object2)
        {
            Debug.WriteLine($"Swapping {object1} and {object2}");
            // Меняем позиции
            Vector2Int object1Pos = object1.worldPos;
            Vector2Int object2Pos = object2.worldPos;
            object1.worldPos = object2Pos;
            object2.worldPos = object1Pos;
            // Запускаем анимации
            MoveAnimation moveAnimation1 = new MoveAnimation(object1, object1Pos, object2Pos, duration: 0.3, blocking: true);
            MoveAnimation moveAnimation2 = new MoveAnimation(object2, object2Pos, object1Pos, duration: 0.3, blocking: true);
            activeAnimations.Add(moveAnimation1);
            activeAnimations.Add(moveAnimation2);
        }

        /// <summary>
        /// Выбор объекта.
        /// </summary>
        public void SelectObject(GameBoardObject gameBoardObject)
        {
            SelectedObject = gameBoardObject;
            selectedObjectPulseAnimation = new PulseAnimation(SelectedObject);
            activeAnimations.Add(selectedObjectPulseAnimation);
        }

        /// <summary>
        /// Убрать выделение объекта.
        /// </summary>
        public void ClearSelection()
        {
            selectedObjectPulseAnimation?.OnDelete();
            selectedObjectPulseAnimation = null;
            SelectedObject = null;
        }

        /// <summary>
        /// Изменяет фазу игры, если это необходимо.
        /// </summary>
        public void ChangeState()
        {
            // Если закончилось время
            if(timeRemaining < 0)
            {
                currentGamePhase = GamePhase.GameOver;
                return;
            }
            // Если есть блокирующие анимации, то состояние изменять нельзя
            if(activeAnimations.FindAll(animation => animation.blocking).Count > 0)
            {
                return;
            }
            // Смена элементов местами
            if(currentGamePhase == GamePhase.ElementSwap)
            {
                ComboList comboList = GetComboList();
                if(comboList.Count == 0)
                {
                    currentGamePhase = GamePhase.SwapBack;
                }
                else
                {
                    currentGamePhase = GamePhase.ComboDeletion;
                }
            }
            // Смена элементов обратно
            else if(currentGamePhase == GamePhase.SwapBack)
            {
                // Сохранение позиций объектов
                Vector2Int object1Pos = objectSwap1.worldPos;
                Vector2Int object2Pos = objectSwap2.worldPos;
                // Смена объектов местами
                objectSwap1.worldPos = object2Pos;
                objectSwap2.worldPos = object1Pos;
                // Запуск анимации
                MoveAnimation moveAnimation1 = new MoveAnimation(objectSwap1, object1Pos, object2Pos, duration: 0.3, blocking: true);
                MoveAnimation moveAnimation2 = new MoveAnimation(objectSwap2, object2Pos, object1Pos, duration: 0.3, blocking: true);
                activeAnimations.Add(moveAnimation1);
                activeAnimations.Add(moveAnimation2);

                ClearSelection();
                currentGamePhase = GamePhase.ComboDeletion;
            }
            // Удаление комбинаций
            else if(currentGamePhase == GamePhase.ComboDeletion)
            {
                ComboList comboList = GetComboList();
                if(comboList.Count == 0)
                {
                    currentGamePhase = GamePhase.Normal;
                }
                else
                {
                    DeleteCombos(comboList);
                    currentGamePhase = GamePhase.ElementSlide;
                }
            }
            // Создание новых элементов
            else if(currentGamePhase == GamePhase.ElementSlide)
            {
                CreateNewObjects();
                currentGamePhase = GamePhase.ComboDeletion;
            }
            // Работа бонусов
            else if(currentGamePhase == GamePhase.Bonus)
            {
                currentGamePhase = GamePhase.ComboDeletion;
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
                GameBoardObject.GameBoardObjectType comboType = GameBoardObject.GameBoardObjectType.None;
                for(int j = 0; j < 8; j++)
                {
                    GameBoardObject obj = vertical ? GetObjectAtPosition(i, j) : GetObjectAtPosition(j, i);
                    if(obj is null)
                    {
                        comboType = GameBoardObject.GameBoardObjectType.None;
                        continue;
                    }
                    if(obj.objectType == comboType)
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
                        comboType = obj.objectType;
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

        /// <summary>
        /// Активирует бонус Line.
        /// </summary>
        public void TriggerLineBonus(LineBonus lineBonus)
        {
            Debug.WriteLine("Creating destroyers");
            // Скорость разрушителей
            Vector2 destroyerDestination1;
            Vector2 destroyerDestination2;
            if(lineBonus.vertical)
            {
                destroyerDestination1 = new Vector2(lineBonus.worldPos.x, -1.0f);
                destroyerDestination2 = new Vector2(lineBonus.worldPos.x, 8.0f);
            }
            else
            {
                destroyerDestination1 = new Vector2(-1.0f, lineBonus.worldPos.y);
                destroyerDestination2 = new Vector2(8.0f, lineBonus.worldPos.y);
            }
            // Создание разрушителей
            Destroyer destroyer1 = new Destroyer(lineBonus.worldPos);
            Destroyer destroyer2 = new Destroyer(lineBonus.worldPos);
            destroyerList.Add(destroyer1);
            destroyerList.Add(destroyer2);
            // Запуск анимации
            MoveAnimation moveAnimation1 = new MoveAnimation(
                destroyer1,
                speed: 10.0,
                lineBonus.worldPos,
                destroyerDestination1,
                blocking: true,
                finishedCallback: _ => destroyerList.Remove(destroyer1)
            );
            MoveAnimation moveAnimation2 = new MoveAnimation(
                destroyer2,
                speed: 10.0,
                lineBonus.worldPos,
                destroyerDestination2,
                blocking: true,
                finishedCallback: _ => destroyerList.Remove(destroyer2)
            );
            activeAnimations.Add(moveAnimation1);
            activeAnimations.Add(moveAnimation2);
        }

        /// <summary>
        /// Активирует бонус Bomb.
        /// </summary>
        public void TriggerBombBonus(BombBonus bombBonus)
        {
            // Объекты вокруг бомбы
            for(int x = bombBonus.worldPos.x - 1; x <= bombBonus.worldPos.x + 1; x++)
            {
                for(int y = bombBonus.worldPos.y - 1; y <= bombBonus.worldPos.y + 1; y++)
                {
                    if(x == bombBonus.worldPos.x && y == bombBonus.worldPos.y)
                    {
                        continue;
                    }
                    GameBoardObject obj = GetObjectAtPosition(x, y);
                    if(obj is null)
                    {
                        continue;
                    }
                    implodingObjects.Add(obj);
                    ScaleAnimation implodeAnimation = new ScaleAnimation(
                        obj,
                        beginScale: 1.0, 
                        endScale: 0.0,
                        delay: 0.25,
                        blocking: true,
                        finishedCallback: _ => objectList.Remove(obj)
                    );
                    activeAnimations.Add(implodeAnimation);
                    score++;
                    // Если это LineBonus
                    if(obj.GetType() == typeof(LineBonus))
                    {
                        TriggerLineBonus((LineBonus)obj);
                    }
                    // Если это BombBonus
                    if(obj.GetType() == typeof(BombBonus))
                    {
                        TriggerBombBonus((BombBonus)obj);
                    }
                }
            }
            // Сама бомба
            implodingObjects.Add(bombBonus);
            ScaleAnimation bombImplodeAnimation = new ScaleAnimation(
                bombBonus,
                beginScale: 1.0,
                endScale: 0.0,
                delay: 0.0,
                blocking: true,
                finishedCallback: _ => objectList.Remove(bombBonus)
            );
            activeAnimations.Add(bombImplodeAnimation);
            score++;
        }

        /// <summary>
        /// Создать бонус Bomb.
        /// </summary>
        /// <param name="baseObject">Базовый объект.</param>
        public void CreateBombBonus(GameBoardObject baseObject)
        {
            // Создаем объект
            BombBonus newBombBonus = new BombBonus(baseObject, baseObject.worldPos, baseObject.worldPos);
            objectList.Add(newBombBonus);
            // Запускаем анимацию появления
            ScaleAnimation spawnAnimation = new ScaleAnimation(newBombBonus, 0.0, 1.0, blocking: true);
            activeAnimations.Add(spawnAnimation);
        }

        /// <summary>
        /// Удаление комбинаций.
        /// </summary>
        public void DeleteCombos(ComboList comboList)
        {
            // Список удаляемых объектов
            List<GameBoardObject> objectsToDelete = comboList.SelectMany(tempList => tempList).ToList();
            score += objectsToDelete.Count;

            // Срабатывание бонусов
            List<LineBonus> lineBonuses = objectsToDelete.FindAll(obj => obj.GetType() == typeof(LineBonus)).Cast<LineBonus>().ToList();
            foreach(LineBonus lineBonus in lineBonuses)
            {
                TriggerLineBonus(lineBonus);
                currentGamePhase = GamePhase.Bonus;
            }
            List<BombBonus> bombBonuses = objectsToDelete.FindAll(obj => obj.GetType() == typeof(BombBonus)).Cast<BombBonus>().ToList();
            foreach(BombBonus bombBonus in bombBonuses)
            {
                TriggerBombBonus(bombBonus);
                currentGamePhase = GamePhase.Bonus;
            }

            // Если бонусы активны, то новые создавать нельзя.
            if(currentGamePhase == GamePhase.Bonus)
            {
                return;
            }

            // Запуск анимации исчезновения
            implodingObjects.Clear();
            foreach(GameBoardObject obj in objectsToDelete)
            {
                implodingObjects.Add(obj);
                ScaleAnimation implodeAnimation = new ScaleAnimation(obj,1.0, 0.0, blocking: true, finishedCallback: _ => objectList.Remove(obj));
                activeAnimations.Add(implodeAnimation);
            }

            // Бонус Bomb
            // Комбинации из 5 и более
            List<Vector2Int> newBombPositions = new List<Vector2Int>();
            ComboList combinationsOf5AndMore = comboList.FindAll(combination => combination.Count >= 5);
            List<GameBoardObject> combination = combinationsOf5AndMore.Find(combination => combination.Contains(objectSwap2));
            if(combination != null)
            {
                CreateBombBonus(objectSwap2);
                newBombPositions.Add(objectSwap2.worldPos);
            }
            // Перекрестные комбинации из 3 и более
            CrossList crosses = new CrossList();
            ComboList verticalCombinations = comboList.FindAll(combination => combination[0].worldPos.x == combination[1].worldPos.x);
            ComboList horizontalCombinations = comboList.FindAll(combination => combination[0].worldPos.y == combination[1].worldPos.y);
            foreach(List<GameBoardObject> verticalCombination in verticalCombinations)
            {
                foreach(List<GameBoardObject> horizontalCombination in horizontalCombinations)
                {
                    List<GameBoardObject> intersection = verticalCombination.Intersect(horizontalCombination).ToList();
                    if(intersection.Count > 0)
                    {
                        GameBoardObject objectAtIntersection = intersection[0];
                        if(!newBombPositions.Contains(objectAtIntersection.worldPos))
                        {
                            CreateBombBonus(objectAtIntersection);
                            newBombPositions.Add(objectAtIntersection.worldPos);
                        }
                    }
                }
            }
            // Бонус Line
            ComboList combinationsOf4 = comboList.FindAll(combination => combination.Count == 4);
            combination = combinationsOf4.Find(combination => combination.Contains(objectSwap2));
            if(combination != null && !newBombPositions.Contains(objectSwap2.worldPos))
            {
                Debug.WriteLine($"Creating line bonus in {objectSwap2.worldPos}");
                bool vertical = combination[0].worldPos.x == combination[1].worldPos.x;
                // Создаем объект
                LineBonus newLineBonus = new LineBonus(objectSwap2, vertical, objectSwap2.worldPos, objectSwap2.worldPos);
                objectList.Add(newLineBonus);
                // Запускаем анимацию появления
                ScaleAnimation spawnAnimation = new ScaleAnimation(newLineBonus, 0.0, 1.0, blocking: true);
                activeAnimations.Add(spawnAnimation);
            }
        }

        /// <summary>
        /// Создает новые объекты сверху.
        /// </summary>
        public void CreateNewObjects()
        {
            Debug.WriteLine("Creating new objects");
            for(int x = 0; x < 8; x++)
            {
                // Сдвигаем висящие элементы
                int objectsUnder = 0;
                for(int y = 7; y >= 0; y--)
                {
                    GameBoardObject gameBoardObject = GetObjectAtPosition(x, y);
                    if(gameBoardObject != null)
                    {
                        Vector2Int newPos = new Vector2Int(gameBoardObject.worldPos.x, 7 - objectsUnder);
                        if(gameBoardObject.worldPos != newPos)
                        {
                            MoveAnimation moveAnimation = new MoveAnimation(gameBoardObject, gameBoardObject.worldPos, newPos, duration: 0.3, blocking: true);
                            activeAnimations.Add(moveAnimation);
                            gameBoardObject.worldPos = newPos;
                        }
                        objectsUnder++;
                    }
                }
                // Добавляем новые элементы
                int newElementCount = 8 - objectsUnder;
                for(int new_i = 0; new_i < newElementCount; new_i++)
                {
                    // Позиция объекта и спрайта
                    Vector2Int pos = new Vector2Int(x, new_i);
                    Vector2 spritePos = pos - new Vector2(0, newElementCount);
                    // Создание объекта
                    GameBoardObject randomObject = CreateRandomElement(pos, spritePos);
                    objectList.Add(randomObject);
                    // Запуск анимации
                    MoveAnimation moveAnimation = new MoveAnimation(randomObject, spritePos, pos, duration: 0.3, blocking: true);
                    activeAnimations.Add(moveAnimation);
                }
            }
        }
    }
}
