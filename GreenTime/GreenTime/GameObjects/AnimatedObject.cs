using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GreenTimeGameData.Components;
using GreenTime.Managers;

namespace GreenTime.GameObjects
{
    public class AnimatedObject : BaseObject
    {
        private Dictionary<string, int[]> animations;
        private double timePerFrame;
        private bool paused;
        private int frameWidth;
        private int frameHeight;

        private double totalElapsed;
        private string currentAnimation;
        private int currentFrameIndex;
        private Rectangle currentFrameRectangle = new Rectangle();
        private bool flipped;

        #region Properties
        public bool IsStopped
        {
            get { return paused; }
        }

        public bool Flipped
        {
            get { return flipped; }
            set { flipped = value; }
        }

        public int CurrentFrame
        {
            get
            {
                return animations[currentAnimation][currentFrameIndex];
            }
        }

        public int FrameWidth
        {
            get
            {
                return frameWidth;
            }
        }
        #endregion

        #region Constructor
        public AnimatedObject(Vector2 position, int frameWidth, int frameHeight, int framesPerSecond, bool shaded, float layer)
            : base(position, shaded, layer)
        {
            this.animations = new Dictionary<string, int[]>();
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.currentFrameRectangle.Width = frameWidth;
            this.currentFrameRectangle.Height = frameHeight;
            this.timePerFrame = (double)1 / framesPerSecond;
            this.paused = false;
            this.flipped = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an animation definition to the object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="frames"></param>
        public void AddAnimation(string name, int[] frames)
        {
            currentAnimation = name;    // set this by default
            animations.Add(name, frames);
        }

        /// <summary>
        /// Adds a set of animations to the object
        /// </summary>
        /// <param name="animations"></param>
        public void AddAnimations( AnimationPlayback[] animations)
        {
            for (int i = 0; i < animations.Length; i++)
            {
                if ( StateManager.Current.DependentStatesSatisfied( animations[i].DependentStates ) )
                    AddAnimation(animations[i].Name, animations[i].Frames);
            }
        }

        /// <summary>
        /// Checks if the given animation is currently playing
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CurrentlyPlaying(string name)
        {
            return currentAnimation == name && !paused;
        }

        /// <summary>
        /// Play a particular animation
        /// </summary>
        /// <param name="name"></param>
        public void PlayAnimation(string name)
        {
            currentAnimation = name;
            paused = false;
            Reset();
        }

        /// <summary>
        /// Resets the animation
        /// </summary>
        public void Reset()
        {
            //currentFrameIndex = animations[currentAnimation][0];
            currentFrameIndex = 0;
            totalElapsed = 0;
            Resume();
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public void Stop()
        {
            paused = true;
            Reset();
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void Resume()
        {
            paused = false;
        }

        /// <summary>
        /// Pause animation
        /// </summary>
        public void Pause()
        {
            paused = true;
        }
        #endregion

        #region Update and Draw
        public void UpdateFrame( double elapsed )
        {
            if (this.paused)
                return;

            this.totalElapsed += elapsed;
            if (totalElapsed > timePerFrame)
            {
                // next frame
                currentFrameIndex++;

                // loop animation if at the end
                if (currentFrameIndex >= animations[currentAnimation].Length)
                    currentFrameIndex = 0;

                // update source texture rectangle for drawing
                UpdateTextureRectangle();

                totalElapsed -= timePerFrame;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color tint)
        {
            spriteBatch.Draw(texture, position, currentFrameRectangle, tint, 0.0f, Vector2.Zero, 1.0f, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer );
        }

        public void Draw(SpriteBatch spriteBatch, Color tint, float scale )
        {
            spriteBatch.Draw(texture, position, currentFrameRectangle, tint, 0.0f, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer );
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the rectangle to point at the correct frame on the texture
        /// </summary>
        private void UpdateTextureRectangle()
        {
            int frame = animations[currentAnimation][currentFrameIndex];

            this.currentFrameRectangle.X = (frame * frameWidth) % texture.Width;
            this.currentFrameRectangle.Y = (int)Math.Floor(frame * frameWidth / (double)texture.Width ) * frameHeight;
        }
        #endregion
    }
}
