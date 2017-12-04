using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class Generation
    {
        public int No { get; private set; }
        public Individual Best { get; private set; }

        Individual[] Population;

        public static Generation Initial(Individual sample, int size)
        {
            Individual[] initial = new Individual[size];

            for (int i = 0; i < size; i++)
            {
                Individual n = (Individual)sample.Clone();
                n.RandomInitialization();
                initial[i] = n;
            }

            return new Generation(initial, 1, null);
        }
        
        Generation(Individual[] population, int no, Individual best)
        {
            Population = population;
            No = no;
            Best = best;
        }

        public void EvaluateAll(Fitness fitness)
        {
            Best = fitness.Evaluate(Population);
        }
        
        public Generation Evolve(IEnumerable<Operator> operators, Fitness fitness, Selector[] matingSelectors, Selector[] environmentalSelectors)
        {
            Individual[] offspring = matingSelectors[0].Select(Population.Length, Population);
            // FIXME: currenlty supporting only one selector ..... (but maybe that's enough?)
            
            foreach (Operator op in operators)
                offspring = op.Operate(offspring);
            
            // TODO: envirnonmental selector(s) should be here!

            Individual best = fitness.Evaluate(offspring);
            
            return new Generation(offspring, No + 1, best);
        }
    }
}