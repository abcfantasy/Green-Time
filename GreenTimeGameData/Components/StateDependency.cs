using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class StateDependency
    {
        #region Properties
        /// <summary>
        /// the name of the dependent state
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// the low value of the state for this dependency to be satisfied
        /// </summary>
        public int StateLowValue { get; set; }

        /// <summary>
        /// the high value of the state for this dependency to be satisfied
        /// </summary>
        public int StateHighValue { get; set; }
        #endregion
    }
}
