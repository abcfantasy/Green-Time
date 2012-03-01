using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public StateDependency[] DependentStates { get; set; }

        /// <summary>
        /// a set of answers for this chat
        /// </summary>
        public Answer[] Answers { get; set; }

        /// <summary>
        /// a set of states that this chat effects
        /// </summary>
        public State[] States { get; set; }
        #endregion
    }
}
