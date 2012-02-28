using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GreenTime.Managers;
using GreenTimeGameData.Components;

namespace GreenTime.Screens
{
    public class PlayScreen : GameScreen
    {
        #region Fields
        ContentManager content;
        SpriteFont gameFont;

        Texture2D playerTexture;
        Vector2 playerPosition;

        Texture2D backgroundTexture;

        List<Texture2D> objectTextures = new List<Texture2D>();
        List<Vector2> objectPositions = new List<Vector2>();

        InteractiveObject interactingObject;
        Effect desaturateShader;
        // This value should be changed according to the progress in the game
        // I left it as a float so that we can easily calculate it based on the progress
        // When it's used, it is cast into a byte
        // 0 = fully desaturated
        // 64 = original colors
        float desaturationAmount = 0;

        float pauseAlpha;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public PlayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            playerPosition = LevelManager.State.PlayerPosition;
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
            playerTexture = content.Load<Texture2D>("player");
            
            // load background
            if ( !String.IsNullOrEmpty( LevelManager.State.CurrentLevel.BackgroundTexture ) )
                backgroundTexture = content.Load<Texture2D>(LevelManager.State.CurrentLevel.BackgroundTexture);

            // load game objects
            for ( int i = 0; i < LevelManager.State.CurrentLevel.GameObjects.Count; i++ )
            {
                if (LevelManager.State.CurrentLevel.GameObjects[i].Sprite.Length > 0 && StateManager.Current.DependentStatesSatisfied( LevelManager.State.CurrentLevel.GameObjects[i].DependentStates ) )
                {
                    objectTextures.Add(content.Load<Texture2D>(LevelManager.State.CurrentLevel.GameObjects[i].Sprite[0].TextureName));
                    objectPositions.Add(new Vector2(LevelManager.State.CurrentLevel.GameObjects[i].Sprite[0].PositionX,
                                                    LevelManager.State.CurrentLevel.GameObjects[i].Sprite[0].PositionY));
                }
            }
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
        /// Unload graphics content used by the game
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, 
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // check if player should return to present
                CheckPastPresentState();

                // check if the player moved out of the screen
                CheckTransitionBoundaries();

                // check if player collided with some objects
                CheckObjectCollisions();
            }

        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin( SpriteSortMode.Immediate, null, null, null, null, desaturateShader );

            // background
            //spriteBatch.Draw(backgroundTexture, Vector2.Zero,
            //                 new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            // player
            spriteBatch.Draw(playerTexture, playerPosition, new Color(255, 255, 255, (byte)desaturationAmount));

            // game objects
            for (int i = 0; i < objectTextures.Count; i++)
            {
                spriteBatch.Draw(objectTextures[i], objectPositions[i], new Color(255, 255, 255, (byte)desaturationAmount));
            }

            // text
            if (interactingObject != null && interactingObject.Text != "" )
            {
                SpriteFont font = ScreenManager.Font;
                Vector2 textSize = font.MeasureString(interactingObject.Text);
                Vector2 textPosition = new Vector2( (SettingsManager.GAME_WIDTH - textSize.X) / 2, 670 );
                spriteBatch.DrawString(ScreenManager.Font, interactingObject.Text, textPosition, Color.White);
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
        #endregion

        #region Input
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(Managers.InputManager input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            KeyboardState keyboardState = input.CurrentKeyboardState;
            GamePadState gamePadState = input.CurrentGamePadState;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected;

            if (input.IsPauseGame() || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseScreen());
            }
            else if ( this.IsActive )
            {
                // check for action button, only if player is over interactive object
                if (input.IsMenuSelect() && interactingObject != null)
                {
                    // handling special news case
                    if (interactingObject.Special == "news")
                    {
                        ScreenManager.AddScreen(new NewsScreen());
                    }
                    // chat if available
                    else if (interactingObject.ChatIndex != LevelManager.EMPTY_VALUE)
                    {
                        ScreenManager.AddScreen(new ChatScreen(LevelManager.State.GetChat(interactingObject.ChatIndex), true));
                    }
                    // transition into past
                    else if ( !String.IsNullOrEmpty( interactingObject.Transition) )
                    {
                        LevelManager.State.TransitionPast(interactingObject.Transition);
                        LoadingScreen.Load(ScreenManager, false, new PlayScreen());
                    }

                }

                // move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                /*
                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;
                */

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                //movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 5;
            }
        }
        #endregion

        #region Private Methods
        private void CheckPastPresentState()
        {
            if (StateManager.Current.ShouldReturnToPresent() && LevelManager.State.LastPresentLevel != null)
            {
                LevelManager.State.TransitionPresent();
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
            }
        }

        private void CheckTransitionBoundaries()
        {
            // If we're trying to move in a direction but there's no level there, we need to stop
            if (playerPosition.X + SettingsManager.PLAYER_WIDTH >= SettingsManager.GAME_WIDTH
                && !LevelManager.State.CanTransitionRight())
            {
                playerPosition.X = SettingsManager.GAME_WIDTH - SettingsManager.PLAYER_WIDTH;
                return;
            }
            else if (playerPosition.X <= 0
                     && !LevelManager.State.CanTransitionLeft())
            {
                playerPosition.X = 0;
                return;
            }

            // if player moves outside the right boundary
            if (playerPosition.X > SettingsManager.GAME_WIDTH)
            {
                // transition to the right
                LevelManager.State.TransitionRight();
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
            }
            // if player moves outside left boundary
            else if (playerPosition.X < -SettingsManager.PLAYER_WIDTH)
            {
                // transition to the left
                LevelManager.State.TransitionLeft();
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
            }
        }

        private void CheckObjectCollisions()
        {
            interactingObject = null;

            for (int i = 0; i < LevelManager.State.CurrentLevel.GameObjects.Count; i++)
            {
                if ( playerPosition.X >= LevelManager.State.CurrentLevel.GameObjects[i].BoundX &&
                     playerPosition.X <= LevelManager.State.CurrentLevel.GameObjects[i].BoundX + LevelManager.State.CurrentLevel.GameObjects[i].BoundWidth &&
                     StateManager.Current.DependentStatesSatisfied( LevelManager.State.CurrentLevel.GameObjects[i].DependentStates ) )
                {
                    interactingObject = LevelManager.State.CurrentLevel.GameObjects[i];

                    // if terrain is impassable, check which direction player is colliding and prevent movement
                    if (LevelManager.State.CurrentLevel.GameObjects[i].Impassable)
                    {
                        // to safely determine the side in which the player collided, we check first if the player is between the left edge of the object and the middle of the object
                        // since player moves more than 1 pixels at a time. This would mean he collided on the left side, otherwise it's the right side
                        if (playerPosition.X >= LevelManager.State.CurrentLevel.GameObjects[i].BoundX &&
                            playerPosition.X <= LevelManager.State.CurrentLevel.GameObjects[i].BoundX + ( LevelManager.State.CurrentLevel.GameObjects[i].BoundWidth / 2 ))
                        {
                            playerPosition.X = LevelManager.State.CurrentLevel.GameObjects[i].BoundX - 1;
                        }
                        else
                            playerPosition.X = LevelManager.State.CurrentLevel.GameObjects[i].BoundX + LevelManager.State.CurrentLevel.GameObjects[i].BoundWidth + 1;
                    }
                }

            }
        }
        #endregion
    }
}
