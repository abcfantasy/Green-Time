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
using Microsoft.Xna.Framework.Audio;

namespace GreenTime.Screens
{
    // Types of transitions that we can run into
    public enum TransitionType
    {
        Room,
        FromPast,
        ToPast,
        FromPresent,
        ToPresent
    }

    public class PlayScreen : GameScreen
    {
        #region Fields
        public static readonly float TEXT_LAYER = 0.0f;
        public static readonly float PLAYER_LAYER = 0.5f;
        public static readonly float BACKGROUND_LAYER = 0.75f;

        static readonly Vector2[] playerHand = new Vector2[] { 
            new Vector2( 44, 208 ),
            new Vector2( 52, 194 ),
            new Vector2( 61, 186 ),
            new Vector2( 79, 196 ),
            new Vector2( 77, 205 ),
            new Vector2( 74, 196 ),
            new Vector2( 57, 178 ),
            new Vector2( 50, 194 ),
            new Vector2( 41, 203 ),
            new Vector2( 21, 188 )
        };

        private List<Sprite> visibleObjects = new List<Sprite>();
        private List<AnimatedSprite> animatedObjects = new List<AnimatedSprite>();
        private List<GameObject> activeObjects = new List<GameObject>();
        private GameObject interactingObject;

        private Player player;
        private Sprite pickedObject;

        private TransitionType transition = TransitionType.Room;

        // HUD states
        private Rectangle hud_states;
        //private Texture2D hud_states;
        //private Texture2D hud_states_green_round;
        //private Texture2D hud_states_grey_round;
        //private Texture2D hud_states_grey_square;

        // HUD time travel
        private Rectangle hud_timetravel;
        //private Texture2D hud_timetravel;

        // This value should be changed according to the progress in the game
        // I left it as a float so that we can easily calculate it based on the progress
        // When it's used, it is cast into a byte
        // 0 = fully desaturated
        // 64 = original colors
        float desaturationAmount = StateManager.Instance.GetState("progress") * 0.64f;

        float pauseAlpha;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public PlayScreen(TransitionType transitionFrom = TransitionType.Room)
        {
            // Handle the different transition values
            transition = transitionFrom;
            switch (transition)
            {
                case TransitionType.Room:
                    TransitionOnTime = TimeSpan.FromSeconds(0.5);
                    break;

                case TransitionType.FromPast:
                    StateManager.Instance.GoToPast();
                    TransitionOnTime = TimeSpan.FromSeconds(0.5);
                    break;
                case TransitionType.FromPresent:
                    TransitionOnTime = TimeSpan.FromSeconds(0.5);
                    break;

                case TransitionType.ToPast:
                case TransitionType.ToPresent:
                    TransitionOnTime = TimeSpan.FromSeconds(2.0);
                    break;
            }
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            player = LevelManager.Instance.Player;

            // play game music
            SoundManager.PlayMusic();
        }

        /// <summary>
        /// Load graphics content for the game
        /// </summary>
        public override void LoadContent()
        {
            LevelManager.Instance.LoadAllLevels();
            ResourceManager.Instance.LoadGameplayTexture();
            ResourceManager.Instance.LoadLevelTexture(LevelManager.Instance.CurrentLevel.texture);

            SoundManager.PlayMusic();

            LoadGameObjects();

            CheckPlayerStatus();

            LoadGameObjects();

            LoadHUDObjects();

            ScreenManager.Game.ResetElapsedTime();

            foreach (AnimatedSprite s in animatedObjects)
                if (s.loop == false)
                    s.Reset();
        }

