using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Fitnesses;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class SimpleFitness : IFitness
    {
        public double Evaluate(Individual ind)
        {
            BooleanIndividual bi = (BooleanIndividual)ind;
            bool[] genes = bi.Genes;

            double fitness = 0.0;

            // ===  IF one possible optimum  === //

            for (int i = 0; i < genes.Length; i++)
                fitness += rewardBit(genes[i], i);

            // ===  ELSE IF two possible optima  === //

            /*double fit1 = 0;
            double fit2 = 0;

            for (int i = 0; i < genes.Length; i++)
            {
                fit1 += rewardBit(genes[i], i);
                fit2 += rewardBit(!genes[i], i);
            }

            fitness = fit1 > fit2 ? fit1 : fit2;*/

            // ===  END IF  === //

            //ind.Objective = fitness; //this sets the objective value, can be different from the fitness function

            return fitness;
        }

        double rewardBit(bool bit, int position)
        {
            if ((position % 2 == 1 && bit) ||
                (position % 2 == 0 && !bit))
                return 1;

            return 0;
        }
    }
}