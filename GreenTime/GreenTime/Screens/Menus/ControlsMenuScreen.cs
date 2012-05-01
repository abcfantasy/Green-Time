using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class ControlsMenuScreen : MenuScreen
    {
        public ControlsMenuScreen()
            : base("Controls", 180.0f)
        {
            // Create our menu entries.
            MenuEntry me1 = new MenuEntry("Move : left/right arrow keys");
            MenuEntry me2 = new MenuEntry("Interact : space bar");
            MenuEntry me3 = new MenuEntry("Time Travel : Z");
            MenuEntry me4 = new MenuEntry("End day : D");
            MenuEntry me5 = new MenuEntry("Pause/Menu: Esc");

            MenuEntry exitMenuEntry = new MenuEntry("Back", "Back to the options menu.");

            // Add entries to the menu.
            MenuEntries.Add(me1);
            MenuEntries.Add(me2);
            MenuEntries.Add(me3);
            MenuEntries.Add(me4);
            MenuEntries.Add(me5);
            MenuEntries.Add(exitMenuEntry);

            selectedEntry = 5;
        }

        public override void HandleInput(Managers.InputManager input)
        {
            if (input.IsMenuSelect() || input.IsMenuCancel())
            {
                SoundManager.PlaySound(SoundManager.SOUND_MENU_CANCEL);
                OnCancel();
            }
        }

        protected override void OnCancel()
        {
            ExitScreen();
        }
    }
}
