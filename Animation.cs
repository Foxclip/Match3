using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Match3
{
    public abstract class Animation
    {
        /// <summary>
        /// Привязанный объект на игровом поле.
        /// </summary>
        protected GenericObject linkedObject;
        /// <summary>
        /// Длительность анимации
        /// </summary>
        protected double duration = 1.0;
        /// <summary>
        /// Время в миллисекундах, прошедшее с начала анимации
        /// </summary>
        protected double timePassed = 0.0;
        /// <summary>
        /// Активна анимация или нет.
        /// </summary>
        public bool active = false;
        /// <summary>
        /// Блокирует ли анимация переход в следующее состояние игры.
        /// </summary>
        public bool blocking;

        /// <summary>
        /// Обновить состояние анимации.
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Действия при удалении анимации.
        /// </summary>
        public abstract void OnDelete();
    }
}
