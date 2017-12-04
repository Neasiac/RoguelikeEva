using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class BitFlipMutation : Operator
    {
        Random Rnd = new Random();
        double MutationProbability;
        double GeneChangeProbability;

        public BitFlipMutation(double mutationProbability, double geneChangeProbability)
        {
            MutationProbability = mutationProbability;
            GeneChangeProbability = geneChangeProbability;
        }

        public override Individual[] Operate(Individual[] parents)
        {
            Individual[] offspring = new Individual[parents.Length];
            
            for (int i = 0; i < parents.Length; i++)
            {
                BooleanIndividual p1 = (BooleanIndividual)parents[i];
                BooleanIndividual o1 = (BooleanIndividual)p1.Clone();

                if (Rnd.NextDouble() < MutationProbability)
                {
                    for (int j = 0; j < o1.Genes.Length; j++)
                    {
                        if (Rnd.NextDouble() < GeneChangeProbability)
                        {
                            bool b = o1.Genes[j];
                            o1.Genes[j] = !b;
                        }
                    }
                }

                offspring[i] = o1;
            }

            return offspring;
        }
    }
}