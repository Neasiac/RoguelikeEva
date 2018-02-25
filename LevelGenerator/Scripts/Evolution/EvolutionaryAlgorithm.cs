using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Fitnesses;
using Vegricht.LevelGenerator.Evolution.Hypervolume;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.LevelGenerator.Evolution.Operators;
using Vegricht.LevelGenerator.Evolution.Selectors;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class EvolutionaryAlgorithm
    {
        public Generation Population { get; private set; }
        public ISelector MatingSelector { get; set; }
        public Individual SampleIndividual { get; set; }
        public HypervolumeIndicator HvIndicator { get; set; }
        public List<IFitness> MultiObjective { get; private set; } = new List<IFitness>();
        public List<IOperator> Operators { get; private set; } = new List<IOperator>();
        
        public IEnumerable<Individual> Run(Predicate<EvolutionaryAlgorithm> endCondition, int populationSize, TextWriter log = null, Action<TextWriter, Generation, double> logAction = null)
        {
            if (MultiObjective == null || MultiObjective.Count == 0)
                throw new InvalidOperationException("Fitness functions need to be set.");

            Population = Generation.Initial(SampleIndividual, populationSize);
            Population.EvaluateAll(MultiObjective);
            Log(log, logAction);

            while (!endCondition(this))
            {
                Population = Population.Evolve(Operators, MultiObjective, MatingSelector);
                Log(log, logAction);
            }

            return Population.GetAproximation();
        }

        void Log(TextWriter log, Action < TextWriter, Generation, double> logAction)
        {
            double hv = HvIndicator.CalculateHypervolume(Population.GetAproximation());
            logAction?.Invoke(log, Population, hv);
        }
    }
}