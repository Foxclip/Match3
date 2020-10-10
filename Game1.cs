using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Linq;

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
        public static float cellSize = 75f;
        // Масштаб спрайтов
        public static float globalSpriteScale = 0.9f;
        // Сдвиг игрового поля в пикселях, чтобы оно было не с краю
        public static Vector2 gameBoardOffset = new Vector2(45f, 45f);
        // Размеры окна
        public static int width = 615;
        public static int height = 615;

        // Текущее и предыдущее состояния клавиатуры и мыши
        private KeyboardState keyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState mouseState;
        private MouseState previousMouseState;

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

        // Выбранный объект
        private GameBoardObject selectedObject = null;

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
                    if(selectedObject is null)
                    {
                        selectedObject = clickedObject;
                        selectedObject.pulseAnimationActive = true;
                        Debug.WriteLine($"Launching animation on {clickedObject}");
                    }
                    // Если выбирается тот же объект
                    else if(clickedObject == selectedObject)
                    {
                        selectedObject.pulseAnimationActive = false;
                        selectedObject = null;
                    }
                    // Если выбирается другой объект
                    else
                    {
                        // Если соседний объект
                        if((clickedObject.worldPos - selectedObject.worldPos).Magnitude == 1.0)
                        {
                            // Меняем местами их позиции
                            Vector2Int clickedObjectPos = clickedObject.worldPos;
                            Vector2Int selectedObjectPos = selectedObject.worldPos;
                            clickedObject.worldPos = selectedObjectPos;
                            selectedObject.worldPos = clickedObjectPos;
                            pendingGamePhase = GamePhase.ElementSwap;
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

            // Обработка клавиатуры
            keyboardState = Keyboard.GetState();
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if(KeyPressed(Keys.Space))
            {
                if(currentGamePhase == GamePhase.Normal)
                {
                    gameBoard.CheckCombo(false);
                    gameBoard.CheckCombo(true);
                }
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
