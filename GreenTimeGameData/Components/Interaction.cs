using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

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
        public string chat = null;

        [ContentSerializer(Optional = true)]
        public Vector2 mouth = Vector2.Zero;

        // An identifier for the object when it's picked up
        [ContentSerializer(Optional = true)]
        public string pickUpName = null;

        // Information about the items that can be dropped in this object and their effects
        [ContentSerializer(Optional = true)]
        public Dropper dropper = null;

        [ContentSerializer(Optional = true)]
        public Sound sound = null;

        // Change of states
        [ContentSerializer(Optional = true, CollectionItemName="state")]
        public List<State> affectedStates = null;
        #endregion
    }
}
