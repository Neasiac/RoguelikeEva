using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class SwapMutation : Operator
    {
        Random Rnd = new Random();
        double MutationProbability;

        public SwapMutation(double mutationProbability)
        {
            MutationProbability = mutationProbability;
        }

        public override Individual[] Operate(Individual[] parents)
        {
            Individual[] offspring = new Individual[parents.Length];

            for (int i = 0; i < parents.Length; i++)
            {
                IntegerIndividual p1 = (IntegerIndividual)parents[i];
                IntegerIndividual o1 = (IntegerIndividual)p1.Clone();

                if (Rnd.NextDouble() < MutationProbability)
                {
                    int i1 = 0, i2 = 0;
                    int size = o1.Genes.Length;

                    while (i1 == i2)
                    {
                        i1 = Rnd.Next(size);
                        i2 = Rnd.Next(size);
                    }

                    int tmp = o1.Genes[i1];
                    o1.Genes[i1] = o1.Genes[i2];
                    o1.Genes[i2] = tmp;
                }

                offspring[i] = o1;
            }

            return offspring;
        }
    }
}