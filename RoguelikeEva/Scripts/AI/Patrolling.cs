using System;
using System.Collections.Generic;
using Vegricht.RoguelikeEva.Components;
using Vegricht.RoguelikeEva.Level;
using Vegricht.RoguelikeEva.Pathfinding;

namespace Vegricht.RoguelikeEva.AI
{
    class Patrolling : AIState
    {
        public Patrolling(Monster monster, HashSet<Hero> heroes)
        {
            Monster = monster;
            Heroes = heroes;
        }

        public override AIState DecideStrategy()
        {
            base.DecideStrategy();

            if (Candidate == null)
                return this;

            else if (CandidateScore >= 0)
                return new Attacking(Monster, Candidate, Heroes);

            else if (Monster.Tile.Room.View == Room.Visibility.Visible)
                return new Retreating(Monster, Heroes);

            else
                return this;
        }

        public override Path InitiateTurn()
        {
            Random rnd = new Random();
            MapNode goal = null;
            int attempts = 0;

            while (goal == null || goal.OccupiedBy != null)
            {
                goal = Monster.Tile.Neighbors[rnd.Next(0, Monster.Tile.Neighbors.Count)];

                if (attempts++ > 10)
                    return null;
            }
            
            AStarPathFinder pf = new AStarPathFinder(Path.PathAction.Move);
            Path path = pf.Find(Monster.Tile, goal);
            Monster.Tile.OccupiedBy = null;
            Monster.Tile = null;

            return path;
        }
    }
}