using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    public class IntroScreen : GameScreen
    {
        //double milliseconds;

        float elapsed = 0.0f;

        float textY = 800.0f;
        Vector2 lineSize;
        float worldGrayscale = 64.0f;
        float blackBackgroundAlpha = 0.0f;
        float watchAlpha = 0.0f;
        float treeAlpha = 0.0f;
        float lightAlpha = 0.0f;
        float playerAlpha = 0.0f;

        string[] lines = new string[] {
            "The world has become grey and bland",
            "Because people are polluting nature",
            "Soon it will be too late",
            "There will be no turning back",
            "",
            "Unless someone can undo the mistakes of the past",
            "Someone who can motivate people to change",
            "So that the world will change with them",
            "",
            "You wake up after a strange dream",
            "You dreamt you could turn back time and make a better future",
            "You dreamt you could bring color back to this world",
            "",
            "Was this really a dream?",
            "As a new day starts,",
            "It's time to find out and start exploring.",
            "",
            "Change yourself!",
            "Change the world!",
        };

        public IntroScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0f);
            TransitionOffTime = TimeSpan.FromSeconds(3.0f);
        }

        /// <summary>
        /// Load graphics content for the game
        /// </summary>
        public override void LoadContent()
        {

            //desaturateShader = content.Load<Effect>("desaturate");
            ResourceManager.Instance.LoadLevelTexture("introTexture");

            lineSize = ScreenManager.Font.MeasureString(lines[0]);


            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (elapsed > 53000)
                SoundManager.UpdateFade(TransitionPosition);

            //milliseconds = SoundManager.TestPosition();
            textY -= 0.35f;

            elapsed += gameTime.ElapsedGameTime.Milliseconds;

            if (elapsed > 500 && elapsed < 4560)
            {
                worldGrayscale -= 0.25f;
            }
            else if (elapsed > 5560 && blackBackgroundAlpha < 1.0)
            {
                blackBackgroundAlpha += 0.002f;
            }
            else if (elapsed > 19000 && elapsed < 23000 )
            {
                watchAlpha += 0.01f;
            }
            else if (elapsed > 25000 && watchAlpha > 0.0f)
            {
                watchAlpha -= 0.01f;
            }
            else if (elapsed > 31000 && elapsed < 35000)
            {
                lightAlpha += 0.01f;
            }
            else if (elapsed > 40000 && elapsed < 43000)
            {
                treeAlpha += 0.01f;
            }
            else if (elapsed > 48000 && elapsed < 51000)
            {
                playerAlpha += 0.01f;
            }
            else if (elapsed > 53000 && (treeAlpha > 0.0f || lightAlpha > 0.0f))
            {
                treeAlpha -= 0.01f;
                lightAlpha -= 0.01f;
            }
            else if (elapsed > 62000)
            {
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
            }
        }

        public override void HandleInput(Managers.InputManager input)
        {
            if (input.IsMenuSelect() || input.IsMenuCancel())
            {
                this.TransitionOffTime = TimeSpan.FromSeconds( 0.5f );
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            if (this.TransitionAlpha < 1.0f && elapsed < 10000)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(ResourceManager.Instance.LevelTexture, Vector2.Zero, ResourceManager.Instance["background"], Color.White * this.TransitionAlpha);
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, ResourceManager.Instance.DesaturationShader);
                spriteBatch.Draw(ResourceManager.Instance.LevelTexture, Vector2.Zero, ResourceManager.Instance["background"], new Color((int)worldGrayscale, 255, 255, 255), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.End();
            }

            spriteBatch.Begin();

            spriteBatch.Draw(ResourceManager.Instance.LevelTexture, new Rectangle( 0, 0, 1280, 720 ), ResourceManager.Instance["intro_black"], Color.Black * blackBackgroundAlpha, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);

            spriteBatch.Draw(ResourceManager.Instance.LevelTexture, new Vector2(880, 250), ResourceManager.Instance["intro_clock"], Color.White * watchAlpha);

            spriteBatch.Draw(ResourceManager.Instance.LevelTexture, new Vector2(0, 0), ResourceManager.Instance["intro_light"], Color.White * lightAlpha, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.6f);

            spriteBatch.Draw(ResourceManager.Instance.LevelTexture, new Vector2(1000, 370), ResourceManager.Instance["intro_tree"], Color.White * treeAlpha, 0.0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0.6f); ;

            spriteBatch.Draw(ResourceManager.Instance.LevelTexture, new Vector2(20, 400), ResourceManager.Instance["intro_player"], Color.White * playerAlpha);

            //spriteBatch.Draw(light, new Vector2(-100, 350), new Rectangle( 0, 0, 561, 359 ), Color.White * treeAlpha, 0.0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0.6f );
            
            float lineSpacing = 0.0f;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "")
                {
                    spriteBatch.DrawString(ScreenManager.Font, lines[i], new Vector2(250, textY + lineSpacing), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                    lineSpacing += lineSize.Y;
                }
                else
                {
                    lineSpacing += lineSize.Y * 3;
                }
            }

            if (elapsed > 62000)
                spriteBatch.Draw(ResourceManager.Instance.LevelTexture, new Rectangle(0, 0, 1280, 720), ResourceManager.Instance["intro_black"], Color.Black * (1.0f - this.TransitionAlpha), 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
 
            spriteBatch.End();
        }
    }
}
