﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class State
    {
        #region Properties
        /// <summary>
        /// the name of this state
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// the value of this state (0 to 100)
        /// </summary>
        public int StateValue { get; set; }
        #endregion
    }
}
