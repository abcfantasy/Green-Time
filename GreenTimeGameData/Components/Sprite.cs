﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTimeGameData.Components
{
    public class Sprite : ICloneable
    {
        #region Fields
        // Whather or not to flip the sprite
        //private bool flipped = false;

        // The name of the texture
        [ContentSerializer(ElementName = "texture")]
        public string textureName;

        [ContentSerializer(Optional = true)]
        public Rectangle textureRect;

        // The actual texture that the sprite will use
        //[ContentSerializerIgnore]
        //public Texture2D texture;

        // Position in the scene
        public Vector2 position;

        // The sprite's layer depth
        public float layer;
        
        // Whether or not the object is affected by the desaturation shader
        [ContentSerializer(Optional = true)]
        public bool shaded = true;

        // The sprite's scale factor
        [ContentSerializer(Optional = true )]
        public float scale = 1.0f;

        // The sprite direction
        [ContentSerializer(Optional = true)]
        public bool flipped = false;

        // If the sprite can be flipped
        [ContentSerializer(Optional = true)]
        public bool flippable = true;
        #endregion

        #region Initialization
        public virtual void Load()
        {
            //texture = content.Load<Texture2D>(textureName);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, Color tint)
        {
            //spriteBatch.Draw(texture, position, texture.Bounds, tint, 0.0f, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer);
        }

        public virtual void Draw(Texture2D texture, SpriteBatch spriteBatch, Rectangle textureRect, Color tint)
        {
            spriteBatch.Draw(texture, position, textureRect, tint, 0.0f, Vector2.Zero, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layer);
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
