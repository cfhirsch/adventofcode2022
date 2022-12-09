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
                            newHeadY = headPos.Y + numToMove;
                            break;

                        case "R":
                            newHeadX = headPos.X + numToMove;
                            break;

                        case "D":
                            newHeadY = headPos.Y + numToMove;
                            break;

                        case "L":
                            newHeadX = headPos.X - numToMove;
                            break;

                        default:
                            throw new ArgumentException($"Unexpected move {move}.");
                    }

                    headPos = new Point(newHeadX, newHeadY);

                    // If the head is ever two steps directly up, down, left, or right from the tail,
                    // the tail must also move one step in that direction so it remains close enough.
                    if (headPos.Y - tailPos.Y == 2)
                    {
                        newTailY = tailPos.Y + 1;
                    }
                    else if (tailPos.Y - headPos.Y == 2)
                    {
                        newTailY = tailPos.Y - 1;
                    }
                    else if (tailPos.X - headPos.X == 2)
                    {
                        newTailX = tailPos.X - 1;
                    }
                    else if (headPos.X - tailPos.X == 2)
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

                    if (headPos.X < minX)
                    {
                        minX = headPos.X;
                    }

                    if (tailPos.X < minX)
                    {
                        minX = tailPos.X;
                    }

                    if (headPos.X > maxX)
                    {
                        maxX = headPos.X;
                    }

                    if (tailPos.X > maxX)
                    {
                        maxX = tailPos.X;
                    }

                    if (headPos.Y < minY)
                    {
                        minY = headPos.Y;
                    }

                    if (tailPos.Y < minY)
                    {
                        minY = tailPos.Y;
                    }

                    if (headPos.Y > maxY)
                    {
                        maxY = headPos.Y;
                    }

                    if (tailPos.X > maxX)
                    {
                        maxY = tailPos.Y;
                    }
                }

                if (show)
                {
                    Display(minX, maxX, minY, maxY, headPos, tailPos);
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
                    else if (current == origin)
                    {
                        Console.Write("s");
                    }
                    else if (current == tail)
                    {
                        Console.Write("T");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine();
            }
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
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
