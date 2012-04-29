using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class Level
    {
        #region Fields
        /// <summary>
        /// the name of the level
        /// </summary>
        public string name;

        [ContentSerializer(Optional = true)]
        public string texture;

        /// <summary>
        /// the background texture of the level
        /// </summary>
        public Sprite backgroundTexture;

        /// <summary>
        /// name of level on the left
        /// </summary>
        public string leftScreenName;

        /// <summary>
        /// name of level on the right
        /// </summary>
        public string rightScreenName;

        /// <summary>
        /// up to one ambient sound in the level
        /// </summary>
        [ContentSerializer(Optional = true)]
        public Sound ambientSound = null;

        /// <summary>
        /// list of game objects in the level
        /// </summary>
        public List<GameObject> gameObjects;
        #endregion
    }
}
