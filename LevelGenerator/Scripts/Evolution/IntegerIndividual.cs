using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution
{
    class IntegerIndividual : Individual
    {
        public int[] Genes { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }

        public override void RandomInitialization()
        {
            Random rnd = new Random();

            for (int i = 0; i < Genes.Length; i++)
                Genes[i] = rnd.Next(Max - Min) + Min;
        }

        public IntegerIndividual(int length, int min, int max)
        {
            Genes = new int[length];
            Min = min;
            Max = max;
        }
    }
}