        public void LoadGameObjects()
        {
            visibleObjects.Clear();
            animatedObjects.Clear();
            activeObjects.Clear();

            // Load the objects that carry across levels
            player = LevelManager.Instance.Player;
            pickedObject = LevelManager.Instance.PickedObject;
            //if( pickedObject != null )
            //    pickedObject.Load();

            // Load ambient soud if any
            if (LevelManager.Instance.CurrentLevel.ambientSound != null)
            {
                if( StateManager.Instance.CheckDependencies( LevelManager.Instance.CurrentLevel.ambientSound.dependencies ) )
                {
                    ResourceManager.Instance.LoadSound(LevelManager.Instance.CurrentLevel.ambientSound.name, true);
                    SoundManager.PlayAmbientSound();
                    /*
                    ambientSound = content.Load<SoundEffect>(LevelManager.Instance.CurrentLevel.ambientSound.name);
                    ambientSoundInstance = ambientSound.CreateInstance();
                    ambientSoundInstance.IsLooped = LevelManager.Instance.CurrentLevel.ambientSound.looping;
                    ambientSoundInstance.Play();*/
                }
            }            

            // Load the level texture
            //ResourceManager.Instance.LoadLevelTexture(LevelManager.Instance.CurrentLevel.texture);

            // Load the background
            visibleObjects.Add(LevelManager.Instance.CurrentLevel.backgroundTexture);

            // Load the objects
            foreach (GameObject io in LevelManager.Instance.CurrentLevel.gameObjects) {
                if (StateManager.Instance.CheckDependencies(io.dependencies)) {
                    if (io.interaction != null)
                    {
                        activeObjects.Add(io);
                        // load interaction sounds
                        if (io.interaction.sound != null)
                            ResourceManager.Instance.LoadSound(io.interaction.sound.name);
                    }
                    if (io.sprite != null) {
                        io.sprite.Load();
                        visibleObjects.Add(io.sprite);

                        if (io.sprite.GetType() == typeof(AnimatedSprite)) {
                            animatedObjects.Add((AnimatedSprite)io.sprite);
                            ((AnimatedSprite)io.sprite).ActiveAnimations.Clear();
                            foreach (FrameSet ap in ((AnimatedSprite)io.sprite).animations)
                                if (StateManager.Instance.CheckDependencies(ap.dependencies))
                                    ((AnimatedSprite)io.sprite).ActiveAnimations[ap.name] = ap.frames;
                        }
                    }
                }
            }
        }

        public void LoadHUDObjects()
        {
            int playerState = StateManager.Instance.GetState(StateManager.STATE_PLAYERSTATUS);

            switch (playerState)
            {
                case 0:
                    hud_states = ResourceManager.Instance["hud_state_square"];
                    break;
                case 50:
                    hud_states = ResourceManager.Instance["hud_state_grey"];
                    break;
                case 100:
                    hud_states = ResourceManager.Instance["hud_state_green"];
                    break;
            }

            if (playerState < 100)
                hud_timetravel = ResourceManager.Instance["hud_timetravel_disabled"];
            else
                hud_timetravel = ResourceManager.Instance["hud_timetravel"];
        }

