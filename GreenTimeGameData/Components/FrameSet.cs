using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class FrameSet
    {
        #region Properties
        [ContentSerializer(Optional = true)]
        public string name = "default";

        /// <summary>
        /// The list of frames to play
        /// </summary>
        public int[] frames;

        /// <summary>
        /// The states this animation depends on to play
        /// </summary>
        [ContentSerializer(ElementName = "playIf", CollectionItemName = "state", Optional = true)]
        public State[] dependencies = null;
        #endregion
    }
}
