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

namespace Vegricht.RoguelikeEva.Serializable
{
    class MonsterPrototype
    {
        public string Name { get; private set; }
        public int Speed { get; private set; }
        public int HitPoints { get; private set; }
        public int Strength { get; private set; }
        public CombatType Type { get; private set; }
        public CombatType EnemyAwareness { get; set; }
        
        public MonsterPrototype(string name, int speed, int hp, int atk, CombatType type)
        {
            Name = name;
            Speed = speed;
            HitPoints = hp;
            Strength = atk;
            Type = type;
            
            EnemyAwareness = CombatType.Lizard |
                             CombatType.Paper |
                             CombatType.Rock |
                             CombatType.Scissors |
                             CombatType.Spock;
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}