using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components.Core;
using Vegricht.RoguelikeEva.Animations;
using Microsoft.Xna.Framework.Input;
using Vegricht.RoguelikeEva.Pathfinding;
using Vegricht.RoguelikeEva.Level;
using Vegricht.RoguelikeEva.AI;
using Vegricht.RoguelikeEva.Scenes.Core;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.RoguelikeEva.Components
{
    class Monster : Character
    {
        public bool Finished { get; set; }
        public bool TakingTurn { get; private set; }
        public MonsterPrototype Species { get; private set; }
        SpriteRenderer Renderer;
        AIState CurrentState;

        public override CombatType EnemyAwareness
        {
            get
            {
                return Species.EnemyAwareness;
            }

            set
            {
                Species.EnemyAwareness = value;
            }
        }

        public Monster(GameObject globalScripts, MapNode position, MonsterPrototype prototype)
        {
            CM = globalScripts.GetComponent<CombatManager>();
            Tile = position;
            Speed = new Status(prototype.Speed);
            HitPoints = new Status(prototype.HitPoints);
            Strength = prototype.Strength;
            Type = prototype.Type;
            Species = prototype;
            CurrentState = new Patrolling(this, globalScripts.GetComponent<TurnManager>().Heroes);
        }
        
        public override void OnStart()
        {
            Renderer = GetComponent<SpriteRenderer>();

            if (Renderer == null)
                throw new InvalidOperationException("Monster requires a SpriteRenderer.");

            base.OnStart();
        }

        public override void Update(GameTime gameTime)
        {
            if (Path != null && CurrentWaypoint < Path.Length)
            {
                if (!Renderer.Active && Path[CurrentWaypoint].Room.View == Room.Visibility.Visible)
                    Renderer.Active = true;

                else if (Renderer.Active && Path[CurrentWaypoint].Room.View != Room.Visibility.Visible)
                    Renderer.Active = false;
            }

            base.Update(gameTime);
        }
        
        public void InitiateTurn()
        {
            CurrentState = CurrentState.DecideStrategy();
            Path = CurrentState.InitiateTurn();
            
            if (Path == null)
                Finished = true;
            else
                TakingTurn = true;
        }

        protected override void FinalizePath()
        {
            base.FinalizePath();

            if (Path == null)
            {
                TakingTurn = false;
                Finished = true;
            }
        }
    }
}