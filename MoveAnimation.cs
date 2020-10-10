using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Match3
{
    class MoveAnimation : Animation
    {

        /// <summary>
        /// Нелинейное изменение скорости: при маленьких значениях анимация замедляется, при больших - ускоряется
        /// </summary>
        public double power = 1;
        /// <summary>
        /// Позиция объекта в начале анимации
        /// </summary>
        private Vector2 beginPos;
        /// <summary>
        /// Позиция объекта в конце анимации
        /// </summary>
        private Vector2 endPos;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="linkedObject">Привязанный объект на игровом поле.</param>
        /// <param name="beginPos">Позиция в начале анимации.</param>
        /// <param name="endPos">Позиция в конце анимации.</param>
        /// <param name="blocking">Блокирует ли анимация переход в следующее состояние игры.</param>
        public MoveAnimation(GameBoardObject linkedObject, Vector2 beginPos, Vector2 endPos, bool blocking = false)
        {
            this.linkedObject = linkedObject;
            this.beginPos = beginPos;
            this.endPos = endPos;
            this.blocking = blocking;
            duration = 0.3;
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
                if(timePassed > duration)
                {
                    timePassed = duration;
                    active = false;
                }
                double completionPercentage = timePassed / duration;
                double nonlinear = Math.Pow(completionPercentage, power);
                Vector2 diff = endPos - beginPos;
                Vector2 diffScaled = diff * (float)nonlinear;
                Vector2 newPos = beginPos + diffScaled;
                linkedObject.spriteWorldPos = newPos;
            }
        }

        /// <summary>
        /// Действия при удалении анимации.
        /// </summary>
        public override void OnDelete()
        {
            // Возвращаем спрайт на место
            linkedObject.spriteWorldPos = linkedObject.worldPos;
            active = false;
        }
    }
}
