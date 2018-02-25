using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Fitnesses
{
    class SpaceFitness : IFitness
    {
        public double Evaluate(Individual ind)
        {
            MapBlueprintIndividual map = (MapBlueprintIndividual)ind;
            double fitness = 0;

            foreach (HashSet<Coords> room in map.Rooms.Values)
                fitness += room.Count;
            
            return fitness;
        }
    }
}