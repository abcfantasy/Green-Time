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
        #region Fields

        // The name of the texture
        [ContentSerializer(ElementName = "texture")]
        public string textureName;

        // The actual texture that the sprite will use
        [ContentSerializerIgnore]
        public Texture2D texture;

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
        #endregion

        #region Initialization
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
