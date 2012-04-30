using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GreenTimeGameData.Components
{
    class MovingAnimatedSprite : AnimatedSprite
    {
        #region Fields
        public Vector2 velocityPerSecond;
        #endregion

        public override void UpdateFrame(double elapsed)
        {
            base.UpdateFrame(elapsed);

            this.position.X += (float)(velocityPerSecond.X * elapsed);
            this.position.Y += (float)(velocityPerSecond.Y * elapsed);
        }
    }
}
