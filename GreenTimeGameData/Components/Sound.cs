using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenTimeGameData.Components
{
    public class Sound
    {
        #region Properties
        public string Resource { get; set; }

        public bool Looping { get; set; }

        public StateDependency[] DependentStates;
        #endregion
    }
}
