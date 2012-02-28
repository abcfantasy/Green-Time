using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTimeGameData.Components;

namespace GreenTime.Managers
{
    public class StateManager
    {
        #region Fields
        private Dictionary<string, bool> states = new Dictionary<string,bool>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the value of a state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        private bool GetState(string stateName)
        {
            // if key is present, return key value
            if (states.ContainsKey(stateName))
                return states[stateName];
            // otherwise return false
            else
                return false;
        }

        /// <summary>
        /// Sets the value of a state
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void SetState(string stateName, bool value)
        {
            if (states.ContainsKey(stateName))
                states[stateName] = value;
            else
                states.Add(stateName, value);
        }

        /// <summary>
        /// Checks the global state 'back_to_present' to see if player should be returned to present
        /// </summary>
        /// <returns></returns>
        public bool ShouldReturnToPresent()
        {
            return GetState("back_to_present");
        }

        /// <summary>
        /// Resets the state to return to present
        /// </summary>
        public void ResetReturnToPresent()
        {
            SetState("back_to_present", false);
        }

        /// <summary>
        /// Helper method to check if one or more states are satisfied
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        public bool DependentStatesSatisfied(State[] states)
        {
            bool satisfied = true;

            for (int i = 0; i < states.Length; i++)
            {
                // if the state is not in the desired value, it is not satisfied
                if (GetState(states[i].StateName) != states[i].StateValue)
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
