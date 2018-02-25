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
    class CompressBranchMutation : IOperator
    {
        Random Rnd = new Random();
        double MutationProbability;
        
        public CompressBranchMutation(double mutationProbability)
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
                    byte branchId = o.BranchStarters.Keys.ElementAt(Rnd.Next(o.BranchStarters.Count));

                    GraphNode next = o.BranchStarters[branchId];
                    GraphNode last = null;

                    do
                    {
                        GraphNode tmp = (from n in next.Neighbors where n.BranchId == next.BranchId && n != last select n).First();
                        last = next;
                        next = tmp;
                    }
                    while (next != null);

                    GraphNode newLast = (from n in last.Neighbors where n.BranchId == branchId select n).First();
                    last.BranchId = 0;
                    last.Status = GraphNode.BranchRoomStatus.None;
                    newLast.Status = GraphNode.BranchRoomStatus.Ending;
                }

                offspring[i] = o;
            });

            return offspring;
        }
    }
}
 