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
    class ExtendEdgeMutation : IOperator
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

        public ExtendEdgeMutation(double mutationProbability)
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

                if (Rnd.NextDouble() < MutationProbability && o.Rooms.Count > 0)
                {
                    byte key = o.RoomsOrdered.ElementAt(GetRandomNumber(0, o.RoomsOrdered.Count, 2));
                    Coords start = o.Rooms[key].ElementAt(Rnd.Next(0, o.Rooms[key].Count));
                    Array values = Enum.GetValues(typeof(Direction));
                    Direction dir = (Direction)values.GetValue(Rnd.Next(values.Length));

                    int x = start.X;
                    int y = start.Y;
                    
                    do
                        switch (dir)
                        {
                            case Direction.Up: y--; break;
                            case Direction.Right: x++; break;
                            case Direction.Down: y++; break;
                            case Direction.Left: x--; break;
                        }
                    while (o.Gene.IsOnMap(x, y) && o.Gene.Encoding[x, y][0] == o.Gene.Encoding[start.X, start.Y][0]);

                    if (o.Gene.IsOnMap(x, y) && o.Gene.Encoding[x, y][0] == 0)
                    {
                        List<Coords> edge = new List<Coords>();

                        if (dir == Direction.Up || dir == Direction.Down)
                        {
                            for (int ix = x; EdgeTerminator(ix, y, start.X, start.Y, dir, o); ix++)
                                edge.Add(new Coords(ix, y));

                            for (int ix = x - 1; EdgeTerminator(ix, y, start.X, start.Y, dir, o); ix--)
                                edge.Add(new Coords(ix, y));
                        }

                        else
                        {
                            for (int iy = y; EdgeTerminator(x, iy, start.X, start.Y, dir, o); iy++)
                                edge.Add(new Coords(x, iy));

                            for (int iy = y - 1; EdgeTerminator(x, iy, start.X, start.Y, dir, o); iy--)
                                edge.Add(new Coords(x, iy));
                        }
                        
                        o.RoomsOrdered.Remove(key);

                        foreach (Coords coords in edge)
                        {
                            if (o.Gene.Encoding[coords.X, coords.Y][0] > 0)
                            {
                                int fdgf = 5;
                            }

                            o.Gene.Encoding[coords.X, coords.Y][0] = o.Gene.Encoding[start.X, start.Y][0];
                            o.Rooms[key].Add(coords);
                        }

                        o.RoomsOrdered.Add(key);
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

        bool EdgeTerminator(int x, int y, int startX, int startY, Direction dir, MapBlueprintIndividual o)
        {
            if (!o.Gene.IsOnMap(x, y))
                return false;

            switch (dir)
            {
                case Direction.Up:
                    return o.Gene.Encoding[x, y][0] == 0 &&
                        o.Gene.Encoding[x, y + 1][0] == o.Gene.Encoding[startX, startY][0];

                case Direction.Right:
                    return o.Gene.Encoding[x, y][0] == 0 &&
                        o.Gene.Encoding[x - 1, y][0] == o.Gene.Encoding[startX, startY][0];

                case Direction.Down:
                    return o.Gene.Encoding[x, y][0] == 0 &&
                        o.Gene.Encoding[x, y - 1][0] == o.Gene.Encoding[startX, startY][0];

                case Direction.Left:
                    return o.Gene.Encoding[x, y][0] == 0 &&
                        o.Gene.Encoding[x + 1, y][0] == o.Gene.Encoding[startX, startY][0];

                default:
                    // we've considered every possible direction, so this will never happen
                    throw new InvalidOperationException();
            }
        }
    }
}
 