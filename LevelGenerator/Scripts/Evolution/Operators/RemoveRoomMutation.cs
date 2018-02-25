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
    class RemoveRoomMutation : IOperator
    {
        Random Rnd = new Random();
        double MutationProbability;
        
        public RemoveRoomMutation(double mutationProbability)
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
                    byte removeAt = o.RoomsOrdered.ElementAt(GetRandomNumber(0, o.RoomsOrdered.Count, 4));
                    
                    foreach (Coords coords in o.Rooms[removeAt])
                        o.Gene.Encoding[coords.X, coords.Y][0] = 0;

                    o.RoomsOrdered.Remove(removeAt);
                    o.Rooms.Remove(removeAt);
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