using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

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
        public Vector2 Position { get; set; }

        /// <summary>
        /// the animation for this sprite
        /// </summary>
        public List<Animation> Animation { get; set; }
        
        /// <summary>
        /// whether or not the object is affected by the desaturation shader
        /// </summary>
        public bool Shaded { get; set; }
        #endregion
    }
}
