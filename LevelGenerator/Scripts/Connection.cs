using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vegricht.LevelGenerator
{
    class Connection
    {
        public byte X { get; set; }
        public byte Y { get; set; }

        public Connection(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            // Szudzik's function (symetric)
            int dir1 = X >= Y ? X * X + X + Y : X + Y * Y;
            int dir2 = Y >= X ? Y * Y + Y + X : Y + X * X;

            return dir1 + dir2;
        }

        public override bool Equals(object obj)
        {
            Connection other = (Connection)obj;

            return X == other.X && Y == other.Y ||
                X == other.Y && Y == other.X;
        }
    }
}