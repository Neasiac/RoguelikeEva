using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;

namespace Vegricht.RoguelikeEva.Components
{
    /// <summary>
    /// Holds information about position, velocity, and scale of the GameObject
    /// and handles movement described by these properties.
    /// </summary>
    class Transform : Component
    {
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        
        public Transform(Vector2 position)
        {
            Position = position;
            Scale = Vector2.One;
        }

        public Transform(Vector2 position, Vector2 scale)
        {
            Position = position;
            Scale = scale;
        }

        public Transform(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = Vector2.One;
        }

        public Transform(Vector2 position, Vector2 scale, float rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}