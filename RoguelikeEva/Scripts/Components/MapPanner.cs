using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Level;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    class MapPanner : Component
    {
        public Map Map { get; set; }

        Camera Camera;
        Point LastMousePosition;
        bool Panning;
        
        public MapPanner(Camera camera)
        {
            Camera = camera;
        }

        public override void OnStart()
        {
            Camera.Trans.Position = MapClampedPosition(Vector2.Zero);
        }

        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (!Panning)
                    Panning = true;
                else
                {
                    Point diffPoint = LastMousePosition - Mouse.GetState().Position;
                    Vector2 diffVector2 = new Vector2(diffPoint.X, diffPoint.Y);

                    Camera.Trans.Position = MapClampedPosition(Camera.Trans.Position + diffVector2);
                }

                LastMousePosition = Mouse.GetState().Position;
            }
            else
                Panning = false;
        }
        
        Vector2 MapClampedPosition(Vector2 position)
        {
            var cameraMax = new Vector2(Map.Width * Map.Size - (Camera.ViewportWidth / 2),
                                        Map.Height * Map.Size - (Camera.ViewportHeight / 2));

            return Vector2.Clamp(
                position,
                new Vector2(Camera.ViewportWidth / 2, Camera.ViewportHeight / 2),
                cameraMax);
        }
    }
}