        /// <summary>
        /// Unload graphics content used by the game
        /// </summary>
        public override void UnloadContent()
        {
            ResourceManager.Instance.UnloadLocalContent();

            SoundManager.UnloadLocal();

            player.moveTo(LevelManager.Instance.StartPosition);
            if (transition == TransitionType.ToPast)
            {
                StateManager.Instance.GoToPast();
            }
            if (transition == TransitionType.ToPresent)
            {
                StateManager.Instance.GoToPresent();
            }
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

            SoundManager.Update(player.Position.X);
            SoundManager.UpdateFade(TransitionPosition);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                player.Update(gameTime);

                #region tutorials
                // press SPACE to interact
                if (StateManager.Instance.GetState("tutorial_movement") < 100 && TransitionAlpha == 1.0f)
                {
                    StateManager.Instance.SetState("tutorial_movement", 100);
                    ShowTutorial("Use the left and right arrow keys to walk around.");
                }
                else if ( StateManager.Instance.GetState("tutorial_timetravel") < 100 && StateManager.Instance.GetState(StateManager.STATE_DAY) >= 2 && LevelManager.Instance.CurrentLevel.name == "neighborhood")
                {
                    StateManager.Instance.SetState("tutorial_timetravel", 100);
                    ShowTutorial("Oh no! It seems the garbages are there again.\nSomeone must be leaving them every morning. Go over the\ngarbages and press Z to time-travel in the past and find out.");
                }
                else if (StateManager.Instance.GetState("tutorial_chat") < 100 && StateManager.Instance.IsInPast() && LevelManager.Instance.CurrentLevel.name == "neighborhood_past")
                {
                    StateManager.Instance.SetState("tutorial_chat", 100);
                    ShowTutorial("You can talk to people by pressing SPACE. During the conversation,\nyou may have to choose an answer. You can change your answer with the up\nand down arrow keys. Try and convince him not to repeat his actions.");
                }
                if (StateManager.Instance.GetState("tutorial_solved") < 100 && StateManager.Instance.GetState("puzzle_garbage_solved") == 100 && LevelManager.Instance.CurrentLevel.name == "bedroom")
                {
                    StateManager.Instance.SetState("tutorial_solved", 100);
                    ShowTutorial("Congratulations! You solved your first puzzle. Go ahead and\nsolve the remaining puzzles, and watch the world become brighter.");
                }
                #endregion

                // update picked up object if any
                if (pickedObject != null)
                {
                    if (player.Sprite.flipped)
                    {
                        pickedObject.position = player.Sprite.position + (new Vector2(player.Sprite.frameSize.X - playerHand[player.Sprite.CurrentFrame].X, playerHand[player.Sprite.CurrentFrame].Y));
                        pickedObject.position.X -= pickedObject.textureRect.Width / 2;
                    }
                    else
                        pickedObject.position = player.Sprite.position + playerHand[player.Sprite.CurrentFrame];

                }

                CheckPlayerStatus();

                // check if player should return to present
                CheckPastPresentState();

                // check if the player moved out of the screen
                CheckTransitionBoundaries();

                // check if player collided with some objects
                CheckObjectCollisions();

                // update animations
                UpdateAnimations(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (transition == TransitionType.ToPast || transition == TransitionType.ToPresent)
                ScreenManager.InitTimeTravel();

            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, null, (StateManager.Instance.IsInPast() ? ResourceManager.Instance.SepiaShader : ResourceManager.Instance.DesaturationShader));

            // hud elements
            spriteBatch.Draw(ResourceManager.Instance.GlobalTexture, new Vector2(1140, 15), hud_states, new Color( 64, 255, 255, 255 ) );
            spriteBatch.Draw(ResourceManager.Instance.GlobalTexture, new Vector2(15, 15), hud_timetravel, new Color(64, 255, 255, 255));

            // player
            player.Draw(ResourceManager.Instance.GlobalTexture, spriteBatch, ResourceManager.Instance[player.Sprite.textureName]);

            // game objects
            foreach (Sprite s in visibleObjects)
            {
                s.Draw(ResourceManager.Instance.LevelTexture, spriteBatch, ResourceManager.Instance[s.textureName], new Color((s.shaded ? (byte)desaturationAmount : 64), 255, 255, 255));
            }
            
            // picked up object if any
            if (pickedObject != null)
                pickedObject.Draw(ResourceManager.Instance.PickablesTexture, spriteBatch, ResourceManager.Instance[pickedObject.textureName], new Color((pickedObject.shaded ? (byte)desaturationAmount : 64), 255, 255, 255));

            // text only if easy mode
            if (SettingsManager.Difficulty == SettingsManager.Game_Difficulties.EASY && interactingObject != null && !String.IsNullOrEmpty(interactingObject.interaction.text))
            {
                SpriteFont font = ScreenManager.Font;
                Vector2 textSize = font.MeasureString(interactingObject.interaction.text);
                Vector2 textPosition = new Vector2((SettingsManager.GAME_WIDTH - textSize.X) / 2, 670);
                spriteBatch.DrawString(ScreenManager.Font, interactingObject.interaction.text, textPosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, TEXT_LAYER);
            }

            spriteBatch.End();

            if (player.Thought != null)
            {
                spriteBatch.Begin();
                ChatScreen.DrawText(spriteBatch, player.Mouth, 300.0f, player.Thought, player.ThoughtAlpha);
                spriteBatch.End();
            }

            // Handle the different transition types
            float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
            switch (transition)
            {
                case TransitionType.Room:
                    ScreenManager.FadeBackBufferToBlack(alpha);
                    break;

                case TransitionType.ToPast:
                case TransitionType.ToPresent:
                    if ( alpha >= 0.8f)
                    {
                        alpha = (alpha - 0.8f) * 5;
                    }
                    else
                    {
                        alpha = 0.0f;
                    }
                    ScreenManager.ApplyTimeTravel(alpha);
                    break;

                case TransitionType.FromPast:
                case TransitionType.FromPresent:
                    ScreenManager.TimeTravelFadeEffect(alpha);
                    break;
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
            else if (this.IsActive && player.IsReady && this.TransitionPosition == 0)
            {
                #region Tutorial Popups
                // grey
                if (StateManager.Instance.GetState("tutorial_grey") < 100 && StateManager.Instance.GetState(StateManager.STATE_PLAYERSTATUS) == 50)
                {
                    StateManager.Instance.SetState("tutorial_grey", 100);
                    ShowTutorial("Oh no! When you do not take care of the environment\nin your house, you become grey. You lose the ability to\ntime travel until you solve a problem in your house the next\nday.");
                }
                // square
                else if (StateManager.Instance.GetState("tutorial_square") < 100 && StateManager.Instance.GetState(StateManager.STATE_PLAYERSTATUS) == 0)
                {
                    StateManager.Instance.SetState("tutorial_square", 100);
                    ShowTutorial("You are a square head! You are not caring about\nthe environment. You'll have a hard time convincing other people\nnow. Press D to start a new day and improve yourself.");
                }
                #endregion

                // check for action button, only if player is over interactive object, and if player is either dropping an object or has no object in hand
                if (interactingObject != null)
                {
                    #region Action Button
                    if (input.IsMenuSelect()
                        && (StateManager.Instance.GetState(StateManager.STATE_PLAYERSTATUS) > 0 || LevelManager.Instance.CurrentLevel.name.Equals("bedroom") || LevelManager.Instance.CurrentLevel.name.Equals("kitchen"))
                        && (StateManager.Instance.GetState("progress") != 100 || (interactingObject.interaction.callback == "news" && StateManager.Instance.GetState("progress") == 100)))
                    {
                        // play sound if available
                        if (interactingObject.interaction.sound != null)
                            SoundManager.PlaySound(interactingObject.interaction.sound.name, interactingObject.interaction.sound.looping);

                        if (interactingObject.interaction.thought != null)
                            player.Thought = interactingObject.interaction.thought;

                        // Handling callbacks (aka special interactions)
                        if (!String.IsNullOrEmpty(interactingObject.interaction.callback))
                        {
                            switch (interactingObject.interaction.callback)
                            {
                                case "news":
                                    StateManager.Instance.SetState("tutorial_computer", 100);   // do not show computer tutorial if already interacted with
                                    ScreenManager.AddScreen(new ComputerScreen());
                                    break;
                            }
                        }

                        // Handling talking
                        if ( interactingObject.interaction.chat != null && pickedObject == null)
                        {
                            // flip NPC to face player
                            if ( interactingObject.sprite != null)
                                interactingObject.sprite.flipped = interactingObject.sprite.flippable && (interactingObject.sprite.position.X < player.Position.X);

                            ScreenManager.AddScreen(new ChatScreen(interactingObject.interaction.chat, true, interactingObject.interaction.mouth, interactingObject.interaction.startChatIndex));
                        }

                        // Pick up the object
                        if (pickedObject == null && !String.IsNullOrEmpty(interactingObject.interaction.pickUpName))
                        {
                            PickupObject(interactingObject);
                        }

                        // This prevents the player from picking up more than one object at a time
                        if (pickedObject == null || String.IsNullOrEmpty(interactingObject.interaction.pickUpName))
                        {
                            // Handling affected states
                            if (interactingObject.interaction.affectedStates != null)
                            {
                                StateManager.Instance.ModifyStates(interactingObject.interaction.affectedStates);
                                LoadGameObjects();
                            }
                        }

                        // Drop the picked up item into this object
                        if (pickedObject != null && interactingObject.interaction.dropper != null) {
                            SoundManager.PlaySound(SoundManager.SOUND_DROP);
                            DropObject(interactingObject);
                            LoadGameObjects();
                        }
                    }
                    #endregion

                    #region Time Warp Button
                    else if (input.IsReverseTime() && StateManager.Instance.CanTimeTravel())
                    {
                        // transition into past
                        if (!String.IsNullOrEmpty(interactingObject.interaction.transition))
                        {
                            LevelManager.Instance.MovePast(interactingObject.interaction.transition);
                            LoadingScreen.Load(ScreenManager, false, new PlayScreen(TransitionType.FromPresent));
                            this.transition = TransitionType.ToPast;
                            ScreenManager.timeTravelInterval = 0f;
                            TransitionOffTime = TimeSpan.FromSeconds(2.0f);
                        }
                    }
                    #endregion
                }

                // TEST CODE: D key advances the day
                #region Advance Day
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    StateManager.Instance.AdvanceDay();
                    LoadingScreen.Load(ScreenManager, false, new PlayScreen());
                }
                #endregion

                // Check movement keys and move the player
                #region Movement
                float movement = 0.0f;

                if (keyboardState.IsKeyDown(Keys.Left))     --movement;
                if (keyboardState.IsKeyDown(Keys.Right))    ++movement;

                // Checking gamepad controls only if we have one
                if (input.GamePadWasConnected) {
                    movement += gamePadState.ThumbSticks.Left.X;
                    if (movement != 0)
                        movement /= Math.Abs(movement);
                }

                /* uncomment this if you wanna be shufflin'
                if (keyboardState.IsKeyDown(Keys.Space))
                    movement = 0;
                
                player.move( movement * -2 );
                */
               
                // Update movement if we have any
                if (movement != 0.0f) {
                    player.move(movement * 15);
                    player.walk();
                }
                else /* if ( !keyboardState.IsKeyDown(Keys.Space) )  // more shufflin' */
                    player.idle();
                #endregion
            }
            else
            {
                player.Sprite.Play( FrameSet.IDLE );
            }
        }
        #endregion

