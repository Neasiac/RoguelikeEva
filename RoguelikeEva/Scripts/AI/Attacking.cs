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
using Vegricht.RoguelikeEva.Level;
using Vegricht.RoguelikeEva.Pathfinding;

namespace Vegricht.RoguelikeEva.AI
{
    class Attacking : AIState
    {
        public Attacking(Monster monster, Hero candidate, HashSet<Hero> heroes)
        {
            Monster = monster;
            Candidate = candidate;
            Heroes = heroes;
        }

        public override AIState DecideStrategy()
        {
            base.DecideStrategy();

            if (Candidate == null)
                return new Chasing(Monster, Candidate.Tile, Heroes);

            else if (CandidateScore >= 0)
                return this;

            else
                return new Retreating(Monster, Heroes);
        }

        public override Path InitiateTurn()
        {
            AStarPathFinder pf = new AStarPathFinder(Path.PathAction.Attack);
            Path path = pf.Find(Monster.Tile, Candidate.Tile);
            Monster.Tile.OccupiedBy = null;
            Monster.Tile = null;

            return path;
        }
    }
}