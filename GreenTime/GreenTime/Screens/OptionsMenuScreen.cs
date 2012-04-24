using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class OptionsMenuScreen : MenuScreen
    {
        private MenuEntry musicMenuEntry;
        private MenuEntry soundMenuEntry;
        private MenuEntry fullscreenMenuEntry;
        private MenuEntry difficultyMenuEntry;
        private MenuEntry controlsMenuEntry;

        public OptionsMenuScreen()
            : base("Options", 180.0f)
        {
            // Create our menu entries.
            musicMenuEntry = new MenuEntry("Music: ");
            soundMenuEntry = new MenuEntry("Sound Effects: ");
            fullscreenMenuEntry = new MenuEntry("Fullscreen Mode: Off");
            difficultyMenuEntry = new MenuEntry("Difficulty: ");
            controlsMenuEntry = new MenuEntry("Controls");
            MenuEntry backMenuEntry = new MenuEntry("Back");
            UpdateMenuEntries();

            // Hook up menu event handlers.
            musicMenuEntry.Selected += MusicMenuEntrySelected;
            soundMenuEntry.Selected += SoundMenuEntrySelected;
            fullscreenMenuEntry.Selected += FullscreenMenuEntrySelected;
            difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
            controlsMenuEntry.Selected += ControlsMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(musicMenuEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(fullscreenMenuEntry);
            MenuEntries.Add(difficultyMenuEntry);
            MenuEntries.Add(controlsMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }

        #region Update
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateMenuEntries();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

        #region Handle Input
        void MusicMenuEntrySelected(object sender, EventArgs e)
        {
            SoundManager.ToggleMusic();
        }

        void SoundMenuEntrySelected(object sender, EventArgs e)
        {
            SoundManager.ToggleSound();
        }

        void FullscreenMenuEntrySelected(object sender, EventArgs e)
        {
            SettingsManager.ToggleFullscreen();
        }

        void DifficultyMenuEntrySelected(object sender, EventArgs e)
        {
            SettingsManager.ToggleDifficulty();
        }

        void ControlsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new ControlsMenuScreen());
        }

        protected override void OnCancel()
        {
            ExitScreen();
        }
        #endregion

        private void UpdateMenuEntries()
        {
            musicMenuEntry.Text = "Music: " + (SettingsManager.MusicEnabled ? "On" : "Off");
            soundMenuEntry.Text = "Sound Effects: " + (SettingsManager.SoundEnabled ? "On" : "Off");
            fullscreenMenuEntry.Text = "Fullscreen Mode: " + (SettingsManager.FullScreenMode ? "On" : "Off");
            difficultyMenuEntry.Text = "Difficulty: " + (SettingsManager.Difficulty.ToString());
        }
    }
}
