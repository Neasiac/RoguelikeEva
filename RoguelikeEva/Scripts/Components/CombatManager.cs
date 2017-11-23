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
    class CombatManager : Component
    {
        public enum CombatType
        {
            Rock,
            Paper,
            Scissors,
            Lizard,
            Spock
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
            Character.Status hp = opponent.HitPoints;
            hp.Remaining -= (int)Math.Round(GetMultiplier(attacker.Type, opponent.Type) * attacker.Strength);
            opponent.HitPoints = hp;

            if (!opponent.Alive)
                opponent.Parent.Active = false;
        }

        float GetMultiplier(CombatType attacker, CombatType opponent)
        {
            if (attacker == opponent)
                return 1;

            if (attacker == CombatType.Rock && (opponent == CombatType.Lizard || opponent == CombatType.Scissors) ||
                attacker == CombatType.Paper && (opponent == CombatType.Rock || opponent == CombatType.Spock) ||
                attacker == CombatType.Scissors && (opponent == CombatType.Scissors || opponent == CombatType.Lizard) ||
                attacker == CombatType.Lizard && (opponent == CombatType.Spock || opponent == CombatType.Paper) ||
                attacker == CombatType.Spock && (opponent == CombatType.Scissors || opponent == CombatType.Rock))
                return 2;

            return 0.5f;
        }
    }
}