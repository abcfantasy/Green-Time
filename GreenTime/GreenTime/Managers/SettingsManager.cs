using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTimeGameData.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;

namespace GreenTime.Managers
{
    [Serializable]
    public class SaveData
    {
        // from state manager
        //public Dictionary<string, int> States;
        public string[] StateKeys;
        public int[] StateValues;

        // from level manager
        public Vector2 PlayerPosition;
        public Sprite PickedObject;
        public Level CurrentLevel;
        public Level LastLevel;

        // from settings manager
        public bool MusicEnabled;
        public bool SoundEnabled;
        public bool FullscreenMode;
        public int Difficulty;
    }

    public class SettingsManager
    {
        #region Constants
        public static readonly int GAME_WIDTH = 1280;
        public static readonly int GAME_HEIGHT = 720;

        public static readonly int PLAYER_WIDTH = 64;

        public enum Game_Difficulties
        {
            EASY,
            NORMAL
        }
        #endregion

        private static bool musicEnabled = true;
        private static bool soundEnabled = true;

        private static bool fullscreen = false;

        private static Game_Difficulties gameDifficulty = Game_Difficulties.NORMAL;

        #region Properties
        public static GraphicsDeviceManager GraphicsDevice { get; set; }

        public static bool MusicEnabled
        {
            get
            {
                return musicEnabled;
            }
            set
            {
                musicEnabled = value;
            }
        }

        public static bool SoundEnabled
        {
            get
            {
                return soundEnabled;
            }
            set
            {
                soundEnabled = value;
            }
        }

        public static bool FullScreenMode
        {
            get
            {
                return fullscreen;
            }
            set
            {
                // if setting is different
                if (fullscreen != value)
                {
                    ToggleFullscreen();
                }
            }
        }

        public static Game_Difficulties Difficulty
        {
            get
            {
                return gameDifficulty;
            }
            set
            {
                gameDifficulty = value;
            }
        }
        #endregion

        private static void Save( IAsyncResult result )
        {
            // create the data
            SaveData data = new SaveData();
            Dictionary<string, int> States = StateManager.Current.AllStates;
            data.StateKeys = States.Keys.ToArray();
            data.StateValues = States.Keys.Select(key => States[key]).ToArray();
            data.CurrentLevel = LevelManager.State.CurrentLevel;
            data.LastLevel = LevelManager.State.LastPresentLevel;
            data.PickedObject = LevelManager.State.PickedObject;
            data.PlayerPosition = LevelManager.State.PlayerPosition;
            data.MusicEnabled = MusicEnabled;
            data.SoundEnabled = SoundEnabled;
            data.FullscreenMode = FullScreenMode;
            data.Difficulty = (int)Difficulty;

            StorageDevice device = (StorageDevice)result.AsyncState;
            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "greentime.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            // Create the file.
            Stream stream = container.CreateFile(filename);

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            serializer.Serialize(stream, data);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        /// <summary>
        /// This method serializes a data object
        /// into the StorageContainer for this game
        /// </summary>
        /// <param name="device"></param>
        public static void SaveGame(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("GreenTimeStorage", Save, device);
        }

        /// <summary>
        /// This method loads a serialized data object
        /// from the StorageContainer for this game.
        /// </summary>
        /// <param name="device"></param>
        public static bool LoadGame( StorageDevice device )
        {
            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("GreenTimeStorage", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "greentime.sav";

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                return false;
            }

            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);

            // Read the data from the file.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            SaveData data = (SaveData)serializer.Deserialize(stream);

            // Close the file.
            stream.Close();

            // Dispose the container.
            container.Dispose();

            // load data
            Dictionary<string, int> states = new Dictionary<string, int>();
            for (int i = 0; i < data.StateKeys.Length; i++)
                states.Add(data.StateKeys[i], data.StateValues[i]);

            StateManager.Current.AllStates = states;
            LevelManager.State.PlayerPosition = data.PlayerPosition;
            LevelManager.State.PickedObject = data.PickedObject;
            LevelManager.State.CurrentLevel = data.CurrentLevel;
            LevelManager.State.LastPresentLevel = data.LastLevel;

            MusicEnabled = data.MusicEnabled;
            SoundEnabled = data.SoundEnabled;
            FullScreenMode = data.FullscreenMode;
            Difficulty = (Game_Difficulties)data.Difficulty;

            // mark as loaded game
            StateManager.Current.SetState(StateManager.STATE_LOAD, 100);

            return true;
        }

        /// <summary>
        /// Toggle full screen mode
        /// </summary>
        public static void ToggleFullscreen()
        {
            GraphicsDevice.ToggleFullScreen();
            fullscreen = GraphicsDevice.IsFullScreen;
        }

        public static void ToggleDifficulty()
        {
            if (SettingsManager.Difficulty == Game_Difficulties.EASY)
                SettingsManager.Difficulty = Game_Difficulties.NORMAL;
            else
                SettingsManager.Difficulty = Game_Difficulties.EASY;
            //SettingsManager.Difficulty = ( SettingsManager.Difficulty == Game_Difficulties.EASY : Game_Difficulties.NORMAL ? Game_Difficulties.EASY );
        }
    }
}
