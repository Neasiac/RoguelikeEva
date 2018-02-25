using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Fitnesses
{
    class BranchRatioFitness : IFitness
    {
        double Ratio;

        public BranchRatioFitness(double ratio)
        {
            Ratio = ratio;
        }
        
        public double Evaluate(Individual ind)
        {
            AdjacencyGraphIndividual graph = (AdjacencyGraphIndividual)ind;
            int @in = 0, @out = 0;

            foreach (GraphNode node in graph.Gene.Values)
            {
                if (node.BranchId > 0)
                    @in++;

                else
                    @out++;
            }

            return 1.0 / Math.Abs((double)@in/@out - Ratio + 1);
        }
    }
}