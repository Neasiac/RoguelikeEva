using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Vegricht.RoguelikeEva.Serializable
{
    [Serializable]
    public class MapBlueprint
    {
        public byte StartRoomId { get; set; }
        public byte[,][] Encoding { get; set; }
        
        public static MapBlueprint FromFile(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (MapBlueprint)formatter.Deserialize(stream);
            }
        }

        public void SaveToFile(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }
    }
}