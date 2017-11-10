using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Pathfinding;

namespace Vegricht.RoguelikeEva.Components
{
    class Chara : Component
    {
        public struct Status
        {
            public int Remaining { get; set; }
            public int Max { get; set; }

            public Status(int max)
            {
                Max = max;
                Remaining = Max;
            }
        }

        struct NodeDepthPair
        {
            public MapNode Node { get; private set; }
            public int Depth { get; private set; }

            public NodeDepthPair(MapNode node, int depth)
            {
                Node = node;
                Depth = depth;
            }
        }

        public bool Selected { get; set; }
        public HashSet<MapNode> Reachable { get; set; }
        public MapNode Tile { get; set; }
        public Status Speed { get; set; }

        const float NextWaypoint = 2;
        const float NextWaypointSquared = NextWaypoint * NextWaypoint;
        
        Transform Trans;
        Player Player;
        MapNode[] Path;
        int CurrentWaypoint;
        float MovementSpeed;
        Color HighlightColor;

        public Chara(Player player, MapNode position, Color highlightColor)
        {
            Player = player;
            Tile = position;
            HighlightColor = highlightColor;
        }
        
        public override void OnStart()
        {
            Trans = GetComponent<Transform>();
            MovementSpeed = 0.25f;
            Speed = new Status(8);

            if (Trans == null)
                throw new InvalidOperationException("Chara requires a Transform.");
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
            
            HandleMovement(gameTime);
        }

        bool CheckSelection()
        {
            if (!Selected // we're not selected
                && Player.SelectedNode == Tile) // player has clicked on our map node
            {
                Selected = true;
                Player.InvalidateSelection();
                FindReachableTiles();

                return true;
            }

            return false;
        }

        bool CheckDeselection()
        {
            if (Selected // we're selected
                && Player.SelectedNode != null // player has clicked on a map node
                && Player.SelectedNode.OccupiedBy != null // the map node is occupied
                && Player.SelectedNode.OccupiedBy.GetComponent<Chara>() != null) // it's occupied by an allied unit (including us)
            {
                Selected = false;
                Player.RequestDehighlight(Reachable);

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
                && Player.SelectedNode.OccupiedBy == null // the map node is not occupied FIXME: if it'S occupied by a monster or an item, we still wanna go!
                && Reachable != null && Reachable.Contains(Player.SelectedNode)) // destination tile is reachable
            {
                Player.InvalidateSelection();
                Player.Mode = Player.PlayerMode.Waiting;
                Player.RequestDehighlight(Reachable);

                AStarPathFinder pf = new AStarPathFinder();
                Path = pf.Find(Tile, Player.SelectedNode);
                CurrentWaypoint = 0;
                Tile.OccupiedBy = null;
                Tile = null;

                return true;
            }

            return false;
        }

        void HandleMovement(GameTime gameTime)
        {
            // No path -> no movement
            if (Path == null)
                return;

            // Are we there yet?
            if (CurrentWaypoint == Path.Length)
            {
                Path = null;
                Trans.Position = Tile.Position; // snap to grid
                Player.Mode = Player.PlayerMode.Thinking;
                Tile.OccupiedBy = Parent;

                if (Speed.Remaining > 0)
                    FindReachableTiles(); // can we keep going? -> show us where!
                else
                    Selected = false; // can we not? -> deselect!

                return;
            }
            
            // Move
            Vector2 dir = Path[CurrentWaypoint].Position - Trans.Position;
            if (dir.LengthSquared() > 0) dir.Normalize();

            Trans.Position += dir * MovementSpeed * gameTime.ElapsedGameTime.Milliseconds;

            // New waypoint?
            if (Vector2.DistanceSquared(Trans.Position, Path[CurrentWaypoint].Position) < NextWaypointSquared)
            {
                if (CurrentWaypoint > 0)
                {
                    Status speed = Speed;
                    speed.Remaining--;
                    Speed = speed;
                }

                Tile = Path[CurrentWaypoint++];
            }
        }

        void FindReachableTiles()
        {
            // Breadth first search with remaining speed as limit
            Reachable = new HashSet<MapNode>();
            Queue<NodeDepthPair> openQ = new Queue<NodeDepthPair>();
            HashSet<MapNode> openH = new HashSet<MapNode>();

            openQ.Enqueue(new NodeDepthPair(Tile, -1));
            openH.Add(Tile);
            
            while (openQ.Count > 0)
            {
                NodeDepthPair node = openQ.Dequeue();

                if (node.Depth == Speed.Remaining)
                    continue;

                if (node.Node.RoomID == Tile.RoomID || !node.Node.IsDoor) // FIXME: this only applies if we don't have view in the next room
                    foreach (MapNode neighbor in node.Node.Neighbors)
                    {
                        if (Reachable.Contains(neighbor))
                            continue;

                        if (!openH.Contains(neighbor))
                        {
                            openQ.Enqueue(new NodeDepthPair(neighbor, node.Depth + 1));
                            openH.Add(neighbor);
                        }
                    }

                Reachable.Add(node.Node);
            }

            // Visualize them
            Player.RequestHighlight(Reachable, HighlightColor);
        }
    }
}