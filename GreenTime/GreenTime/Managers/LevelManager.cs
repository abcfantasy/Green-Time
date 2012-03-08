using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using GreenTimeGameData.Components;
using GreenTime.GameObjects;

namespace GreenTime.Managers
{
    public sealed class LevelManager
    {
        #region Enumerations
        private enum Transition
        {
            Right,
            Left,
            Time
        }
        #endregion

        #region Constants
        public const int EMPTY_VALUE = -99;
        #endregion

        #region Fields
        private Vector2 playerPosition = new Vector2( 0, 300 );
        private Sprite pickedObject;    // currently picked up object - save it here to know when changing scenes
        private Level currentLevel = null;
        private Level lastLevel = null;     // the level the player was in before going to the past

        private Dictionary<String, Level> levels = new Dictionary<string,Level>();
        private Chat[] chats;
        #endregion

        #region Properties
        public Vector2 PlayerPosition
        {
            get
            {
                return playerPosition;
            }
        }

        public Sprite PickedObject
        {
            get
            {
                return pickedObject;
            }
            set
            {
                pickedObject = value;
            }
        }

        public Level CurrentLevel
        {
            get
            {
                return currentLevel;
            }
        }

        public Level LastPresentLevel
        {
            get
            {
                return lastLevel;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parses levels and chats XML file and stores information in memory
        /// </summary>
        public void LoadAllLevels( ContentManager content )
        {
            Level[] levelArray = content.Load<Level[]>("levels");
            for (int i = 0; i < levelArray.Length; i++)
            {
                levels.Add(levelArray[i].Name, levelArray[i]);
            }
            chats = content.Load<Chat[]>("chats");

            // TEST
            currentLevel = levels["bedroom"];
        }

        public Boolean CanTransitionRight()
        {
            return !currentLevel.RightScreenName.Equals("");
        }

        public Boolean CanTransitionLeft()
        {
            return !currentLevel.LeftScreenName.Equals("");
        }

        /// <summary>
        /// Moves the level state to one screen to the right
        /// </summary>
        public void TransitionRight()
        {
            // modify current level
            currentLevel = levels[currentLevel.RightScreenName];

            // set player position
            this.playerPosition = new Vector2(0, 300);
        }

        /// <summary>
        /// Moves the level state to one screen to the left
        /// </summary>
        public void TransitionLeft()
        {
            // modify current level
            currentLevel = levels[currentLevel.LeftScreenName];

            // set player position
            this.playerPosition = new Vector2(SettingsManager.GAME_WIDTH - SettingsManager.PLAYER_WIDTH, 300);
        }

        /// <summary>
        /// Moves the level state to a past transition
        /// </summary>
        /// <param name="transitionIndex"></param>
        public void TransitionPast( string transitionIndex )
        {
            // save current level
            lastLevel = currentLevel;

            // modify current level 
            currentLevel = levels[transitionIndex];
        }

        public void TransitionPresent()
        {
            // reset back to present state
            StateManager.Current.ResetReturnToPresent();

            // modify current level
            currentLevel = lastLevel;
            lastLevel = null;
        }

        /// <summary>
        /// Gets a specific chat by index
        /// </summary>
        /// <param name="chatIndex"></param>
        /// <returns></returns>
        public Chat GetChat(int chatIndex)
        {
            for (int i = 0; i < chats.Length; i++)
            {
                if (chats[i].Index == chatIndex)
                    return chats[i];
            }
            return null;
        }

        /// <summary>
        /// Gives the name of a news texture file that matches with the current game state
        /// </summary>
        /// <returns>The name of the news texture file</returns>
        public string GetNewsTexture()
        {
            // TODO: change this to match with game state
            return "news1";
        }
        #endregion

        #region Singleton
        private static readonly LevelManager instance = new LevelManager();

        private LevelManager() { }

        public static LevelManager State
        {
            get
            {
                return instance;
            }
        }
        #endregion
    }
}
