using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GreenTimeGameData.Components
{
    public class Player
    {
        #region Constants
        // The transition speed (in milliseconds) between grey/colored or round/square
        private const float TRANSITION_SPEED = 1500.0f;
        private static readonly Vector2 PLAYER_MOUTH = new Vector2(90, 30);
        #endregion

        #region Fields
        // The two sprites (round/square) which swap with one another
        // We don't need to know which one is which
        private AnimatedSprite current_sprite;
        private AnimatedSprite opposite_sprite;

        // Variables related to the transitions (green/grey, round/square)
        private float colorState = 1.0f;
        private float shapeState = 1.0f;
        private float colorTransition = 0.0f;
        private float shapeTransition = 0.0f;
        #endregion

        #region Properties
        // Returns the current sprite (the caller doesn't need to know which one it is)
        public AnimatedSprite Sprite { get { return current_sprite; } }

        // Returns the current position
        public Vector2 Position { get { return current_sprite.position; } set { current_sprite.position = value; } }

        // Returns the mouth position (for chat)
        public Vector2 Mouth { get { return current_sprite.position + PLAYER_MOUTH; } }

        // Returns true if the player is ready for interaction (not transitioning)
        public bool IsReady { get { return (colorTransition == 0.0f && shapeTransition == 0.0f); } }
        #endregion

        #region Initialization
        public Player(ContentManager content)
        {
            // Loading the player from the XML
            current_sprite = content.Load<AnimatedSprite>("player");

            // We need a shallow copy instance to handle the square player texture
            opposite_sprite = (AnimatedSprite)current_sprite.Clone();            
            opposite_sprite.textureName += "_square";
            
            // Loading the content
            current_sprite.Load(content);
            opposite_sprite.Load(content);

            // Because we made only a shallow copy, this method will affect the square player too
            current_sprite.AddAllAnimations();            
        }
        #endregion

        #region Update and Draw
        // Takes care of the transitioning
        public void Update(GameTime gameTime)
        {
            if (colorTransition != 0.0f)
            {
                colorState += colorTransition * (gameTime.ElapsedGameTime.Milliseconds / TRANSITION_SPEED);

                // Check to see if we've reached the maximum / minimum saturation
                if ((colorTransition < 0.0f && colorState <= 0.0f)
                    || (colorTransition > 0.0f && colorState >= 1.0f))
                {
                    colorState = MathHelper.Clamp(colorState, 0.0f, 1.0f);
                    colorTransition = 0.0f;
                }
            }

            if (shapeTransition != 0.0f)
            {
                shapeState += shapeTransition * (gameTime.ElapsedGameTime.Milliseconds / TRANSITION_SPEED);

                // Check to see if the transition is done
                if (shapeState <= 0.0f)
                {
                    // We switch the opacity back to 1, but we swap the sprites so we only draw one at a time
                    shapeState = 1.0f;
                    shapeTransition = 0.0f;
                    swap();
                }
            }
        }

        // Draws the currently active sprite
        public void Draw(SpriteBatch spriteBatch)
        {
            current_sprite.Draw(spriteBatch, new Color((byte)(colorState * 64.0f), 255, 255, (byte)(shapeState * 255.0f)));

            // We only draw the second sprite if we're transitioning between the two
            if (shapeTransition != 0)
                opposite_sprite.Draw(spriteBatch, new Color((byte)(colorState * 64.0f), 255, 255, (byte)((1 - shapeState) * 255.0f)));
        }
        #endregion

        #region Public Methods
        // Swaps between the round and square sprite
        public void swap()
        {           
            AnimatedSprite aux  = current_sprite;
            current_sprite      = opposite_sprite;
            opposite_sprite     = aux;
        }

        // Transformation methods
        public void turnGreen() { colorTransition = 1.0f; }
        public void turnGrey()  { colorTransition = -1.0f; }
        public void transformShape()
        {
            // Synchronizing the sprites
            opposite_sprite.Flipped = current_sprite.Flipped;
            opposite_sprite.position.X = current_sprite.position.X;
            shapeTransition = -1.0f; 
        }

        // Convenience methods
        public void faceLeft()              { current_sprite.Flipped = true; }
        public void faceRight()             { current_sprite.Flipped = false; }
        public void move(float amount) {
            current_sprite.position.X += amount;
            if (amount > 0)         faceRight();
            else if (amount < 0)    faceLeft();
        }
        public void moveTo(float position)  { current_sprite.position.X = position; }
        public void walk()                  { current_sprite.Play("walk"); }
        public void idle()                  { current_sprite.Play(FrameSet.IDLE); }
        #endregion
    }
}
