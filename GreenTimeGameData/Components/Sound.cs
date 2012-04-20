using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class Sound
    {
        #region Fields
        public string name;

        [ContentSerializer(Optional = true)]
        public bool looping = true;

        [ContentSerializer(ElementName = "playIf", CollectionItemName = "state", Optional = true)]        
        public List<State> dependencies = null;
        #endregion
    }
}
