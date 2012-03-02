using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class Sprite
    {
        #region Properties
        /// <summary>
        /// the texture of the sprite
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// the x position
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// the y position
        /// </summary>
        public int PositionY { get; set; }

        /// <summary>
        /// the animation for this sprite
        /// </summary>
        public List<Animation> Animation { get; set; }
        #endregion
    }
}
