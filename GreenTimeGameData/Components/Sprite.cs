using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTimeGameData.Components
{
    public class Sprite
    {
        #region Properties

        [ContentSerializerIgnore]
        public Texture2D texture;

        /// <summary>
        /// the texture name of the sprite
        /// </summary>
        [ContentSerializer(ElementName = "texture")]
        public string textureName;

        /// <summary>
        /// the x position
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// the sprite's layer depth
        /// </summary>        
        public float layer;
        
        /// <summary>
        /// whether or not the object is affected by the desaturation shader
        /// </summary>
        [ContentSerializer(Optional = true)]
        public bool shaded = true;

        /// <summary>
        /// the sprite's scale factor
        /// </summary>
        [ContentSerializer(Optional = true )]
        public float scale = 1.0f;
        #endregion

        #region Public Methods
        public virtual void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>(textureName);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, Color tint)
        {
            spriteBatch.Draw(texture, position, texture.Bounds, tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layer);
        }
        #endregion
    }
}