        #region Private Methods
        // check if player should change color or shape
        private void CheckPlayerStatus()
        {
            int playerStatus;
            if ((!SettingsManager.TutorialsEnabled || (StateManager.Instance.GetState(StateManager.STATE_DAY) > 2 ))    // with enabled tutorials, player only changes status after 2nd day 
                && LevelManager.Instance.CurrentLevel.name.Equals("neighborhood") 
                && StateManager.Instance.GetState("just_went_out") == 100 
                && StateManager.Instance.GetState(StateManager.STATE_LOAD) == 0 
                && StateManager.Instance.GetState("progress") != 100)
            {
                StateManager.Instance.SetState("just_went_out", 0);
                if (StateManager.Instance.IndoorPuzzleSolved())
                {
                    playerStatus = StateManager.Instance.GetState(StateManager.STATE_PLAYERSTATUS);
                    if (playerStatus == 50)     player.turnGreen();
                    else if (playerStatus == 0) player.transformShape();

                    StateManager.Instance.SetState(StateManager.STATE_PLAYERSTATUS, Math.Min(playerStatus + 50, 100));
                }
                else
                {
                    playerStatus = StateManager.Instance.GetState(StateManager.STATE_PLAYERSTATUS);
                    if (playerStatus == 100)        player.turnGrey();
                    else if (playerStatus == 50)    player.transformShape();

                    StateManager.Instance.SetState(StateManager.STATE_PLAYERSTATUS, Math.Max(playerStatus - 50, 0));                   
                }
            }
             
        }
        /// <summary>
        /// Checks to see if the screen should transition to the present
        /// </summary>
        private void CheckPastPresentState()
        {
            if ( StateManager.Instance.IsInPast() && StateManager.Instance.ShouldReturnToPresent() && LevelManager.Instance.LastPresentLevel != null)
            {
                LevelManager.Instance.MovePresent();
                LoadingScreen.Load(ScreenManager, false, new PlayScreen(TransitionType.FromPast));
                this.transition = TransitionType.ToPresent;
                ScreenManager.timeTravelInterval = 0f;
                TransitionOffTime = TimeSpan.FromSeconds(2.0f);
            }
            else if (StateManager.Instance.ShouldAdvanceDay())
            {
                StateManager.Instance.AdvanceDay();
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
            }
        }

