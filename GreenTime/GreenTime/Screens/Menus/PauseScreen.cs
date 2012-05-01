using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseScreen : MenuScreen
    {
        bool GameSaveRequested = false;
        bool GameLoadRequested = false;
        IAsyncResult result;

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseScreen()
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game", "Continue playing GreenTime.");
            //MenuEntry saveGameMenuEntry = new MenuEntry("Save Game");
            //MenuEntry loadGameMenuEntry = new MenuEntry("Load Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options", "Take a look at options such as audio and controls.");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game", "End your session and go back to the main menu (progress is NOT saved)");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            //saveGameMenuEntry.Selected += SaveGameMenuEntrySelected;
            //loadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            //MenuEntries.Add(saveGameMenuEntry);
            //MenuEntries.Add(loadGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Update
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            /*
            // If a save is pending, save as soon as the
            // storage device is chosen
            if ((GameSaveRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    SettingsManager.SaveGame(device);
                    ExitScreen();
                }
                // Reset the request flag
                GameSaveRequested = false;
            }
             
            // If a load is pending, load as soon as the
            // storage device is chosen
            else if ((GameLoadRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    if (SettingsManager.LoadGame(device))
                    {
                        LoadingScreen.Load(ScreenManager, false, new PlayScreen());
                        ExitScreen();
                    }
                }
                // Reset the request flag
                GameLoadRequested = false;
            }
             * */
        }
        #endregion

        #region Handle Input

        void SaveGameMenuEntrySelected(object sender, EventArgs e)
        {
            // Set the request flag
            /*
            if ((!Guide.IsVisible) && (GameSaveRequested == false))
            {
                GameSaveRequested = true;
                result = StorageDevice.BeginShowSelector(
                        PlayerIndex.One, null, null);
            }*/
        }

        void LoadGameMenuEntrySelected(object sender, EventArgs e)
        {
            /*
            // Set the request flag
            if ((!Guide.IsVisible) && (GameLoadRequested == false))
            {
                GameLoadRequested = true;
                result = StorageDevice.BeginShowSelector(
                        PlayerIndex.One, null, null);
            }*/
        }

        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new LogoScreen(true),
                                                           new MainMenuScreen());
        }


        #endregion
    }
}
