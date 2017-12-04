using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Scenes.Core;
using Vegricht.RoguelikeEva.Level;

namespace Vegricht.RoguelikeEva.Components
{
    class CombatManager : Component
    {
        public bool MidFight
        {
            get
            {
                return CombatInitiator != null;
            }
        }

        TurnManager TM;
        GameObject Sword;
        Character CombatInitiator;
        int CombatAnimationTotal;
        int CombatAnimationCountDown;
        
        public CombatManager(GameObject sword)
        {
            Sword = sword ?? throw new ArgumentNullException();
            CombatAnimationTotal = Sword.GetComponent<Animator>().Animations["rotate"].Length;
        }
        
        public override void OnStart()
        {
            TM = GetComponent<TurnManager>();

            if (TM == null)
                throw new InvalidOperationException("CombatManager requires a TurnManager.");
        }

        public override void Update(GameTime gameTime)
        {
            if (Sword.Active)
            {
                CombatAnimationCountDown += gameTime.ElapsedGameTime.Milliseconds;

                if (CombatAnimationCountDown >= CombatAnimationTotal)
                {
                    CombatAnimationCountDown = 0;
                    Sword.Active = false;
                    CombatInitiator.Attacking = false;
                    CombatInitiator = null;
                }
            }
        }

        public void Attack(Character attacker, Character opponent)
        {
            CombatInitiator = attacker;
            attacker.Attacking = true;

            Sword.GetComponent<Transform>().Position = attacker.GetComponent<Transform>().Position;
            Sword.Active = true;

            SingleAttack(attacker, opponent);

            if (!opponent.Alive)
                return;

            SingleAttack(opponent, attacker);
        }

        void SingleAttack(Character attacker, Character opponent)
        {
            TypeRelation relation = Types.GetRelation(attacker.Type, opponent.Type);
            opponent.EnemyAwareness &= Types.GetPossibleTypes(attacker.Type, relation);
            float multiplier = relation == TypeRelation.Neutral ? 1 : (relation == TypeRelation.Advantage ? 2 : 0.5f);
            
            Character.Status hp = opponent.HitPoints;
            hp.Remaining -= (int)Math.Round(multiplier * attacker.Strength);
            opponent.HitPoints = hp;

            if (!opponent.Alive)
            {
                opponent.Parent.Active = false;

                if (opponent is Hero && !((Hero)opponent).AlliedUnitInRoom(opponent.Tile.Room))
                {
                    opponent.Tile.Room.UpdateGraphics(Room.Visibility.Darkened);
                    attacker.GetComponent<SpriteRenderer>().Active = false;
                }
            }
        }
    }
}