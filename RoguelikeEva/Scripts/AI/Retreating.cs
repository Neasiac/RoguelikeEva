using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components;
using Vegricht.RoguelikeEva.Scenes.Core;
using System.Collections;
using Vegricht.RoguelikeEva.Level;
using Vegricht.RoguelikeEva.Pathfinding;

namespace Vegricht.RoguelikeEva.AI
{
    class Retreating : AIState
    {
        bool PatrolNext;

        public Retreating(Monster monster, HashSet<Hero> heroes)
        {
            Monster = monster;
            Heroes = heroes;
        }

        public override AIState DecideStrategy()
        {
            base.DecideStrategy();
            
            if (PatrolNext || Candidate == null || Monster.Tile.Room.View != Room.Visibility.Visible)
                return new Patrolling(Monster, Heroes);

            else
                return this;
        }

        public override Path InitiateTurn()
        {
            MapNode goal = Monster.Reachable.First();
            int bestDist = DistancesSum(goal);

            foreach (MapNode node in Monster.Reachable)
            {
                int currDist = DistancesSum(node);

                if (currDist > bestDist)
                {
                    goal = node;
                    bestDist = currDist;
                }
            }

            if (goal == Monster.Tile)
            {
                PatrolNext = true;
                return null;
            }

            Path.PathAction action;
            if (goal.OccupiedBy == null)
                action = Path.PathAction.Move;
            else if (goal.OccupiedBy.GetComponent<Monster>() != null)
                action = Path.PathAction.Attack;
            else
                action = Path.PathAction.Use;

            AStarPathFinder pf = new AStarPathFinder(action);
            Path path = pf.Find(Monster.Tile, goal);
            Monster.Tile.OccupiedBy = null;
            Monster.Tile = null;

            return path;
        }
        
        int ManhattanDistance(MapNode from, MapNode to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        int DistancesSum(MapNode goal)
        {
            int sum = 0;

            foreach (Hero hero in Heroes)
                if (hero.Alive && ScoreHero(hero) < 1)
                    sum += ManhattanDistance(hero.Tile, goal);

            return sum;
        }
    }
}