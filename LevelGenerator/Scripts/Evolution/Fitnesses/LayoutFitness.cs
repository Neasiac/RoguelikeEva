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
    class LayoutFitness : IFitness
    {
        public double Evaluate(Individual ind)
        {
            MapBlueprintIndividual map = (MapBlueprintIndividual)ind;
            double fitness = 0;

            foreach (HashSet<Coords> room in map.Rooms.Values)
            {
                int maxdist = int.MinValue;

                foreach (Coords coordsA in room)
                    foreach (Coords coordsB in room)
                    {
                        if (coordsA.Equals(coordsB))
                            continue;

                        int dist = distance(coordsA.X, coordsA.Y, coordsB.X, coordsB.Y);

                        if (dist > maxdist)
                            maxdist = dist;
                    }

                fitness += room.Count / (double)maxdist;
            }
            
            return fitness / map.Rooms.Count;
        }

        // Manhattan distance
        int distance(int ax, int ay, int bx, int by)
        {
            return Math.Abs(ax - bx) + Math.Abs(ay - by);
        }
    }
}