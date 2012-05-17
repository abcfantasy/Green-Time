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

        float objectFading = 0;
        float pauseTime = 0.0f;

        public FinalScreen()
        {
            StateManager.Instance.SetState("progress", 0);   // to set full saturation

            TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            TransitionOffTime = TimeSpan.FromSeconds(3.0f);
        }

        /// <summary>
        /// Load graphics content for the game
        /// </summary>
        public override void LoadContent()
        {
            LevelManager.Instance.LoadAllLevels();
            ResourceManager.Instance.LoadGameplayTexture();
            ResourceManager.Instance.LoadLevelTexture(LevelManager.Instance.CurrentLevel.texture);

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
            ResourceManager.Instance.UnloadLocalContent();

            SoundManager.UnloadLocal();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                // update object animations
                for (int i = 0; i < visibleObjects.Count; i++)
                {
                    if (visibleObjects[i].GetType() == typeof(AnimatedSprite))
                    {
                        ((AnimatedSprite)visibleObjects[i]).UpdateFrame(gameTime.ElapsedGameTime.Seconds);
                    }
                }


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
                else
                {
                    pauseTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (pauseTime > 3000)
                    {
                        LoadingScreen.Load(ScreenManager, false,
                            new TextOnBlackScreen("AVANTgarde - Credits", "Andrei Livadariu\nAndrew Borg Cardona\nNina Croitoru\nVirgil Tanase\n\nMentored by: Terry Guijt",
                                new GameScreen[] { new TextOnBlackScreen( "Other credits", "freesound.org\nopenclipart.org\nheathersanimations.com", 
                                    new GameScreen[] { new TextOnBlackScreen( "Thank you for playing", "", 
                                        new GameScreen[] { new BackgroundScreen(), new LogoScreen(true), new MainMenuScreen() }, false ) }, false, 4000, 150 ) }, false, 5000, 150));
                         
                    }
                }
                #endregion
            }

        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, ResourceManager.Instance.DesaturationShader);

            // bg
            visibleObjects[0].Draw(ResourceManager.Instance.LevelTexture, spriteBatch, ResourceManager.Instance["background"], new Color((visibleObjects[0].shaded ? 0 : 64), 255, 255, 255));
            visibleObjects[4].Draw(ResourceManager.Instance.LevelTexture, spriteBatch, ResourceManager.Instance["final_room_text1"], new Color((visibleObjects[4].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_text1") * 2.55))));
            visibleObjects[5].Draw(ResourceManager.Instance.LevelTexture, spriteBatch, ResourceManager.Instance["final_room_text2"], new Color((visibleObjects[5].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_text2") * 2.55))));

            // player
            visibleObjects[1].Draw(ResourceManager.Instance.GlobalTexture, spriteBatch, ResourceManager.Instance["character_square"], new Color((visibleObjects[1].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_square_grey") * 2.55))));
            visibleObjects[2].Draw(ResourceManager.Instance.GlobalTexture, spriteBatch, ResourceManager.Instance["character_round"], new Color((visibleObjects[2].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_round_grey") * 2.55))));
            visibleObjects[3].Draw(ResourceManager.Instance.GlobalTexture, spriteBatch, ResourceManager.Instance["character_round"], new Color((visibleObjects[3].shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Instance.GetState("state_final_round_green") * 2.55))));
           

            spriteBatch.End();      
        }
    }
}
