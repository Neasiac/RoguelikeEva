using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class BooleanIndividual : Individual
    {
        public bool[] Genes { get; private set; }

        public override void RandomInitialization()
        {
            Random rnd = new Random();
            
            for (int i = 0; i < Genes.Length; i++)
                Genes[i] = rnd.NextDouble() > 0.5;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("[");
            foreach (bool b in Genes)
            {
                if (b)
                    s.Append("1");
                else
                    s.Append("0");
            }
            s.Append("]");
            return s.ToString();
        }

        public BooleanIndividual(int length)
        {
            Genes = new bool[length];
        }
    }
}