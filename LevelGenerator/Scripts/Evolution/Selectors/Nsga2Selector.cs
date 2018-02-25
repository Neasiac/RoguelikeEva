using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;
using Vegricht.LevelGenerator.Evolution.Individuals;

namespace Vegricht.LevelGenerator.Evolution.Selectors
{
    class Nsga2Selector : ISelector
    {
        Random Rnd = new Random();
        
        public Individual[] Select(int amount, Individual[] population)
        {
            AssignFrontAndSsc(population);

            Array.Sort(population, (o1, o2) =>
            {
                if (o1.Front < o2.Front) return -1;
                if (o1.Front > o2.Front) return 1;

                if (o1.CrowdingDistance > o2.CrowdingDistance) return -1;
                if (o1.CrowdingDistance < o2.CrowdingDistance) return 1;

                return 0;
            });

            Individual[] selected = new Individual[amount];

            for (int i = 0; i < amount; i++)
                selected[i] = population[i];

            return selected;
        }

        public HashSet<Individual> GetNonDominatedFront(IList<Individual> population)
        {
            if (population.Count == 0)
                return null;

            HashSet<Individual> front = new HashSet<Individual>();

            for (int i = 0; i < population.Count; i++)
            {
                bool dominated = false;

                for (int j = 0; j < population.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (Dominates(population[j], population[i]))
                        dominated = true;
                }

                if (!dominated)
                    front.Add(population[i]);
            }

            return front;
        }

        void AssignFrontAndSsc(Individual[] population)
        {
            List<Individual> inds = new List<Individual>(population);

            HashSet<Individual> front = GetNonDominatedFront(inds);
            int frontNo = 1;
            while (front != null)
            {
                inds.RemoveAll(ind => front.Contains(ind));

                foreach (Individual ind in front)
                    ind.Front = frontNo;
                
                AssignCrowdingDistance(front);
                front = GetNonDominatedFront(inds);
                frontNo++;
            }
        }
        
        bool Dominates(Individual i1, Individual i2)
        {
            double[] mri1Values = i1.MultiObjective;
            double[] mri2Values = i2.MultiObjective;

            bool strong = false;

            for (int i = 0; i < mri1Values.Length; i++)
            {
                if (mri1Values[i] > mri2Values[i])
                    strong = true;

                if (mri1Values[i] < mri2Values[i])
                    return false;
            }

            return strong;
        }
        
        void AssignCrowdingDistance(HashSet<Individual> front)
        {
            if (front.Count == 0)
                return;
            
            for (int i = 0; i < front.First().MultiObjective.Length; i++)
            {
                List<Individual> inds = new List<Individual>(front);
                inds.Sort((o1, o2) =>
                {
                    if (o1.MultiObjective[i] - o2.MultiObjective[i] < 0) return -1;
                    else return 1;
                });

                inds[0].CrowdingDistance = double.PositiveInfinity;
                inds[inds.Count - 1].CrowdingDistance = double.PositiveInfinity;

                for (int j = 1; j < inds.Count - 1; j++)
                {
                    /*double ssc = inds[j + 1].MultiObjective[0] - inds[j - 1].MultiObjective[0];
                    ssc += inds[j - 1].MultiObjective[i] - inds[j + 1].MultiObjective[i];
                    inds[j].Ssc = ssc;*/

                    inds[j].CrowdingDistance += inds[j + 1].MultiObjective[i] - inds[j - 1].MultiObjective[i];
                }
            }
        }
    }
}