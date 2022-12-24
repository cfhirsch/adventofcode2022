using AdventOfCode2022.Utilities;
using System.Drawing;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec24
    {
        public static void SolvePartOne(bool isTest)
        {
            List<string> lines = PuzzleReader.ReadLines(24, isTest).ToList();

            int maxX = lines[0].Length;
            int maxY = lines.Count;

            var blizzards = new List<Blizzard>();

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (lines[y][x] != '.' && lines[y][x] != '#')
                    {
                        blizzards.Add(new Blizzard(new Point(x, y), lines[y][x]));
                    }
                }
            }

            var start = new Point(1, 0);
            var exit = new Point(maxX - 2, maxY - 1);

            var queue = new PriorityQueue<BlizzardQueueEntry>();
            queue.Enqueue(new BlizzardQueueEntry(start, blizzards, 0, maxX, maxY));
            bool found = false;

            while (queue.Count() > 0) 
            {
                BlizzardQueueEntry current = queue.Dequeue();
                if (current.Location == exit)
                {
                    Console.WriteLine($"Reached exit in {current.Minute} minutes.");
                    found = true;
                    break;
                }

                // Update the blizzards.
                var nextBliz = current.Blizzards.Select(b => b.Move(maxX, maxY)).ToList();

                foreach (Point next in GetNeighbors(current.Location, maxX, maxY))
                {
                    if (!nextBliz.Any(b => b.Location == next))
                    {
                        queue.Enqueue(new BlizzardQueueEntry(next, nextBliz, current.Minute + 1, maxX, maxY));
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("Did not find exit.");
            }
        }

        private static IEnumerable<Point> GetNeighbors(Point pt, int maxX, int maxY)
        {
            yield return pt;

            if (pt.X > 1)
            {
                yield return new Point(pt.X - 1, pt.Y);
            }

            if (pt.X < maxX - 2)
            {
                yield return new Point(pt.X + 1, pt.Y);
            }

            if (pt.Y > 1)
            {
                yield return new Point(pt.X, pt.Y - 1);
            }

            if (pt.X < maxY- 2)
            {
                yield return new Point(pt.X, pt.Y + 1);
            }
        }
    }

    internal struct Blizzard
    {
        public Blizzard(Point pt, char dir)
        {
            this.Location = pt;
            this.Direction = dir;
        }

        public Point Location { get; private set; }

        public char Direction { get; private set; }

        public Blizzard Move(int maxX, int maxY)
        {
            int x = this.Location.X;
            int y = this.Location.Y;
            switch (this.Direction)
            {
                case '^':
                    y--;
                    break;

                case '>':
                    x++;
                    break;

                case 'v':
                    y++;
                    break;

                case '<':
                    x--;
                    break;

                default:
                    throw new ArgumentException($"Unexpected symbol {this.Direction}.");
            }

            // Bounds checks.
            if (x < 1)
            {
                x = maxX - 2;
            }

            if (x >= maxX)
            {
                x = 1;
            }

            if (y < 1)
            {
                y = maxY - 2;
            }

            if (y >= maxY)
            {
                y = 1;
            }

            return new Blizzard(new Point(x, y), this.Direction);
        }
    }

    internal struct BlizzardQueueEntry : IComparable<BlizzardQueueEntry>
    {
        private readonly int maxX, maxY;

        private Point start, end;

        public BlizzardQueueEntry(Point location, List<Blizzard> blizzards, int minute, int maxX, int maxY)
        {
            this.Location = location;
            this.Blizzards = blizzards;
            this.Minute = minute;
            this.maxX = maxX;
            this.maxY = maxY;

            this.start = new Point(1, 0);
            this.end = new Point(this.maxX - 2, this.maxY - 1);
        }

        public Point Location { get; private set; }

        public List<Blizzard> Blizzards { get; private set; }

        public int Minute { get; private set; }

        public int CompareTo(BlizzardQueueEntry other)
        {
            int thisScore = this.GetScore();
            int otherScore = other.GetScore();

            if (thisScore < otherScore)
            {
                return -1;
            }

            if (otherScore > thisScore)
            {
                return 1;
            }

            return 0;
        }

        internal int GetScore()
        {
            return Manhattan(this.Location, this.start) + Manhattan(this.Location, this.end);
        }

        private static int Manhattan(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
    }
}
