using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Operators
{
    class IntegerMutation : IOperator
    {
        Random Rnd = new Random();
        double MutationProbability;
        double GeneChangeProbability;

        public IntegerMutation(double mutationProbability, double geneChangeProbability)
        {
            MutationProbability = mutationProbability;
            GeneChangeProbability = geneChangeProbability;
        }

        public Individual[] Operate(Individual[] parents)
        {
            Individual[] offspring = new Individual[parents.Length];

            for (int i = 0; i < parents.Length; i++)
            {
                IntegerIndividual p1 = (IntegerIndividual)parents[i];
                IntegerIndividual o1 = (IntegerIndividual)p1.Clone();

                if (Rnd.NextDouble() < MutationProbability)
                {
                    for (int j = 0; j < o1.Genes.Length; j++)
                    {
                        if (Rnd.NextDouble() < GeneChangeProbability)
                        {
                            o1.Genes[j] = Rnd.Next(o1.Max - o1.Min) + o1.Min;
                        }
                    }
                }

                offspring[i] = o1;
            }

            return offspring;
        }
    }
}