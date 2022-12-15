using AdventOfCode2022.Utilities;
using System.Drawing;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec14
    {
        public static void SolvePartOne()
        {
            var sandGrains = new HashSet<Point>();
            var paths = new List<Path>();

            foreach (string line in PuzzleReader.ReadLines(14))
            {
                string[] lineParts = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                var path = new Path();
                foreach (string linePart in lineParts)
                {
                    string[] coords = linePart.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    path.Points.Add(new Point(Int32.Parse(coords[0]), Int32.Parse(coords[1])));
                }

                paths.Add(path);
            }

            int minX = paths.Select(p => p.Points.Min(x => x.X)).Min();
            int maxX = paths.Select(p => p.Points.Max(x => x.X)).Max();

            int minY = 0;
            int maxY = paths.Select(p => p.Points.Max(x => x.Y)).Max();

            bool fellToAbyss = false;
            while (!fellToAbyss)
            {
                var current = new Point(500, 0);

                while (true)
                {
                    var down = new Point(current.X, current.Y + 1);
                    if (!Intersects(sandGrains, paths, down))
                    {
                        current = down;
                    }
                    else
                    {
                        var downToLeft = new Point(current.X - 1, current.Y + 1);
                        if (!Intersects(sandGrains, paths, downToLeft))
                        {
                            current = downToLeft;
                        }
                        else
                        {
                            var downToRight = new Point(current.X + 1, current.Y + 1);
                            if (!Intersects(sandGrains, paths, downToRight))
                            {
                                current = downToRight;
                            }
                            else
                            {
                                // Sand grain can't move any further.
                                sandGrains.Add(current);
                                break;
                            }
                        }
                    }

                    if (current.Y > maxY)
                    {
                        fellToAbyss = true;
                        break;
                    }

                    minX = Math.Min(minX, current.X);
                    maxX = Math.Max(maxX, current.X);
                    minY = Math.Min(minY, current.Y);
                    maxY = Math.Max(maxY, current.Y);
                }

                //Console.WriteLine($"{sandGrains.Count} grains of sand.");

                //Draw(minX, maxX, minY, maxY, paths, sandGrains);
                //Console.Read();
            }

            Console.WriteLine($"{sandGrains.Count} grains of sand came to rest.");
        }

        public static void SolvePartTwo()
        {
            var sandGrains = new HashSet<Point>();
            var paths = new List<Path>();

            foreach (string line in PuzzleReader.ReadLines(14))
            {
                string[] lineParts = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                var path = new Path();
                foreach (string linePart in lineParts)
                {
                    string[] coords = linePart.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    path.Points.Add(new Point(Int32.Parse(coords[0]), Int32.Parse(coords[1])));
                }

                paths.Add(path);
            }

            int minX = paths.Select(p => p.Points.Min(x => x.X)).Min();
            int maxX = paths.Select(p => p.Points.Max(x => x.X)).Max();

            int minY = 0;
            int maxY = paths.Select(p => p.Points.Max(x => x.Y)).Max() + 2;

            var start = new Point(500, 0);
            
            while (!sandGrains.Contains(start))
            {
                var current = start;
                while (true)
                {
                    var down = new Point(current.X, current.Y + 1);
                    if (!Intersects(sandGrains, paths, down, maxY))
                    {
                        current = down;
                    }
                    else
                    {
                        var downToLeft = new Point(current.X - 1, current.Y + 1);
                        if (!Intersects(sandGrains, paths, downToLeft, maxY))
                        {
                            current = downToLeft;
                        }
                        else
                        {
                            var downToRight = new Point(current.X + 1, current.Y + 1);
                            if (!Intersects(sandGrains, paths, downToRight, maxY))
                            {
                                current = downToRight;
                            }
                            else
                            {
                                // Sand grain can't move any further.
                                sandGrains.Add(current);
                                break;
                            }
                        }
                    }

                    minX = Math.Min(minX, current.X);
                    maxX = Math.Max(maxX, current.X);
                    minY = Math.Min(minY, current.Y);
                }
            }

            Console.WriteLine($"{sandGrains.Count} grains of sand came to rest.");
        }

        private static void Draw(int minX, int maxX, int minY, int maxY, List<Path> paths, HashSet<Point> sandGrains, bool drawMaxY = false)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var pt = new Point(x, y);
                    if (x == 500 && y == 0)
                    {
                        Console.Write("+");
                    }
                    else if (y == maxY || paths.Any(p => p.Intersects(pt)))
                    {
                        Console.Write("#");
                    }
                    else if (sandGrains.Contains(pt))
                    {
                        Console.Write("o");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine();
            }
        }

        private static bool Intersects(HashSet<Point> sandGrains, List<Path> paths, Point pt, int maxY)
        {
            return (pt.Y == maxY) || sandGrains.Contains(pt) || paths.Any(p => p.Intersects(pt));
        }

        private static bool Intersects(HashSet<Point> sandGrains, List<Path> paths, Point pt)
        {
            return sandGrains.Contains(pt) || paths.Any(p => p.Intersects(pt));
        }
    }

    internal class Path
    {
        public Path()
        {
            this.Points = new List<Point>();
        }

        public List<Point> Points { get; private set; }

        public bool Intersects(Point pt)
        {
            return Enumerable.Range(0, this.Points.Count - 1).Any(i => Intersects(pt, this.Points[i], this.Points[i + 1]));
        }

        private static bool Intersects(Point p1, Point p2, Point p3)
        { 
            if (p2.X == p3.X)
            {
                int minY = Math.Min(p2.Y, p3.Y);
                int maxY = Math.Max(p2.Y, p3.Y);

                return p1.X == p2.X && p1.Y >= minY && p1.Y <= maxY;
            }
            else if (p2.Y == p3.Y)
            {
                int minX = Math.Min(p2.X, p3.X);
                int maxX = Math.Max(p2.X, p3.X);
                return p1.Y == p2.Y && p1.X >= minX && p1.X <= maxX;
            }
            else
            {
                throw new Exception("Was expecting horizontal or vertical line.");
            }
        }
    }
}
