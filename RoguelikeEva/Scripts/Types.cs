using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Scenes.Core;
using Vegricht.RoguelikeEva.Level;

namespace Vegricht.RoguelikeEva
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

    public class Types
    {
        public static TypeRelation GetRelation(CombatType attacker, CombatType opponent)
        {
            if (attacker == opponent)
                return TypeRelation.Neutral;

            if (attacker == CombatType.Rock && (opponent == CombatType.Lizard || opponent == CombatType.Scissors) ||
                attacker == CombatType.Paper && (opponent == CombatType.Rock || opponent == CombatType.Spock) ||
                attacker == CombatType.Scissors && (opponent == CombatType.Paper || opponent == CombatType.Lizard) ||
                attacker == CombatType.Lizard && (opponent == CombatType.Spock || opponent == CombatType.Paper) ||
                attacker == CombatType.Spock && (opponent == CombatType.Scissors || opponent == CombatType.Rock))
                return TypeRelation.Advantage;

            return TypeRelation.Disadvantage;
        }

        public static CombatType GetPossibleTypes(CombatType attacker, TypeRelation result)
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