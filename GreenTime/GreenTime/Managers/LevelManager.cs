using System;
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
        private Sprite pickedObject;    // currently picked up object - save it here to know when changing scenes
        private string pickedObjectState;
        private Level currentLevel = null;
        private Level lastLevel = null;     // the level the player was in before going to the past

        private Dictionary<String, Level> levels = new Dictionary<string,Level>();
        private Dictionary<string, Dictionary<int, Chat>> chats = new Dictionary<string,Dictionary<int, Chat>>();

        private Player player;
        private float startPosition = 0.0f;
        #endregion

        #region Properties
        public Player Player
        {
            get { return player; }
        }

        public Sprite PickedObject
        {
            get { return pickedObject; }
            set {
                pickedObject = value;
                if( pickedObject != null ) pickedObject.layer = 0.4f;
            }
        }

        public string PickedObjectState { 
            get { return pickedObjectState; }
            set { pickedObjectState = value; }
        }

        public Level CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        public Level LastPresentLevel
        {
            get { return lastLevel; }
            set { lastLevel = value; }
        }

        public float StartPosition
        {
            get { return startPosition; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parses levels and chats XML file and stores information in memory
        /// </summary>
        public void LoadAllLevels( ContentManager content )
        {
            this.content = content;
            player = new Player(content);
            StateManager.Instance.AdvanceDay();            
        }

        public void GoTo( string level )
        {
            if (!levels.ContainsKey(level))
            {
                levels[level] = content.Load<Level>(@"levels\" + level);
                foreach (GameObject o in levels[level].gameObjects)
                    o.Load();
            }

            currentLevel = levels[level];
        }

        public Dictionary<int,Chat> StartChat(string chat)
        {
            if (!chats.ContainsKey(chat))
                chats[chat] = content.Load<Dictionary<int, Chat>>(@"chats\" + chat);

            return chats[chat];
        }

        public void GoHome()
        {
            GoTo( HOME );
            startPosition = 250.0f;
        }

        public Boolean CanMoveRight()
        {
            return !currentLevel.rightScreenName.Equals("");
        }

        public Boolean CanMoveLeft()
        {
            return !currentLevel.leftScreenName.Equals("");
        }

        /// <summary>
        /// Moves the level state to one screen to the right
        /// </summary>
        public void MoveRight()
        {
            // reset loaded state (only happens on the starting screen)
            StateManager.Instance.SetState(StateManager.STATE_LOAD, 0);
            GoTo(currentLevel.rightScreenName);
            startPosition = 0.0f;
        }

        /// <summary>
        /// Moves the level state to one screen to the left
        /// </summary>
        public void MoveLeft()
        {
            // reset loaded state (only happens on the starting screen)
            StateManager.Instance.SetState(StateManager.STATE_LOAD, 0);
            GoTo(currentLevel.leftScreenName);
            startPosition = SettingsManager.GAME_WIDTH - SettingsManager.PLAYER_WIDTH;           
        }

        /// <summary>
        /// Moves the level state to a past transition
        /// </summary>
        /// <param name="transitionIndex"></param>
        public void MovePast( string transitionIndex )
        {
            // save current level
            lastLevel = currentLevel;

            GoTo(transitionIndex);

            SoundManager.PlaySound(SoundManager.SOUND_TIMETRAVEL);
            startPosition = player.Position.X;
        }

        public void MovePresent()
        {
            // reset back to present state
            StateManager.Instance.ResetReturnToPresent();

            // modify current level
            lastLevel = null;

            StateManager.Instance.AdvanceDay();

            SoundManager.PlaySound(SoundManager.SOUND_TIMETRAVEL);
            startPosition = player.Position.X;
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
