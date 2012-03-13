using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GreenTime.GameObjects;
using GreenTimeGameData.Components;
using Microsoft.Xna.Framework;
using GreenTime.Managers;

namespace GreenTime.Screens
{
    class FinalScreen : GameScreen
    {
        static readonly float BACKGROUND_LAYER  = 0.75f;
        List<BaseObject> gameObjects = new List<BaseObject>();

        ContentManager content;
        SpriteFont gameFont;

        Effect desaturateShader;

        float objectFading = 0;

        public FinalScreen()
        {
            StateManager.Current.SetState("progress", 0);   // to set full saturation
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
            BaseObject newGameObject;
            Sprite sprite;
            Animation animation;

            gameObjects.Clear();

            // Load the background
            newGameObject = new BaseObject(Vector2.Zero, LevelManager.State.CurrentLevel.BackgroundTexture.Shaded, BACKGROUND_LAYER, 1.0f);
            newGameObject.Load(content, LevelManager.State.CurrentLevel.BackgroundTexture.TextureName);
            gameObjects.Add(newGameObject);

            for (int i = 0; i < LevelManager.State.CurrentLevel.GameObjects.Count; i++)
            {
                if (LevelManager.State.CurrentLevel.GameObjects[i].Sprite.Length > 0)
                {
                    sprite = LevelManager.State.CurrentLevel.GameObjects[i].Sprite[0];

                    // static object
                    if (sprite.Animation.Count == 0)
                    {
                        newGameObject = new BaseObject(sprite.Position, sprite.Shaded, sprite.Layer, sprite.Scale);
                    }
                    // animated object
                    else
                    {
                        animation = sprite.Animation[0];
                        newGameObject = new AnimatedObject(sprite.Position, animation.FrameWidth, animation.FrameHeight, animation.FramesPerSecond, sprite.Shaded, sprite.Layer, sprite.Scale);
                        // add animations
                        ((AnimatedObject)newGameObject).AddAnimations(animation.Playbacks);
                    }
                    newGameObject.Load(content, sprite.TextureName);
                    gameObjects.Add(newGameObject);
                }
            }
        }

        public override void HandleInput(InputManager input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if ( this.IsActive && this.TransitionPosition == 0 )
            {
                // check for action button, only if player is over interactive object, and if player is either dropping an object or has no object in hand
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
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].GetType() == typeof(AnimatedObject))
                {
                    ((AnimatedObject)gameObjects[i]).UpdateFrame(gameTime.ElapsedGameTime.Seconds);
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            #region /* The following states manage the player fading from green to grey or circle to square or vice versa */
            if (StateManager.Current.GetState("state_final_fading_square_grey") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Current.SetState("state_final_square_grey", (int)objectFading);
                if (StateManager.Current.GetState("state_final_square_grey") >= 100)
                {
                    objectFading = 0;
                    StateManager.Current.SetState("state_final_square_grey", 100);
                    StateManager.Current.SetState("state_final_fading_square_grey", 0);
                    StateManager.Current.SetState("state_final_fading_round_grey", 100);
                }
            }
            else if (StateManager.Current.GetState("state_final_fading_round_grey") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Current.SetState("state_final_round_grey", (int)objectFading);
                if (StateManager.Current.GetState("state_final_round_grey") >= 100)
                {
                    objectFading = 0;
                    StateManager.Current.SetState("state_final_round_grey", 100);
                    StateManager.Current.SetState("state_final_fading_round_grey", 0);
                    StateManager.Current.SetState("state_final_fading_round_green", 100);
                }
            }
            else if (StateManager.Current.GetState("state_final_fading_round_green") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Current.SetState("state_final_round_green", (int)objectFading);
                if (StateManager.Current.GetState("state_final_round_green") >= 100)
                {
                    objectFading = 0;
                    StateManager.Current.SetState("state_final_round_green", 100);
                    StateManager.Current.SetState("state_final_fading_round_green", 0);
                    StateManager.Current.SetState("state_final_fading_text1", 100);
                }
            }
            else if (StateManager.Current.GetState("state_final_fading_text1") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Current.SetState("state_final_text1", (int)objectFading);
                if (StateManager.Current.GetState("state_final_text1") >= 100)
                {
                    objectFading = 0;
                    StateManager.Current.SetState("state_final_text1", 100);
                    StateManager.Current.SetState("state_final_fading_text1", 0);
                    StateManager.Current.SetState("state_final_fading_text2", 100);
                }
            }
            else if (StateManager.Current.GetState("state_final_fading_text2") == 100 && TransitionPosition == 0)
            {
                objectFading += gameTime.ElapsedGameTime.Milliseconds / 15.0f;
                StateManager.Current.SetState("state_final_text2", (int)objectFading);
                if (StateManager.Current.GetState("state_final_text2") >= 100)
                {
                    objectFading = 0;
                    StateManager.Current.SetState("state_final_text2", 100);
                    StateManager.Current.SetState("state_final_fading_text2", 0);
                }
            }
            else if (StateManager.Current.GetState("state_final_square_grey") == 0)
                StateManager.Current.SetState("state_final_fading_square_grey", 100);
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
            gameObjects[0].Draw(spriteBatch, new Color((gameObjects[0].Shaded ? 0 : 64), 255, 255, 255), gameObjects[0].Scale);

            gameObjects[1].Draw(spriteBatch, new Color((gameObjects[1].Shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Current.GetState("state_final_square_grey") * 2.55)) ), gameObjects[1].Scale );
            gameObjects[2].Draw(spriteBatch, new Color((gameObjects[2].Shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Current.GetState("state_final_round_grey") * 2.55))), gameObjects[2].Scale);
            gameObjects[3].Draw(spriteBatch, new Color((gameObjects[3].Shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Current.GetState("state_final_round_green") * 2.55))), gameObjects[3].Scale);
            gameObjects[4].Draw(spriteBatch, new Color((gameObjects[4].Shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Current.GetState("state_final_text1") * 2.55))), gameObjects[4].Scale);
            gameObjects[5].Draw(spriteBatch, new Color((gameObjects[5].Shaded ? 0 : 64), 255, 255, ((byte)(StateManager.Current.GetState("state_final_text2") * 2.55))), gameObjects[5].Scale);

            spriteBatch.End();      
        }
    }
}
