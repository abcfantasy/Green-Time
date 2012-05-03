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
        private const int SCREEN_WIDTH = 818 - 25;
        private const int SCREEN_HEIGHT = 685 - 185;
        #endregion

        #region Fields
        ContentManager content;
        Texture2D screenBackground;
        Texture2D screenOutline;
        Texture2D screenPicture;
        Rectangle screenRect = new Rectangle( 0, 0, 818, 685 );

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
            centerScreenY = (int)Math.Round((SettingsManager.GAME_HEIGHT / 2.0f)) + 20;
            finalScreenX = (int)Math.Round((SettingsManager.GAME_WIDTH / 2.0f) - (SCREEN_WIDTH / 2.0f));
            finalScreenY = (int)Math.Round((SettingsManager.GAME_HEIGHT / 2.0f) - (SCREEN_HEIGHT / 2.0f)) - 5;

            localCenterScreenX = (int)Math.Round(SCREEN_WIDTH / 2.0f);
            localCenterScreenY = (int)Math.Round(SCREEN_HEIGHT / 2.0f) - 15;
            
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
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            if (content != null)
                content.Unload();
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

            spriteBatch.Draw(screenOutline, new Vector2(centerScreenX, centerScreenY), screenRect, Color.White, 0.0f, new Vector2(screenRect.Width / 2.0f, screenRect.Height / 2.0f), 1.0f, SpriteEffects.None, 0);

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
                // draw title
                spriteBatch.Draw(screenPicture, new Vector2(finalScreenX + 52.0f, finalScreenY), new Rectangle(0, 0, 691, 482), Color.White);
            }

            spriteBatch.End();
        }
        #endregion

    }
}
