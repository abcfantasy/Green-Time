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
        public static readonly string STATE_BACKTOPRESENT = "back_to_present";
        // state to keep track of the indoor puzzles and whether or not they're solved
        public static readonly string STATE_INDOOR = "indoor_puzzle";
        // state to keep track of player status (0 = grey square head; 50 = grey round head; 100 = green round head;)
        public static readonly string STATE_PLAYERSTATUS = "player_status";
        // state to keep track of how green the player is (0 = grey)
        public static readonly string STATE_PLAYERGREEN = "player_green";
        // state to mark that player should fade from grey to green
        public static readonly string STATE_PLAYERFADETOGREEN = "player_fadeToGreen";
        // state to mark that player should fade from green to grey
        public static readonly string STATE_PLAYERFADETOGREY = "player_fadeToGrey";

        // state to keep track of which day it is
        public static readonly string STATE_DAY = "day";
        #endregion

        #region Fields
        private Dictionary<string, int> states = new Dictionary<string,int>();
        #endregion

        #region Properties
        public Dictionary<string, int> AllStates
        {
            get
            {
                return states;
            }
            set
            {
                states = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the value of a state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public int GetState(string stateName)
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
        public void SetState(string stateName, int value)
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

        public void AdvanceDay()
        {
            int day = GetState( STATE_DAY );
            ++day;
            SetState( STATE_DAY, day);
            SetState( STATE_INDOOR, (new Random()).Next( 1, 7 ) * 10 );
            SetState("news_taken", 0);

            // reset other states (hardcoded!)
            SetState("garbage1_picked", 0);
            SetState("garbage2_picked", 0);
            SetState("garbage3_picked", 0);

            LevelManager.State.GoHome();
        }

        #endregion

        #region Singleton
        private static readonly StateManager instance = new StateManager();

        private StateManager() {
            SetState( STATE_PLAYERSTATUS, 100 );
            SetState( STATE_DAY, 0);
        }

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
