﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        // Масштаб спрайтов в клетках, чтобы они не занимали всю клекту
        public static float spriteDownScale = 0.9f;
        // Сдвиг игрового поля в пикселях, чтобы оно было не с краю
        public static Vector2 gameBoardOffset = new Vector2(10f, 10f);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
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

        protected override void Update(GameTime gameTime)
        {
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here

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
