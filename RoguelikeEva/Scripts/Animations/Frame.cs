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
    /// Describes a single frame of an animation.
    /// </summary>
    struct Frame
    {
        public Texture2D Sprite
        {
            get;
            private set;
        }

        public int Duration
        {
            get;
            private set;
        }

        public Rectangle? SourceRectangle
        {
            get;
            private set;
        }

        public Frame(Texture2D sprite, int duration, Rectangle? sourceRectangle = null)
        {
            Sprite = sprite;
            Duration = duration;
            SourceRectangle = sourceRectangle;
        }
    }
}