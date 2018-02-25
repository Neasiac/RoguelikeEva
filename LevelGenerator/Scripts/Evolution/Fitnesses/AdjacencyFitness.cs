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
    class AdjacencyFitness : IFitness
    {
        public double Evaluate(Individual ind)
        {
            MapBlueprintIndividual map = (MapBlueprintIndividual)ind;
            double fitness = 0;

            foreach (HashSet<Coords> room in map.Rooms.Values)
            {
                HashSet<byte> neighbors = new HashSet<byte>();

                foreach (Coords tile in room)
                {
                    CheckAdjacency(tile.X - 1, tile.Y, tile, map, neighbors);
                    CheckAdjacency(tile.X + 1, tile.Y, tile, map, neighbors);
                    CheckAdjacency(tile.X, tile.Y - 1, tile, map, neighbors);
                    CheckAdjacency(tile.X, tile.Y + 1, tile, map, neighbors);
                }

                fitness += neighbors.Count;
            }
            
            return fitness / map.Rooms.Count;
        }
        
        void CheckAdjacency(int x, int y, Coords tile, MapBlueprintIndividual map, HashSet<byte> neighbors)
        {
            if (map.Gene.IsOnMap(x, y) &&
                map.Gene.Encoding[x, y][0] > 0 &&
                map.Gene.Encoding[x, y][0] != map.Gene.Encoding[tile.X, tile.Y][0] &&
                !neighbors.Contains(map.Gene.Encoding[x, y][0]))

                neighbors.Add(map.Gene.Encoding[x, y][0]);
        }
    }
}