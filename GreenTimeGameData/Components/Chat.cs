using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class Chat
    {
        #region Properties
        /// <summary>
        /// the index of the chat
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// the text of the chat
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// a set of states that this chat depends on
        /// </summary>
        [ContentSerializer(ElementName = "showIf", CollectionItemName = "state", Optional = true)]
        public List<State> dependencies = null;

        /// <summary>
        /// a set of answers for this chat
        /// </summary>
        [ContentSerializer(Optional = true)]
        public Answer[] answers = null;

        /// <summary>
        /// a set of states that this chat effects
        /// </summary>
        [ContentSerializer(CollectionItemName = "state", Optional = true)]
        public List<State> affectedStates = null;
        #endregion
    }
}
