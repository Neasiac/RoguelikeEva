using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Pathfinding;
using Vegricht.RoguelikeEva.Level;
using Vegricht.RoguelikeEva.Scenes.Core;

namespace Vegricht.RoguelikeEva.Components
{
    class Hero : Character
    {
        public string Name { get; private set; }
        public bool Selected { get; set; }
        public override CombatType EnemyAwareness { get; set; }

        Player Player;
        Color HighlightColor;
        
        public Hero(string name, GameObject globalScripts, MapNode position, Color highlightColor, int speed, int hp, int atk, CombatType type)
        {
            Name = name;
            CM = globalScripts.GetComponent<CombatManager>();
            Player = globalScripts.GetComponent<Player>();
            Tile = position;
            HighlightColor = highlightColor;
            Speed = new Status(speed);
            HitPoints = new Status(hp);
            Strength = atk;
            Type = type;

            EnemyAwareness = CombatType.Lizard |
                             CombatType.Paper |
                             CombatType.Rock |
                             CombatType.Scissors |
                             CombatType.Spock;
        }
        
        public override void Update(GameTime gameTime)
        {
            // Can player currently choose an action?
            if (Player.Mode == Player.PlayerMode.Thinking)
            {
                // Only one action per update!
                if (CheckSelection()) return;
                if (CheckDeselection()) return;
                if (CheckMovementInitialization()) return;
            }

            base.Update(gameTime);
        }

        bool CheckSelection()
        {
            if (!Selected // we're not selected
                && Player.SelectedNode == Tile) // player has clicked on our map node
            {
                Selected = true;
                Player.InvalidateSelection();
                FindReachableTiles(ExpandCondition);

                return true;
            }

            return false;
        }

        bool CheckDeselection()
        {
            if (Selected // we're selected
                && Player.SelectedNode != null // player has clicked on a map node
                && Player.SelectedNode.OccupiedBy != null // the map node is occupied
                && Player.SelectedNode.OccupiedBy.GetComponent<Hero>() != null) // it's occupied by an allied unit (including us)
            {
                Selected = false;
                Player.InvalidateHighlight(Reachable);

                if (Player.SelectedNode == Tile)
                    Player.InvalidateSelection();

                return true;
            }

            return false;
        }

        bool CheckMovementInitialization()
        {
            if (Selected // we're selected
                && Player.SelectedNode != null // player has clicked on a map node
                && (Player.SelectedNode.OccupiedBy == null || Player.SelectedNode.OccupiedBy.GetComponent<Hero>() == null) // the map node is not occupied by an allied unit
                && Reachable != null && Reachable.Contains(Player.SelectedNode)) // destination tile is reachable
            {
                // action upon completion
                Path.PathAction action;
                if (Player.SelectedNode.OccupiedBy == null)
                    action = Path.PathAction.Move;
                else if (Player.SelectedNode.OccupiedBy.GetComponent<Monster>() != null)
                    action = Path.PathAction.Attack;
                else
                    action = Path.PathAction.Use;

                // if we're attacking, check whether we still can
                if (action == Path.PathAction.Attack)
                {
                    if (AlreadyAttacked)
                        return false;

                    AlreadyAttacked = true;
                }

                // forbid user interaction while moving
                Player.InvalidateSelection();
                Player.Mode = Player.PlayerMode.Waiting;
                Player.InvalidateHighlight(Reachable);

                // initilize movement
                AStarPathFinder pf = new AStarPathFinder(action);
                Path = pf.Find(Tile, Player.SelectedNode);
                Tile.OccupiedBy = null;
                Tile = null;

                return true;
            }

            return false;
        }

        bool ExpandCondition(MapNode node)
        {
            return node.Room.View == Room.Visibility.Visible;
        }

        public bool AlliedUnitInRoom(Room room)
        {
            foreach (Hero chara in Player.GetHeroes())
                if (chara.Tile.Room == room && chara.Alive)
                    return true;

            return false;
        }

        public override void FindReachableTiles(Predicate<MapNode> expandCondition)
        {
            base.FindReachableTiles(expandCondition);

            // Visualize reachable tiles
            Player.RequestHighlight(Reachable, HighlightColor);
        }

        protected override void FinalizePath()
        {
            // if destination room isn't visible, make it
            if (Tile.Room.View != Room.Visibility.Visible)
                Tile.Room.UpdateGraphics(Room.Visibility.Visible);

            // if there are no allied units left in the starting room, darken it
            if (!AlliedUnitInRoom(Path[0].Room))
                Path[0].Room.UpdateGraphics(Room.Visibility.Darkened);

            base.FinalizePath();

            if (Path == null)
            {
                Player.Mode = Player.PlayerMode.Thinking;

                if (Speed.Remaining > 0 && Alive)
                    FindReachableTiles(ExpandCondition); // can we keep going? -> show us where!
                else
                    Selected = false; // can we not? -> deselect!
            }
        }
    }
}