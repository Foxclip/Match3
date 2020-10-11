using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Match3
{
    public class ScaleAnimation : Animation
    {
        /// <summary>
        /// Нелинейное изменение размера: при значениях >1 анимация замедляется, при <1 - ускоряется
        /// </summary>
        readonly double power = 1.0;
        /// <summary>
        /// Масштаб в начале анимации.
        /// </summary>
        readonly double beginScale;
        /// <summary>
        /// Масштаб в конце анимации.
        /// </summary>
        readonly double endScale;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="linkedObject">Привязанный обеъект на игровом поле.</param>
        /// <param name="blocking">Блокирует ли анимация переход в следующее состояние игры.</param>
        public ScaleAnimation(GameBoardObject linkedObject, double beginScale, double endScale, bool blocking = false)
        {
            this.linkedObject = linkedObject;
            this.beginScale = beginScale;
            this.endScale = endScale;
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
                double objectScale = Utils.MapRange(nonlinear, 0.0f, 1.0f, beginScale, endScale);
                linkedObject.spriteAnimatedScale = (float)objectScale;
            }
        }

        /// <summary>
        /// Действия при удалении анимации.
        /// </summary>
        public override void OnDelete()
        {
            linkedObject.spriteAnimatedScale = (float)endScale;
            active = false;
        }
    }
}
