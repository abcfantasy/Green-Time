using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTimeGameData.Components;

namespace GreenTime.Managers
{
    public class StateManager
    {
        #region State Constants
        // state to mark game should transition back to present
        public readonly string STATE_BACKTOPRESENT = "back_to_present";
        // state to keep track if indoor puzzle is solved or not
        public readonly string STATE_INDOORSOLVED = "indoor_solved";
        // state to keep track of player status (0-32 = grey square head; 33-62 = grey round head; 63-100 = green round head;)
        public readonly string STATE_PLAYERSTATUS = "player_status";
        #endregion

        #region Fields
        private Dictionary<string, int> states = new Dictionary<string,int>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the value of a state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        private int GetState(string stateName)
        {
            // if key is present, return key value
            if (states.ContainsKey(stateName))
                return states[stateName];
            // otherwise return false
            else
                return 0;
        }

        /// <summary>
        /// Sets the value of a state
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void SetState(string stateName, int value)
        {
            if (states.ContainsKey(stateName))
                states[stateName] = value;
            else
                states.Add(stateName, value);
        }

        /// <summary>
        /// Checks the global state 'back_to_present' to see if player should be returned to present (100 = return)
        /// </summary>
        /// <returns></returns>
        public bool ShouldReturnToPresent()
        {
            return GetState(STATE_BACKTOPRESENT) == 100;
        }

        /// <summary>
        /// Resets the state to return to present
        /// </summary>
        public void ResetReturnToPresent()
        {
            SetState(STATE_BACKTOPRESENT, 0);
        }

        /// <summary>
        /// Helper method to check if one or more states are satisfied
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        public bool DependentStatesSatisfied(StateDependency[] states)
        {
            if (states.Length == 0)
                return true;

            bool satisfied = true;

            for (int i = 0; i < states.Length; i++)
            {
                int currentStateValue = GetState(states[i].StateName);
                // state value must be between state dependency values
                if (currentStateValue < states[i].StateLowValue || currentStateValue > states[i].StateHighValue)
                    satisfied = false;
            }

            return satisfied;
        }

        /// <summary>
        /// Helper method to change one or more states
        /// </summary>
        /// <param name="states"></param>
        public void ModifyStates(State[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                SetState(states[i].StateName, states[i].StateValue);
            }
        }
        #endregion

        #region Singleton
        private static readonly StateManager instance = new StateManager();

        private StateManager() { }

        public static StateManager Current
        {
            get
            {
                return instance;
            }
        }
        #endregion
    }
}
