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
    class Camera : Component, IDisposable
    {
        public Transform Trans { get; private set; }
        public float Zoom { get; private set; }

        // Height and width of the viewport window which we need to adjust
        // any time the player resizes the game window. Not handled, since
        // resizing game window is disabled.
        public int ViewportWidth { get; private set; }
        public int ViewportHeight { get; private set; }
        
        public override void OnStart()
        {
            SceneManager.TransformMatrixGenerator += TranslationMatrix;
            Zoom = 1.0f;

            ViewportWidth = SceneManager.ViewportRectangle.Width;
            ViewportHeight = SceneManager.ViewportRectangle.Height;

            Trans = GetComponent<Transform>();

            if (Trans == null)
                throw new InvalidOperationException("Camera requires a Transform.");
        }
        
        // Center of the Viewport which does not account for scale
        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }

        // Create a matrix for the camera to offset everything we draw,
        // the map and our objects. since the camera coordinates are where
        // the camera is, we offset everything by the negative of that to simulate
        // a camera moving. We also cast to integers to avoid filtering artifacts.
        public Matrix TranslationMatrix()
        {
            return Matrix.CreateTranslation(-(int)Trans.Position.X, -(int)Trans.Position.Y, 0) *
                Matrix.CreateRotationZ(Trans.Rotation) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
        }

        // Call this method with negative values to zoom out
        // or positive values to zoom in. It looks at the current zoom
        // and adjusts it by the specified amount. If we were at a 1.0f
        // zoom level and specified -0.5f amount it would leave us with
        // 1.0f - 0.5f = 0.5f so everything would be drawn at half size.
        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom < 0.25f)
            {
                Zoom = 0.25f;
            }
        }
        
        public void Dispose()
        {
            SceneManager.TransformMatrixGenerator -= TranslationMatrix;
        }
    }
}