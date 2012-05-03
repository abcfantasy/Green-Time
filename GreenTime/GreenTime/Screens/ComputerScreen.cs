using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GreenTime.Managers;
using Microsoft.Xna.Framework;

namespace GreenTime.Screens
{
    public class ComputerScreen : GameScreen
    {
        #region Constants
        private const int SCREEN_WIDTH = 836;
        private const int SCREEN_HEIGHT = 685 - 135;
        #endregion

        #region Fields
        ContentManager content;
        Texture2D screenBackground;
        Texture2D screenOutline;
        Texture2D screenPicture;
        Rectangle screenRect = new Rectangle( 0, 0, 836, 685 );

        // search options
        List<SearchEntry> searchEntries = new List<SearchEntry>();
        int selectedEntry = 0;
        string searchTitle;

        int finalScreenX;
        int finalScreenY;
        int centerScreenX;
        int centerScreenY;

        int localCenterScreenX;
        int localCenterScreenY;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public ComputerScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.4);
            TransitionOffTime = TimeSpan.FromSeconds(0.3);

            centerScreenX = (int)Math.Round((SettingsManager.GAME_WIDTH / 2.0f));
            centerScreenY = (int)Math.Round((SettingsManager.GAME_HEIGHT / 2.0f));
            finalScreenX = (int)Math.Round((SettingsManager.GAME_WIDTH / 2.0f) - (SCREEN_WIDTH / 2.0f));
            finalScreenY = (int)Math.Round((SettingsManager.GAME_HEIGHT / 2.0f) - (SCREEN_HEIGHT / 2.0f));

            localCenterScreenX = (int)Math.Round(SCREEN_WIDTH / 2.0f);
            localCenterScreenY = (int)Math.Round(SCREEN_HEIGHT / 2.0f);
            
        }

        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            screenOutline = content.Load<Texture2D>("computer\\screen");
            screenPicture = content.Load<Texture2D>(StateManager.Instance.NewsTextureName);

            screenBackground = new Texture2D(SettingsManager.GraphicsDevice.GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
            Color[] bgColor = new Color[SCREEN_WIDTH * SCREEN_HEIGHT];
            for (int i = 0; i < SCREEN_WIDTH * SCREEN_HEIGHT; i++)
                bgColor[i] = Color.White;
            screenBackground.SetData(bgColor);

            //AddSearchEntries();
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            if (content != null)
                content.Unload();
        }

        /// <summary>
        /// Add the available search entries
        /// </summary>
        private void AddSearchEntries()
        {
            //SearchEntry entryNews = new SearchEntry("News");
            //searchEntries.Add(entryNews);
        }
        #endregion

        #region Handle Input
        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputManager input)
        {
            if (input.IsMenuSelect() || input.IsMenuCancel())
                OnCancel();
            /*
            // Move to the previous menu entry?
            if (input.IsMenuUp())
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = searchEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown())
            {
                selectedEntry++;

                if (selectedEntry >= searchEntries.Count)
                    selectedEntry = 0;
            }

            // update title
            searchTitle = searchEntries[selectedEntry].Text;

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.

            if (input.IsMenuSelect())
            {
                OnSelectEntry(selectedEntry);
            }
            else if (input.IsMenuCancel())
            {
                OnCancel();
            }
             */
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            //searchEntries[entryIndex].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            SoundManager.PlaySound(SoundManager.SOUND_COMPUTEROFF);
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion

        #region Update and Draw
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend );

            spriteBatch.Draw(screenOutline, new Vector2(centerScreenX, centerScreenY), screenRect, Color.White, 0.0f, new Vector2( screenOutline.Width / 2.0f, screenOutline.Height / 2.0f ), 1.0f, SpriteEffects.None, 0 );

            // this beautiful piece of code handles the effect of turning on/off the screen
            // this beautiful piece of code was written in one go and executed perfectly! I should drink to this
            if (TransitionAlpha < 1.0)
                spriteBatch.Draw(screenBackground, new Vector2(finalScreenX, finalScreenY), null, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            if ( TransitionAlpha <= 0.5 )
            {
                int x = (int)(centerScreenX - ( ( centerScreenX - finalScreenX ) * (TransitionAlpha * 2)));
                int localX = (int)(localCenterScreenX - ( localCenterScreenX * TransitionAlpha * 2 ) );
                spriteBatch.Draw(
                    screenBackground,
                    new Vector2(x, centerScreenY - 2),
                    new Rectangle(localX, (int)(SCREEN_HEIGHT / 2.0f) - 2, (localCenterScreenX - localX) * 2, 4), Color.Gainsboro, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.9f);
            }
            else
            {
                int y = (int)(centerScreenY - ( ( centerScreenY - finalScreenY ) * ((TransitionAlpha - 0.5) * 2)));
                int localY = (int)(localCenterScreenY - ( localCenterScreenY * (TransitionAlpha - 0.5) * 2 ) );
                spriteBatch.Draw(
                    screenBackground,
                    new Vector2(finalScreenX, y),
                    new Rectangle(0, localY, SCREEN_WIDTH, (localCenterScreenY - localY) * 2),
                    Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.9f);
            }

            // draw the normal stuff
            if (TransitionAlpha == 1.0)
            {
                // make sure our entries are in the right place before we draw them
                UpdateMenuEntryLocations();

                // Draw each menu entry in turn.
                /*
                for (int i = 0; i < searchEntries.Count; i++)
                {
                    SearchEntry searchEntry = searchEntries[i];

                    bool isSelected = IsActive && (i == selectedEntry);

                    searchEntry.Draw(this, isSelected, gameTime);
                }

                // draw middle border
                spriteBatch.Draw(screenBackground, new Vector2(finalScreenX + 250, finalScreenY), new Rectangle(0, 0, 2, screenBackground.Height), Color.Gray);
                */
                // draw title
                spriteBatch.Draw(screenPicture, new Vector2(finalScreenX + 252.0f, finalScreenY + 48.0f), new Rectangle(0, 0, 691, 482), Color.White);
                //spriteBatch.DrawString(this.ScreenManager.Font, searchTitle, new Vector2(finalScreenX + 270.0f, finalScreenY + 50.0f), Color.DarkGreen, 0.0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                 
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        private void UpdateMenuEntryLocations()
        {
            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(finalScreenX + 70.0f, finalScreenY + 75.0f);

            // update each menu entry's location in turn
            for (int i = 0; i < searchEntries.Count; i++)
            {
                SearchEntry searchEntry = searchEntries[i];

                // set the entry's position
                searchEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += searchEntry.GetHeight(this);
            }
        }
        #endregion

    }
}
