using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Vegricht.RoguelikeEva.Level;
using System.Drawing.Imaging;

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

        public void SaveMapToFile(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public void SaveImageToFile(string filepath, int width, int height)
        {
            int tileWidth = width / Encoding.GetLength(0);
            int tileHeight = height / Encoding.GetLength(1);

            using (Font font = new Font("Arial", 10))
            using (Brush brush = new SolidBrush(Color.Black))
            using (Pen pen = new Pen(brush, 1))
            using (Bitmap map = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(map))
            {
                using (Brush bleach = new SolidBrush(Color.White))
                    g.FillRectangle(bleach, 0, 0, width, height);

                for (int x = 0; x < Encoding.GetLength(0); x++)
                    for (int y = 0; y < Encoding.GetLength(1); y++)
                    {
                        if (Encoding[x, y][0] == 0)
                            DrawWall(g, pen, x, y, tileWidth, tileHeight);
                        else
                        {
                            DrawObstacle(g, pen, x, y, x - 1, y, x * tileWidth, y * tileHeight, x * tileWidth, (y + 1) * tileHeight);
                            DrawObstacle(g, pen, x, y, x, y - 1, x * tileWidth, y * tileHeight, (x + 1) * tileWidth, y * tileHeight);
                            DrawObstacle(g, pen, x, y, x + 1, y, (x + 1) * tileWidth, y * tileHeight, (x + 1) * tileWidth, (y + 1) * tileHeight);
                            DrawObstacle(g, pen, x, y, x, y + 1, x * tileWidth, (y + 1) * tileHeight, (x + 1) * tileWidth, (y + 1) * tileHeight);
                            DrawRoomId(g, font, brush, x, y, tileWidth, tileHeight);
                        }
                    }

                map.Save(filepath, ImageFormat.Png);
            }
        }
        
        public bool IsOnMap(int x, int y)
        {
            return x >= 0 && x < Encoding.GetLength(0) && y >= 0 && y < Encoding.GetLength(1);
        }

        void DrawRoomId(Graphics g, Font font, Brush brush, int x, int y, float tileWidth, float tileHeight)
        {
            int above = IsOnMap(x, y - 1) ? Encoding[x, y - 1][0] : -1;
            int toleft = IsOnMap(x - 1, y) ? Encoding[x - 1, y][0] : -1;

            if (above != Encoding[x, y][0] && toleft != Encoding[x, y][0])
                g.DrawString(Encoding[x, y][0] + "", font, brush, x * tileWidth, y * tileHeight);
        }

        void DrawObstacle(Graphics g, Pen pen, int x, int y, int xN, int yN, int xStart, int yStart, int xEnd, int yEnd)
        {
            // is candidate on map?
            if (!IsOnMap(xN, yN))
                return;

            // is candidate in the same room OR if we're both door, do we have the same door ID?
            if (Encoding[xN, yN][0] != Encoding[x, y][0] && (Encoding[x, y][1] == 0 || Encoding[x, y][1] != Encoding[xN, yN][1]))
                g.DrawLine(pen, xStart, yStart, xEnd, yEnd);
        }

        void DrawWall(Graphics g, Pen pen, int x, int y, float tileWidth, float tileHeight)
        {
            g.DrawLine(pen, x * tileWidth, (2 * y + 1) * tileHeight / 2, (2 * x + 1) * tileWidth / 2, y * tileHeight);
            g.DrawLine(pen, x * tileWidth, (y + 1) * tileHeight, (x + 1) * tileWidth, y * tileHeight);
            g.DrawLine(pen, (2 * x + 1) * tileWidth / 2, (y + 1) * tileHeight, (x + 1) * tileWidth, (2 * y + 1) * tileHeight / 2);
        }
    }
}