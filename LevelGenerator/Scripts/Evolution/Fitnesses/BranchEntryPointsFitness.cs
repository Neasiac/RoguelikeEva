using FibonacciHeap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.RoguelikeEva.Pathfinding;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Fitnesses
{
    class BranchEntryPointsFitness : IFitness
    {
        /*class PathNode
        {
            public GraphNode Node { get; set; }
            public int Cost { get; set; }

            public PathNode(GraphNode node, int cost)
            {
                Node = node;
                Cost = cost;
            }

            public override int GetHashCode()
            {
                return Node.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                PathNode other = (PathNode)obj;
                return Node == other.Node;
            }
        }*/

        public double Evaluate(Individual ind)
        {
            AdjacencyGraphIndividual graph = (AdjacencyGraphIndividual)ind;
            
            HashSet<GraphNode> closed = new HashSet<GraphNode>();
            Dictionary<GraphNode, int> distance = new Dictionary<GraphNode, int>();
            HashedFibonacciHeap<GraphNode, int> open = new HashedFibonacciHeap<GraphNode, int>(0);
            open.Insert(graph.StartingRoom, 0);

            while (!open.IsEmpty())
            {
                GraphNode current = open.RemoveMin();
                closed.Add(current);
                
                foreach (GraphNode neighbor in current.Neighbors)
                {
                    if (closed.Contains(neighbor))
                        continue;

                    int alt = distance[current] + 1;

                    if (distance.ContainsKey(neighbor) && alt >= distance[neighbor])
                        continue;

                    distance[neighbor] = alt;

                    if (open.Contains(neighbor))
                        open.DecreasePriority(neighbor, distance[neighbor]);
                    else
                        open.Insert(neighbor, distance[neighbor]);
                }
            }

            double fitness = 0;

            foreach (GraphNode starter in graph.BranchStarters.Values)
                fitness += distance[starter];

            // f/C ^ -1
            return graph.BranchStarters.Count / fitness;
        }
    }
}