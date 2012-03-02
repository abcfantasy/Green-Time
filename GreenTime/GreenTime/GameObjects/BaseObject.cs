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
        #endregion

        #region Constructor
        public BaseObject( Vector2 position )
        {
            this.position = position;            
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
            spriteBatch.Draw(texture, position, tint);
        }
        #endregion
    }
}
