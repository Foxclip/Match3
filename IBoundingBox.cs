using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Match3
{
    interface IBoundingBox
    {
        public Rectangle GetScreenBoundingBox();
    }
}
