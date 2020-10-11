using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public class Destroyer : GenericObject
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="spriteWorldPos">Позиция спрайта.</param>
        public Destroyer(Vector2 spriteWorldPos)
        {
            this.spriteWorldPos = spriteWorldPos;
            sprite = Game1.destroyerSprite;
            spriteScale = 0.3f;
        }
    }
}
