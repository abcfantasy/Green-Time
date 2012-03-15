using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class State
    {
        #region Properties
        public string name;

        [ContentSerializer(Optional = true)]
        public int value = -1;

        [ContentSerializer(ElementName = "between", Optional = true)]
        public Vector2 minmax = new Vector2( -1, -1 );
        #endregion
    }
}
