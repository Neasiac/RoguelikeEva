using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components
{
    class CameraFollow : Component
    {
        Transform Trans;
        Camera Camera;

        public CameraFollow(Camera camera)
        {
            Camera = camera ?? throw new ArgumentNullException();
        }
        
        public override void OnStart()
        {
            Trans = GetComponent<Transform>();

            if (Trans == null)
                throw new InvalidOperationException("CameraFollow requires a Camera.");
        }

        public override void Update(GameTime gameTime)
        {
            Trans.Position = Camera.Trans.Position - new Vector2(Camera.ViewportWidth / 2, Camera.ViewportHeight / 2);
        }
    }
}