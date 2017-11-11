using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Scenes.Core;
using Vegricht.RoguelikeEva.Level;

namespace Vegricht.RoguelikeEva.Components
{
    class MapNode : Component
    {
        public GameObject OccupiedBy { get; set; }
        public List<MapNode> Neighbors { get; private set; }
        public Room.LinkedNeighbors Linked { get; set; }
        public Room Room { get; private set; }
        public byte DoorID { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Passable
        {
            get
            {
                return Room.ID > 0;
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

        public MapNode(int x, int y, byte doorID, Room room)
        {
            Neighbors = new List<MapNode>(4);
            DoorID = doorID;
            Room = room;
            X = x;
            Y = y;
        }
    }
}