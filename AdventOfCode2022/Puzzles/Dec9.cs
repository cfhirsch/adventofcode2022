using AdventOfCode2022.Utilities;
using System.Drawing;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec9
    {
        public static void SolvePartOne(bool show)
        {
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            Point headPos = new Point(0, 0);
            Point tailPos = new Point(0, 0);
            var visitedByTail = new HashSet<Point>();
            visitedByTail.Add(tailPos);

            if (show)
            {
                Display(minX, maxX, minY, maxY, headPos, tailPos);
            }

            foreach (string line in PuzzleReader.ReadLines(9))
            {
                string[] lineParts = line.Split(" ");
                string move = lineParts[0];
                int numToMove = Int32.Parse(lineParts[1]);

                // Move the head of the rope.
                for (int i = 0; i < numToMove; i++)
                {
                    int newHeadX = headPos.X, newHeadY = headPos.Y;
                    int newTailX = tailPos.X, newTailY = tailPos.Y;
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

                    // If the head is ever two steps directly up, down, left, or right from the tail,
                    // the tail must also move one step in that direction so it remains close enough.
                    if (headPos.X == tailPos.X && headPos.Y - tailPos.Y == 2)
                    {
                        newTailY = tailPos.Y + 1;
                    }
                    else if (headPos.X == tailPos.X && tailPos.Y - headPos.Y == 2)
                    {
                        newTailY = tailPos.Y - 1;
                    }
                    else if (headPos.Y == tailPos.Y && tailPos.X - headPos.X == 2)
                    {
                        newTailX = tailPos.X - 1;
                    }
                    else if (headPos.Y == tailPos.Y && headPos.X - tailPos.X == 2)
                    {
                        newTailX = tailPos.X + 1;
                    }
                    // Otherwise, if the head and tail aren't touching and aren't in the same row or column,
                    // the tail always moves one step diagonally to keep up
                    else if (!AreTouching(headPos, tailPos))
                    {
                        bool above = headPos.Y - tailPos.Y > 0;
                        bool left = headPos.X - tailPos.X < 0;

                        if (above)
                        {
                            if (left)
                            {
                                newTailX = tailPos.X - 1;
                                newTailY = tailPos.Y + 1;
                            }
                            else
                            {
                                newTailX = tailPos.X + 1;
                                newTailY = tailPos.Y + 1;
                            }
                        }
                        else
                        {
                            if (left)
                            {
                                newTailX = tailPos.X - 1;
                                newTailY = tailPos.Y - 1;
                            }
                            else
                            {
                                newTailX = tailPos.X + 1;
                                newTailY = tailPos.Y - 1;
                            }
                        }
                    }

                    tailPos = new Point(newTailX, newTailY);
                    visitedByTail.Add(tailPos);

                    minX = Min(minX, headPos.X, tailPos.X);
                    maxX = Max(maxX, headPos.X, tailPos.X);
                    minY = Min(minY, headPos.Y, tailPos.Y);
                    maxY = Max(maxY, headPos.Y, tailPos.Y);

                    if (show)
                    {
                        Display(minX, maxX, minY, maxY, headPos, tailPos);
                    }
                }
            }

            if (show)
            {
                DisplayTailVisited(minX, maxX, minY, maxY, visitedByTail);
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

        private static void Display(int minX, int maxX, int minY, int maxY, Point head, Point tail)
        {
            Point origin = new Point(0, 0);

            for (int y = maxY; y >= minY; y--)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Point current = new Point(x, y);
                    if (current == head)
                    {
                        Console.Write("H");
                    }
                    else if (current == tail)
                    {
                        Console.Write("T");
                    }
                    else if (current == origin)
                    {
                        Console.Write("s");
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