        /// <summary>
        /// Checks if the player is at the edge and the screen should transition to another screen
        /// </summary>
        private void CheckTransitionBoundaries()
        {
            // If we're trying to move in a direction but there's no level there, we need to stop
            if (player.Position.X + SettingsManager.PLAYER_WIDTH >= SettingsManager.GAME_WIDTH
                && !LevelManager.Instance.CanMoveRight())
            {
                player.moveTo( SettingsManager.GAME_WIDTH - SettingsManager.PLAYER_WIDTH );
                return;
            }
            else if (player.Position.X < 0
                     && !LevelManager.Instance.CanMoveLeft())
            {
                player.moveTo( 0 );
                return;
            }

            // if player moves outside the right boundary
            if (player.Position.X > SettingsManager.GAME_WIDTH)
            {
                // is it final room?
                if (LevelManager.Instance.CurrentLevel.rightScreenName == "final_room")
                {
                    if (StateManager.Instance.GetState("progress") == 100)
                    {
                        // transition to the right
                        LevelManager.Instance.MoveRight();
                        LoadingScreen.Load(ScreenManager, false, new FinalScreen());
                    }
                    // let player go through whole 
                    else if (StateManager.Instance.GetState("progress") >= 95)
                    {
                        StateManager.Instance.SetState("progress", 100);
                        StateManager.Instance.AdvanceDay();
                        LoadingScreen.Load(ScreenManager, false, new PlayScreen());

                    }
                    else
                    {
                        // start a new day
                        StateManager.Instance.AdvanceDay();
                        LoadingScreen.Load(ScreenManager, false, new PlayScreen());
                    }

                }
                else
                {
                    if (LevelManager.Instance.CurrentLevel.name == "kitchen")
                        StateManager.Instance.SetState("just_went_out", 100);

                    // transition to the right
                    LevelManager.Instance.MoveRight();
                    LoadingScreen.Load(ScreenManager, false, new PlayScreen());
                }
                transition = TransitionType.Room;
            }
            // if player moves outside left boundary
            else if (player.Position.X < -SettingsManager.PLAYER_WIDTH)
            {
                // transition to the left
                LevelManager.Instance.MoveLeft();
                LoadingScreen.Load(ScreenManager, false, new PlayScreen());
                transition = TransitionType.Room;
            }
        }

