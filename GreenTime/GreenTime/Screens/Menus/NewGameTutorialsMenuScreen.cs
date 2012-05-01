using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class NewGameTutorialsMenuScreen : MenuScreen
    {
        public NewGameTutorialsMenuScreen()
            : base("Do you want to enable in-game tutorial popups?", 180.0f, false)
        {
            // Create our menu entries.
            MenuEntry yesEntry  = new MenuEntry("Yes", "Popup boxes will pause the game to give you some tips and tutorials.");
            MenuEntry noEntry = new MenuEntry("No", "You will not be bothered by tutorial boxes.");

            // Hook up menu event handlers.
            yesEntry.Selected += YesMenuEntrySelected;
            noEntry.Selected += NoMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(yesEntry);
            MenuEntries.Add(noEntry);
        }

        void YesMenuEntrySelected(object sender, EventArgs e)
        {
            SettingsManager.TutorialsEnabled = true;
            ScreenManager.AddScreen(new NewGameDifficultyMenuScreen());
            ExitScreen();
        }

        void NoMenuEntrySelected(object sender, EventArgs e)
        {
            SettingsManager.TutorialsEnabled = false;
            ScreenManager.AddScreen(new NewGameDifficultyMenuScreen());
            ExitScreen();
            
        }

        protected override void OnCancel()
        {
            ScreenManager.AddScreen(new MainMenuScreen());
            this.ExitScreen();
        }
    }
}
