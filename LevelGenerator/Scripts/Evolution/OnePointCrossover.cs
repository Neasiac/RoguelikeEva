using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class OnePointCrossover : Operator
    {
        Random Rnd = new Random();
        double XOverProbability;

        public OnePointCrossover(double xOverProbability)
        {
            XOverProbability = xOverProbability;
        }

        public override Individual[] Operate(Individual[] parents)
        {
            Individual[] offspring = new Individual[parents.Length];

            for (int i = 0; i < parents.Length / 2; i++)
            {
                IntegerIndividual p1 = (IntegerIndividual)parents[2 * i];
                IntegerIndividual p2 = (IntegerIndividual)parents[2 * i + 1];

                IntegerIndividual o1 = (IntegerIndividual)p1.Clone();
                IntegerIndividual o2 = (IntegerIndividual)p2.Clone();

                if (Rnd.NextDouble() < XOverProbability)
                {
                    int point = Rnd.Next(p1.Genes.Length);

                    for (int j = point; j < p1.Genes.Length; j++)
                    {
                        int tmp = o1.Genes[j];
                        o1.Genes[j] = o2.Genes[j];
                        o2.Genes[j] = tmp;
                    }

                }

                offspring[2 * i] = o1;
                offspring[2 * i + 1] = o2;
            }

            return offspring;
        }
    }
}