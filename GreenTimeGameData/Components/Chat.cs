using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class Chat
    {
        #region Properties
        [ContentSerializer(CollectionItemName = "line")]
        public List<string> text;

        [ContentSerializer(ElementName = "showIf", CollectionItemName = "state", Optional = true)]
        public List<State> dependencies = null;

        [ContentSerializer(ElementName = "leadsTo", CollectionItemName = "answer", Optional = true)]
        public List<int> answers = null;

        [ContentSerializer(CollectionItemName = "state", Optional = true)]
        public List<State> affectedStates = null;
        #endregion
    }
}
