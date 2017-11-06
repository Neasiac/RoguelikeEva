using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    class MapPanner : Component
    {
        Camera Camera;
        Point LastMousePosition;
        bool Panning;

        float MapWidth = 30;
        float MapHeight = 20;

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
            var cameraMax = new Vector2(MapWidth * MapNode.Size - (Camera.ViewportWidth / Camera.Zoom / 2),
                                        MapHeight * MapNode.Size - (Camera.ViewportHeight / Camera.Zoom / 2));

            return Vector2.Clamp(
                position,
                new Vector2(Camera.ViewportWidth / Camera.Zoom / 2, Camera.ViewportHeight / Camera.Zoom / 2),
                cameraMax);
        }
    }
}