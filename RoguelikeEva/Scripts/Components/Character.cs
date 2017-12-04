using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Pathfinding;
using Vegricht.RoguelikeEva.Level;

namespace Vegricht.RoguelikeEva.Components
{
    abstract class Character : Component
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
        
        public HashSet<MapNode> Reachable { get; set; }
        public MapNode Tile { get; set; }
        public Status Speed { get; set; }
        public Status HitPoints { get; set; }
        public int Strength { get; set; }
        public bool Attacking { get; set; }
        public bool AlreadyAttacked { get; set; }
        public Path Path { get; protected set; }
        public int CurrentWaypoint { get; protected set; }
        public CombatType Type { get; protected set; }
        public abstract CombatType EnemyAwareness { get; set; }

        const float NextWaypoint = 2;
        const float NextWaypointSquared = NextWaypoint * NextWaypoint;
        
        protected Transform Trans;
        protected CombatManager CM;
        protected float MovementSpeed;
        
        public bool Alive
        {
            get
            {
                return HitPoints.Remaining > 0;
            }
        }
        
        public override void OnStart()
        {
            Trans = GetComponent<Transform>();
            MovementSpeed = 0.25f;
            
            if (Trans == null)
                throw new InvalidOperationException("Character requires a Transform.");
        }

        public override void Update(GameTime gameTime)
        {
            // No path -> no movement
            // Attacking -> playing the sword animation -> no movement
            if (Path == null || Attacking)
                return;

            // Are we there yet?
            if (CurrentWaypoint == Path.Length
                || Path.Action == Path.PathAction.Retreat && CurrentWaypoint > 0 && Path[CurrentWaypoint - 1].OccupiedBy == null)
            {
                FinalizePath();
                return;
            }

            // Move
            Vector2 dir = Path[CurrentWaypoint].Position - Trans.Position;
            if (dir.LengthSquared() > 0) dir.Normalize();

            Trans.Position += dir * MovementSpeed * gameTime.ElapsedGameTime.Milliseconds;

            // New waypoint?
            if (Vector2.DistanceSquared(Trans.Position, Path[CurrentWaypoint].Position) < NextWaypointSquared)
                Tile = Path[CurrentWaypoint++];
        }

        public virtual void FindReachableTiles(Predicate<MapNode> expandCondition)
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

                if (expandCondition == null || expandCondition(node.Node))
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
        }

        protected virtual void FinalizePath()
        {
            // Update speed
            if (Path.Action != Path.PathAction.Retreat)
            {
                Status speed = Speed;
                speed.Remaining -= Path.Length - 1;
                Speed = speed;
            }

            // Using an item?
            if (Path.Action == Path.PathAction.Use)
                Tile.OccupiedBy.GetComponent<Item>().Use(this);

            // Attacking?
            bool startedRetreating = false;
            if (Path.Action == Path.PathAction.Attack)
            {
                Character opponent = Tile.OccupiedBy.GetComponent<Character>();
                CM.Attack(this, opponent);
                
                // Both participants are still alive -> go backward to first empty spot
                if (opponent.Alive && Alive)
                {
                    Path.Retreat();
                    CurrentWaypoint = 0;
                    startedRetreating = true;
                }
            }

            // Finalize
            if (!startedRetreating)
            {
                Path = null;
                CurrentWaypoint = 0;
                Trans.Position = Tile.Position; // snap to grid

                if (Alive)
                    Tile.OccupiedBy = Parent;
            }
        }
    }
}