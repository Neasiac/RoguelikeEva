using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;

namespace Vegricht.LevelGenerator.Evolution.Hypervolume
{
    abstract class HypervolumeIndicator
    {
        public double CalculateHypervolume(IEnumerable<Individual> inds)
        {
            int size = inds.Count();
            double[][] points = new double[size][];
            int i = 0;

            foreach (Individual ind in inds)
                points[i++] = ind.MultiObjective;

            return CalculateHypervolume(points);
        }

        public abstract double CalculateHypervolume(double[][] points);

        protected class Point2D : IComparable<Point2D>
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Point2D(double x, double y)
            {
                X = x;
                Y = y;
            }

            public int CompareTo(Point2D other)
            {
                if (Math.Abs(X - other.X) < 0.001)
                    return 0;

                if (X < other.X) return -1;
                else return 1;
            }
        }
    }
}