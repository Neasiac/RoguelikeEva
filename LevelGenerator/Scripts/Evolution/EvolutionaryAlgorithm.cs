using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class EvolutionaryAlgorithm
    {
        public Generation Population { get; private set; }
        public Fitness Fitness { get; set; }
        public List<Selector> Selector { get; set; } = new List<Selector>();
        public List<Operator> Operators { get; private set; } = new List<Operator>();
        public Individual SampleIndividual { get; set; }
        
        public Individual Run(Predicate<EvolutionaryAlgorithm> endCondition, int populationSize, TextWriter log = null, Action<TextWriter, Generation, Individual> logAction = null)
        {
            Population = Generation.Initial(SampleIndividual, populationSize);
            Population.EvaluateAll(Fitness);

            while (!endCondition(this))
            {
                Population = Population.Evolve(Operators, Fitness, Selector.ToArray(), null);
                logAction?.Invoke(log, Population, Population.Best);
            }

            return Population.Best;
        }
    }
}