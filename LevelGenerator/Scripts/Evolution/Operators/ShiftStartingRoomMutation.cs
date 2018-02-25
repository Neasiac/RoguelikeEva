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
    class ShiftStartingRoomMutation : IOperator
    {
        Random Rnd = new Random();
        double MutationProbability;
        
        public ShiftStartingRoomMutation(double mutationProbability)
        {
            MutationProbability = mutationProbability;
        }

        public Individual[] Operate(Individual[] parents)
        {
            Individual[] offspring = new Individual[parents.Length];

            Parallel.For(0, parents.Length, i =>
            {
                AdjacencyGraphIndividual p = (AdjacencyGraphIndividual)parents[i];
                AdjacencyGraphIndividual o = (AdjacencyGraphIndividual)p.Clone();

                if (Rnd.NextDouble() < MutationProbability)
                {
                    var candidates = from n in o.StartingRoom.Neighbors where n.IsFree select n;
                    int size = candidates.Count();
                    o.StartingRoom.IsStartingRoom = false;

                    if (candidates.Any())
                    {
                        GraphNode neo = candidates.ElementAt(Rnd.Next(size));
                        neo.IsStartingRoom = true;
                        o.StartingRoom = neo;
                    }
                }

                offspring[i] = o;
            });

            return offspring;
        }
    }
}
 