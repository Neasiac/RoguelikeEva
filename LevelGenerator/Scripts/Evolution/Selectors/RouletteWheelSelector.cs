using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Selectors
{
    class RouletteWheelSelector : ISelector
    {
        Random Rnd = new Random();
        
        public Individual[] Select(int amount, Individual[] population)
        {
            //int amount = population.Length; // FIXME: dynamic amount for more selectors?
            double fitnessSum = 0;
            double[] fitnesses = new double[population.Length];
            //Individual[] selected = new Individual[population.Length];
            List<Individual> selected = new List<Individual>(); // FIXME: use just array

            /*for (int i = 0; i < population.Length; i++)
                fitnessSum += population[i].Fitness;

            for (int i = 0; i < population.Length; i++)
                fitnesses[i] = population[i].Fitness / fitnessSum;*/
            
            for (int i = 0; i < amount; i++)
            {
                double ball = Rnd.NextDouble();
                double sum = 0;

                for (int j = 0; j < fitnesses.Length; j++)
                {
                    sum += fitnesses[j];

                    if (sum > ball)
                    {
                        selected.Add((Individual)population[j].Clone());
                        break;
                    }
                }
            }

            return selected.ToArray();
        }
    }
}