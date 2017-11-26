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
using Vegricht.RoguelikeEva.Pathfinding;

namespace Vegricht.RoguelikeEva.AI
{
    interface IState
    {
        IState DecideStrategy();
        Path InitiateTurn();
    }
}