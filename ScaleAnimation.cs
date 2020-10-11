using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// Действие, выполняющееся после завершения анимации.
        /// </summary>
        private Action<GenericObject> finishedCallback;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="linkedObject">Привязанный обеъект на игровом поле.</param>
        /// <param name="blocking">Блокирует ли анимация переход в следующее состояние игры.</param>
        public ScaleAnimation(GameBoardObject linkedObject, double beginScale, double endScale, double delay = 0.0, bool blocking = false, Action<GenericObject> finishedCallback = null)
        {
            this.linkedObject = linkedObject;
            this.beginScale = beginScale;
            this.endScale = endScale;
            this.blocking = blocking;
            this.finishedCallback = finishedCallback;
            this.delay = delay;
            duration = 0.3;
            timePassed = -delay;
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
                if(timePassed < 0)
                {
                    return;
                }
                if(timePassed > duration)
                {
                    timePassed = duration;
                    active = false;
                    finishedCallback?.Invoke(linkedObject);
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
