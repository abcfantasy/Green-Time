using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace GreenTimeGameData.Components
{
    public class GameObject
    {
        #region Fields

        // Graphic representation of the object
        [ContentSerializer(Optional = true)]
        public Sprite sprite = null;

        // Game states where the object is visible
        [ContentSerializer(ElementName = "showIf", CollectionItemName = "state", Optional = true)]
        public List<State> dependencies = null;

        // Interaction object that determines how the object reacts when we interact with it
        [ContentSerializer(ElementName = "onInteract", Optional = true)]
        public Interaction interaction = null;

        #endregion

        #region Initialization
        public void Load()
        {
            if (interaction != null)
            {
                if (!String.IsNullOrEmpty(interaction.pickUpName))
                {
                    if (dependencies == null) dependencies = new List<State>();
                    dependencies.Add(new State(interaction.pickUpName + "_picked", 0));

                    if (interaction.affectedStates == null) interaction.affectedStates = new List<State>();
                    interaction.affectedStates.Add(new State(interaction.pickUpName + "_picked", 50));
                    interaction.affectedStates.Add(new State("item_picked", 100));
                }
                else if (interaction.dropper != null)
                {
                    Dropper dropper = interaction.dropper;
                    if (dependencies == null) dependencies = new List<State>();
                    if (dropper.drops.Count == 1) dependencies.Add(new State(dropper.drops[0] + "_picked", 50));
                    else dependencies.Add(new State("item_picked", 100));

                    if (interaction.affectedStates == null) interaction.affectedStates = new List<State>();
                    interaction.affectedStates.Add(new State("item_picked", 0));
                }
            }
        }
        #endregion
    }
}
