using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class NewGameDifficultyMenuScreen : MenuScreen
    {
        public NewGameDifficultyMenuScreen()
            : base("What about difficulty?", 180.0f, false)
        {
            // Create our menu entries.
            MenuEntry normalEntry = new MenuEntry("Normal", "Explore the levels and find out what you can interact with.");
            MenuEntry easyEntry = new MenuEntry("Easy", "See which objects you can interact with.");

            // Hook up menu event handlers.
            normalEntry.Selected += NormalMenuEntrySelected;
            easyEntry.Selected += EasyMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(normalEntry);
            MenuEntries.Add(easyEntry);
        }

        void NormalMenuEntrySelected(object sender, EventArgs e)
        {
            SettingsManager.Difficulty = SettingsManager.Game_Difficulties.NORMAL;
            StartGame();
        }

        void EasyMenuEntrySelected(object sender, EventArgs e)
        {
            SettingsManager.Difficulty = SettingsManager.Game_Difficulties.EASY;
            StartGame();
        }

        protected override void OnCancel()
        {
            ScreenManager.AddScreen(new NewGameTutorialsMenuScreen());
            this.ExitScreen();
        }

        void StartGame()
        {
            StateManager.Instance.SetState(StateManager.STATE_LOAD, 0);
            StateManager.Instance.NewGame();
            LevelManager.Instance.NewGame();
            //LoadingScreen.Load(ScreenManager, true, new PlayScreen());
            LoadingScreen.Load(ScreenManager, true, new TextOnBlackScreen("AVANTgarde", "presents", new GameScreen[] { new TextOnBlackScreen("GreenTime", "", new GameScreen[] { new IntroScreen() }) }, true));   // uncomment this line and comment the line above to start with storyline
        }
    }
}
