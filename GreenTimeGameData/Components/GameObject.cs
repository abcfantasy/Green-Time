using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class GameObject
    {
        #region Fields

        // Graphic representation of the object
        [ContentSerializer(Optional = true)]
        public Sprite sprite = null;

        // Game states where the object is visible
        [ContentSerializer(ElementName = "showIf", CollectionItemName = "state", Optional = true)]
        public State[] dependencies = null;

        // Interaction object that determines how the object reacts when we interact with it
        [ContentSerializer(ElementName = "onInteract", Optional = true)]
        public Interaction interaction = null;

        #endregion
    }
}
