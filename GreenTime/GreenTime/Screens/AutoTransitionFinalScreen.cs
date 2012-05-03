using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenTime.Managers;
using GreenTimeGameData.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTime.Screens
{
    class AutoTransitionFinalScreen : GameScreen
    {
        #region Fields
        public static readonly float TEXT_LAYER = 0.0f;
        public static readonly float PLAYER_LAYER = 0.5f;
        public static readonly float BACKGROUND_LAYER = 0.75f;

        private List<Sprite> visibleObjects = new List<Sprite>();
        private List<AnimatedSprite> animatedObjects = new List<AnimatedSprite>();
        private List<GameObject> activeObjects = new List<GameObject>();

        float timeInEachScreen = 2.0f;
        float elapsedTime = 0.0f;

        private Calendar calendar = null;
        #endregion

        public AutoTransitionFinalScreen()
        {
            // set transition times
            TransitionOnTime = TimeSpan.FromSeconds(2.5f);
            TransitionOffTime = TimeSpan.FromSeconds(2.5f);

            // play music here
            //SoundManager.PlayMusic();
        }

        public override void LoadContent()
        {
            LevelManager.Instance.LoadAllLevels();
            ResourceManager.Instance.LoadGameplayTexture();
            //ResourceManager.Instance.LoadSong(SoundManager.SONG_INTRO);
            ResourceManager.Instance.LoadLevelTexture(LevelManager.Instance.CurrentLevel.texture);

            //SoundManager.PlayMusic();

            LoadGameObjects();

            ScreenManager.Game.ResetElapsedTime();
        }

        public void LoadGameObjects()
        {
            visibleObjects.Clear();
            animatedObjects.Clear();
            activeObjects.Clear();

            // Load ambient soud if any
            if (LevelManager.Instance.CurrentLevel.ambientSound != null)
            {
                if (StateManager.Instance.CheckDependencies(LevelManager.Instance.CurrentLevel.ambientSound.dependencies))
                {
                    ResourceManager.Instance.LoadSound(LevelManager.Instance.CurrentLevel.ambientSound.name, true);
                    SoundManager.PlayAmbientSound();
                }
            }
            // Load the background
            visibleObjects.Add(LevelManager.Instance.CurrentLevel.backgroundTexture);

            // Load the objects
            foreach (GameObject io in LevelManager.Instance.CurrentLevel.gameObjects)
            {
                if (StateManager.Instance.CheckDependencies(io.dependencies))
                {
                    if (io.interaction != null)
                    {
                        activeObjects.Add(io);
                    }
                    if (io.sprite != null)
                    {
                        if (io.sprite.GetType() == typeof(Calendar))
                        {
                            calendar = (Calendar)io.sprite;
                            calendar.Initialize(StateManager.Instance.GetCurrentGameDate(), ScreenManager.Font);
                        }
                        else
                        {
                            io.sprite.Load();
                            visibleObjects.Add(io.sprite);

                            if (io.sprite.GetType() == typeof(AnimatedSprite))
                            {
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
        }

        public override void UnloadContent()
        {
            ResourceManager.Instance.UnloadLocalContent();

            SoundManager.UnloadLocal();
        }

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


            //SoundManager.UpdateFade(1.0f);

            if (IsActive)
            {
                if (TransitionAlpha == 1.0f)
                {
                    elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (elapsedTime > (timeInEachScreen * 1000.0f))
                        NextScreen();
                }
                // update animations
                UpdateAnimations(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // draw stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.BackToFront, null);

            // game objects
            foreach (Sprite s in visibleObjects)
            {
                s.Draw(ResourceManager.Instance.LevelTexture, spriteBatch, ResourceManager.Instance[s.textureName], Color.White );
            }
            
            if (calendar != null)
                calendar.Draw(ResourceManager.Instance.LevelTexture, spriteBatch, ResourceManager.Instance[calendar.textureName], Color.White);

            spriteBatch.End();

            // Handle the different transition types
             ScreenManager.FadeBackBufferToBlack(TransitionPosition);
        }
        #endregion

        private void NextScreen()
        {
            // is it final room?
            if (LevelManager.Instance.CurrentLevel.rightScreenName == "final_room")
            {
                // transition to the right
                LevelManager.Instance.MoveRight();
                LoadingScreen.Load(ScreenManager, false, new FinalScreen());
            }
            else
            {
                if (LevelManager.Instance.CurrentLevel.name == "bedroom")   // skip kitchen
                {
                    LevelManager.Instance.MoveRight();
                    LevelManager.Instance.MoveRight();
                    LoadingScreen.Load(ScreenManager, false, new TextOnBlackScreen("But what about our world?", "We cannot travel back in time when it's too late.",
                        new GameScreen[] { new AutoTransitionFinalScreen() }, false, 4500, 0, false));
                }
                else
                {
                    // transition to the right
                    LevelManager.Instance.MoveRight();
                    LoadingScreen.Load(ScreenManager, false, new AutoTransitionFinalScreen());
                }
            }
        }

        /// <summary>
        /// Updates the animation frames of player and animated game objects
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateAnimations(GameTime gameTime)
        {
            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            // update object animations
            foreach (AnimatedSprite a in animatedObjects)
                a.UpdateFrame(elapsedSeconds);
        }
    }
}
