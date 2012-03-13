﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GreenTime.GameObjects
{
    public class BaseObject
    {
        #region Fields
        protected Texture2D texture;
        protected Vector2 position;
        protected bool shaded;
        protected float layer;
        protected float scale;
        #endregion

        #region Properties
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        
        public float X
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }

        public int Width
        {
            get
            {
                return texture.Width;
            }
        }

        public bool Shaded { get { return shaded; } set { shaded = value; } }
        public float Layer { get { return layer; } set { layer = value; } }
        public float Scale { get { return scale; } set { scale = value; } }
        #endregion

        #region Constructor
        public BaseObject( Vector2 position, bool shaded, float layer, float scale )
        {
            this.position = position;
            this.shaded = shaded;
            this.layer = layer;
            this.scale = scale;
        }
        #endregion

        #region Public Methods
        public virtual void Load(ContentManager content, string asset)
        {
            this.texture = content.Load<Texture2D>(asset);
        }
        #endregion

        #region Draw
        public virtual void Draw(SpriteBatch spriteBatch, Color tint)
        {         
            Draw(spriteBatch, tint, 1.0f);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color tint, float scale)
        {
            spriteBatch.Draw(texture, position, texture.Bounds, tint, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layer); 
        }
        #endregion
    }
}
