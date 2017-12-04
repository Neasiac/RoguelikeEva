using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    abstract class Fitness
    {
        public Individual Evaluate(IEnumerable<Individual> inds)
        {
            Individual best = null;

            foreach (Individual ind in inds)
            {
                ind.Fitness = Evaluate(ind);

                if (best == null || ind.Fitness > best.Fitness)
                    best = ind;
            }

            return best;
        }

        public abstract double Evaluate(Individual ind);
    }
}