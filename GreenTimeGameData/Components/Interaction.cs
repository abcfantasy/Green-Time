using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class Interaction
    {
        #region Properties
        public int boundX;
        public int boundWidth;
        public string text;

        [ContentSerializer(Optional = true)]
        public bool solid = false;

        [ContentSerializer(Optional = true)]
        public string callback = null;

        [ContentSerializer(Optional = true)]
        public string transition = null;

        [ContentSerializer(Optional = true)]
        public int chatIndex = -99;

        [ContentSerializer(Optional = true, CollectionItemName="state")]
        public State[] affectedStates = null;
        #endregion
    }
}
