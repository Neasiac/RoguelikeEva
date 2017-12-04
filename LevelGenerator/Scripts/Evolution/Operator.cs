using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    abstract class Operator
    {
        public abstract Individual[] Operate(Individual[] parents);
    }
}