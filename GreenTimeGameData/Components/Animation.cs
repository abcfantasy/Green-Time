using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class Animation
    {
        #region Properties
        /// <summary>
        /// The width of each frame
        /// </summary>
        public int FrameWidth { get; set; }

        /// <summary>
        /// The height of each frame
        /// </summary>
        public int FrameHeight { get; set; }

        /// <summary>
        /// The number of frames to play per seconds
        /// </summary>
        public int FramesPerSecond { get; set; }

        /// <summary>
        /// A list of animations
        /// </summary>
        public AnimationPlayback[] Playbacks { get; set; }
        #endregion
    }
}
