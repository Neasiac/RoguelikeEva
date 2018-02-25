using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution;
using Vegricht.LevelGenerator.Evolution.Fitnesses;
using Vegricht.LevelGenerator.Evolution.Hypervolume;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.LevelGenerator.Evolution.Operators;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Phase2();
        }

        static void Phase2()
        {
            EvolutionaryAlgorithm eva = new EvolutionaryAlgorithm();
            MapBlueprintIndividual input = (MapBlueprintIndividual)Individual.FromFile(@"C:\Users\Jan\Source\Repos\RoguelikeEva\Results\door test\_RESULT 9.bin");
            var graph = ExtractRoomAdjacencyGraph(input.Rooms.First().Key, input);
            
            eva.Operators.Add(new ExtendBranchMutation(1));
            /*eva.Operators.Add(new CompressBranchMutation(1));
            eva.Operators.Add(new ReplaceBranchMutation(1));
            eva.Operators.Add(new ReverseBranchMutation(1));
            eva.Operators.Add(new ShiftStartingRoomMutation(1));*/
            
            eva.MultiObjective.Add(new BranchRatioFitness(3.0/2));
            eva.MultiObjective.Add(new BranchEntryPointsFitness());

            eva.SampleIndividual = new AdjacencyGraphIndividual(new List<byte>(input.Rooms.Keys), 3, 3);
            eva.HvIndicator = new BiobjectiveHvIndicator();

            using (StreamWriter file = new StreamWriter(@"C:\Users\Jan\Source\Repos\RoguelikeEva\Results\_out.txt"))
            {
                file.WriteLine("Gen\tHv");
                int solutionNo = 0;

                IEnumerable<Individual> result = eva.Run(e => e.Population.No == 10, 100, file, (writer, gen, hv) =>
                {
                    writer.WriteLine("{0}\t{1}", gen.No, hv);

                    Console.Clear();
                    Console.WriteLine("Current generation: " + gen.No);
                });

                foreach (Individual ind in result)
                {
                    AdjacencyGraphIndividual map = (AdjacencyGraphIndividual)ind;

                    // TODO:
                    // transform "map" to correct MapBlueprint

                    // "correct MapBlueprint".SaveImageToFile(@"C:\Users\Jan\Source\Repos\RoguelikeEva\Results\_RESULT " + solutionNo + ".png", 1536, 1024);
                    map.SaveToFile(@"C:\Users\Jan\Source\Repos\RoguelikeEva\Results\_RESULT " + (solutionNo++) + ".bin");
                }
            }
        }

        static Dictionary<byte, GraphNode> ExtractRoomAdjacencyGraph(byte start, MapBlueprintIndividual map)
        {
            // bfs
            Queue<byte> open = new Queue<byte>();
            HashSet<byte> closed = new HashSet<byte>();

            Dictionary<byte, GraphNode> adjacencyGraph = new Dictionary<byte, GraphNode>();
            adjacencyGraph.Add(start, new GraphNode(start));
            open.Enqueue(start);
            
            while (open.Count > 0)
            {
                byte next = open.Dequeue();
                IEnumerable<byte> connections = FindNeighboringRooms(map.Gene, map.Rooms[next].First());

                foreach (byte neighbor in connections)
                {
                    if (closed.Contains(neighbor))
                        continue;

                    if (open.Contains(neighbor))
                    {
                        adjacencyGraph[neighbor].Neighbors.Add(adjacencyGraph[next]);
                        adjacencyGraph[next].Neighbors.Add(adjacencyGraph[neighbor]);
                    }
                    else
                    {
                        GraphNode newNode = new GraphNode(neighbor);
                        newNode.Neighbors.Add(adjacencyGraph[next]);
                        adjacencyGraph[next].Neighbors.Add(newNode);
                        adjacencyGraph.Add(neighbor, newNode);

                        open.Enqueue(neighbor);
                    }
                }

                closed.Add(next);
            }

            return adjacencyGraph;
        }

        static HashSet<byte> FindNeighboringRooms(MapBlueprint map, Coords start)
        {
            // flood fill
            Stack<Coords> open = new Stack<Coords>();
            HashSet<Coords> closed = new HashSet<Coords>();
            HashSet<byte> neighbors = new HashSet<byte>();
            open.Push(start);

            while (open.Count > 0)
            {
                Coords tile = open.Pop();

                FloodFillNeighbor(tile, new Coords(tile.X - 1, tile.Y), map, open, neighbors, closed);
                FloodFillNeighbor(tile, new Coords(tile.X + 1, tile.Y), map, open, neighbors, closed);
                FloodFillNeighbor(tile, new Coords(tile.X, tile.Y - 1), map, open, neighbors, closed);
                FloodFillNeighbor(tile, new Coords(tile.X, tile.Y + 1), map, open, neighbors, closed);

                closed.Add(tile);
            }

            return neighbors;
        }

        static void FloodFillNeighbor(Coords start, Coords neighbor, MapBlueprint map, Stack<Coords> open, HashSet<byte> neighbors, HashSet<Coords> closed)
        {
            if (map.IsOnMap(neighbor.X, neighbor.Y) && !closed.Contains(neighbor))
            {
                if (map.Encoding[start.X, start.Y][0] == map.Encoding[neighbor.X, neighbor.Y][0])
                    open.Push(neighbor);

                else if (map.Encoding[start.X, start.Y][1] > 0 && map.Encoding[start.X, start.Y][1] == map.Encoding[neighbor.X, neighbor.Y][1])
                    neighbors.Add(map.Encoding[neighbor.X, neighbor.Y][0]);
            }
        }

        static void AddDoors(MapBlueprint map)
        {
            byte nextDoorId = 1;
            HashSet<Connection> existingConnections = new HashSet<Connection>();

            for (int x = 0; x < map.Encoding.GetLength(0) - 1; x++)
                for (int y = 0; y < map.Encoding.GetLength(1) - 1; y++)
                    if (!TryAddingDoor(x, y, x + 1, y, map, existingConnections, ref nextDoorId))
                        TryAddingDoor(x, y, x, y + 1, map, existingConnections, ref nextDoorId);
        }

        static bool TryAddingDoor(int x1, int y1, int x2, int y2, MapBlueprint map, HashSet<Connection> existingConnections, ref byte nextDoorId)
        {
            Connection connection = new Connection(map.Encoding[x1, y1][0], map.Encoding[x2, y2][0]);
            bool added = false;

            if (map.Encoding[x1, y1][0] > 0 && map.Encoding[x1, y1][1] == 0 &&
                map.Encoding[x2, y2][0] > 0 && map.Encoding[x2, y2][1] == 0 &&
                map.Encoding[x1, y1][0] != map.Encoding[x2, y2][0] &&
                !existingConnections.Contains(connection))
            {
                map.Encoding[x1, y1][1] = nextDoorId;
                map.Encoding[x2, y2][1] = nextDoorId++;

                existingConnections.Add(connection);
                added = true;
            }

            return added;
        }

        static void Phase1()
        {
            EvolutionaryAlgorithm eva = new EvolutionaryAlgorithm();

            eva.Operators.Add(new FloorTypeMutation(0.5));
            eva.Operators.Add(new JoinRoomsMutation(0.8));
            eva.Operators.Add(new RemoveRoomMutation(0.8));
            eva.Operators.Add(new FillHoleMutation(1));
            eva.Operators.Add(new ExtendEdgeMutation(1));

            eva.MultiObjective.Add(new AdjacencyFitness());
            eva.MultiObjective.Add(new RoomFitness(20));
            eva.MultiObjective.Add(new SpaceFitness());

            eva.SampleIndividual = new MapBlueprintIndividual(30, 20, 0.5);
            eva.HvIndicator = new HvSweep3D();
            //eva.MatingSelector = new RouletteWheelSelector();

            using (StreamWriter file = new StreamWriter(@"C:\Users\Jan\Source\Repos\RoguelikeEva\Results\_out.txt"))
            {
                file.WriteLine("Gen\tHv");
                int solutionNo = 0;

                IEnumerable<Individual> result = eva.Run(e => e.Population.No == 10, 100, file, (writer, gen, hv) =>
                {
                    writer.WriteLine("{0}\t{1}", gen.No, hv);

                    Console.Clear();
                    Console.WriteLine("Current generation: " + gen.No);
                });

                foreach (Individual ind in result)
                {
                    MapBlueprintIndividual map = (MapBlueprintIndividual)ind;

                    AddDoors(map.Gene);
                    // TODO: skip map which are not correct (discontinuous)

                    map.Gene.SaveImageToFile(@"C:\Users\Jan\Source\Repos\RoguelikeEva\Results\_RESULT " + solutionNo + ".png", 1536, 1024);
                    map.SaveToFile(@"C:\Users\Jan\Source\Repos\RoguelikeEva\Results\_RESULT " + (solutionNo++) + ".bin");
                }
            }
        }
        
        static void GenerateSmart(string filepath)
        {
            int w = 30, h = 20, rooms = 20;
            const int numheroes = 5;

            MapBlueprint map = new MapBlueprint();
            map.StartRoomId = 1;
            map.Encoding = new byte[w, h][];

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    map.Encoding[x, y] = new byte[3];

            Rectangle[] initialRooms = new Rectangle[rooms];
            Random rnd = new Random();
            /*int maxW = (int)(w * 2f / rooms);
            int maxH = (int)(h * 2f / rooms);*/
            int maxW = 7, maxH = 7;
            int index = 0;

            /*do
                initialRooms[index] = new Rectangle(rnd.Next(w - maxW), rnd.Next(h - maxH), rnd.Next(1, maxW), rnd.Next(1, maxH));
            while (++attempts < 1000 && initialRooms[0].Width * initialRooms[0].Height >= numheroes &&
                  (Overlaps(index, initialRooms) || ++index < rooms));*/

            do // FIXME: slow!
            {
                initialRooms = new Rectangle[rooms];
                index = 0;
                int attempts = 0;

                do
                    initialRooms[index] = new Rectangle(rnd.Next(w - maxW), rnd.Next(h - maxH), rnd.Next(1, maxW), rnd.Next(1, maxH));
                while (++attempts < 1000 && (Overlaps(index, initialRooms) || ++index < rooms));

                if (attempts == 1000)
                {
                    Rectangle[] tmp = new Rectangle[index];
                    Array.Copy(initialRooms, tmp, index);
                    initialRooms = tmp;
                }
            }
            while (initialRooms[0].Width * initialRooms[0].Height < numheroes);

            for (int i = 0; i < initialRooms.Length; i++)
            {
                Rectangle r = Rectangle.Zero;
                int width = 1;

                do
                {
                    r = new Rectangle(initialRooms[i].X + initialRooms[i].Width,
                                      initialRooms[i].Y,
                                      width++,
                                      initialRooms[i].Height);

                    if (r.X + r.Width - 1 > w)
                    {
                        r.Width = 0;
                        break;
                    }
                }
                while (!Overlaps(r, initialRooms));

                initialRooms[i].Width += r.Width - 1;

                int height = 1;

                do
                {
                    r = new Rectangle(initialRooms[i].X,
                                      initialRooms[i].Y + initialRooms[i].Height,
                                      initialRooms[i].Width,
                                      height++);

                    if (r.Y + r.Height - 1 > h)
                    {
                        r.Height = 0;
                        break;
                    }
                }
                while (!Overlaps(r, initialRooms));

                initialRooms[i].Height += r.Height - 1;
            }

            for (int i = 0; i < initialRooms.Length; i++)
                for (int x = initialRooms[i].X; x < initialRooms[i].X + initialRooms[i].Width; x++)
                    for (int y = initialRooms[i].Y; y < initialRooms[i].Y + initialRooms[i].Height; y++)
                    {
                        if (map.Encoding[x, y][0] > 0)
                        {
                            Rectangle a = initialRooms[i];
                            Rectangle b = initialRooms[map.Encoding[x, y][0] - 1];

                            bool inter = a.Overlaps(b);
                            throw new InvalidOperationException("Overlap!");
                        }

                        map.Encoding[x, y][0] = (byte)(i + 1);
                    }

            byte doors = 1;
            HashSet<int> doorsExistBetweenRooms = new HashSet<int>();

            for (int x = 0; x < w - 1; x++)
                for (int y = 0; y < h - 1; y++)
                {
                    int hash = (851 + map.Encoding[x, y][0]) * 37 + map.Encoding[x + 1, y][0];
                    bool doorsAdded = false;

                    if (map.Encoding[x, y][0] > 0 &&
                        map.Encoding[x + 1, y][0] > 0 &&
                        map.Encoding[x, y][0] != map.Encoding[x + 1, y][0] &&
                        !doorsExistBetweenRooms.Contains(hash))
                    {
                        map.Encoding[x + 1, y][1] = doors;
                        map.Encoding[x, y][1] = doors++;

                        doorsExistBetweenRooms.Add(hash);
                        doorsAdded = true;
                    }

                    if (!doorsAdded)
                    {
                        hash = (851 + map.Encoding[x, y][0]) * 37 + map.Encoding[x, y + 1][0];

                        if (map.Encoding[x, y][0] > 0 &&
                            map.Encoding[x, y + 1][0] > 0 &&
                            map.Encoding[x, y][0] != map.Encoding[x, y + 1][0] &&
                            !doorsExistBetweenRooms.Contains(hash))
                        {
                            map.Encoding[x, y][1] = doors;
                            map.Encoding[x, y + 1][1] = doors++;

                            doorsExistBetweenRooms.Add(hash);
                        }
                    }
                }

            map.SaveMapToFile(filepath + "map.bin");
            map.SaveImageToFile(filepath + "map.png", 1536, 1024);
        }
        
        static bool Overlaps(int index, Rectangle[] recs)
        {
            for (int i = 0; i < recs.Length; i++)
            {
                if (i == index)
                    continue;

                if (recs[i].Overlaps(recs[index]))
                    return true;
            }

            return false;
        }

        static bool Overlaps(Rectangle r, Rectangle[] recs)
        {
            for (int i = 0; i < recs.Length; i++)
                if (recs[i].Overlaps(r))
                    return true;

            return false;
        }
    }
}