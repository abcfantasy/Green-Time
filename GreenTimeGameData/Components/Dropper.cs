using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class Dropper
    {
        public const string ANY = "any";
        public const string ALL = "all";
      
        public List<string> drops;

        [ContentSerializer(Optional = true)]
        public string trigger = ANY;

        [ContentSerializer(Optional = true)]
        public bool reset = false;

        [ContentSerializer(CollectionItemName = "state", Optional = true)]
        public List<State> effects = null;
    }
}
