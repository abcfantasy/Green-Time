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
        public static readonly int TOTAL_PUZZLES = 4;

        // state to mark game should transition back to present
        public const string STATE_BACKTOPRESENT = "back_to_present";
        // state to keep track of the indoor puzzles and whether or not they're solved
        public const string STATE_INDOOR = "indoor_puzzle";
        // state to keep track of player status (0 = grey square head; 50 = grey round head; 100 = green round head;)
        public const string STATE_PLAYERSTATUS = "player_status";
        // state to mark if game is being loaded from a saved game
        public const string STATE_LOAD = "game_load";
        // state to keep track of which day it is
        public const string STATE_DAY = "day";
        // state that tells us whether or not we're in the past
        public const string STATE_IN_PAST = "state_in_past";
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

        #region Time Travelling
        public bool CanTimeTravel()
        {
            // Can only do timetravelling if you haven't finished the game
            // And if you are green and round (daily puzzle completed)
            return ( GetState("progress") != 100 && GetState( STATE_PLAYERSTATUS ) > 50 );
        }

        public bool IsInPast()
        {
            return (GetState(STATE_IN_PAST) == 100);
        }

        public void GoToPast()
        {
            SetState(STATE_IN_PAST, 100);
        }

        public void GoToPresent()
        {
            SetState(STATE_IN_PAST, 0);
        }

        public bool ShouldReturnToPresent()
        {
            return GetState(STATE_BACKTOPRESENT) == 100;
        }

        public void ResetReturnToPresent()
        {
            SetState(STATE_BACKTOPRESENT, 0);
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
            // check if game is completed
            if (stateName.StartsWith("puzzle") && GetState("progress") == 100)
                return 0;

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
        public void SetState(State s)
        {
            SetState(s.name, s.value);
        }

        public void SetState(string stateName, int value)
        {
            if (value < 0) return;
            // if puzzle solved
            if (stateName.StartsWith("puzzle") && value == 100)
            {
                SetState("progress", GetState("progress") + 99 / TOTAL_PUZZLES);
            }

            if (states.ContainsKey(stateName))
                states[stateName] = value;
            else
                states.Add(stateName, value);
        }

        /// <summary>
        /// Checks the global state 'advance_day' to see if player should advance the day
        /// </summary>
        /// <returns></returns>
        public bool ShouldAdvanceDay()
        {
            return GetState("advance_day") == 100;
        }

        /// <summary>
        /// Helper method to check if one or more states are satisfied
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        public bool CheckDependencies( List<State> states )
        {
            if (states == null || states.Count == 0)
                return true;

            bool satisfied;
            int currentStateValue;

            foreach( State s in states ) {
                currentStateValue = GetState(s.name);
                if (s.value != -1)
                    satisfied = s.value == currentStateValue;
                else if (s.minmax.X != -1 && s.minmax.Y != -1)
                    satisfied = (s.minmax.X <= currentStateValue && currentStateValue <= s.minmax.Y);
                else
                    satisfied = false;

                if (!satisfied)
                    return false;
            }

            return true;
        }

        public bool AllTrue(List<State> states)
        {
            if (states == null || states.Count == 0)
                return true;

            bool satisfied;
            int currentStateValue;

            foreach (State s in states)
            {
                currentStateValue = GetState(s.name);
                if (s.value != -1)
                    satisfied = s.value == currentStateValue;
                else if (s.minmax.X != -1 && s.minmax.Y != -1)
                    satisfied = (s.minmax.X <= currentStateValue && currentStateValue <= s.minmax.Y);
                else
                    satisfied = false;

                if (!satisfied)
                    return false;
            }

            return true;
        }

        public bool AnyTrue(List<State> states)
        {
            if (states == null || states.Count == 0)
                return true;

            bool satisfied;
            int currentStateValue;

            foreach (State s in states)
            {
                currentStateValue = GetState(s.name);
                if (s.value != -1)
                    satisfied = s.value == currentStateValue;
                else if (s.minmax.X != -1 && s.minmax.Y != -1)
                    satisfied = (s.minmax.X <= currentStateValue && currentStateValue <= s.minmax.Y);
                else
                    satisfied = false;

                if (satisfied)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to change one or more states
        /// </summary>
        /// <param name="states"></param>
        public void ModifyStates(List<State> states)
        {
            if (states == null) return;
            foreach (State s in states) {
                SetState( s );
            }
        }

        public void AdvanceDay()
        {
            SetState("advance_day", 0);

            int day = GetState( STATE_DAY );
            ++day;
            SetState( STATE_DAY, day);
            SetState( STATE_INDOOR, (new Random()).Next( 1, 7 ) * 10 );
            SetState("news_taken", 0);
            //SetState("is_in_past", 0);

            // The picked objects reset daily
            List<State> toModify = new List<State>();
            foreach (string s in states.Keys)
                if (s.EndsWith("_picked"))
                    toModify.Add(new State(s, 0));
            ModifyStates(toModify);

            LevelManager.Instance.PickedObject = null;
            LevelManager.Instance.GoHome();
        }

        #endregion

        #region Singleton
        private static readonly StateManager instance = new StateManager();

        private StateManager() {
            SetState(STATE_PLAYERSTATUS, 100);
            SetState(STATE_DAY, 0);
        }

        public static StateManager Instance { get { return instance; } }
        #endregion
    }
}
