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

namespace Vegricht.RoguelikeEva
{
    class Map
    {
        public static readonly int Size = 48;
        public MapNode[,] Grid;
        
        [Flags]
        public enum LinkedNeighbors : byte
        {
            North = 1 << 0,
            East = 1 << 1,
            South = 1 << 2,
            West = 1 << 3
        }

        public MapNode this[int x, int y]
        {
            get
            {
                return Grid[x, y];
            }
        }

        public Map(GameObject[,] grid)
        {
            Grid = new MapNode[grid.GetLength(0), grid.GetLength(1)];

            for (int x = 0; x < 30; x++)
                for (int y = 0; y < 20; y++)
                    Grid[x, y] = grid[x, y].GetComponent<MapNode>();
        }

        public Map(MapNode[,] grid)
        {
            Grid = grid;
        }

        public void SetNeighbors()
        {
            if (Grid == null)
                throw new InvalidOperationException("MapNode::Map has not been initialized.");

            for (int x = 0; x < 30; x++)
                for (int y = 0; y < 20; y++)
                {
                    MapNode node = Grid[x, y];

                    HandleNeighbor(node, node.X, node.Y - 1, LinkedNeighbors.North);
                    HandleNeighbor(node, node.X + 1, node.Y, LinkedNeighbors.East);
                    HandleNeighbor(node, node.X, node.Y + 1, LinkedNeighbors.South);
                    HandleNeighbor(node, node.X - 1, node.Y, LinkedNeighbors.West);
                }
        }

        public void SetTileGraphics(Texture2D tiles)
        {
            for (int x = 0; x < 30; x++)
                for (int y = 0; y < 20; y++)
                {
                    if (!Grid[x, y].Passable)
                        continue;

                    SpriteRenderer renderer = Grid[x, y].GetComponent<SpriteRenderer>();
                    Grid[x, y].GetComponent<Transform>().Scale = Vector2.One;
                    renderer.Sprite = tiles;
                    bool configurationFound = false;

                    LinkedNeighbors[] configurations = new LinkedNeighbors[16]
                        {
                            LinkedNeighbors.East | LinkedNeighbors.South | LinkedNeighbors.West,
                            LinkedNeighbors.East | LinkedNeighbors.North | LinkedNeighbors.West,
                            LinkedNeighbors.South | LinkedNeighbors.North | LinkedNeighbors.West,
                            LinkedNeighbors.East | LinkedNeighbors.North | LinkedNeighbors.South,
                            LinkedNeighbors.East | LinkedNeighbors.South,
                            LinkedNeighbors.West | LinkedNeighbors.South,
                            LinkedNeighbors.North | LinkedNeighbors.East,
                            LinkedNeighbors.West | LinkedNeighbors.North,
                            LinkedNeighbors.South | LinkedNeighbors.North,
                            LinkedNeighbors.East | LinkedNeighbors.West,
                            LinkedNeighbors.North | LinkedNeighbors.East | LinkedNeighbors.South | LinkedNeighbors.West,
                            0,
                            LinkedNeighbors.South,
                            LinkedNeighbors.West,
                            LinkedNeighbors.East,
                            LinkedNeighbors.North
                        };

                    for (int xi = 0; xi < 4; xi++)
                    {
                        for (int yi = 0; yi < 4; yi++)
                            if (Grid[x, y].Linked == configurations[xi + yi * 4])
                            {
                                renderer.SourceRectangle = new Rectangle(Map.Size * xi, Map.Size * yi, Map.Size, Map.Size);
                                configurationFound = true;
                                break;
                            }

                        if (configurationFound)
                            break;
                    }

                    if (!configurationFound)
                        throw new InvalidOperationException("We went through every single configuration, so this should never occur.");
                }
        }

        void HandleNeighbor(MapNode node, int x, int y, LinkedNeighbors relativePosition)
        {
            // is candidate on map?
            if (x < 0 || x >= Grid.GetLength(0) || y < 0 || y >= Grid.GetLength(1))
                return;

            // is candidate passable?
            if (!Grid[x, y].Passable)
                return;

            // is candidate in the same room OR if we're both door, do we have the same door ID?
            if (Grid[x, y].RoomID != node.RoomID && (!node.IsDoor || node.DoorID != Grid[x, y].DoorID))
                return;

            // if we made it this far, everything's good!
            node.Linked |= relativePosition;
            node.Neighbors.Add(Grid[x, y]);
        }
    }
}