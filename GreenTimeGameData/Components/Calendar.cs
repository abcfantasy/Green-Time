using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTimeGameData.Components
{
    public class Calendar : Sprite
    {
        private string day;
        private string monthYear;
        private SpriteFont font;

        public void Initialize( DateTime dateToShow, SpriteFont font )
        {
            day = dateToShow.ToString("dd");
            monthYear = dateToShow.ToString("MMM yy");
            this.font = font;
        }

        public override void Draw(Texture2D texture, SpriteBatch spriteBatch, Rectangle textureRect, Color tint)
        {
            Vector2 daySize = font.MeasureString(day);
            Vector2 monthYearSize = font.MeasureString(monthYear);

            // draw the calendar
            spriteBatch.Draw(texture, position, textureRect, tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layer);

            // draw the day
            spriteBatch.DrawString(font, day, new Vector2(position.X + ( textureRect.Width / 2 ) - (daySize.X / 2) + 2, position.Y + 40), Color.Black, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, layer - 0.01f);

            // draw the month and year
            spriteBatch.DrawString(font, monthYear, new Vector2(position.X + (textureRect.Width / 2 ) - (monthYearSize.X / 2) + 17, position.Y + daySize.Y + 28.0f), Color.Black, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, layer - 0.01f);
        }
    }
}
