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
        public static MapNode[,] Map;
        public static readonly int Size = 48;

        public IEnumerable<MapNode> Neighbors { get; private set; }
        public GameObject OccupiedBy { get; set; }
        public int RoomID { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Passable
        {
            get
            {
                return RoomID == 1;
            }
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2(X * Size, Y * Size);
            }
        }

        public MapNode(int x, int y, int roomID)
        {
            RoomID = roomID;
            X = x;
            Y = y;
        }

        public override void OnStart()
        {
            if (Map == null)
                throw new InvalidOperationException("MapNode::Map has not been initialized.");

            List<MapNode> neighbors = new List<MapNode>(4);
            MapNode candidate;

            candidate = GetNeighbor(X, Y - 1);
            if (candidate != null) neighbors.Add(candidate);

            candidate = GetNeighbor(X + 1, Y);
            if (candidate != null) neighbors.Add(candidate);

            candidate = GetNeighbor(X, Y + 1);
            if (candidate != null) neighbors.Add(candidate);

            candidate = GetNeighbor(X - 1, Y);
            if (candidate != null) neighbors.Add(candidate);

            Neighbors = neighbors;
        }

        MapNode GetNeighbor(int x, int y)
        {
            if (x < 0 || x >= Map.GetLength(0) || y < 0 || y >= Map.GetLength(1))
                return null;

            if (!Map[x, y].Passable)
                return null;

            return Map[x, y];
        }
    }
}