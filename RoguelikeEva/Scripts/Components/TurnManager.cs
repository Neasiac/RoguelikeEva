using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components
{
    class TurnManager : Component
    {
        public Player Player { get; set; }
        public CombatManager CM { get; set; }
        public HashSet<Hero> Heroes { get; private set; } = new HashSet<Hero>();
        public List<Monster> Monsters { get; private set; } = new List<Monster>();
        int CurrentEnemy;
        
        public override void OnStart()
        {
            Player = GetComponent<Player>();
            CM = GetComponent<CombatManager>();

            if (Player == null)
                throw new InvalidOperationException("TurnManager requires a Player.");

            if (CM == null)
                throw new InvalidOperationException("TurnManager requires a CombatManager.");
        }

        public override void Update(GameTime gameTime)
        {
            if (Player.Mode == Player.PlayerMode.EnemyTurn)
            {
                if (Monsters.Count == 0)
                {
                    Player.Mode = Player.PlayerMode.Thinking;
                    return;
                }

                if ((Monsters[CurrentEnemy].Finished || !Monsters[CurrentEnemy].Alive) && !CM.MidFight)
                {
                    Monsters[CurrentEnemy].Finished = false;

                    if (++CurrentEnemy == Monsters.Count)
                    {
                        CurrentEnemy = 0;
                        Player.Mode = Player.PlayerMode.Thinking;
                        return;
                    }
                }

                if (!Monsters[CurrentEnemy].TakingTurn && !CM.MidFight)
                    Monsters[CurrentEnemy].InitiateTurn();
            }
        }

        public void NextTurn()
        {
            Player.Mode = Player.PlayerMode.EnemyTurn;

            // reset speed for heroes snd dehighlight any highlighted tiles
            foreach (Hero hero in Heroes)
            {
                if (!hero.Alive)
                    continue;

                Character.Status speed = hero.Speed;
                speed.Remaining = speed.Max;
                hero.Speed = speed;
                hero.AlreadyAttacked = false;

                if (hero.Selected)
                {
                    hero.Selected = false;
                    Player.InvalidateHighlight(hero.Reachable);
                }
            }

            // reset speed for monsters
            foreach (Monster mon in Monsters)
            {
                if (!mon.Alive)
                    continue;

                Character.Status speed = mon.Speed;
                speed.Remaining = speed.Max;
                mon.Speed = speed;
                mon.AlreadyAttacked = false;
            }
        }
    }
}