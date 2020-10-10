using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Match3
{
    public class PulseAnimation : Animation
    {

        /// <summary>
        /// Период анимациию
        /// </summary>
        public double period = 1;
        /// <summary>
        /// Сдвиг фазы анимации.
        /// </summary>
        public double offset = Math.PI;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="linkedObject">Привязанный обеъект на игровом поле.</param>
        /// <param name="blocking">Блокирует ли анимация переход в следующее состояние игры.</param>
        public PulseAnimation(GameBoardObject linkedObject, bool blocking = false)
        {
            this.linkedObject = linkedObject;
            this.blocking = blocking;
            timePassed = 0.0;
            active = true;
        }

        /// <summary>
        /// Обновить состояние анимации.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if(active)
            {
                timePassed += gameTime.ElapsedGameTime.TotalSeconds;
                double newScaleRaw = Math.Sin(timePassed * Math.PI * 2.0f / period + offset);
                double newScaleScaled = Utils.MapRange(newScaleRaw, -1, 1, 0.75, 1);
                linkedObject.spriteAnimatedScale = (float)newScaleScaled;
            }
        }

        /// <summary>
        /// Действия при удалении анимации.
        /// </summary>
        public override void OnDelete()
        {
            // Возвращаем исходный масштаб
            linkedObject.spriteAnimatedScale = 1.0f;
            active = false;
        }
    }
}
