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

        // Текущее и предыдущее состояния клавиатуры
        private KeyboardState keyboardState;
        private KeyboardState previousKeyboardState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.SynchronizeWithVerticalRetrace = true;
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

            gameBoard = new GameBoard();

        }

        /// <summary>
        /// Проверяет, была ли нажата клавиша.
        /// </summary>
        /// <param name="key">Клавиша клавиатуры.</param>
        private bool CheckKeyPress(Keys key)
        {
            return keyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        protected override void Update(GameTime gameTime)
        {
            // Обработка клавиатуры
            keyboardState = Keyboard.GetState();
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if(CheckKeyPress(Keys.Space))
            {
                gameBoard.CheckCombo(false);
                gameBoard.CheckCombo(true);
                Debug.WriteLine("SPACE");
            }
            // Сохраняем состояние клавиатуры
            previousKeyboardState = keyboardState;

            // Двигаем спрайты
            foreach(GameBoardObject gameBoardObject in gameBoard.objectList)
            {
                gameBoardObject.MoveSprite();
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
