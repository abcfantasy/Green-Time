using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTimeGameData.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    /// <summary>
    /// A popup message box screen, used to display game chats
    /// </summary>
    class ChatScreen : GameScreen
    {
        #region Fields
        private string currentText;
        private List<AnswerEntry> answerEntries = new List<AnswerEntry>();
        private int selectedEntry = 0;

        private Chat chat;
        Texture2D gradientTexture;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public ChatScreen(Chat chat, bool transition)
        {
            IsPopup = true;

            if (transition)
                TransitionOnTime = TimeSpan.FromSeconds(0.2);
            else
                TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            this.chat = chat;
            currentText = chat.Text;

            // add any answers if available
            if (chat.answers != null)
            {
                foreach (Answer a in chat.answers)
                    answerEntries.Add(new AnswerEntry(a.Text));
            }
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>("gradient");
        }
        #endregion

        #region Handle Input
        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputManager input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp())
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = answerEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown())
            {
                selectedEntry++;

                if (selectedEntry >= answerEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.

            if (input.IsMenuSelect())
            {
                OnSelectEntry(selectedEntry);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            // change states if any
            if( chat.affectedStates != null )
                StateManager.Instance.ModifyStates(chat.affectedStates);

            // if answers available, check chosen answer
            if (answerEntries.Count > 0)
            {
                ChatScreen s = new ChatScreen(LevelManager.Instance.GetChat(chat.answers[entryIndex].ResponseIndex), false);
                ScreenManager.AddScreen(s);
                ScreenManager.RemoveScreen(this);
            }

            // otherwise, just close this screen
            else
            {
                this.ExitScreen();
            }
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < answerEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                answerEntries[i].Update(isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(currentText);
            Vector2 textPosition = (viewportSize - textSize) / 2;
            textPosition.Y -= 256;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            // Draw the message box text.
            spriteBatch.DrawString(font, currentText, textPosition, color);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, textPosition.Y + textSize.Y);

            // update each menu entry's location in turn
            for (int i = 0; i < answerEntries.Count; i++)
            {
                AnswerEntry answerEntry = answerEntries[i];

                // move down for the next entry the size of this entry
                position.Y += ScreenManager.Font.LineSpacing;

                // each entry is to be centered horizontally
                position.X = 50; // -(viewportSize.X / 3);

                // set the entry's position
                answerEntry.Position = position;


                // draw
                bool isSelected = IsActive && (i == selectedEntry);
                answerEntry.Draw(this, isSelected, gameTime);
            }

            spriteBatch.End();
        }
        #endregion
    }
}
