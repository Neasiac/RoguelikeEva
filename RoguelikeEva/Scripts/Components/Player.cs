﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;

namespace Vegricht.RoguelikeEva.Components
{
    class Player : Component
    {
        public MapNode SelectedNode { get; private set; }
        public PlayerMode Mode { get; set; }

        bool SelectionInvalidated;
        Color HighlightColor;
        HashSet<MapNode> ToHighlight;
        HashSet<MapNode> ToDehighlight;
        TurnManager TM;

        public enum PlayerMode
        {
            Thinking,
            Waiting
        }

        public void SelectNode(MapNode node)
        {
            SelectedNode = node ?? throw new ArgumentNullException();
        }

        public void InvalidateSelection()
        {
            SelectionInvalidated = true;
        }
        
        public void RequestHighlight(HashSet<MapNode> nodes, Color color)
        {
            ToHighlight = nodes;
            HighlightColor = color;
        }

        public void RequestDehighlight(HashSet<MapNode> nodes)
        {
            ToDehighlight = nodes;
        }

        public HashSet<Hero> GetHeroes()
        {
            return TM.Heroes;
        }

        public override void OnStart()
        {
            TM = GetComponent<TurnManager>();

            if (TM == null)
                throw new InvalidOperationException("Player requires a TurnManager.");
        }

        public override void Update(GameTime gameTime)
        {
            /*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                TerminateGame();*/

            /*if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                //
            }*/

            if (SelectionInvalidated)
            {
                SelectedNode = null;
                SelectionInvalidated = false;
            }
            
            if (ToDehighlight != null)
            {
                foreach (MapNode node in ToDehighlight)
                    node.GetComponent<SpriteRenderer>().Color = Color.White;

                ToDehighlight = null;
            }

            if (ToHighlight != null)
            {
                foreach (MapNode node in ToHighlight)
                    node.GetComponent<SpriteRenderer>().Color = HighlightColor;

                ToHighlight = null;
            }
        }
    }
}