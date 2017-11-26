using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    class Hoverable : MouseInteractable
    {
        public event Action EnterCallback;
        public event Action LeaveCallback;
        bool Entered;

        public Hoverable(Action enterCallback = null, Action leaveCallback = null)
        {
            if (enterCallback != null)
                EnterCallback += enterCallback;

            if (leaveCallback != null)
                LeaveCallback += leaveCallback;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (MouseOver())
            {
                if (!Entered && EnterCallback != null)
                    EnterCallback();

                Entered = true;
            }
            else
            {
                if (Entered && LeaveCallback != null)
                    LeaveCallback();

                Entered = false;
            }
        }
    }
}