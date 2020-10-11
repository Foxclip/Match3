using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Match3.GameBoard;

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
        public static Texture2D lineBonusSprite;
        public static Texture2D destroyerSprite;

        // Размер клетки игрового поля в пикселях
        public readonly static float cellSize = 75f;
        // Масштаб спрайтов
        public readonly static float globalSpriteScale = 0.9f;
        // Сдвиг игрового поля в пикселях, чтобы оно было не с краю
        public readonly static Vector2 gameBoardOffset = new Vector2(45f, 95f);
        // Размеры окна
        public readonly static int width = 615;
        public readonly static int height = 665;

        // Текущее и предыдущее состояния клавиатуры и мыши
        private KeyboardState keyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState mouseState;
        private MouseState previousMouseState;

        // Кнопки
        private Button playButton;
        private Button okButton;

        // Текстура верхней панели
        private Texture2D topPanel;
        // Шрифт
        private SpriteFont font;
        // Большой шрифт
        private SpriteFont bigFont;
        // Сдвиг текста с очками (от левого верхнего угла)
        private Vector2 scoreTextOffset = new Vector2(20f, 15f);
        // Сдвиг текста с оставшимся временем (от правого верхнего угла)
        private Vector2 timeTextOffset = new Vector2(20f, 15f);
        // Сдвиг надписи GameOver
        private Vector2 gameOverTextOffset = new Vector2(width / 2, height / 4);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.SynchronizeWithVerticalRetrace = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Инициализация перед началом игры.
        /// </summary>
        protected override void Initialize()
        {
            // Установка размера окна
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            // Создание верхней панели
            topPanel = new Texture2D(GraphicsDevice, 1, 1);
            topPanel.SetData(new[] { Color.White });

            base.Initialize();
        }

        /// <summary>
        /// Загрузка ресурсов.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Загрузка спрайтов
            squareSprite = Content.Load<Texture2D>("square");
            circleSprite = Content.Load<Texture2D>("circle");
            triangleSprite = Content.Load<Texture2D>("triangle");
            hexagonSprite = Content.Load<Texture2D>("hexagon");
            diamondSprite = Content.Load<Texture2D>("diamond");
            lineBonusSprite = Content.Load<Texture2D>("line_bonus");
            destroyerSprite = Content.Load<Texture2D>("destroyer");

            // Спрайт кнопки Play
            Texture2D playButtonSprite = Content.Load<Texture2D>("play_button");
            // Спрайт кнопки Ok
            Texture2D okButtonSprite = Content.Load<Texture2D>("ok_button");

            // Загрузка шрифтов
            font = Content.Load<SpriteFont>("Arial");
            bigFont = Content.Load<SpriteFont>("Arial_big");

            // Игровое поле, должно быть инициализировано после загрузки спрайтов
            gameBoard = new GameBoard();

            // Кнопка Play
            playButton = new Button(playButtonSprite, new Vector2(width / 2, height / 1.5f));
            // Кнопка Ok
            okButton = new Button(okButtonSprite, new Vector2(width / 2, height / 2));

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
        /// Обновление состояния игры. Вызывается автоматически.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Пытаемся изменить состояние игры
            gameBoard.ChangeState();

            // Обработка клавиатуры
            keyboardState = Keyboard.GetState();
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Обработка мыши
            mouseState = Mouse.GetState();
            if(LeftMouseButtonPressed())
            {
                Point mousePos = mouseState.Position;
                // Главное меню
                if(gameBoard.currentGamePhase == GamePhase.MainMenu)
                {
                    // Нажатие на кнопку Play
                    if(playButton.GetScreenBoundingBox().Contains(mousePos))
                    {
                        gameBoard.currentGamePhase = GamePhase.Normal;
                    }
                }
                // Экран Game Over
                else if(gameBoard.currentGamePhase == GamePhase.GameOver)
                {
                    // Нажатие на кнопку Ok
                    if(okButton.GetScreenBoundingBox().Contains(mousePos))
                    {
                        gameBoard = new GameBoard();
                    }
                }
                // Обычный режим игры
                else if(gameBoard.currentGamePhase == GamePhase.Normal)
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
            }

            // Сохраняем состояние клавиатуры и мыши
            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;

            // Обновляем состояние анимаций
            gameBoard.activeAnimations.ForEach(animation => animation.Update(gameTime));
            // Удаляем завершившиеся анимации
            List<Animation> animationsToDelete = gameBoard.activeAnimations.FindAll(animation => !animation.active);
            animationsToDelete.ForEach(animation => animation.OnDelete());
            gameBoard.activeAnimations = gameBoard.activeAnimations.Except(animationsToDelete).ToList();

            // Уменьшаем остаток времени
            if(gameBoard.currentGamePhase != GamePhase.MainMenu && gameBoard.currentGamePhase != GamePhase.GameOver)
            {
                gameBoard.timeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Отрисовка объектов на экране. Вызывается автоматически.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Заливка фона
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Главное меню
            if(gameBoard.currentGamePhase == GamePhase.MainMenu)
            {
                playButton.Draw(_spriteBatch);
            }
            // Экран Game Over
            else if(gameBoard.currentGamePhase == GamePhase.GameOver)
            {
                Vector2 gameOverTextSize = bigFont.MeasureString("Game Over");
                Vector2 halfSize = gameOverTextSize / 2;
                Vector2 finalGameOverTextOffset = gameOverTextOffset - halfSize;
                _spriteBatch.DrawString(bigFont, "Game Over", finalGameOverTextOffset, Color.Orange);
                Vector2 pointsTextOffset = new Vector2(10.0f, gameOverTextSize.Y);
                Vector2 pointsTextPosition = finalGameOverTextOffset + pointsTextOffset;
                _spriteBatch.DrawString(font, $"Очки: {gameBoard.score}", pointsTextPosition, Color.Yellow);
                okButton.Draw(_spriteBatch);
            }
            // Основной режим игры
            else
            {
                // Объектоы на игровом поле
                gameBoard.objectList.ForEach(obj => obj.Draw(_spriteBatch));
                // Разрушители
                gameBoard.destroyerList.ForEach(obj => obj.Draw(_spriteBatch));
                // Верхняя панель
                _spriteBatch.Draw(topPanel, new Rectangle(0, 0, width, 50), Color.Black);
                // Количество очков
                Vector2 scoreTextPosition = new Vector2(0, 0) + scoreTextOffset;
                _spriteBatch.DrawString(font, $"Очки: {gameBoard.score}", scoreTextPosition, Color.Yellow);
                // Оставшееся время
                Vector2 timeTextSize = font.MeasureString($"Время: {gameBoard.timeRemaining:0}");
                Vector2 timeTextPosition = new Vector2(width - timeTextSize.X - timeTextOffset.X, timeTextOffset.Y);
                _spriteBatch.DrawString(font, $"Время: {gameBoard.timeRemaining:0}", timeTextPosition, Color.Yellow);
            }

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
