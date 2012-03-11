using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GreenTime.Managers;
using Microsoft.Xna.Framework;

namespace GreenTime.Screens
{
    public class PastPresentTransitionScreen : GameScreen
    {
        private Texture2D backgroundTexture;

        public PastPresentTransitionScreen(Texture2D background)
        {
            this.backgroundTexture = background;

            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.5f);

            TransitionPosition = 0;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (TransitionPosition == 0)
                ExitScreen();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture,
                Vector2.Zero, new Rectangle( 0, 0, backgroundTexture.Width, backgroundTexture.Height ),
                Color.White, 0.0f, Vector2.Zero, TransitionAlpha, SpriteEffects.None, 0 );

            spriteBatch.End();
        }
    }
}
