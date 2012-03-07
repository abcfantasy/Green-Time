using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class InteractiveObject
    {
        #region Properties
        /// <summary>
        /// whether object is impassable or not
        /// </summary>
        public bool Impassable { get; set; }

        /// <summary>
        /// the starting X coordinate where player collides with object
        /// </summary>
        public int BoundX { get; set; }

        /// <summary>
        /// the width of the boundix box of the object
        /// </summary>
        public int BoundWidth { get; set; }

        /// <summary>
        /// text displayed when player over object
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// secial field to handle particular situations
        /// </summary>
        public string Special { get; set; }

        /// <summary>
        /// name of level to which to transition to (-99 for no transition)
        /// </summary>
        public string Transition { get; set; }

        /// <summary>
        /// the index of the chat associated with object (-99 if none)
        /// </summary>
        public int ChatIndex { get; set; }

        /// <summary>
        /// sprite graphic for the object (null if no graphic)
        /// </summary>
        public Sprite[] Sprite { get; set; }

        /// <summary>
        /// game states that the object relies on
        /// </summary>
        public StateDependency[] DependentStates { get; set; }

        /// <summary>
        /// game states that the object changes upon acting on it
        /// </summary>
        public State[] EffectedStates { get; set; }
        #endregion
    }
}
