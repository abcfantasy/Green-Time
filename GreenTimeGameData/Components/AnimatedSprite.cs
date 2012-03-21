using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTimeGameData.Components
{
    public class AnimatedSprite : Sprite, ICloneable
    {
        public Vector2 frameSize;
        public int framesPerSecond;

        [ContentSerializer(CollectionItemName = "frameSet")]
        public FrameSet[] animations;

        private double timePerFrame;
        private double totalElapsed;
        private string currentFrameSet = "default";
        private int currentFrameIndex = 0;
        private Rectangle currentFrameBounds;
        private bool flipped = false;
        private bool paused = false;
        private Dictionary<string, int[]> activeAnimations = new Dictionary<string, int[]>();

        public bool IsStopped
        {
            get { return paused; }
        }

        [ContentSerializerIgnore]
        public bool Flipped
        {
            get { return flipped; }
            set { flipped = value; }
        }

        [ContentSerializerIgnore]
        public int CurrentFrame
        {
            get { return activeAnimations[currentFrameSet][currentFrameIndex]; }
        }

        [ContentSerializerIgnore]
        public Dictionary<string, int[]> ActiveAnimations
        {
            get { return activeAnimations; }
        }

        #region Public Methods
        override public void Load(ContentManager content)
        {
            base.Load(content);
            timePerFrame = 1.0d / framesPerSecond;
            this.currentFrameBounds.Width = (int)frameSize.X;
            this.currentFrameBounds.Height = (int)frameSize.Y;
        }

        /// <summary>
        /// Checks if the given animation is currently playing
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CurrentlyPlaying(string name)
        {
            return currentFrameSet == name && !paused;
        }

        /// <summary>
        /// Play a particular animation
        /// </summary>
        /// <param name="name"></param>
        public void PlayAnimation(string name)
        {
            if (CurrentlyPlaying(name))
                return;
            currentFrameSet = name;
            paused = false;
            Reset();
        }

        public void Reset()
        {
            currentFrameIndex = 0;
            totalElapsed = 0;
            UpdateTextureRectangle();
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
        public void UpdateFrame(double elapsed)
        {
            // Frames aren't updated while the animation is paused
            if (paused) return;

            // Early return if we're using an idle frameset
            if (activeAnimations[currentFrameSet].Length == 1) return;

            totalElapsed += elapsed;
            if (totalElapsed > timePerFrame)
            {
                // Next frame (looping if at the end)                
                currentFrameIndex = (currentFrameIndex + 1) % activeAnimations[currentFrameSet].Length;

                // Update source texture rectangle for drawing
                UpdateTextureRectangle();

                totalElapsed -= timePerFrame;
            }
        }

        override public void Draw(SpriteBatch spriteBatch, Color tint)
        {
            spriteBatch.Draw(texture, position, currentFrameBounds, tint, 0.0f, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer);
        }

        public void AddAnimation( string name, int[] frames )
        {
            activeAnimations[name] = frames;
        }

        public void AddAllAnimations()
        {
            foreach (FrameSet fs in animations)
            {
                activeAnimations[fs.name] = fs.frames;
            }
        }

        private void UpdateTextureRectangle()
        {
            int frame = activeAnimations[currentFrameSet][currentFrameIndex];

            currentFrameBounds.X = (frame * (int)frameSize.X) % texture.Width;
            currentFrameBounds.Y = (int)Math.Floor(frame * frameSize.X / (double)texture.Width) * (int)frameSize.Y;
        }        
        #endregion

        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
