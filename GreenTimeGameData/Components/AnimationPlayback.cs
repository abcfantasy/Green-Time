using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class AnimationPlayback
    {
        #region Properties
        /// <summary>
        /// The name of this animation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of frames to play
        /// </summary>
        public int[] Frames { get; set; }

        /// <summary>
        /// The states this animation depends on to play
        /// </summary>
        [ContentSerializer(ElementName = "playIf", CollectionItemName = "state", Optional = true)]
        public State[] dependencies = null;
        #endregion
    }
}
