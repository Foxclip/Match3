using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Linq;
using static Match3.GameBoard;
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
        private void ObjectClick(Point mousePos)
        {
            // Выбор объекта
            foreach(GameBoardObject clickedObject in gameBoard.objectList.Reverse<GameBoardObject>())
            {
                Rectangle boundingBox = clickedObject.GetScreenBoundingBox();
                if(boundingBox.Contains(mousePos))
                {
                    gameBoard.ObjectClick(clickedObject);
                    break;
                }
            }
        }

        /// <summary>
        /// Обновление состояния игры. Вызывается автоматически.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            gameBoard.ProcessElementSwap(gameTime);

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

                if(gameBoard.currentGamePhase == GamePhase.Normal)
                {
                    Debug.WriteLine("SELECT OBJECT");
                    ObjectClick(mousePos);
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
