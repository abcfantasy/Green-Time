using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GreenTimeGameData.Components;
using Microsoft.Xna.Framework;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class FinalScreen : GameScreen
    {
        static readonly float BACKGROUND_LAYER  = 0.75f;
        List<Sprite> visibleObjects = new List<Sprite>();

        ContentManager content;
        SpriteFont gameFont;

        Effect desaturateShader;

        float objectFading = 0;

        public FinalScreen()
        {
            StateManager.Instance.SetState("progress", 0);   // to set full saturation
        }

        /// <summary>
        /// Load graphics content for the game
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            desaturateShader = content.Load<Effect>("desaturate");
            gameFont = content.Load<SpriteFont>("gamefont");

            LoadGameObjects();

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //System.Threading.Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        public void LoadGameObjects()
        {            
            visibleObjects.Clear();

            // Load the background
            visibleObjects.Add( LevelManager.Instance.CurrentLevel.backgroundTexture );
            LevelManager.Instance.CurrentLevel.backgroundTexture.Load();

            for (int i = 0; i < LevelManager.Instance.CurrentLevel.gameObjects.Count; i++)
            {
                if (LevelManager.Instance.CurrentLevel.gameObjects[i].sprite != null)
                {
                    LevelManager.Instance.CurrentLevel.gameObjects[i].sprite.Load();
                    visibleObjects.Add(LevelManager.Instance.CurrentLevel.gameObjects[i].sprite);

                    if (LevelManager.Instance.CurrentLevel.gameObjects[i].sprite.GetType() == typeof(AnimatedSprite))
                    {
                        ((AnimatedSprite)LevelManager.Instance.CurrentLevel.gameObjects[i].sprite).ActiveAnimations.Clear();
                        foreach (FrameSet ap in ((AnimatedSprite)LevelManager.Instance.CurrentLevel.gameObjects[i].sprite).animations)
                            if (StateManager.Instance.CheckDependencies(ap.dependencies))
                                ((AnimatedSprite)LevelManager.Instance.CurrentLevel.gameObjects[i].sprite).ActiveAnimations[ap.name] = ap.frames;
                    }
                }
            }
        }

        public override void HandleInput(InputManager input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if ( this.IsActive && this.TransitionPosition == 0 )
            {
                if (input.IsMenuCancel())
                    ScreenManager.Game.Exit();
            }
        }
        /// <summary>
        /// Unload graphics content used by the game
        /// </summary>
        public override void UnloadContent()
        {
            if (content != null)
                content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            // update object animations
            for (int i = 0; i < visibleObjects.Count; i++)
            {
                if (visibleObjects[i].GetType() == typeof(AnimatedSprite))
                {
                    ((AnimatedSprite)visibleObjects[i]).UpdateFrame(gameTime.ElapsedGameTime.Seconds);
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            #region /* The following states manage the player fading from green to grey or circle to square or vice versa */
            if (StateManager.Instance.GetState("state_final_fading_square_grey") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Instance.SetState("state_final_square_grey", (int)objectFading);
                if (StateManager.Instance.GetState("state_final_square_grey") >= 100)
                {
                    objectFading = 0;
                    StateManager.Instance.SetState("state_final_square_grey", 100);
                    StateManager.Instance.SetState("state_final_fading_square_grey", 0);
                    StateManager.Instance.SetState("state_final_fading_round_grey", 100);
                }
            }
            else if (StateManager.Instance.GetState("state_final_fading_round_grey") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Instance.SetState("state_final_round_grey", (int)objectFading);
                if (StateManager.Instance.GetState("state_final_round_grey") >= 100)
                {
                    objectFading = 0;
                    StateManager.Instance.SetState("state_final_round_grey", 100);
                    StateManager.Instance.SetState("state_final_fading_round_grey", 0);
                    StateManager.Instance.SetState("state_final_fading_round_green", 100);
                }
            }
            else if (StateManager.Instance.GetState("state_final_fading_round_green") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Instance.SetState("state_final_round_green", (int)objectFading);
                if (StateManager.Instance.GetState("state_final_round_green") >= 100)
                {
                    objectFading = 0;
                    StateManager.Instance.SetState("state_final_round_green", 100);
                    StateManager.Instance.SetState("state_final_fading_round_green", 0);
                    StateManager.Instance.SetState("state_final_fading_text1", 100);
                }
            }
            else if (StateManager.Instance.GetState("state_final_fading_text1") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Instance.SetState("state_final_text1", (int)objectFading);
                if (StateManager.Instance.GetState("state_final_text1") >= 100)
                {
                    objectFading = 0;
                    StateManager.Instance.SetState("state_final_text1", 100);
                    StateManager.Instance.SetState("state_final_fading_text1", 0);
                    StateManager.Instance.SetState("state_final_fading_text2", 100);
                }
            }
            else if (StateManager.Instance.GetState("state_final_fading_text2") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Instance.SetState("state_final_text2", (int)objectFading);
                if (StateManager.Instance.GetState("state_final_text2") >= 100)
                {
                    objectFading = 0;
                    StateManager.Instance.SetState("state_final_text2", 100);
                    StateManager.Instance.SetState("state_final_fading_text2", 0);
                }
            }
            else if (StateManager.Instance.GetState("state_final_square_grey") == 0)
                StateManager.Instance.SetState("state_final_fading_square_grey", 100);
            #endregion

        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, desaturateShader);

            // bg
            visibleObjects[0].Draw(spriteBatch, new Color((visibleObjects[0].shaded ? 0 : 64), 255, 255, 255));

            visibleObjects[1].Draw(spriteBatch, new Color((visibleObjects[1].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_square_grey") * 2.55))));
            visibleObjects[2].Draw(spriteBatch, new Color((visibleObjects[2].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_round_grey") * 2.55))));
            visibleObjects[3].Draw(spriteBatch, new Color((visibleObjects[3].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_round_green") * 2.55))));
            visibleObjects[4].Draw(spriteBatch, new Color((visibleObjects[4].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_text1") * 2.55))));
            visibleObjects[5].Draw(spriteBatch, new Color((visibleObjects[5].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_text2") * 2.55))));

            spriteBatch.End();      
        }
    }
}
