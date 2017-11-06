using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    class Dimentions : Component
    {
        public Rectangle Area { get; set; }

        public Dimentions(Rectangle area)
        {
            Area = area;
        }

        public Dimentions(int x, int y, int width, int height)
        {
            Area = new Rectangle(x, y, width, height);
        }

        public override void OnStart()
        {
            if (GetComponent<Transform>() == null)
                throw new InvalidOperationException("Dimentions require a Transform.");
        }
    }
}