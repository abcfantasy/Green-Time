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
        Texture2D chatBubble;
        Texture2D chatArrow;
        SpriteFont chatFont;

        private Vector2 playerPosition;
        private Vector2 npcPosition;

        private bool npcSpeaking;
        private double optionArrowsBlinking = 0.0;
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

            InitializeChat(chat);
        }

        public ChatScreen(Chat chat, bool transition, Vector2 playerPosition, Vector2 npcPosition)
            : this(chat, transition)
        {
            this.playerPosition = playerPosition;
            this.npcPosition = npcPosition;
        }

        private void InitializeChat(Chat chat)
        {
            this.chat = chat;
            currentText = chat.Text;

            answerEntries.Clear();

            npcSpeaking = true;

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
            chatBubble = content.Load<Texture2D>("chatBubble");
            chatArrow = content.Load<Texture2D>("chat_arrow");
            chatFont = content.Load<SpriteFont>("chatfont");
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
            // show player options
            if (npcSpeaking && answerEntries != null )
            {
                npcSpeaking = false;
                return;
            }

            // change states if any
            if( chat.affectedStates != null )
                StateManager.Instance.ModifyStates(chat.affectedStates);

            // if answers available, check chosen answer
            if (answerEntries.Count > 0)
            {
                InitializeChat(LevelManager.Instance.GetChat(chat.answers[entryIndex].ResponseIndex));
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
            //for (int i = 0; i < answerEntries.Count; i++)
            //{
            //    bool isSelected = IsActive && (i == selectedEntry);

            //    answerEntries[i].Update(isSelected, gameTime);
            //}
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = chatFont.MeasureString(currentText);
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

            if (npcSpeaking)
            {
                // draw npc bubble
                DrawText(spriteBatch, chatFont, npcPosition - new Vector2(0.0f, 200.0f), 300.0f, currentText);
            }
            else
            {
                // draw player bubble
                if (answerEntries != null && answerEntries.Count > 0 )
                {
                    // if there is 1 answer, draw simple text without arrows blinking
                    if (answerEntries.Count == 1)
                        DrawText(spriteBatch, chatFont, playerPosition, 300.0f, answerEntries[selectedEntry].Text);
                    else
                        DrawAnswer(gameTime, spriteBatch, chatFont, playerPosition, 300.0f, answerEntries[selectedEntry].Text);
                }
            }

            spriteBatch.End();
        }

        public void DrawText(SpriteBatch spriteBatch, SpriteFont font, Vector2 topLeftCorner, float width, string text)
        {
            List<string> lines = WrapText(font, text, width);
            Vector2 currentPosition = topLeftCorner;

            spriteBatch.Draw(chatBubble, topLeftCorner, Color.White);

            foreach (string line in lines)
            {
                spriteBatch.DrawString(font, line, currentPosition + new Vector2( 30.0f, 35.0f ), Color.Black, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.5f);
                currentPosition.Y += ( font.LineSpacing / 2 );
            }
        }

        public void DrawAnswer(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, Vector2 topLeftCorner, float width, string text)
        {
            DrawText(spriteBatch, font, topLeftCorner, width, text);

            // update blinking counter
            optionArrowsBlinking += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (optionArrowsBlinking > 1000) optionArrowsBlinking = 0;

            // draw blinking arrows
            if (optionArrowsBlinking > 500)
            {
                spriteBatch.Draw(chatArrow, new Vector2( topLeftCorner.X + width - 55.0f, topLeftCorner.Y + chatBubble.Height - 70.0f), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f );
                spriteBatch.Draw(chatArrow, new Vector2( topLeftCorner.X + width - 35.0f, topLeftCorner.Y + 40.0f), null, Color.White, (float)Math.PI, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
            }
        }

        public List<string> WrapText(SpriteFont font, string text, float width)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            string line = "";

            foreach (string word in words)
            {
                if (word == words[0])
                {
                    line = word;
                    continue;
                }

                if (font.MeasureString(line + " " + word).X > width)
                {
                    lines.Add(line);
                    line = word;
                }
                else
                {
                    line += " " + word;
                }
            }

            if (!string.IsNullOrEmpty(line))
            {
                lines.Add(line);
            }

            return lines;
        }
        #endregion
    }
}
