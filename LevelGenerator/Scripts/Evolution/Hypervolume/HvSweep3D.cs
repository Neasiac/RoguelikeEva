using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegricht.LevelGenerator.Evolution.Hypervolume
{
    class HvSweep3D : HypervolumeIndicator
    {
        public override double CalculateHypervolume(double[][] points)
        {
            Array.Sort(points, (p1, p2) =>
            {
                // decreasing order
                if (p1[2] < p2[2]) return 1;
                if (p1[2] > p2[2]) return -1;

                return 0;
            });

            SweeperBinaryTree<Point2D> Sweeper = new SweeperBinaryTree<Point2D>();
            Sweeper.Add(new Point2D(0, double.PositiveInfinity));
            Sweeper.Add(new Point2D(double.PositiveInfinity, 0));

            double vol = 0;

            Sweeper.Add(new Point2D(points[0][0], points[0][1]));
            double area = points[0][0] * points[0][1];
            double[] z = points[0];

            for (int i = 1; i < points.Length; i++)
            {
                Point2D q = Sweeper.Successor(new Point2D(points[i][0], points[i][1]));

                // points[i] is not dominated by q
                if (q.Y < points[i][1])
                {
                    vol += area * (z[2] - points[i][1]);
                    z = points[i];

                    Point2D s = Sweeper.Predecessor(q);

                    while (s.Y < points[i][1])
                    {
                        Point2D sPred = Sweeper.Predecessor(s);

                        area -= (s.X - sPred.X) * (s.Y - q.Y);
                        Sweeper.Remove(s);

                        s = sPred;
                    }
                    
                    area += (points[i][0] - s.X) * (points[i][1] - q.Y);
                    Sweeper.Add(new Point2D(points[i][0], points[i][1]));
                }
            }

            return vol + area * z[2];
        }
    }
}