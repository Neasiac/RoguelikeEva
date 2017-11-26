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

namespace Vegricht.RoguelikeEva
{
    class MonsterPrototype
    {
        public string Name { get; private set; }
        public int Speed { get; private set; }
        public int HitPoints { get; private set; }
        public int Strength { get; private set; }
        public CombatManager.CombatType Type { get; private set; }
        public CombatManager.CombatType PossibleTypes { get; set; }
        
        public MonsterPrototype(string name, int speed, int hp, int atk, CombatManager.CombatType type)
        {
            Name = name;
            Speed = speed;
            HitPoints = hp;
            Strength = atk;
            Type = type;
            
            PossibleTypes = CombatManager.CombatType.Lizard |
                            CombatManager.CombatType.Paper |
                            CombatManager.CombatType.Rock |
                            CombatManager.CombatType.Scissors |
                            CombatManager.CombatType.Spock;
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}