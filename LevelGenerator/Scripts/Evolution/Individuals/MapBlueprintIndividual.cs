using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Individuals
{
    [Serializable]
    class MapBlueprintIndividual : Individual
    {
        public MapBlueprint Gene { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public double WallProbability { get; private set; }
        public Dictionary<byte, HashSet<Coords>> Rooms { get; set; }
        public SortedSet<byte> RoomsOrdered { get; set; }

        [Serializable]
        class RoomSizeComparer : IComparer<byte>
        {
            Dictionary<byte, HashSet<Coords>> Rooms;

            public RoomSizeComparer(Dictionary<byte, HashSet<Coords>> rooms)
            {
                Rooms = rooms;
            }

            public int Compare(byte x, byte y)
            {
                if (Rooms[x].Count < Rooms[y].Count)
                    return -1;

                if (Rooms[x].Count > Rooms[y].Count)
                    return 1;

                return x.CompareTo(y);
            }
        }

        public MapBlueprintIndividual(int width, int height, double wallProbability)
        {
            Width = width;
            Height = height;
            WallProbability = wallProbability;
        }

        public override void RandomInitialization()
        {
            Gene = new MapBlueprint();
            Gene.Encoding = new byte[Width, Height][];
            Rooms = new Dictionary<byte, HashSet<Coords>>();
            RoomsOrdered = new SortedSet<byte>(new RoomSizeComparer(Rooms));
            Random rnd = new Random();
            byte nextRoomId = 1;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Gene.Encoding[x, y] = new byte[3];

            for (int x = 0; x < Width - 1; x++)
                for (int y = 0; y < Height - 1; y++)
                {
                    if (rnd.NextDouble() < WallProbability)
                        continue;

                    if (Gene.IsOnMap(x - 1, y) && Gene.Encoding[x - 1, y][0] > 0)
                    {
                        Gene.Encoding[x, y][0] = Gene.Encoding[x - 1, y][0];
                        AddTileToRoom(Gene.Encoding[x, y][0], new Coords(x, y));
                    }

                    else if (Gene.IsOnMap(x + 1, y) && Gene.Encoding[x + 1, y][0] > 0)
                    {
                        Gene.Encoding[x, y][0] = Gene.Encoding[x + 1, y][0];
                        AddTileToRoom(Gene.Encoding[x, y][0], new Coords(x, y));
                    }

                    else if (Gene.IsOnMap(x, y - 1) && Gene.Encoding[x, y - 1][0] > 0)
                    {
                        Gene.Encoding[x, y][0] = Gene.Encoding[x, y - 1][0];
                        AddTileToRoom(Gene.Encoding[x, y][0], new Coords(x, y));
                    }

                    else if (Gene.IsOnMap(x, y + 1) && Gene.Encoding[x, y + 1][0] > 0)
                    {
                        Gene.Encoding[x, y][0] = Gene.Encoding[x, y + 1][0];
                        AddTileToRoom(Gene.Encoding[x, y][0], new Coords(x, y));
                    }

                    else
                    {
                        Gene.Encoding[x, y][0] = nextRoomId;
                        Rooms.Add(nextRoomId, new HashSet<Coords>());
                        Rooms[nextRoomId].Add(new Coords(x, y));
                        RoomsOrdered.Add(nextRoomId++);
                    }
                }
        }

        public void AddTileToRoom(byte id, Coords tile)
        {
            RoomsOrdered.Remove(id);
            Rooms[id].Add(tile);
            RoomsOrdered.Add(id);
        }

        public void RemoveTileFromRoom(byte id, Coords tile)
        {
            RoomsOrdered.Remove(id);
            Rooms[id].Remove(tile);
            RoomsOrdered.Add(id);
        }
    }
}