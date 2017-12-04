using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator
{
    struct Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static Rectangle Zero = new Rectangle(0, 0, 0, 0);
        
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Overlaps(Rectangle other)
        {
            bool xOverlap = ValueInRange(X, other.X, other.X + other.Width) ||
                            ValueInRange(other.X, X, X + Width);

            bool yOverlap = ValueInRange(Y, other.Y, other.Y + other.Height) ||
                            ValueInRange(other.Y, Y, Y + Height);

            return xOverlap && yOverlap;
        }
        
        bool ValueInRange(int value, int min, int max)
        {
            return value >= min && value < max;
        }
    }
}