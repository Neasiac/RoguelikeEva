using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Level
{
    class Map
    {
        public static readonly int Size = 48;
        public MapNode[,] Grid { get; private set; }
        public IEnumerable<Room> Rooms { get; private set; }
        public Room StartRoom { get; private set; }

        public Texture2D TileGraphics { get; private set; }
        public Texture2D DarkTilesGraphics { get; private set; }
        public Texture2D WallGraphics { get; private set; }
        
        public MapNode this[int x, int y]
        {
            get
            {
                return Grid[x, y];
            }
        }

        public int Width
        {
            get
            {
                return Grid.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return Grid.GetLength(1);
            }
        }

        public Map(GameObject[,] grid, IEnumerable<Room> rooms)
        {
            Grid = new MapNode[grid.GetLength(0), grid.GetLength(1)];
            Rooms = rooms;

            for (int x = 0; x < Grid.GetLength(0); x++)
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    Grid[x, y] = grid[x, y].GetComponent<MapNode>();
                    Grid[x, y].Room.Nodes.Add(Grid[x, y]);
                }
        }

        public Map(MapNode[,] grid, IEnumerable<Room> rooms)
        {
            Grid = grid;
            Rooms = rooms;

            for (int x = 0; x < Grid.GetLength(0); x++)
                for (int y = 0; y < Grid.GetLength(1); y++)
                    Grid[x, y].Room.Nodes.Add(Grid[x, y]);
        }

        public void SetupRooms(int startRoomId)
        {
            foreach (Room room in Rooms)
            {
                if (room.ID == startRoomId)
                    StartRoom = room;

                room.Map = this;
            }
        }

        public void SetNeighbors()
        {
            for (int x = 0; x < Grid.GetLength(0); x++)
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    MapNode node = Grid[x, y];

                    HandleNeighbor(node, node.X, node.Y - 1, Room.LinkedNeighbors.North);
                    HandleNeighbor(node, node.X + 1, node.Y, Room.LinkedNeighbors.East);
                    HandleNeighbor(node, node.X, node.Y + 1, Room.LinkedNeighbors.South);
                    HandleNeighbor(node, node.X - 1, node.Y, Room.LinkedNeighbors.West);
                }
        }

        public void SetTileGraphics(Texture2D tiles, Texture2D tilesDark, Texture2D wall)
        {
            TileGraphics = tiles;
            DarkTilesGraphics = tilesDark;
            WallGraphics = wall;

            foreach (Room room in Rooms)
                room.UpdateGraphics(Room.Visibility.Undiscovered);
        }

        void HandleNeighbor(MapNode node, int x, int y, Room.LinkedNeighbors relativePosition)
        {
            // is candidate on map?
            if (x < 0 || x >= Grid.GetLength(0) || y < 0 || y >= Grid.GetLength(1))
                return;

            // is candidate passable?
            if (!Grid[x, y].Passable)
                return;

            // is candidate in the same room OR if we're both door, do we have the same door ID?
            if (Grid[x, y].Room.ID != node.Room.ID && (!node.IsDoor || node.DoorID != Grid[x, y].DoorID))
                return;

            // if we made it this far, everything's good!
            node.Linked |= relativePosition;
            node.Neighbors.Add(Grid[x, y]);
        }
    }
}