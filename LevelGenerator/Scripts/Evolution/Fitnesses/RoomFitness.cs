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
    class RoomFitness : IFitness
    {
        int Max;

        public RoomFitness(int max)
        {
            Max = max;
        }
        
        public double Evaluate(Individual ind)
        {
            MapBlueprintIndividual map = (MapBlueprintIndividual)ind;
            return 100 - Math.Abs(Max - map.Rooms.Count);
        }
    }
}