        /// <summary>
        /// Checks if the player collides with a game object
        /// </summary>
        private void CheckObjectCollisions()
        {
            interactingObject = null;

            foreach (GameObject io in activeObjects)
            {
                if (io.interaction != null)
                {
                    // 1D collision detection
                    // Disabled: doesn't work
                    // We have 2 intervals (a, b) and (c, d)
                    // If (a-d)*(b-c) <= 0 then we have collision

                    //if( ( player.position.X - ( io.interaction.boundX + io.interaction.boundWidth ) )
                    //    * ( ( player.position.X + player.texture.Width ) - io.interaction.boundX ) <= 0 )
                    
                    if (player.Position.X >= io.interaction.boundX
                        && player.Position.X <= (io.interaction.boundX + io.interaction.boundWidth))
                    {
                        interactingObject = io;

                        #region Tutorial Popups
                        if (interactingObject.interaction != null )
                        {
                            if (interactingObject.interaction.affectedStates != null)
                            {
                                // press SPACE to interact with heater
                                if (StateManager.Instance.GetState("tutorial_interaction") < 100)
                                {
                                    StateManager.Instance.SetState("tutorial_interaction", 100);
                                    ShowTutorial("Press SPACE to interact with objects,\nsuch as turning off the heater.");
                                }
                            }
                            // interact with computer
                            if (interactingObject.interaction.callback == "news" && StateManager.Instance.GetState(StateManager.STATE_DAY) == 2 && StateManager.Instance.GetState("tutorial_computer_interact") < 100)
                            {
                                StateManager.Instance.SetState("tutorial_computer_interact", 100);
                                ShowTutorial("You can interact with your computer to look at some news.\nThere might also be some tips within them.");
                            }
                            // pick up garbage bags
                            if (interactingObject.interaction.pickUpName != null && interactingObject.interaction.pickUpName.StartsWith("garbage") && StateManager.Instance.GetState("tutorial_pickup") < 100)
                            {
                                StateManager.Instance.SetState("tutorial_pickup", 100);
                                ShowTutorial("You can also pickup objects and drop them into particular\nplaces using SPACE. Try picking up each garbage bag and\ndropping them in the bin.");
                            }
                        }
                        #endregion

                        if (io.interaction.solid)
                        {
                            if (player.Position.X <= io.interaction.boundX + (io.interaction.boundWidth / 2))
                                player.moveTo( io.interaction.boundX );
                            else
                                player.moveTo( io.interaction.boundX + io.interaction.boundWidth );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Drop an object into the interacting object
        /// </summary>
        /// <param name="interactingObject"></param>
        private void DropObject(GameObject interactingObject)
        {
            LevelManager.Instance.PickedObject = null;
            pickedObject = null;

            StateManager.Instance.SetState( "item_picked", 0);
            StateManager.Instance.SetState( LevelManager.Instance.PickedObjectState, 100 );

            List<State> checkedStates = new List<State>();
            foreach (string s in interactingObject.interaction.dropper.drops)
                checkedStates.Add( new State( (s + "_picked"), 100 ) );

            if( ( interactingObject.interaction.dropper.trigger.Equals( Dropper.ANY ) && StateManager.Instance.AnyTrue( checkedStates ) ) ||
                ( interactingObject.interaction.dropper.trigger.Equals( Dropper.ALL ) && StateManager.Instance.AllTrue( checkedStates ) ) )
            {
                #region Tutorial
                if (interactingObject.interaction.text == "Bin" && StateManager.Instance.GetState("tutorial_drop") < 100)
                {
                    StateManager.Instance.SetState("tutorial_drop", 100);
                    ShowTutorial("Good work! Whenever you attempt to solve a puzzle,\nthe day ends and you wake up again in your room.");
                }
                #endregion
                StateManager.Instance.ModifyStates( interactingObject.interaction.dropper.effects );
            }
        }

        /// <summary>
        /// Pickup up an object you are interacting with
        /// </summary>
        /// <param name="interactingObject"></param>
        private void PickupObject(GameObject interactingObject)
        {
            LevelManager.Instance.PickedObject = (Sprite)interactingObject.sprite.Clone();
            LevelManager.Instance.PickedObject.textureName = "pick_" + LevelManager.Instance.PickedObject.textureName;
            LevelManager.Instance.PickedObjectState = interactingObject.interaction.pickUpName + "_picked";
        }

        /// <summary>
        /// Updates the animation frames of player and animated game objects
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateAnimations(GameTime gameTime)
        {
            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            player.Sprite.UpdateFrame(elapsedSeconds);

            // update object animations
            foreach (AnimatedSprite a in animatedObjects)
                a.UpdateFrame(elapsedSeconds);
        }

        private void ShowTutorial( string message )
        {
            // only show tutorials if enabled
            if (SettingsManager.TutorialsEnabled)
            {
                MessageBoxScreen tutorialMessageBox = new MessageBoxScreen("Tutorial: " + message, false, true);
                ScreenManager.AddScreen(tutorialMessageBox);
            }
        }
        #endregion
    }
}
