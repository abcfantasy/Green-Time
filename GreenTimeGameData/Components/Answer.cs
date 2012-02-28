using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class Answer
    {
        #region Properties
        /// <summary>
        /// the text of this answer
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// the index that the chat leads to upon choosing this answer
        /// </summary>
        public int ResponseIndex { get; set; }
        #endregion
    }
}
