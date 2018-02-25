using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.LevelGenerator.Evolution.Selectors;

namespace Vegricht.LevelGenerator.Evolution.Hypervolume
{
    class BiobjectiveHvIndicator : HypervolumeIndicator
    {
        public override double CalculateHypervolume(double[][] points)
        {
            Array.Sort(points, (p1, p2) =>
            {
                if (p1[0] < p2[0]) return -1;
                if (p1[0] > p2[0]) return 1;

                return 0;
            });

            double volume = 0.0;
            int size = points.GetLength(0);

            for (int i = 0; i < size - 1; i++)
                volume += (-points[i][1]) * (points[i + 1][0] - points[i][0]);
            
            return volume + points[size - 1][1] * points[size - 1][0];
        }
    }
}