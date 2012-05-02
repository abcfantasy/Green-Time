using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GreenTime.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GreenTime.Screens
{
    public class MapScreen : GameScreen
    {
        #region Fields
        ContentManager content;
        Texture2D screenBackground;
        Vector2 screenDimensions;
        Vector2 screenPosition;

        List<string> places = new List<string>();
        int selected = 0;
        float desaturationAmount = StateManager.Instance.GetState("progress") * 0.64f;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public MapScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.4);
            TransitionOffTime = TimeSpan.FromSeconds(0.3);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            List<string> tempPlaces = new List<string>();

            tempPlaces.Add("neighborhood");
            tempPlaces.Add("neighborhood_2");
            tempPlaces.Add("shop");
            tempPlaces.Add("forest");

            foreach( string place in tempPlaces )
                if( StateManager.Instance.GetState( "visited_" + place ) == 100 )
                    places.Add( place );

            screenDimensions = new Vector2( 30 + places.Count * 220, 153 );

            screenPosition = new Vector2(
               (int)Math.Round((SettingsManager.GAME_WIDTH / 2.0f) - (screenDimensions.X / 2.0f)),
               (int)Math.Round((SettingsManager.GAME_HEIGHT / 2.0f) - (screenDimensions.Y / 2.0f))
            );

            screenBackground = new Texture2D(SettingsManager.GraphicsDevice.GraphicsDevice, (int)screenDimensions.X, (int)screenDimensions.Y);
            Color[] bgColor = new Color[(int)screenDimensions.X * (int)screenDimensions.Y];
            for (int i = 0; i < bgColor.Length; i++)
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
            if (input.IsNewKeyPress(Keys.Left))     selected = ( selected - 1 + places.Count ) % places.Count;
            if (input.IsNewKeyPress(Keys.Right)) selected = (selected + 1) % places.Count;

            if (input.IsMenuSelect())
            {
                LevelManager.Instance.GoTo(places[ selected ]);
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
            }
            else if (input.IsMenuCancel())
            {
                ExitScreen();
            }
            
        }
        #endregion

        #region Update and Draw
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(screenBackground, screenPosition, null, Color.Black * 0.8f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(ResourceManager.Instance.GlobalTexture, screenPosition + new Vector2( 15 + selected * 220, 15 ), ResourceManager.Instance["map_glow"], Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, ResourceManager.Instance.DesaturationShader);
            for (int i = 0; i < places.Count; i++)
                spriteBatch.Draw( ResourceManager.Instance.GlobalTexture, screenPosition + new Vector2( 20 + i * 220, 20 ), ResourceManager.Instance[places[i] + "_t"], new Color( (byte)desaturationAmount, 255, 255, 255 ) );
            spriteBatch.End();
        }
        #endregion

    }
}
