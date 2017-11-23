using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Pathfinding;
using Vegricht.RoguelikeEva.Level;

namespace Vegricht.RoguelikeEva.Components
{
    class Monster : Character
    {
        public Monster(CombatManager cm, MapNode position, int speed, int hp, int atk, CombatManager.CombatType type)
        {
            CM = cm;
            Tile = position;
            Speed = new Status(speed);
            HitPoints = new Status(hp);
            Strength = atk;
            Type = type;
        }
        
        public override void Update(GameTime gameTime)
        {
            // TODO stuff

            base.Update(gameTime);
        }
    }
}