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
    class FloorTypeMutation : IOperator
    {
        Random Rnd = new Random();
        double MutationProbability;

        public FloorTypeMutation(double mutationProbability)
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

                if (Rnd.NextDouble() < MutationProbability)
                {
                    int x = Rnd.Next(0, o.Width);
                    int y = Rnd.Next(0, o.Height);

                    if (o.Gene.Encoding[x, y][0] > 0)
                    {
                        if (!o.Rooms[o.Gene.Encoding[x, y][0]].Contains(new Coords(x, y)))
                        {
                            int sgf = 5;
                        }

                        //bool rem = o.Rooms[o.Gene.Encoding[x, y][0]].Remove(new Coords(x, y));
                        o.RemoveTileFromRoom(o.Gene.Encoding[x, y][0], new Coords(x, y));
                        /*if (rem == false)
                        {
                            int dkfjdf = 2;
                        }*/

                        // did we get empty room? -> o.Rooms.Remove(o.Gene.Encoding[x, y][0]);
                        if (o.Rooms[o.Gene.Encoding[x, y][0]].Count == 0)
                        {
                            o.RoomsOrdered.Remove(o.Gene.Encoding[x, y][0]);
                            o.Rooms.Remove(o.Gene.Encoding[x, y][0]);
                        }

                        o.Gene.Encoding[x, y][0] = 0;
                        // FIXME: maybe I caused a room to be split !
                    }
                    else
                    {
                        if (o.Gene.IsOnMap(x - 1, y) && o.Gene.Encoding[x - 1, y][0] > 0)
                        {
                            o.Gene.Encoding[x, y][0] = o.Gene.Encoding[x - 1, y][0];
                            o.AddTileToRoom(o.Gene.Encoding[x, y][0], new Coords(x, y));
                            //o.Rooms[o.Gene.Encoding[x - 1, y][0]].Add(new Coords(x, y));
                        }

                        else if (o.Gene.IsOnMap(x + 1, y) && o.Gene.Encoding[x + 1, y][0] > 0)
                        {
                            o.Gene.Encoding[x, y][0] = o.Gene.Encoding[x + 1, y][0];
                            o.AddTileToRoom(o.Gene.Encoding[x, y][0], new Coords(x, y));
                        }

                        else if (o.Gene.IsOnMap(x, y - 1) && o.Gene.Encoding[x, y - 1][0] > 0)
                        {
                            o.Gene.Encoding[x, y][0] = o.Gene.Encoding[x, y - 1][0];
                            o.AddTileToRoom(o.Gene.Encoding[x, y][0], new Coords(x, y));
                        }

                        else if (o.Gene.IsOnMap(x, y + 1) && o.Gene.Encoding[x, y + 1][0] > 0)
                        {
                            o.Gene.Encoding[x, y][0] = o.Gene.Encoding[x, y + 1][0];
                            o.AddTileToRoom(o.Gene.Encoding[x, y][0], new Coords(x, y));
                        }

                        else
                        {
                            // FIXME: nepouziva o.AddTileToRoom(o.Gene.Encoding[x, y][0], new Coords(x, y));
                            /*byte newRoomId = 0;

                            do
                                // FIXME: potential infinite loop!
                                newRoomId = (byte)Rnd.Next(0, byte.MaxValue);
                            while (o.Rooms.ContainsKey(newRoomId));

                            o.Rooms.Add(newRoomId, new HashSet<int[]>());
                            o.Rooms[newRoomId].Add(new int[2] { x, y });*/
                        }
                    }
                }

                offspring[i] = o;
            });

            return offspring;
        }
    }
}