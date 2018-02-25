using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;
using Vegricht.LevelGenerator.Evolution.Operators;
using Vegricht.LevelGenerator.Evolution.Fitnesses;
using Vegricht.LevelGenerator.Evolution.Selectors;
using Vegricht.LevelGenerator.Evolution.Individuals;

namespace Vegricht.LevelGenerator.Evolution
{
    class Generation
    {
        public int No { get; private set; }
        Individual[] Population;
        Nsga2Selector Nsga = new Nsga2Selector();

        public static Generation Initial(Individual sample, int size)
        {
            Individual[] initial = new Individual[size];

            Parallel.For(0, size, i =>
            {
                Individual n = (Individual)sample.Clone();
                n.RandomInitialization();
                initial[i] = n;
            });
            
            return new Generation(initial, 1);
        }
        
        Generation(Individual[] population, int no)
        {
            Population = population;
            No = no;
        }

        public void EvaluateAll(IList<IFitness> fitnesses, Individual[] population = null)
        {
            if (population == null)
                population = Population;

            Parallel.ForEach(population, ind =>
            {
                ind.MultiObjective = new double[fitnesses.Count];

                for (int i = 0; i < fitnesses.Count; i++)
                    ind.MultiObjective[i] = fitnesses[i].Evaluate(ind);
            });
        }
        
        public Generation Evolve(IEnumerable<IOperator> operators, IList<IFitness> fitnesses, ISelector matingSelector)
        {
            Individual[] offspring = matingSelector != null
                ? matingSelector.Select(Population.Length, Population)
                : Population;
            
            foreach (IOperator op in operators)
                offspring = op.Operate(offspring);

            EvaluateAll(fitnesses, offspring);

            Individual[] selected = new Individual[Population.Length + offspring.Length];

            for (int i = 0; i < Population.Length; i++)
                selected[i] = (Individual)Population[i].Clone();
            Array.Copy(offspring, 0, selected, Population.Length, offspring.Length);

            selected = Nsga.Select(Population.Length, selected);
            EvaluateAll(fitnesses, selected);

            return new Generation(selected, No + 1);
        }

        public IEnumerable<Individual> GetAproximation()
        {
            return Nsga.GetNonDominatedFront(Population);
        }
    }
}