using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Linq;
using ComboList = System.Collections.Generic.List<System.Collections.Generic.List<Match3.GameBoardObject>>;

namespace Match3
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Игровое поле
        public GameBoard gameBoard;

        // Спрайты объектов
        public static Texture2D squareSprite;
        public static Texture2D circleSprite;
        public static Texture2D triangleSprite;
        public static Texture2D hexagonSprite;
        public static Texture2D diamondSprite;

        // Размер клетки игрового поля в пикселях
        public readonly static float cellSize = 75f;
        // Масштаб спрайтов
        public readonly static float globalSpriteScale = 0.9f;
        // Сдвиг игрового поля в пикселях, чтобы оно было не с краю
        public readonly static Vector2 gameBoardOffset = new Vector2(45f, 45f);
        // Размеры окна
        public readonly static int width = 615;
        public readonly static int height = 615;

        // Текущее и предыдущее состояния клавиатуры и мыши
        private KeyboardState keyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState mouseState;
        private MouseState previousMouseState;

        /// <summary>
        /// Время анимации смены элементов местами в миллисекундах
        /// </summary>
        public readonly static double elementSwapTimeout = 1000;

        /// <summary>
        /// Время, прошедшее с начала анимации смены элементов местами.
        /// </summary>
        private double elementSwapTimer = 0;

        // Объекты, меняемые местами
        private GameBoardObject objectSwap1;
        private GameBoardObject objectSwap2;

        /// <summary>
        /// Фазы игры.
        /// </summary>
        private enum GamePhase
        {
            /// <summary>
            /// Обычное состояние, игрок может совершать действия.
            /// </summary>
            Normal,
            /// <summary>
            /// Перестановка элементов.
            /// </summary>
            ElementSwap,
        }

        /// <summary>
        /// Текущая фаза игры. Должна устанавливаться не напрямую, а через pendingGamePhase.
        /// </summary>
        private GamePhase currentGamePhase = GamePhase.Normal;

        /// <summary>
        /// Какое фаза игры будет в следующем кадре.
        /// </summary>
        private GamePhase pendingGamePhase = GamePhase.Normal;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.SynchronizeWithVerticalRetrace = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Установка размера окна
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Загрузка спрайтов
            squareSprite = Content.Load<Texture2D>("square");
            circleSprite = Content.Load<Texture2D>("circle");
            triangleSprite = Content.Load<Texture2D>("triangle");
            hexagonSprite = Content.Load<Texture2D>("hexagon");
            diamondSprite = Content.Load<Texture2D>("diamond");

            // Игровое поле, должно быть инициализировано после загрузки спрайтов
            gameBoard = new GameBoard();

        }

        /// <summary>
        /// Проверяет, была ли нажата кнопка клавиатуры.
        /// </summary>
        /// <param name="key">Кнопка клавиатуры.</param>
        private bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Проверяет, была ли нажата левая кнопка мыши.
        /// </summary>
        private bool LeftMouseButtonPressed()
        {
            return mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
        }

        /// <summary>
        /// Выбор объекта на экране.
        /// </summary>
        /// <param name="mousePos">Позиция мыши в экранных координатах.</param>
        private void SelectObject(Point mousePos)
        {
            // Выбор объекта
            foreach(GameBoardObject clickedObject in gameBoard.objectList.Reverse<GameBoardObject>())
            {
                Rectangle boundingBox = clickedObject.GetScreenBoundingBox();
                if(boundingBox.Contains(mousePos))
                {
                    Debug.WriteLine($"CLICKED ON {clickedObject.GetType()}");
                    // Если нет выбранного объекта
                    if(gameBoard.SelectedObject is null)
                    {
                        gameBoard.SelectObject(clickedObject);
                    }
                    // Если выбирается тот же объект
                    else if(clickedObject == gameBoard.SelectedObject)
                    {
                        gameBoard.ClearSelection();
                    }
                    // Если выбирается другой объект
                    else
                    {
                        // Если соседний объект
                        if((clickedObject.worldPos - gameBoard.SelectedObject.worldPos).Magnitude == 1.0)
                        {
                            // Запоминаем какие объекты меняем, чтобы потом поменять их обратно
                            objectSwap1 = clickedObject;
                            objectSwap2 = gameBoard.SelectedObject;
                            // Меняем местами их позиции
                            gameBoard.SwapObjectPositions(clickedObject, gameBoard.SelectedObject);
                            gameBoard.ClearSelection();

                            elementSwapTimer = 0;
                            pendingGamePhase = GamePhase.ElementSwap;
                        }
                        else
                        {
                            gameBoard.ClearSelection();
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Обновление состояния игры. Вызывается автоматически.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            pendingGamePhase = currentGamePhase;

            // Если идет смена элементов местами
            if(currentGamePhase == GamePhase.ElementSwap)
            {
                if(elementSwapTimer >= elementSwapTimeout)
                {
                    pendingGamePhase = GamePhase.Normal;
                    // Если нет комбинаций, то меняем элементы обратно
                    ComboList comboList = gameBoard.GetComboList();
                    if(comboList.Count == 0)
                    {
                        gameBoard.SwapObjectPositions(objectSwap1, objectSwap2);
                    }
                    else
                    {
                        gameBoard.DeleteCombos(comboList);
                    }
                    elementSwapTimer = 0;
                }
                elementSwapTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                Debug.WriteLine(elementSwapTimer);
            }

            // Обработка клавиатуры
            keyboardState = Keyboard.GetState();
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if(KeyPressed(Keys.Space))
            {
                Debug.WriteLine("SPACE");
            }

            // Обработка мыши
            mouseState = Mouse.GetState();
            if(LeftMouseButtonPressed())
            {
                Debug.WriteLine("LEFT MOUSE");
                Point mousePos = mouseState.Position;

                if(currentGamePhase == GamePhase.Normal)
                {
                    Debug.WriteLine("SELECT OBJECT");
                    SelectObject(mousePos);
                }
            }

            // Сохраняем состояние клавиатуры и мыши
            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;

            // Анимируем спрайты
            foreach(GameBoardObject gameBoardObject in gameBoard.objectList)
            {
                gameBoardObject.SpriteAnimation(gameTime);
            }

            // Меняем состояние игры
            currentGamePhase = pendingGamePhase;

            base.Update(gameTime);
        }

        /// <summary>
        /// Отрисовка объектов на экране. Вызывается автоматически.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            // Заливка фона
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Отрисовка объектов на игровом поле
            foreach(GameBoardObject gameBoardObject in gameBoard.objectList)
            {
                gameBoardObject.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
