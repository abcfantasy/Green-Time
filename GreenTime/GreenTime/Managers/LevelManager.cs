﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using GreenTimeGameData.Components;

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
        public const string HOME = "bedroom";
        #endregion

        #region Fields
        private ContentManager content;
        private Vector2 playerPosition = new Vector2( 0, 250 );
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
            set
            {
                playerPosition = value;
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
            set
            {
                currentLevel = value;
            }
        }

        public Level LastPresentLevel
        {
            get
            {
                return lastLevel;
            }
            set
            {
                lastLevel = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parses levels and chats XML file and stores information in memory
        /// </summary>
        public void LoadAllLevels( ContentManager content )
        {
            this.content = content;
            /*string[] levelsList = content.Load<String[]>("levels_list");

            Level[] levelArray = content.Load<Level[]>("levels");
            for (int i = 0; i < levelArray.Length; i++)
            {
                levels.Add(levelArray[i].name, levelArray[i]);
            }*/
            chats = content.Load<Chat[]>("chats");
            StateManager.Instance.AdvanceDay();
        }

        public void GoTo( string level )
        {
            if ( ! levels.ContainsKey( level ))
                levels[level] = content.Load<Level>( @"levels\" + level );

            currentLevel = levels[level];
        }

        public void GoHome()
        {
            GoTo( HOME );
            playerPosition = new Vector2(250, 250);
        }

        public Boolean CanTransitionRight()
        {
            return !currentLevel.rightScreenName.Equals("");
        }

        public Boolean CanTransitionLeft()
        {
            return !currentLevel.leftScreenName.Equals("");
        }

        /// <summary>
        /// Moves the level state to one screen to the right
        /// </summary>
        public void TransitionRight()
        {
            // reset loaded state (only happens on the starting screen)
            StateManager.Instance.SetState(StateManager.STATE_LOAD, 0);

            GoTo(currentLevel.rightScreenName);

            // set player position
            this.playerPosition = new Vector2(0, 250);
        }

        /// <summary>
        /// Moves the level state to one screen to the left
        /// </summary>
        public void TransitionLeft()
        {
            // reset loaded state (only happens on the starting screen)
            StateManager.Instance.SetState(StateManager.STATE_LOAD, 0);

            GoTo(currentLevel.leftScreenName);

            // set player position
            this.playerPosition = new Vector2(SettingsManager.GAME_WIDTH - SettingsManager.PLAYER_WIDTH, 250);
        }

        /// <summary>
        /// Moves the level state to a past transition
        /// </summary>
        /// <param name="transitionIndex"></param>
        public void TransitionPast( string transitionIndex )
        {
            // save current level
            lastLevel = currentLevel;

            GoTo(transitionIndex);

            StateManager.Instance.SetState("is_in_past", 100);

            SoundManager.PlaySound(SoundManager.SOUND_TIMETRAVEL);
        }

        public void TransitionPresent()
        {
            // reset back to present state
            StateManager.Instance.ResetReturnToPresent();

            // modify current level
            lastLevel = null;

            StateManager.Instance.AdvanceDay();

            SoundManager.PlaySound(SoundManager.SOUND_TIMETRAVEL);
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
                if (chats[i].Index == chatIndex && StateManager.Instance.CheckDependencies( chats[i].dependencies ) )
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
            // return final newspaper when game completed
            if (StateManager.Instance.GetState("progress") == 100)
                return "news\\news_final";

            int newsIndex = new Random().Next(1, 5);
            return "news\\news" + newsIndex.ToString();
        }
        #endregion

        #region Singleton
        private static readonly LevelManager instance = new LevelManager();

        private LevelManager() { }

        public static LevelManager Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion
    }
}
