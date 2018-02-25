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
    class FillHoleMutation : IOperator
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

        public FillHoleMutation(double mutationProbability)
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
                    int x = Rnd.Next(0, o.Width);
                    int y = Rnd.Next(0, o.Height);
                    Array values = Enum.GetValues(typeof(Direction));
                    Direction dir = (Direction)values.GetValue(Rnd.Next(values.Length));

                    do
                    {
                        if (!IsHole(x, y, o))
                            switch (dir)
                            {
                                case Direction.Up: y--; break;
                                case Direction.Right: x++; break;
                                case Direction.Down: y++; break;
                                case Direction.Left: x--; break;
                            }
                        else
                            break;
                    }
                    while (o.Gene.IsOnMap(x, y));

                    if (o.Gene.IsOnMap(x, y))
                    {
                        byte foam = GetFoam(x, y, o);

                        o.Gene.Encoding[x, y][0] = foam;
                        o.AddTileToRoom(foam, new Coords(x, y));
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

        bool IsHole(int x, int y, MapBlueprintIndividual o)
        {
            return o.Gene.Encoding[x, y][0] == 0 &&
                (!o.Gene.IsOnMap(x + 1, y) || o.Gene.Encoding[x + 1, y][0] > 0) &&
                (!o.Gene.IsOnMap(x - 1, y) || o.Gene.Encoding[x - 1, y][0] > 0) &&
                (!o.Gene.IsOnMap(x, y + 1) || o.Gene.Encoding[x, y + 1][0] > 0) &&
                (!o.Gene.IsOnMap(x, y - 1) || o.Gene.Encoding[x, y - 1][0] > 0);
        }

        byte GetFoam(int x, int y, MapBlueprintIndividual o)
        {
            byte foam = 0;

            if (o.Gene.IsOnMap(x + 1, y)) foam = o.Gene.Encoding[x + 1, y][0];
            else if (o.Gene.IsOnMap(x - 1, y)) foam = o.Gene.Encoding[x - 1, y][0];
            else if (o.Gene.IsOnMap(x, y + 1)) foam = o.Gene.Encoding[x, y + 1][0];
            else if (o.Gene.IsOnMap(x, y - 1)) foam = o.Gene.Encoding[x, y - 1][0];

            return foam;
        }
    }
}
 