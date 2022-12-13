using AdventOfCode2022.Utilities;
using System.Drawing;

namespace AdventOfCode2022.Puzzles
{
    internal class Dec12
    {
        public static void SolvePartOne()
        {
            List<string> lines = PuzzleReader.ReadLines(12).ToList();

            int maxY = lines.Count;
            int maxX = lines[0].Length;

            var grid = new char[maxY, maxX];
            Point start = new Point(-1, -1);
            Point destination = new Point(-1, -1);
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    grid[y, x] = lines[y][x];

                    if (grid[y, x] == 'S')
                    {
                        start = new Point(x, y);
                    }

                    if (grid[y, x] == 'E')
                    {
                        destination = new Point(x, y);
                    }
                }
            }

            var predecessor = new Dictionary<Point, Point>();
            var visited = new HashSet<Point>();
            var queue = new Queue<Point>();
            visited.Add(start);
            queue.Enqueue(start);

            var moves = new Dictionary<Point, char>();

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();

                if (current == destination)
                {
                    break;
                }

                foreach (Point neighbor in GetNeighbors(current, grid))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        predecessor[neighbor] = current;

                        if (neighbor.Y == current.Y - 1)
                        {
                            moves[current] = '^';
                        }

                        if (neighbor.Y == current.Y + 1)
                        {
                            moves[current] = 'v';
                        }

                        if (neighbor.X == current.X - 1)
                        {
                            moves[current] = '<';
                        }

                        if (neighbor.X == current.X + 1)
                        {
                            moves[current] = '>';
                        }

                        queue.Enqueue(neighbor);

                        //Console.Read();
                    }
                }
            }

            if (!predecessor.ContainsKey(destination))
            {
                Console.WriteLine("Failed to find destination.");
            }
            else
            {
                Point current = destination;
                int pathLength = 0;
                while (predecessor.ContainsKey(current))
                {
                    current = predecessor[current];
                    pathLength++;
                }

                Console.WriteLine($"Path length = {pathLength}");
            }
        }

        private static IEnumerable<Point> GetNeighbors(Point current, char[,] grid)
        {
            int maxX = grid.GetLength(1);
            int maxY = grid.GetLength(0);

            int currentHeight = GetHeight(current.X, current.Y, grid);

            foreach (Point candidate in GetMoves(current.X, current.Y, maxX, maxY))
            { 
                int neighborHeight = GetHeight(candidate.X, candidate.Y, grid);
                if (neighborHeight <= currentHeight + 1)
                {
                    yield return candidate;
                }  
            }
        }

        private static int GetHeight(int x, int y, char[,] grid)
        {
            if (grid[y, x] == 'S')
            {
                return 0;
            }
            else if (grid[y, x] == 'E')
            {
                return 25;
            }

            return (int)grid[y, x] - (int)'a';
        }

        private static IEnumerable<Point> GetMoves(int x, int y, int maxX, int maxY)
        {
            // Up.
            if (y > 0)
            {
                yield return new Point(x, y - 1);
            }

            // Down
            if (y < maxY - 1)
            {
                yield return new Point(x, y + 1);
            }

            // Left
            if (x > 0)
            {
                yield return new Point(x - 1, y);
            }

            if (x < maxX - 1)
            {
                yield return new Point(x + 1, y);
            }
        }
    }
}
