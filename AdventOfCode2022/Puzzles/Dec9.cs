using AdventOfCode2022.Utilities;
using System.Drawing;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec9
    {
        public static void SolvePartOne(bool show)
        {
            var knots = new List<Point>
            {
                new Point(0, 0),
                new Point(0, 0)
            };

            Solve(knots, show);
        }

        public static void SolvePartTwo(bool show)
        {
            var knots = new List<Point>();
            for (int i = 0; i < 10; i++)
            {
                knots.Add(new Point(0, 0));
            }

            Solve(knots, show);
        }

        public static void Solve(List<Point> knots,  bool show)
        {
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            var visitedByTail = new HashSet<Point>();

            visitedByTail.Add(knots.Last());

            if (show)
            {
                Display(minX, maxX, minY, maxY, knots);
            }

            foreach (string line in PuzzleReader.ReadLines(9))
            {
                string[] lineParts = line.Split(" ");
                string move = lineParts[0];
                int numToMove = Int32.Parse(lineParts[1]);

                for (int i = 0; i < numToMove; i++)
                {
                    var nextKnots = new List<Point>();

                    // Move the head knot of the rope.
                    Point headPos = knots.First();
                    int newHeadX = headPos.X, newHeadY = headPos.Y;
                    switch (move)
                    {
                        case "U":
                            newHeadY++;
                            break;

                        case "R":
                            newHeadX++;
                            break;

                        case "D":
                            newHeadY--;
                            break;

                        case "L":
                            newHeadX--;
                            break;

                        default:
                            throw new ArgumentException($"Unexpected move {move}.");
                    }

                    headPos = new Point(newHeadX, newHeadY);
                    nextKnots.Add(headPos);

                    for (int j = 1; j < knots.Count; j++)
                    {
                        Point currentPos = nextKnots[j - 1];
                        Point nextPos = knots[j];

                        int newNextX = nextPos.X, newNextY = nextPos.Y;

                        // If a knot is ever two steps directly up, down, left, or right from the next knot,
                        // the next knot must also move one step in that direction so it remains close enough.
                        if (currentPos.X == nextPos.X && currentPos.Y - nextPos.Y == 2)
                        {
                            newNextY = nextPos.Y + 1;
                        }
                        else if (currentPos.X == nextPos.X && nextPos.Y - currentPos.Y == 2)
                        {
                            newNextY = nextPos.Y - 1;
                        }
                        else if (currentPos.Y == nextPos.Y && nextPos.X - currentPos.X == 2)
                        {
                            newNextX = nextPos.X - 1;
                        }
                        else if (currentPos.Y == nextPos.Y && currentPos.X - nextPos.X == 2)
                        {
                            newNextX = nextPos.X + 1;
                        }
                        // Otherwise, if a knot and the next knot aren't touching and aren't in the same row or column,
                        // the next knot always moves one step diagonally to keep up.
                        else if (!AreTouching(currentPos, nextPos))
                        {
                            bool above = currentPos.Y - nextPos.Y > 0;
                            bool left = currentPos.X - nextPos.X < 0;

                            if (above)
                            {
                                if (left)
                                {
                                    newNextX = nextPos.X - 1;
                                    newNextY = nextPos.Y + 1;
                                }
                                else
                                {
                                    newNextX = nextPos.X + 1;
                                    newNextY = nextPos.Y + 1;
                                }
                            }
                            else
                            {
                                if (left)
                                {
                                    newNextX = nextPos.X - 1;
                                    newNextY = nextPos.Y - 1;
                                }
                                else
                                {
                                    newNextX = nextPos.X + 1;
                                    newNextY = nextPos.Y - 1;
                                }
                            }
                        }

                        nextPos = new Point(newNextX, newNextY);
                        nextKnots.Add(nextPos);

                        minX = Min(minX, headPos.X, nextPos.X);
                        maxX = Max(maxX, headPos.X, nextPos.X);
                        minY = Min(minY, headPos.Y, nextPos.Y);
                        maxY = Max(maxY, headPos.Y, nextPos.Y);
                    }

                    knots = nextKnots;
                    visitedByTail.Add(knots.Last());

                    if (show)
                    {
                        Display(minX, maxX, minY, maxY, knots);
                        Console.ReadLine();
                    }
                }
            }

            if (show)
            {
                DisplayTailVisited(minX, maxX, minY, maxY, visitedByTail);
                Console.ReadLine();
            }

            Console.WriteLine($"The tail of the rope visited {visitedByTail.Count} positions.");
        }

        private static bool AreTouching(Point p1, Point p2)
        {
            // Same location.
            if (p1 == p2)
            {
                return true;
            }

            // Adjacent on same column
            if (p1.X == p2.X && Math.Abs(p1.Y - p2.Y) == 1)
            {
                return true;
            }

            // Adjacent on same row
            if (p1.Y == p2.Y && Math.Abs(p1.X - p2.X) == 1)
            {
                return true;
            }

            // Diagonally adjacent
            if (Math.Abs(p1.X - p2.X) == 1 && Math.Abs(p1.Y - p2.Y) == 1)
            {
                return true;
            }

            return false;
        }

        private static void Display(int minX, int maxX, int minY, int maxY, List<Point> knots)
        {
            Point origin = new Point(0, 0);

            for (int y = maxY; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Point current = new Point(x, y);

                    string symbol = current == origin ? "s" : ".";
                    for (int i = 0; i < knots.Count; i++)
                    {
                        if (current == knots[i])
                        {
                            symbol = (i == 0) ? "H" : $"{i}";
                            break;
                        }
                    }

                    Console.Write(symbol);
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static void DisplayTailVisited(int minX, int maxX, int minY, int maxY, HashSet<Point> tailVisited)
        {
            Point origin = new Point(0, 0);

            for (int y = maxY; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Point current = new Point(x, y);
                    if (current == origin)
                    {
                        Console.Write("s");
                    }
                    else if (tailVisited.Contains(current))
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static int Max(params int[] nums)
        {
            return nums.Max(x => x);
        }

        private static int Min(params int[] nums)
        {
            return nums.Min(x => x);
        }
    }
}
