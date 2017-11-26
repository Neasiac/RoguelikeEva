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
        [Flags]
        public enum CombatType : byte
        {
            Rock = 1 << 0,
            Paper = 1 << 1,
            Scissors = 1 << 2,
            Lizard = 1 << 3,
            Spock = 1 << 4
        }
        
        public enum TypeRelation
        {
            Advantage,
            Disadvantage,
            Neutral
        }

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
            TypeRelation relation = GetRelation(attacker.Type, opponent.Type);
            float multiplier = relation == TypeRelation.Neutral ? 1 : (relation == TypeRelation.Advantage ? 2 : 0.5f);

            if (opponent is Monster)
                ((Monster)opponent).UpdatePossibleTypes(GetPossibleTypes(attacker.Type, relation));

            Character.Status hp = opponent.HitPoints;
            hp.Remaining -= (int)Math.Round(multiplier * attacker.Strength);
            opponent.HitPoints = hp;

            if (!opponent.Alive)
            {
                opponent.Parent.Active = false;

                if (opponent is Hero && !((Hero)opponent).AlliedUnitInRoom(opponent.Tile.Room))
                    TM.Player.RequestDarkenRoom(opponent.Tile.Room);
            }
        }

        TypeRelation GetRelation(CombatType attacker, CombatType opponent)
        {
            if (attacker == opponent)
                return TypeRelation.Neutral;

            if (attacker == CombatType.Rock && (opponent == CombatType.Lizard || opponent == CombatType.Scissors) ||
                attacker == CombatType.Paper && (opponent == CombatType.Rock || opponent == CombatType.Spock) ||
                attacker == CombatType.Scissors && (opponent == CombatType.Scissors || opponent == CombatType.Lizard) ||
                attacker == CombatType.Lizard && (opponent == CombatType.Spock || opponent == CombatType.Paper) ||
                attacker == CombatType.Spock && (opponent == CombatType.Scissors || opponent == CombatType.Rock))
                return TypeRelation.Advantage;

            return TypeRelation.Disadvantage;
        }
        
        CombatType GetPossibleTypes(CombatType attacker, TypeRelation result)
        {
            if (result == TypeRelation.Neutral)
                return attacker;

            switch (attacker)
            {
                case CombatType.Lizard:
                    return result == TypeRelation.Advantage ?
                        CombatType.Spock | CombatType.Paper :
                        CombatType.Scissors | CombatType.Rock;

                case CombatType.Paper:
                    return result == TypeRelation.Advantage ?
                        CombatType.Rock | CombatType.Spock :
                        CombatType.Scissors | CombatType.Lizard;

                case CombatType.Rock:
                    return result == TypeRelation.Advantage ?
                        CombatType.Scissors | CombatType.Lizard :
                        CombatType.Paper | CombatType.Spock;

                case CombatType.Scissors:
                    return result == TypeRelation.Advantage ?
                        CombatType.Paper | CombatType.Lizard :
                        CombatType.Rock | CombatType.Spock;

                case CombatType.Spock:
                    return result == TypeRelation.Advantage ?
                        CombatType.Scissors | CombatType.Rock :
                        CombatType.Paper | CombatType.Lizard;

                default:
                    throw new InvalidOperationException("This will never happen.");
            }
        }
    }
}