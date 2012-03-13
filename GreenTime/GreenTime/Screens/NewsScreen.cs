using System;
using GreenTime.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTime.Screens
{
    public class NewsScreen : GameScreen
    {
        #region Constants
        private const int NEWS_HEIGHT = 680;
        private const int NEWS_WIDTH  = 479;
        #endregion

        #region Fields
        ContentManager content;
        Texture2D newsTexture;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public NewsScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
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

            newsTexture = content.Load<Texture2D>(LevelManager.State.GetNewsTexture());
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

        #region Update and Draw
        /// <summary>
        /// Updates the news screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the news screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin();

            // rotate the newspaper
            spriteBatch.Draw(newsTexture,
                new Vector2((SettingsManager.GAME_WIDTH / 2) /*- (NEWS_WIDTH / 2)*/, (SettingsManager.GAME_HEIGHT / 2) /*- (NEWS_HEIGHT / 2)*/),
                new Rectangle(0, 0, newsTexture.Width, newsTexture.Height),
                Color.White,
                MathHelper.ToRadians(TransitionAlpha * 1800),
                new Vector2( newsTexture.Width / 2, newsTexture.Height / 2 ),
                TransitionAlpha, SpriteEffects.None, 0);
            /*
            spriteBatch.Draw(newsTexture, 
                             new Vector2( ( SettingsManager.GAME_WIDTH / 2 ) - ( NEWS_WIDTH / 2 ), ( SettingsManager.GAME_HEIGHT / 2 ) - ( NEWS_HEIGHT / 2 ) ),
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            */
            spriteBatch.End();
        }
        #endregion

        #region Input
        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputManager input)
        {
            if (this.IsActive && ( input.IsMenuSelect() || input.IsMenuCancel()) )
            {
                ExitScreen();
            }
        }
        #endregion
    }
}
