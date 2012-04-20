using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GreenTime.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTime.Screens
{
    class SearchEntry
    {
        #region Fields
        const int fixedBoxWidth = 150;

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

        Texture2D borderPixel;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value.PadRight(10); }
        }


        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public SearchEntry(string text)
        {
            this.Text = text;

            borderPixel = new Texture2D(SettingsManager.GraphicsDevice.GraphicsDevice, 1, 1);
            borderPixel.SetData<Color>(new Color[] { Color.White });
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(GameScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(GameScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.DarkGreen : Color.DarkGray;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;
            
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);

            if (isSelected)
            {
                int width = GetWidth(screen);
                int height = GetHeight(screen);
                float heightOffset = (font.LineSpacing / 2.0f);
                //// top border
                //spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X - 4), (int)(position.Y - heightOffset - 2), width + 8, 2), Color.Green);
                //// left border
                //spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X  - 4), (int)(position.Y - heightOffset - 2), 2, height + 4), Color.Green);
                //// right border
                //spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X + width + 2), (int)(position.Y - heightOffset - 2), 2, height + 4), Color.DarkGreen);
                //// bottom border
                //spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X - 4), (int)(position.Y + heightOffset + 2), width + 8, 2), Color.DarkGreen);
                // top border
                spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X - 4), (int)(position.Y - heightOffset - 2), fixedBoxWidth + 8, 2), Color.Green);
                // left border
                spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X - 4), (int)(position.Y - heightOffset - 2), 2, height + 4), Color.Green);
                // right border
                spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X + fixedBoxWidth + 2), (int)(position.Y - heightOffset - 2), 2, height + 4), Color.DarkGreen);
                // bottom border
                spriteBatch.Draw(borderPixel, new Rectangle((int)(position.X - 4), (int)(position.Y + heightOffset + 2), fixedBoxWidth + 8, 2), Color.DarkGreen);
            }
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(GameScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(GameScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }
        #endregion
    }
}
