using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Vegricht.RoguelikeEva.Animations
{
    /// <summary>
    /// Helps to create animations.
    /// </summary>
    class AnimationBuilder
    {
        Animation Animation;

        public AnimationBuilder()
        {
            Animation = new Animation();
        }

        public AnimationBuilder AddFrame(Texture2D sprite, int duration, Rectangle? sourceRectangle = null)
        {
            Animation.Frames.Add(new Frame(sprite, duration, sourceRectangle));
            return this;
        }

        public Animation Build()
        {
            return Animation;
        }
    }
}