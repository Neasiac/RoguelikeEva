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
    class Chasing : AIState
    {
        MapNode Target;

        public Chasing(Monster monster, MapNode target, HashSet<Hero> heroes)
        {
            Monster = monster;
            Target = target;
            Heroes = heroes;
        }

        public override AIState DecideStrategy()
        {
            base.DecideStrategy();

            if (Candidate == null)
                return new Patrolling(Monster, Heroes);

            else if (CandidateScore >= 0)
                return new Attacking(Monster, Candidate, Heroes);

            else
                return new Retreating(Monster, Heroes);
        }

        public override Path InitiateTurn()
        {
            Path.PathAction action;

            if (Target.OccupiedBy == null)
                action = Path.PathAction.Move;
            else if (Target.OccupiedBy.GetComponent<Monster>() != null)
            {
                action = Path.PathAction.Attack;
                Console.WriteLine("Chasing::InitiateTurn() found out that Target tile is occupied by a hero -> this shouldn't happen.");
            }
            else
                action = Path.PathAction.Use;
            
            AStarPathFinder pf = new AStarPathFinder(action);
            Path path = pf.Find(Monster.Tile, Target);
            Monster.Tile.OccupiedBy = null;
            Monster.Tile = null;

            return path;
        }
    }
}