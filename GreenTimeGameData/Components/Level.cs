using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class Level
    {
        #region Properties
        /// <summary>
        /// the name of the level
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the background texture of the level
        /// </summary>
        public string BackgroundTexture { get; set; }

        /// <summary>
        /// name of level on the left
        /// </summary>
        public string LeftScreenName { get; set; }

        /// <summary>
        /// name of level on the right
        /// </summary>
        public string RightScreenName { get; set; }

        /// <summary>
        /// list of game objects in the level
        /// </summary>
        public List<InteractiveObject> GameObjects { get; set; }
        #endregion
    }
}
