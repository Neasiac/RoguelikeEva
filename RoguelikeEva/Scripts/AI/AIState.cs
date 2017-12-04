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
using Vegricht.RoguelikeEva.Pathfinding;
using Vegricht.RoguelikeEva.Level;

namespace Vegricht.RoguelikeEva.AI
{
    abstract class AIState
    {
        protected HashSet<Hero> Heroes;
        protected Monster Monster;
        protected Hero Candidate;
        protected sbyte CandidateScore;

        public virtual AIState DecideStrategy()
        {
            Monster.FindReachableTiles(node => Monster.Tile.Room.ID == node.Room.ID || node.Room.View != Room.Visibility.Visible);
            List<Hero> reachableHeroes = new List<Hero>();

            foreach (MapNode node in Monster.Reachable)
                if (node.OccupiedBy != null)
                {
                    Hero hero = node.OccupiedBy.GetComponent<Hero>();

                    if (hero != null)
                        reachableHeroes.Add(hero);
                }

            if (reachableHeroes.Count > 0)
            {
                Candidate = reachableHeroes[0];
                CandidateScore = ScoreHero(Candidate);

                foreach (Hero hero in reachableHeroes)
                {
                    sbyte currentScore = ScoreHero(hero);

                    if (currentScore > CandidateScore)
                    {
                        Candidate = hero;
                        CandidateScore = currentScore;
                    }
                }
            }
            
            return this;
        }

        public abstract Path InitiateTurn();

        protected sbyte ScoreHero(Hero hero)
        {
            sbyte score = 0;

            foreach (CombatType type in Enum.GetValues(typeof(CombatType)))
                if (hero.EnemyAwareness.HasFlag(type))
                {
                    TypeRelation result = Types.GetRelation(Monster.Type, type);

                    if (result == TypeRelation.Advantage) score++;
                    if (result == TypeRelation.Disadvantage) score--;
                }

            return score;
        }
    }
}