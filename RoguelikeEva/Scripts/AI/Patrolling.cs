using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components;
using Vegricht.RoguelikeEva.Scenes.Core;
using System.Collections;
using Vegricht.RoguelikeEva.Level;
using Vegricht.RoguelikeEva.Pathfinding;

namespace Vegricht.RoguelikeEva.AI
{
    class Patrolling : IState
    {
        Monster Monster;

        public Patrolling(Monster monster)
        {
            Monster = monster;
        }

        public IState DecideStrategy()
        {
            return this;
        }

        public Path InitiateTurn()
        {
            // action upon completion
            Path.PathAction action;
            if (Monster.Tile.Neighbors[0].OccupiedBy == null)
                action = Path.PathAction.Move;
            else if (Monster.Tile.Neighbors[0].OccupiedBy.GetComponent<Hero>() != null)
                action = Path.PathAction.Attack;
            else
                action = Path.PathAction.Use;

            // if we're attacking, check whether we still can
            if (action == Path.PathAction.Attack)
            {
                if (Monster.AlreadyAttacked)
                    return null;

                Monster.AlreadyAttacked = true;
            }

            // initilize movement
            AStarPathFinder pf = new AStarPathFinder(action);
            Path p = pf.Find(Monster.Tile, Monster.Tile.Neighbors[0]);
            Monster.Tile.OccupiedBy = null;
            Monster.Tile = null;

            return p;
        }
    }
}