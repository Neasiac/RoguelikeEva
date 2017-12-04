using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class BinPackingFitness : Fitness
    {
        List<double> weights;
        int K;

        public BinPackingFitness(List<double> weights, int K)
        {
            this.weights = weights;
            this.K = K;
        }

        public override double Evaluate(Individual ind)
        {
            int[] binWeights = getBinWeights(ind);

            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            for (int i = 0; i < K; i++)
            {
                if (binWeights[i] < min)
                {
                    min = binWeights[i];
                }
                if (binWeights[i] > max)
                {
                    max = binWeights[i];
                }
            }

            ind.Objective = max - min;    // tohle doporucuji zachovat

            //return 1 / (ind.Objective + 1);

            double optimal = average(binWeights);
            double leastSquares = 0;

            for (int i = 0; i < binWeights.Length; i++)
                leastSquares += (optimal - binWeights[i]) * (optimal - binWeights[i]);

            return 1 / (leastSquares + 1);
            //return 1 / (max - min + 1);
            //return Math.exp((min - max)/1000);
            //return Math.exp(-leastSquares/100000);
        }

        public int[] getBinWeights(Individual ind)
        {
            int[] binWeights = new int[K];

            int[] bins = ((IntegerIndividual)ind).Genes;

            for (int i = 0; i < bins.Length; i++)
            {
                binWeights[bins[i]] += (int)weights[i];
            }

            return binWeights;

        }

        double average(int[] array)
        {
            double avg = 0;

            for (int i = 0; i < array.Length; i++)
                avg += array[i];

            return avg / array.Length;
        }
    }
}