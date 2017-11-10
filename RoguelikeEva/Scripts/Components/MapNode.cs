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
    class MapNode : Component
    {
        public GameObject OccupiedBy { get; set; }
        public List<MapNode> Neighbors { get; private set; }
        public Map.LinkedNeighbors Linked { get; set; }
        public byte RoomID { get; private set; }
        public byte DoorID { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Passable
        {
            get
            {
                return RoomID > 0;
            }
        }

        public bool IsDoor
        {
            get
            {
                return DoorID > 0;
            }
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2(X * Map.Size, Y * Map.Size);
            }
        }

        public MapNode(int x, int y, int code)
        {
            Neighbors = new List<MapNode>(4);
            RoomID = (byte)(code & 0x00FF);
            DoorID = (byte)((code & 0xFF00) >> 4);
            X = x;
            Y = y;
        }
    }
}