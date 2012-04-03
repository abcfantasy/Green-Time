using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GreenTimeGameData.Components;
using Microsoft.Xna.Framework.Content;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class StateTransitionScreen : GameScreen
    {
        private Player player;
        private Texture2D background;
        private ContentManager content;
        private Effect desaturateShader;
        private int elapsedTime;

        #region Initialization
        public StateTransitionScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            player = LevelManager.Instance.Player;

            elapsedTime = 0;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            desaturateShader = content.Load<Effect>("desaturate");

            background = new Texture2D(SettingsManager.GraphicsDevice.GraphicsDevice, SettingsManager.GAME_WIDTH, SettingsManager.GAME_HEIGHT);
            Color[] bgColor = new Color[SettingsManager.GAME_WIDTH * SettingsManager.GAME_HEIGHT];
            for (int i = 0; i < SettingsManager.GAME_WIDTH * SettingsManager.GAME_HEIGHT; i++)
                bgColor[i] = Color.White;
            background.SetData(bgColor);

            CheckPlayerStatus();

        }
        #endregion
        #region Draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            player.Update(gameTime);

            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            if (elapsedTime > 2000)
            {
                LoadingScreen.Load(ScreenManager, false, 0.0f, new PlayScreen());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.White, 0, 0);

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, desaturateShader);

            spriteBatch.Draw(background, new Vector2(0, 0), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.8f);

            // player
            player.Draw(spriteBatch);

            spriteBatch.End();
        }
        #endregion

        // check if player should change color or shape
        private void CheckPlayerStatus()
        {
            int playerStatus = StateManager.Instance.GetState(StateManager.STATE_PLAYERSTATUS);
            //StateManager.Instance.SetState("just_went_out", 0);
            if (StateManager.Instance.GetState(StateManager.STATE_INDOOR) == 100)
            {
                if (playerStatus == 50) player.turnGreen();
                else if (playerStatus == 0) player.transformShape();

                StateManager.Instance.SetState(StateManager.STATE_PLAYERSTATUS, Math.Min(playerStatus + 50, 100));
            }
            else
            {
                if (playerStatus == 100) player.turnGrey();
                else if (playerStatus == 50) player.transformShape();

                StateManager.Instance.SetState(StateManager.STATE_PLAYERSTATUS, Math.Max(playerStatus - 50, 0));
            }
        }
    }
}
