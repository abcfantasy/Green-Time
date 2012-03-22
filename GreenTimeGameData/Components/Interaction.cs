using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class Interaction
    {
        #region Fields
        // The bounding interval of the interaction
        public int boundX;
        public int boundWidth;

        // Text to be displayed when the player is over the object
        public string text;

        // Whether or not we collide with the object
        [ContentSerializer(Optional = true)]
        public bool solid = false;

        // The effects that a successful interaction can have
        // Callback for special cases (e.g. news)
        [ContentSerializer(Optional = true)]
        public string callback = null;

        // Transition to the past
        [ContentSerializer(Optional = true)]
        public string transition = null;

        // Start of a conversation
        [ContentSerializer(Optional = true)]
        public int chatIndex = -99;

        // Change of states
        [ContentSerializer(Optional = true, CollectionItemName="state")]
        public State[] affectedStates = null;
        
        #endregion
    }
}
