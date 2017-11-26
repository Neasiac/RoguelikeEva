using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    class Clickable : MouseInteractable
    {
        public event Action Callback;
        bool Clicked;

        public Clickable(Action callback = null)
        {
            if (callback != null)
                Callback += callback;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!Clicked && Callback != null && MouseOver())
                    Callback();

                Clicked = true;
            }
            else
                Clicked = false;
        }
    }
}