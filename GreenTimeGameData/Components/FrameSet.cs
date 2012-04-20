using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class FrameSet
    {
        #region Constants
        // Constant string that represents the default, idle animation
        public const string IDLE = "idle";
        #endregion

        #region Fields
        [ContentSerializer(Optional = true)]
        public string name = IDLE;

        // The list of frames to play
        public int[] frames;

        // The states this animation depends on to play
        [ContentSerializer(ElementName = "playIf", CollectionItemName = "state", Optional = true)]
        public List<State> dependencies = null;
        #endregion
    }
}
