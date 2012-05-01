using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class MainMenuScreen : MenuScreen
    {
        #region Initialization
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu", 180.0f)
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Start a New Game", "Press ENTER to start a new game of GreenTime.");
            //MenuEntry loadGameMenuEntry = new MenuEntry("Load Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options", "Take a look at options such as audio and controls.");
            MenuEntry exitMenuEntry = new MenuEntry("Exit", "Quit the game.");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            //loadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            //MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        public override void LoadContent()
        {
            ResourceManager.Instance.LoadMenuTexture();
            SoundManager.PlayMusic();
        }
        #endregion

        #region Update
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            /*
            if ((GameLoadRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    if (SettingsManager.LoadGame(device))
                    {
                        //System.Threading.Thread.Sleep(1);
                        LoadingScreen.Load(ScreenManager, false, new PlayScreen());
                    }
                }
                // Reset the request flag
                GameLoadRequested = false;
            }*/
        }
        #endregion

        #region Handle Input
        void LoadGameMenuEntrySelected(object sender, EventArgs e)
        {
            /*
            // Set the request flag
            if ((!Guide.IsVisible) && (GameLoadRequested == false))
            {
                GameLoadRequested = true;
                result = StorageDevice.BeginShowSelector(
                        Microsoft.Xna.Framework.PlayerIndex.One, null, null);
            }*/
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new NewGameTutorialsMenuScreen());
            this.ExitScreen();
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit this game?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        #endregion
    }
}
