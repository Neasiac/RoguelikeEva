using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components;

namespace Vegricht.RoguelikeEva.Pathfinding
{
    class AStarPathFinder
    {
        Path.PathAction Action;

        public AStarPathFinder(Path.PathAction action)
        {
            Action = action;
        }

        public Path Find(MapNode from, MapNode to)
        {
            HashSet<MapNode> closed = new HashSet<MapNode>();
            HashedFibonacciHeap<MapNode, int> fringe = new HashedFibonacciHeap<MapNode, int>(0);
            Dictionary<MapNode, MapNode> cameFrom = new Dictionary<MapNode, MapNode>();
            Score gScore = new Score();
            Score fScore = new Score();

            fringe.Insert(from, heuristic(from, to));
            gScore[from] = 0;
            fScore[from] = heuristic(from, to);

            while (!fringe.IsEmpty())
            {
                MapNode current = fringe.RemoveMin();

                if (current == to)
                    return ReconstructPath(cameFrom, current);

                closed.Add(current);

                foreach (MapNode neighbor in current.Neighbors)
                {
                    if (closed.Contains(neighbor))
                        continue;

                    int tentativeGScore = gScore[current] + 1; // dist_between(current, neighbor) = 1

                    if (tentativeGScore >= gScore[neighbor])
                        continue;

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + heuristic(neighbor, to);

                    if (fringe.Contains(neighbor))
                        fringe.DecreasePriority(neighbor, fScore[neighbor]);
                    else
                        fringe.Insert(neighbor, fScore[neighbor]);
                }
            }

            return null;
        }

        // Manhattan distance
        int heuristic(MapNode from, MapNode to)
        {
            return Math.Abs(from.X - to.X) - Math.Abs(from.Y - to.Y);
        }

        Path ReconstructPath(Dictionary<MapNode, MapNode> cameFrom, MapNode current)
        {
            List<MapNode> path = new List<MapNode>();
            path.Add(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return new Path(path, Action);
        }
        
        class Score
        {
            Dictionary<MapNode, int> Mapping;

            public Score()
            {
                Mapping = new Dictionary<MapNode, int>();
            }

            public int this[MapNode node]
            {
                get
                {
                    if (!Mapping.ContainsKey(node))
                    {
                        Mapping.Add(node, int.MaxValue);
                        return int.MaxValue;
                    }

                    return Mapping[node];
                }

                set
                {
                    Mapping[node] = value;
                }
            }
        }
    }
}