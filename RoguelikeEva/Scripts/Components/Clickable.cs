using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    class Clickable : Component
    {
        public event Action Callback;

        Dimentions Dims;
        Transform Trans;
        bool Clicked;

        public Clickable(Action callback = null)
        {
            if (callback != null)
                Callback += callback;
        }

        public override void OnStart()
        {
            Dims = GetComponent<Dimentions>();
            Trans = GetComponent<Transform>(); // check for presence of Transform is hidden in Dimentions component

            if (Dims == null)
                throw new InvalidOperationException("Clickable requires Dimentions.");
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

        bool MouseOver()
        {
            Vector2 position = Vector2.Transform(Trans.Position, SceneManager.TransformMatrix);
            Rectangle area = new Rectangle((int)position.X + Dims.Area.X, (int)position.Y + Dims.Area.Y, Dims.Area.Width, Dims.Area.Height);

            return area.Contains(Mouse.GetState().Position) && SceneManager.ViewportRectangle.Contains(Mouse.GetState().Position);
        }
    }
}