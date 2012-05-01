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
        private string _title;
        private string _text;
        private int elapsed = 0;

        private float titleAlpha = 0.0f;
        private float textAlpha = 0.0f;
        private bool fadeIn = true;

        private bool playMusic;

        public TextOnBlackScreen(string title, string text, GameScreen[] nextScreens, bool playAlternateMusic = false)
        {
            _title = title;
            _text = text;
            _nextScreens = nextScreens;

            playMusic = playAlternateMusic;

            TransitionOnTime = TimeSpan.FromSeconds(3);
            TransitionOffTime = TimeSpan.FromSeconds(3);
        }

        public override void LoadContent()
        {
            if ( playMusic )
                SoundManager.PlayMusic(true, false, 1.0f);
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsMenuCancel() || input.IsMenuSelect() )
            {
                this.TransitionOffTime = TimeSpan.FromSeconds(0.0);
                elapsed = 3000;
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            elapsed += gameTime.ElapsedGameTime.Milliseconds;

            if (fadeIn)
            {
                titleAlpha = MathHelper.Clamp(TransitionAlpha * 2.0f, 0.0f, 1.0f);
                textAlpha = MathHelper.Clamp((TransitionAlpha - 0.5f) * 2.0f, 0.0f, 1.0f);
            }
            else
            {
                textAlpha = MathHelper.Clamp(TransitionAlpha * 2.0f, 0.0f, 1.0f);
                titleAlpha = MathHelper.Clamp((TransitionAlpha - 0.5f) * 2.0f, 0.0f, 1.0f);
            }

            if (fadeIn && elapsed > 3000)
                fadeIn = false;

            if (elapsed > 3000)
            {
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

            Vector2 titleSize = ScreenManager.Font.MeasureString(_title);
            Vector2 textSize = ScreenManager.Font.MeasureString( _text );

            spriteBatch.DrawString(ScreenManager.Font, _title, new Vector2((SettingsManager.GAME_WIDTH / 2), (SettingsManager.GAME_HEIGHT / 2) - (titleSize.Y * 2)), Color.White * titleAlpha, 0.0f, new Vector2( titleSize.X / 2, titleSize.Y / 2 ), 1.3f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(ScreenManager.Font, _text, new Vector2((SettingsManager.GAME_WIDTH / 2) - (textSize.X / 2), (SettingsManager.GAME_HEIGHT / 2) - (textSize.Y / 2)), Color.White * textAlpha );

            spriteBatch.End();
        }
    }
}
