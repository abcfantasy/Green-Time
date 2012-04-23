using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GreenTimeGameData.Components
{
    public class Leaf
    {
        private static Random randomizer = new Random();

        float leafX = 0.0f;
        float leafY = -80.0f;
        float leafRotation = 0.0f;
        float leafScale = 1.0f;
        float leafVelocity = 1.0f;
        float leafRotationSpeed = 0.02f;
        bool leafRotationIncreasing = true;
        SpriteEffects effects;

        public SpriteEffects Effects { get { return effects; } }
        public bool Kind { get; set; }
        public float X { get { return leafX; } }
        public float Y { get { return leafY; } }
        public float Scale { get { return leafScale; } }
        public float Rotation { get { return leafRotation; } }

        public Leaf(  )
        {
            if ( randomizer.Next(0, 10 ) > 5 )
                leafRotationIncreasing = false;
            leafX = (float)randomizer.Next(80, 1200);
            leafScale = randomizer.Next(50, 100) / 100.0f;
            leafVelocity = randomizer.Next(80, 200) / 100.0f;
            leafRotationSpeed = randomizer.Next(20, 40) / 1000.0f;
            Kind = randomizer.Next(0, 10) > 5 ? true : false;
            effects = randomizer.Next(0, 10) > 5 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            //leafRotation = (float)randomizer.Next( -0.5, 0.5 );
        }

        public void Update()
        {
            leafY += leafVelocity;
            if (leafRotationIncreasing)
            {
                leafRotation += leafRotationSpeed;
                if (leafRotation >= 1.3f)
                    leafRotationIncreasing = false;
            }
            else
            {
                leafRotation -= leafRotationSpeed;
                if (leafRotation <= -1.0f)
                    leafRotationIncreasing = true;
            }
        }
    }
}
