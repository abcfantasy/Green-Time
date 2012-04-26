using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace GreenTimeGameData.Components
{
    public class Sound
    {
        #region Fields
        public string name;

        [ContentSerializer(Optional = true)]
        public bool looping = true;

        [ContentSerializer(Optional = true)]
        public int position = 0;

        [ContentSerializer(ElementName = "playIf", CollectionItemName = "state", Optional = true)]        
        public List<State> dependencies = null;
        #endregion
    }
}
