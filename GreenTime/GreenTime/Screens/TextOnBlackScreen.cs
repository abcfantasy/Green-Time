using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    public class TextOnBlackScreen : GameScreen
    {
        private GameScreen[] _nextScreens;
        private string _text;
        private int elapsed = 0;

        public TextOnBlackScreen(string text, GameScreen[] nextScreens)
        {
            _text = text;
            _nextScreens = nextScreens;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            elapsed += gameTime.ElapsedGameTime.Milliseconds;

            if (elapsed > 3000)
            {
                //LoadingScreen.Load(ScreenManager, true, _nextScreens);
                //this.ExitScreen();
                //this.ExitScreen();
                //ScreenManager.AddScreen(_nextScreens[0]);
                //ScreenManager.AddScreen(_nextScreens[1]);
                LoadingScreen.Load(ScreenManager, false, _nextScreens);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

           

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            //spriteBatch.Draw(, new Vector2(0, 0), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.8f);
            Vector2 textSize = ScreenManager.Font.MeasureString( _text );

            spriteBatch.DrawString(ScreenManager.Font, _text, new Vector2((SettingsManager.GAME_WIDTH / 2) - (textSize.X / 2), (SettingsManager.GAME_HEIGHT / 2) - (textSize.Y / 2)), Color.White * this.TransitionAlpha );
            spriteBatch.End();
        }
    }
}
