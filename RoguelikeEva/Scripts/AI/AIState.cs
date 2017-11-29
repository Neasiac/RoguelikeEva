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
        protected int CandidateScore;

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
                    int currentScore = ScoreHero(hero);

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

        protected int ScoreHero(Hero hero)
        {
            byte score = 0;

            foreach (CombatManager.CombatType type in Enum.GetValues(typeof(CombatManager.CombatType)))
                if (hero.EnemyAwareness.HasFlag(type))
                {
                    CombatManager.TypeRelation result = CombatManager.GetRelation(Monster.Type, type);

                    if (result == CombatManager.TypeRelation.Advantage) score++;
                    if (result == CombatManager.TypeRelation.Disadvantage) score--;
                }

            return score;
        }
    }
}