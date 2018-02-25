using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    abstract class MouseInteractable : Component
    {
        protected Dimentions Dims;
        protected Transform Trans;
        
        public override void OnStart()
        {
            Dims = GetComponent<Dimentions>();
            Trans = GetComponent<Transform>(); // check for presence of Transform is hidden in Dimentions component

            if (Dims == null)
                throw new InvalidOperationException("MouseInteractable requires Dimentions.");
        }
        
        protected bool MouseOver()
        {
            Vector2 position = Vector2.Transform(Trans.Position, SceneManager.TransformMatrix);
            Rectangle area = new Rectangle((int)position.X + Dims.Area.X, (int)position.Y + Dims.Area.Y, Dims.Area.Width, Dims.Area.Height);

            return area.Contains(Mouse.GetState().Position) && SceneManager.ViewportRectangle.Contains(Mouse.GetState().Position);
        }
    }
}