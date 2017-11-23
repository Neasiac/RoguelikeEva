﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components
{
    class AnimationStateMachine : Component
    {
        public Dictionary<string, Dictionary<string, Predicate<Character>>> Transitions { get; private set; }
        public string CurrentState { get; private set; }

        Character Chara;
        Animator Anim;

        public AnimationStateMachine(string initialState)
        {
            CurrentState = initialState;
            Transitions = new Dictionary<string, Dictionary<string, Predicate<Character>>>();
        }
        
        public override void OnStart()
        {
            Chara = GetComponent<Character>();
            Anim = GetComponent<Animator>();

            if (Chara == null)
                throw new InvalidOperationException("AnimationStateMachine requires a Character.");

            if (Anim == null)
                throw new InvalidOperationException("AnimationStateMachine requires an Animator.");
        }

        public override void Update(GameTime gameTime)
        {
            foreach (string to in Transitions[CurrentState].Keys)
            {
                if (Transitions[CurrentState][to](Chara))
                {
                    CurrentState = to;
                    Anim.Play(to);
                    break;
                }
            }
        }

        public void AddTransition(string from, string to, Predicate<Character> condition)
        {
            if (!Transitions.ContainsKey(from))
                Transitions.Add(from, new Dictionary<string, Predicate<Character>>());

            else if (Transitions[from].ContainsKey(to))
                throw new InvalidOperationException("The state machine already contains this transition.");

            Transitions[from].Add(to, condition);
        }
    }
}