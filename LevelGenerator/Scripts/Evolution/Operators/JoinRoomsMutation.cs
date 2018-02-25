using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;
using Vegricht.LevelGenerator.Evolution.Individuals;

namespace Vegricht.LevelGenerator.Evolution.Operators
{
    class JoinRoomsMutation : IOperator
    {
        Random Rnd = new Random();
        double MutationProbability;

        enum Direction : byte
        {
            Up,
            Right,
            Down,
            Left
        }

        public JoinRoomsMutation(double mutationProbability)
        {
            MutationProbability = mutationProbability;
        }

        public Individual[] Operate(Individual[] parents)
        {
            Individual[] offspring = new Individual[parents.Length];

            Parallel.For(0, parents.Length, i =>
            {
                MapBlueprintIndividual p = (MapBlueprintIndividual)parents[i];
                MapBlueprintIndividual o = (MapBlueprintIndividual)p.Clone();

                if (Rnd.NextDouble() < MutationProbability && o.Rooms.Count > 1)
                {
                    /*Dictionary<byte, int> histogram = new Dictionary<byte, int>();

                    for (int k = 0; k < 1000000; k++)
                    {
                        byte n = o.RoomsOrdered.ElementAt(GetRandomNumber(0, o.RoomsOrdered.Count, 4));

                        if (histogram.ContainsKey(n))
                            histogram[n]++;
                        else
                            histogram.Add(n, 1);
                    }

                    foreach (byte k in histogram.Keys)
                    {
                        Console.WriteLine(k + ": " + histogram[k] + ", tiles: " + o.Rooms[k].Count);
                    }

                    throw new Exception();*/

                    int rooms = o.Rooms.Count;
                    int ordered = o.RoomsOrdered.Count;

                    if (rooms != ordered)
                    {
                        int sgsg = 5;
                    }
                    
                    byte key = o.RoomsOrdered.ElementAt(GetRandomNumber(0, o.RoomsOrdered.Count, 2));
                    Coords start = o.Rooms[key].ElementAt(Rnd.Next(0, o.Rooms[key].Count));
                    Array values = Enum.GetValues(typeof(Direction));
                    Direction dir = (Direction)values.GetValue(Rnd.Next(values.Length));

                    int x = start.X;
                    int y = start.Y;

                    if (o.Gene.Encoding[start.X, start.Y][0] != key)
                    {
                        int fdfdfd = 0;
                    }

                    do
                        switch (dir)
                        {
                            case Direction.Up: y--; break;
                            case Direction.Right: x++; break;
                            case Direction.Down: y++; break;
                            case Direction.Left: x--; break;
                        }
                    while (o.Gene.IsOnMap(x, y) && o.Gene.Encoding[x, y][0] == o.Gene.Encoding[start.X, start.Y][0]);

                    if (o.Gene.IsOnMap(x, y) && o.Gene.Encoding[x, y][0] > 0)
                    {
                        byte removeAt = o.Gene.Encoding[x, y][0];
                        o.RoomsOrdered.Remove(removeAt);
                        o.RoomsOrdered.Remove(key);

                        foreach (Coords coords in o.Rooms[removeAt])
                        {
                            o.Gene.Encoding[coords.X, coords.Y][0] = o.Gene.Encoding[start.X, start.Y][0];
                            o.Rooms[key].Add(coords);
                        }

                        o.RoomsOrdered.Add(key);
                        o.Rooms.Remove(removeAt);
                    }
                }

                offspring[i] = o;
            });

            return offspring;
        }

        int GetRandomNumber(int min, int max, double probabilityPower)
        {
            return (int)Math.Floor(min + (max - min) * (Math.Pow(Rnd.NextDouble(), probabilityPower)));
        }
    }
}