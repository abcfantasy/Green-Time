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
        private const float PLAYER_POSITION_Y = 250.0f;

        AnimatedSprite round_sprite;
        AnimatedSprite square_sprite;
        AnimatedSprite current_sprite;

        public Player(ContentManager content)
        {
            round_sprite = new AnimatedSprite();
            round_sprite.textureName = @"animations\AnimationRoundGreen";
            round_sprite.frameSize = new Vector2(110, 326);
            round_sprite.framesPerSecond = 15;
            round_sprite.layer = 0.5f;
            round_sprite.scale = 1.2f;
            round_sprite.Load(content);
            round_sprite.AddAnimation("walk", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            round_sprite.AddAnimation("idle", new int[] { 3 });
            round_sprite.PlayAnimation("idle");
            round_sprite.position = new Vector2(0, PLAYER_POSITION_Y);

            square_sprite = new AnimatedSprite();
            square_sprite.textureName = @"animations\AnimationSquareGreen";
            square_sprite.frameSize = new Vector2(110, 326);
            square_sprite.framesPerSecond = 15;
            square_sprite.layer = 0.5f;
            square_sprite.scale = 1.2f;
            square_sprite.Load(content);
            square_sprite.AddAnimation("walk", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            square_sprite.AddAnimation("idle", new int[] { 3 });
            square_sprite.PlayAnimation("idle");
            square_sprite.position = new Vector2(0, PLAYER_POSITION_Y);

            current_sprite = round_sprite;
        }

        public AnimatedSprite Sprite
        {
            get { return current_sprite; }
        }

        public void swap()
        {
            if (current_sprite == round_sprite) current_sprite = square_sprite;
            else current_sprite = round_sprite;
        }

        public Vector2 Position
        {
            get { return current_sprite.position; }
            set { round_sprite.position = value; square_sprite.position = value; }
        }

        public void Draw(SpriteBatch spriteBatch, Color tint)
        {
            round_sprite.Draw(spriteBatch, tint);
            //square_sprite.Draw(spriteBatch, tint);
        }

        public void faceLeft()
        {
            round_sprite.Flipped = true;
            square_sprite.Flipped = true;
        }

        public void faceRight()
        {
            round_sprite.Flipped = false;
            square_sprite.Flipped = false;
        }

        public void move(float amount)
        {
            round_sprite.position.X += amount;
            square_sprite.position.X += amount;
        }

        public void moveTo(float position)
        {
            round_sprite.position.X = position;
            square_sprite.position.X = position;
        }
    }
}
