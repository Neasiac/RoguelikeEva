using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator.Evolution.Individuals
{
    [Serializable]
    abstract class Individual : ICloneable
    {
        public int Front { get; set; }
        public double CrowdingDistance { get; set; }
        public double[] MultiObjective { get; set; }

        public abstract void RandomInitialization();

        public object Clone()
        {
            return this.Copy();
        }
        
        public void SaveToFile(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public static Individual FromFile(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Individual)formatter.Deserialize(stream);
            }
        }
    }
}