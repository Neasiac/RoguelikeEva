using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vegricht.LevelGenerator.Evolution;
using Vegricht.RoguelikeEva.Serializable;

namespace Vegricht.LevelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Generate(@"C:\Users\Jan\Source\Repos\RoguelikeEva\map.bin");
            return;

            List<double> weights = new List<double>();

            using (StreamReader stream = new StreamReader(@"C:\Users\Jan\workspace\Evolution\resources\packingInput-easier.txt"))
            {
                string line = null;

                while ((line = stream.ReadLine()) != null)
                    weights.Add(double.Parse(line));
            }
            
            EvolutionaryAlgorithm eva = new EvolutionaryAlgorithm();
            
            /*eva.Operators.Add(new BitFlipMutation(0.04, 0.02));
            eva.Operators.Add(new OnePointCrossover(0.8));
            eva.SampleIndividual = new BooleanIndividual(25);
            eva.Fitness = new SimpleFitness();
            eva.Selector.Add(new RouletteWheelSelector());
            eva.Run(e => e.Population.No == 50, 100, Console.Out, (writer, gen, ind) => writer.WriteLine("Generation " + gen.No + ", fitness: " + ind.Fitness + " " + ind.ToString()));*/

            int K = 10;
            double mutProb = 0.1;
            double xoverProb = 0.8;

            BinPackingFitness fitness = new BinPackingFitness(weights, K);
            eva.Operators.Add(new Heavy2LightMutation(mutProb, weights, K));
            eva.Operators.Add(new SwapMutation(mutProb));
            eva.Operators.Add(new OnePointCrossover(xoverProb));
            eva.SampleIndividual = new IntegerIndividual(weights.Count, 0, K);
            eva.Fitness = fitness;
            eva.Selector.Add(new RouletteWheelSelector());
            eva.Run(e => e.Population.No == 500, 500, Console.Out, (writer, gen, ind) => writer.WriteLine("Generation " + gen.No + ": " + ind.Objective + " [" + String.Join(", ", fitness.getBinWeights(ind)) + "]"));
            
            Console.ReadKey();
        }

        static void Generate(string filename)
        {
            int w = 30, h = 20, rooms = 20;
            const int numheroes = 5;

            MapBlueprint map = new MapBlueprint();
            map.StartRoomId = 1;
            map.Encoding = new byte[w, h][];

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    map.Encoding[x, y] = new byte[3];

            Rectangle[] initialRooms = new Rectangle[rooms];
            Random rnd = new Random();
            /*int maxW = (int)(w * 2f / rooms);
            int maxH = (int)(h * 2f / rooms);*/
            int maxW = 7, maxH = 7;
            int index = 0;

            /*do
                initialRooms[index] = new Rectangle(rnd.Next(w - maxW), rnd.Next(h - maxH), rnd.Next(1, maxW), rnd.Next(1, maxH));
            while (++attempts < 1000 && initialRooms[0].Width * initialRooms[0].Height >= numheroes &&
                  (Overlaps(index, initialRooms) || ++index < rooms));*/

            do // FIXME: slow!
            {
                initialRooms = new Rectangle[rooms];
                index = 0;
                int attempts = 0;

                do
                    initialRooms[index] = new Rectangle(rnd.Next(w - maxW), rnd.Next(h - maxH), rnd.Next(1, maxW), rnd.Next(1, maxH));
                while (++attempts < 1000 && (Overlaps(index, initialRooms) || ++index < rooms));

                if (attempts == 1000)
                {
                    Rectangle[] tmp = new Rectangle[index];
                    Array.Copy(initialRooms, tmp, index);
                    initialRooms = tmp;
                }
            }
            while (initialRooms[0].Width * initialRooms[0].Height < numheroes);

            for (int i = 0; i < initialRooms.Length; i++)
            {
                Rectangle r = Rectangle.Zero;
                int width = 1;

                do
                {
                    r = new Rectangle(initialRooms[i].X + initialRooms[i].Width,
                                      initialRooms[i].Y,
                                      width++,
                                      initialRooms[i].Height);

                    if (r.X + r.Width - 1 > w)
                    {
                        r.Width = 0;
                        break;
                    }
                }
                while (!Overlaps(r, initialRooms));

                initialRooms[i].Width += r.Width - 1;

                int height = 1;

                do
                {
                    r = new Rectangle(initialRooms[i].X,
                                      initialRooms[i].Y + initialRooms[i].Height,
                                      initialRooms[i].Width,
                                      height++);

                    if (r.Y + r.Height - 1 > h)
                    {
                        r.Height = 0;
                        break;
                    }
                }
                while (!Overlaps(r, initialRooms));

                initialRooms[i].Height += r.Height - 1;
            }

            for (int i = 0; i < initialRooms.Length; i++)
                for (int x = initialRooms[i].X; x < initialRooms[i].X + initialRooms[i].Width; x++)
                    for (int y = initialRooms[i].Y; y < initialRooms[i].Y + initialRooms[i].Height; y++)
                    {
                        if (map.Encoding[x, y][0] > 0)
                        {
                            Rectangle a = initialRooms[i];
                            Rectangle b = initialRooms[map.Encoding[x, y][0] - 1];

                            bool inter = a.Overlaps(b);
                            throw new InvalidOperationException("Overlap!");
                        }

                        map.Encoding[x, y][0] = (byte)(i + 1);
                    }

            byte doors = 1;
            HashSet<int> doorsExistBetweenRooms = new HashSet<int>();

            for (int x = 0; x < w - 1; x++)
                for (int y = 0; y < h - 1; y++)
                {
                    int hash = (851 + map.Encoding[x, y][0]) * 37 + map.Encoding[x + 1, y][0];
                    bool doorsAdded = false;

                    if (map.Encoding[x, y][0] > 0 &&
                        map.Encoding[x + 1, y][0] > 0 &&
                        map.Encoding[x, y][0] != map.Encoding[x + 1, y][0] &&
                        !doorsExistBetweenRooms.Contains(hash))
                    {
                        map.Encoding[x + 1, y][1] = doors;
                        map.Encoding[x, y][1] = doors++;

                        doorsExistBetweenRooms.Add(hash);
                        doorsAdded = true;
                    }

                    if (!doorsAdded)
                    {
                        hash = (851 + map.Encoding[x, y][0]) * 37 + map.Encoding[x, y + 1][0];

                        if (map.Encoding[x, y][0] > 0 &&
                            map.Encoding[x, y + 1][0] > 0 &&
                            map.Encoding[x, y][0] != map.Encoding[x, y + 1][0] &&
                            !doorsExistBetweenRooms.Contains(hash))
                        {
                            map.Encoding[x, y][1] = doors;
                            map.Encoding[x, y + 1][1] = doors++;

                            doorsExistBetweenRooms.Add(hash);
                        }
                    }
                }

            map.SaveToFile(filename);
        }
        
        static bool Overlaps(int index, Rectangle[] recs)
        {
            for (int i = 0; i < recs.Length; i++)
            {
                if (i == index)
                    continue;

                if (recs[i].Overlaps(recs[index]))
                    return true;
            }

            return false;
        }

        static bool Overlaps(Rectangle r, Rectangle[] recs)
        {
            for (int i = 0; i < recs.Length; i++)
                if (recs[i].Overlaps(r))
                    return true;

            return false;
        }

        static byte[,][] TestMap()
        {
            return new byte[30, 20][]
            {
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 2, 6, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 2, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 1, 1, 0 }, new byte[3] { 1, 2, 0 }, new byte[3] { 1, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 6, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 3, 1, 0 }, new byte[3] { 3, 2, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 3, 3, 0 }, new byte[3] { 3, 4, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 5, 5, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 5, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 3, 0 }, new byte[3] { 4, 4, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 5, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 7, 0 }, new byte[3] { 6, 7, 0 }, new byte[3] { 6, 0, 0 }, new byte[3] { 6, 0, 0 }, new byte[3] { 6, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 6, 0, 0 }, new byte[3] { 6, 0, 0 }, new byte[3] { 6, 0, 0 }, new byte[3] { 6, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 4, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } },
                { new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 } }
            };
        }
    }
}