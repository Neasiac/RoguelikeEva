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
    class Room
    {
        public Map Map { get; set; }
        public byte ID { get; private set; }
        public Visibility View { get; private set; }
        public List<MapNode> Nodes { get; private set; }
        
        public enum Visibility : byte
        {
            Visible,
            Darkened,
            Undiscovered
        }

        [Flags]
        public enum LinkedNeighbors : byte
        {
            North = 1 << 0,
            East = 1 << 1,
            South = 1 << 2,
            West = 1 << 3
        }

        public Room(byte id)
        {
            ID = id;
            Nodes = new List<MapNode>();
        }

        public void UpdateGraphics(Visibility view)
        {
            if (view == View)
                return;

            View = view;

            foreach (MapNode node in Nodes)
            {
                SpriteRenderer renderer = node.GetComponent<SpriteRenderer>();
               // node.GetComponent<Transform>().Scale = View != Visibility.Undiscovered ? Vector2.One : new Vector2(0.16f); // FIXME: united scale !
                bool configurationFound = false;

                switch (View)
                {
                    case Visibility.Visible:
                        renderer.Sprite = Map.TileGraphics;
                        break;

                    case Visibility.Darkened:
                        renderer.Sprite = Map.DarkTilesGraphics;
                        break;

                    case Visibility.Undiscovered:
                        renderer.Sprite = Map.WallGraphics;
                        return;
                }

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
                        if (node.Linked == configurations[xi + yi * 4])
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
    }
}