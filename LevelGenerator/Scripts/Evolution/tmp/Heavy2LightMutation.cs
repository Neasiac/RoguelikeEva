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
    class Heavy2LightMutation : IOperator
    {
        Random Rnd = new Random();
        double MutationProbability;
        List<double> Weights;
        int Bins;

        public Heavy2LightMutation(double mutationProbability, List<double> weights, int K)
        {
            MutationProbability = mutationProbability;
            Weights = weights;
            Bins = K;
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
                    int iH = -1, iL = -1;
                    double heavyBinSum = 0, lightBinSum = double.PositiveInfinity;

                    for (int j = 0; j < Bins; j++)
                    {
                        double sum = getTotalWeight(o1, j);

                        if (heavyBinSum < sum)
                        {
                            heavyBinSum = sum;
                            iH = j;
                        }
                        else if (lightBinSum > sum)
                        {
                            lightBinSum = sum;
                            iL = j;
                        }
                    }

                    while (heavyBinSum > lightBinSum)
                        for (int j = 0; j < o1.Genes.Length; j++)
                        {
                            if ((int)o1.Genes[j] == iH)
                            {
                                o1.Genes[j] = iL;

                                heavyBinSum -= Weights[j];
                                lightBinSum += Weights[j];

                                break;
                            }
                        }
                }

                offspring[i] = o1;
            }

            return offspring;
        }

        double getTotalWeight(IntegerIndividual individual, int j)
        {
            double sum = 0;

            for (int i = 0; i < individual.Genes.Length; i++)
                if (individual.Genes[i] == j)
                    sum += Weights[i];

            return sum;
        }
    }
}