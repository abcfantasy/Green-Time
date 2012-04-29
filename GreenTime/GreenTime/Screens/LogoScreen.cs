using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GreenTime.Managers;
using GreenTimeGameData.Components;

namespace GreenTime.Screens
{
    public class LogoScreen : GameScreen
    {
        //ContentManager content;
        //Texture2D logo;
        float elapsed = 0;
        float yOffset = 0;
        float scaleOffset = 1.0f;
        float backgroundAlpha = 1.0f;

        //Texture2D background;
        //Texture2D backgroundTexture;
        //Texture2D leaf;
        //Texture2D alternateLeaf;
        List<Leaf> leaves = new List<Leaf>();

        //float leafY = 0.0f;
        //float leafRotation = 0.0f;
        //bool leafRotationIncreasing = true;
        float leafInterval = 0.0f;
        float nextLeaf = 100.0f;
        float maxLeaves = 30;

        public LogoScreen( bool noAnimation = false )
        {
            if (noAnimation)
            {
                TransitionOnTime = TimeSpan.FromSeconds(0.0);
                TransitionOffTime = TimeSpan.FromSeconds(0.0);
                elapsed = 2000;
                yOffset = 120;
                backgroundAlpha = 0.0f;
                scaleOffset = 0.519999f;
            }
            else
            {
                TransitionOnTime = TimeSpan.FromSeconds(1.5);
                TransitionOffTime = TimeSpan.FromSeconds(0.5);
            }
        }

        /// <summary>
        /// Load graphics content for the game
        /// </summary>
        public override void LoadContent()
        {
            //if (content == null)
            //    content = new ContentManager(ScreenManager.Game.Services, "Content");

            //logo = content.Load<Texture2D>("GreenTimeLogo");
            //backgroundTexture = content.Load<Texture2D>("background");
            //leaf = content.Load<Texture2D>("leaf");
            //alternateLeaf = content.Load<Texture2D>("leaf2");

            // create the rectangle texture without colors
            //background = new Texture2D(
            //    ScreenManager.GraphicsDevice,
            //    1,
            //    1);

            // Set the color data for the texture
            //background.SetData(new Color[] { Color.Black });

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //System.Threading.Thread.Sleep(1000);

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
            //if (content != null)
            //    content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (IsActive)
            {
                elapsed += gameTime.ElapsedGameTime.Milliseconds;

                if (elapsed > 2000)
                {
                    if (yOffset == 120)
                    {
                        ScreenManager.AddScreen(new MainMenuScreen());
                    }
                    else
                    {
                        yOffset += 2;
                        scaleOffset -= 0.008f;
                    }

                }
            }
            else
            {
                leafInterval += gameTime.ElapsedGameTime.Milliseconds;

                if (backgroundAlpha > 0.0f)
                    backgroundAlpha -= 0.01f;

                for (int i = 0; i < leaves.Count; i++)
                {
                    leaves[i].Update();
                    if (leaves[i].Y > SettingsManager.GAME_HEIGHT + 50)
                    {
                        leaves.RemoveAt(i);
                        i--;
                    }
                    
                }

                if (leafInterval >= nextLeaf)
                {
                    leafInterval = 0.0f;
                    nextLeaf = new Random().Next(1000, 2500);
                    if ( leaves.Count < maxLeaves )
                        leaves.Add(new Leaf());
                }
            }

        }
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!

            if (IsActive)
            {
                ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                                   Color.Black, 0, 0);
            }

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            if (!IsActive)
            {
                spriteBatch.Draw(ResourceManager.Instance.GlobalTexture, new Rectangle(0, 0, SettingsManager.GAME_WIDTH, SettingsManager.GAME_HEIGHT), ResourceManager.Instance["menu_black_pixel"], Color.White * backgroundAlpha);

                // leaf
                for ( int i = 0; i < leaves.Count; i++ )
                    spriteBatch.Draw(ResourceManager.Instance.GlobalTexture, new Vector2(leaves[i].X, leaves[i].Y), leaves[i].Kind ? ResourceManager.Instance["menu_leaf1"] : ResourceManager.Instance["menu_leaf2"], Color.White * 0.5f, (float)Math.Atan(leaves[i].Rotation * 2.0f) / 2.0f, new Vector2(42.5f, -50), leaves[i].Scale, leaves[i].Effects, 0.0f);
            }

            spriteBatch.Draw(ResourceManager.Instance.GlobalTexture, new Vector2((SettingsManager.GAME_WIDTH / 2) - (320.0f * scaleOffset), (SettingsManager.GAME_HEIGHT / 2) - 240.0f - yOffset), ResourceManager.Instance["menu_logo"], Color.White * this.TransitionAlpha, 0.0f, Vector2.Zero, scaleOffset, SpriteEffects.None, 0.0f);
            
            spriteBatch.End();
        }
    }
}
