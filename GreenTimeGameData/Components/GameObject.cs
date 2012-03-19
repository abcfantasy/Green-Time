using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class GameObject
    {
        #region Properties
        /// <summary>
        /// sprite graphic for the object (null if no graphic)
        /// </summary>
        [ContentSerializer(Optional = true)]
        public Sprite sprite = null;

        /// <summary>
        /// game states that the object relies on
        /// </summary>
        [ContentSerializer(ElementName = "showIf", CollectionItemName = "state", Optional = true)]
        public State[] dependencies = null;

        [ContentSerializer(ElementName = "onInteract", Optional = true)]
        public Interaction interaction = null;

        #endregion
    }
}
