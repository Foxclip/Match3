using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Match3
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
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

        protected override void Update(GameTime gameTime)
        {
            // Обработка клавиатуры
            keyboardState = Keyboard.GetState();
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if(KeyPressed(Keys.Space))
            {
                gameBoard.CheckCombo(false);
                gameBoard.CheckCombo(true);
                Debug.WriteLine("SPACE");
            }

            // Обработка мыши
            mouseState = Mouse.GetState();
            if(LeftMouseButtonPressed())
            {
                Debug.WriteLine("LEFT MOUSE");
                Point mousePos = mouseState.Position;

                // Выбор объекта
                foreach(GameBoardObject obj in gameBoard.objectList)
                {
                    Rectangle boundingBox = obj.GetScreenBoundingBox();
                    if(boundingBox.Contains(mousePos))
                    {
                        // Если нет выбранного объекта или выбирается тот же объект
                        if(selectedObject is null || obj == selectedObject)
                        {
                            selectedObject = obj;
                            obj.pulseAnimationActive = true;
                        }
                        // Если выбирается другой объект
                        if(selectedObject != null && obj != selectedObject)
                        {
                            selectedObject.pulseAnimationActive = false;
                            selectedObject = obj;
                            obj.pulseAnimationActive = true;
                        }
                    }
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
