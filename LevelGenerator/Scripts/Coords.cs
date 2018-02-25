using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegricht.LevelGenerator
{
    [Serializable]
    class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            // Szudzik's function
            return X >= Y ? X * X + X + Y : X + Y * Y;
        }

        public override bool Equals(object obj)
        {
            Coords other = (Coords)obj;
            return X == other.X && Y == other.Y;
        }
    }
}