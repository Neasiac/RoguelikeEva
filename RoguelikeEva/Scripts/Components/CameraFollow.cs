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
        Vector2 Offset;

        public CameraFollow(Camera camera, Vector2 offset)
        {
            Camera = camera ?? throw new ArgumentNullException();
            Offset = offset;
        }
        
        public override void OnStart()
        {
            Trans = GetComponent<Transform>();

            if (Trans == null)
                throw new InvalidOperationException("CameraFollow requires a Camera.");
        }

        public override void Update(GameTime gameTime)
        {
            Trans.Position = Offset + Camera.Trans.Position - new Vector2(Camera.ViewportWidth / 2, Camera.ViewportHeight / 2);
        }
    }
}