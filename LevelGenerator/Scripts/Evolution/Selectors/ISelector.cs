﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution.Individuals;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Selectors
{
    interface ISelector
    {
        Individual[] Select(int amount, Individual[] population);
    }
}