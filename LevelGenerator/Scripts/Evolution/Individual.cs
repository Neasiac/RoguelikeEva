using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    abstract class Individual : ICloneable
    {
        public double Objective { get; set; }
        public double Fitness { get; set; }

        public abstract void RandomInitialization();

        public object Clone()
        {
            //return MemberwiseClone();
            return this.Copy();

            Individual ind = (Individual)MemberwiseClone();
            ind.Fitness = double.NegativeInfinity;
            //ind.Objective = Objective;

            return ind;
        }
    }
}