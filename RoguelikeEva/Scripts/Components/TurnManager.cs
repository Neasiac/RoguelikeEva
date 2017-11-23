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
        public HashSet<Monster> Monsters { get; private set; } = new HashSet<Monster>();
        
        public override void OnStart()
        {
            Player = GetComponent<Player>();
            CM = GetComponent<CombatManager>();

            if (Player == null)
                throw new InvalidOperationException("TurnManager requires a Player.");

            if (CM == null)
                throw new InvalidOperationException("TurnManager requires a CombatManager.");
        }
        
        public void NextTurn()
        {
            //Player.Mode = Player.PlayerMode.Waiting;

            // reset speed for heroes snd dehighlight any highlighted tiles
            foreach (Hero hero in Heroes)
            {
                Character.Status speed = hero.Speed;
                speed.Remaining = speed.Max;
                hero.Speed = speed;

                if (hero.Selected)
                {
                    hero.Selected = false;
                    Player.RequestDehighlight(hero.Reachable);
                }
            }

            // reset speed for monsters and play out their turns
            foreach (Monster mon in Monsters)
            {
                //
            }
        }
    }
}