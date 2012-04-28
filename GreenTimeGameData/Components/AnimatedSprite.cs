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
        #region Fields
        // Dimensions of a single frame
        public Vector2 frameSize;

        // How fast to play
        public int framesPerSecond;

        // True to fade between frames
        [ContentSerializer(Optional = true)]
        public bool crossFade = false;

        // All the possible animations for an object (without dependency checking)
        [ContentSerializer(CollectionItemName = "frameSet")]
        public FrameSet[] animations;

        // Private variables are ignored by the serializer
        
        // Time per frame ( 1 / framesPerSecond )
        private double timePerFrame;

        // How far we are in the current animation
        private double totalElapsed;
        
        // The current animation that is playing
        private string currentFrameSet = FrameSet.IDLE;
        
        // The current frame in the animation
        private int currentFrameIndex = 0;
        
        // The current area of the texture that we are drawing
        private Rectangle currentFrameBounds;

        // The next area of the texture to draw
        private Rectangle nextFrameBounds;

        // Whather or not to flip the sprite
        //private bool flipped = false;

        // Whether or not to play animations
        private bool paused = false;

        // The percentage of fading between frames
        private float fadePercentage = 0.0f;

        // The list of active animations; they are the ones that satisfy the dependencies
        private Dictionary<string, int[]> activeAnimations = new Dictionary<string, int[]>();

        #endregion

        #region Properties

        // Getters and setters for things we want to access from outside
        [ContentSerializerIgnore]
        public bool IsStopped { get { return paused; } }

        //[ContentSerializerIgnore]
        //public bool Flipped { get { return flipped; } set { flipped = value; } }

        [ContentSerializerIgnore]
        public int CurrentFrame { get { return activeAnimations[currentFrameSet][currentFrameIndex]; } }

        [ContentSerializerIgnore]
        public Dictionary<string, int[]> ActiveAnimations { get { return activeAnimations; } }

        #endregion

        #region Initialization
        // Loads the texture and performs initializations
        override public void Load(ContentManager content)
        {
            base.Load(content);
            timePerFrame = 1.0d / framesPerSecond;
            this.currentFrameBounds.Width = (int)frameSize.X;
            this.currentFrameBounds.Height = (int)frameSize.Y;

            this.nextFrameBounds.Width = currentFrameBounds.Width;
            this.nextFrameBounds.Height = currentFrameBounds.Height;
        }
        #endregion

        #region Public Methods

        // Play an animation
        public void Play(string name)
        {
            if (IsPlaying(name)) return;
            currentFrameSet = name;
            paused = false;
            Reset();
        }
        
        // Reset the animation
        public void Reset()
        {
            currentFrameIndex = 0;
            totalElapsed = 0;

            /*UpdateTextureRectangle();*/
            Point currentPoint = GetTextureRectangleXY(currentFrameSet, currentFrameIndex);
            currentFrameBounds.X = currentPoint.X;
            currentFrameBounds.Y = currentPoint.Y;

            Point nextPoint = GetTextureRectangleXY(currentFrameSet, (currentFrameIndex + 1) % activeAnimations[currentFrameSet].Length);
            nextFrameBounds.X = nextPoint.X;
            nextFrameBounds.Y = nextPoint.Y;

            Resume();
        }

        // Convenience methods
        public void Pause()     { paused = true; }
        public void Resume()    { paused = false; }
        public void Stop()      { Reset(); Pause(); }
        public bool IsPlaying(string name) { return currentFrameSet == name && !paused; }

        // Add an animation to the active list
        public void AddAnimation(string name, int[] frames)
        {
            activeAnimations[name] = frames;
        }

        // Load all the available animations (without dependency checking)
        public void AddAllAnimations()
        {
            foreach (FrameSet fs in animations)            
                activeAnimations[fs.name] = fs.frames;            
        }

        #endregion

        #region Update and Draw
        public void UpdateFrame(double elapsed)
        {
            // Frames aren't updated while the animation is paused
            if (paused) return;

            // Early return if we're using a single-frame frameset
            if (activeAnimations[currentFrameSet].Length == 1) return;

            totalElapsed += elapsed;

            // calculate cross fading between frames
            fadePercentage = MathHelper.Clamp( (float)(totalElapsed / timePerFrame), 0.0f, 1.0f ) ;

            if (totalElapsed > timePerFrame)
            {
                // reset fading percentage
                fadePercentage = 0.0f;

                // Next frame (looping if at the end)
                currentFrameIndex = (currentFrameIndex + 1) % activeAnimations[currentFrameSet].Length;

                // Update source texture rectangle for drawing
                UpdateTextureRectangle();

                totalElapsed -= timePerFrame;
            }
        }

        private void UpdateTextureRectangle()
        {
            /*
            int frame = activeAnimations[currentFrameSet][currentFrameIndex];

            currentFrameBounds.X = (frame * (int)frameSize.X) % texture.Width;
            currentFrameBounds.Y = (int)Math.Floor(frame * frameSize.X / (double)texture.Width) * (int)frameSize.Y;
             */
            /*
            Point currentPoint = GetTextureRectangleXY(currentFrameSet, currentFrameIndex);
            currentFrameBounds.X = currentPoint.X;
            currentFrameBounds.Y = currentPoint.Y;
            */
            currentFrameBounds.X = nextFrameBounds.X;
            currentFrameBounds.Y = nextFrameBounds.Y;

            Point nextPoint = GetTextureRectangleXY(currentFrameSet, (currentFrameIndex + 1) % activeAnimations[currentFrameSet].Length);
            nextFrameBounds.X = nextPoint.X;
            nextFrameBounds.Y = nextPoint.Y;
        }

        // TEST
        private Point GetTextureRectangleXY(string frameSet, int frameIndex)
        {
            int frame = activeAnimations[frameSet][frameIndex];

            return new Point( (frame * (int)frameSize.X) % texture.Width, (int)Math.Floor(frame * frameSize.X / (double)texture.Width) * (int)frameSize.Y );
        }

        override public void Draw(SpriteBatch spriteBatch, Color tint)
        {
            if (crossFade)
            {
                spriteBatch.Draw(texture, position, currentFrameBounds, new Color(tint.R, tint.G, tint.B, (byte)((1 - fadePercentage) * 255.0f)), 0.0f, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer);
                spriteBatch.Draw(texture, position, nextFrameBounds, new Color(tint.R, tint.G, tint.B, (byte)(fadePercentage * 255.0f)), 0.0f, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer);
            }
            else
                spriteBatch.Draw(texture, position, currentFrameBounds, tint, 0.0f, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer);
        }
        #endregion

        #region ICloneable
        new public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
