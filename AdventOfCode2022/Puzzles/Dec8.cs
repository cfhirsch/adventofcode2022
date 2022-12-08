using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec8
    {
        public static void SolvePartOne()
        {
            int[,] grid = ReadGrid();
            int visibleTreeCount = 0;

            int xMax = grid.GetLength(0);
            int yMax = grid.GetLength(1);
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                { 
                    if (x == 0 || x == xMax - 1 || y == 0 || y == yMax - 1)
                    {
                        visibleTreeCount++;
                    }
                    else
                    {
                        // Is the tree visible along its row?
                        bool visibleAlongRow = true;
                        bool visibleAlongCol = true;
                        for (int y1 = 0; y1 < y; y1++)
                        {
                            if (grid[x, y1] >= grid[x, y])
                            {
                                visibleAlongRow = false;
                                break;
                            }
                        }

                        if (!visibleAlongRow)
                        {
                            visibleAlongRow = true;
                            for (int y1 = y + 1; y1 < yMax; y1++)
                            {
                                if (grid[x, y1] >= grid[x, y])
                                {
                                    visibleAlongRow = false;
                                    break;
                                }
                            }
                        }

                        if (!visibleAlongRow)
                        {
                            // Is the tree visible along its column?
                            for (int x1 = 0; x1 < x; x1++)
                            {
                                if (x1 == x)
                                {
                                    continue;
                                }

                                if (grid[x1, y] >= grid[x, y])
                                {
                                    visibleAlongCol = false;
                                    break;
                                }
                            }

                            if (!visibleAlongCol)
                            {
                                visibleAlongCol = true;
                                for (int x1 = x + 1; x1 < xMax; x1++)
                                {
                                    if (x1 == x)
                                    {
                                        continue;
                                    }

                                    if (grid[x1, y] >= grid[x, y])
                                    {
                                        visibleAlongCol = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (visibleAlongRow || visibleAlongCol)
                        {
                            visibleTreeCount++;
                        }
                    }
                }
            }

            Console.WriteLine($"{visibleTreeCount} trees are visible.");
        }

        public static void SolvePartTwo()
        {
            int[,] grid = ReadGrid();

            int xMax = grid.GetLength(0);
            int yMax = grid.GetLength(1);

            int maxScore = Int32.MinValue;
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    int score = ScenicScore(grid, x, y);
                    if (score > maxScore)
                    {
                        maxScore = score;
                    }
                }
            }

            Console.WriteLine($"Max scenic score = {maxScore}.");
        }

        private static int ScenicScore(int[,] grid, int x, int y)
        {
            int xMax = grid.GetLength(0);
            int yMax = grid.GetLength(1);

            if (x == 0 || x == xMax - 1 || y == 0 || y == yMax - 1)
            {
                return 0;
            }

            int product = 1;
            // Up.
            int viewCount = 0;
            for (int y1 = y - 1; y1 >= 0; y1--)
            {
                viewCount++;
                if (grid[x, y1] >= grid[x, y])
                {
                    break;
                }
            }

            product *= viewCount;
            if (product == 0)
            {
                return 0;
            }

            // Right
            viewCount = 0;
            for (int x1 = x + 1; x1 < xMax; x1++)
            {
                viewCount++;
                if (grid[x1, y] >= grid[x, y])
                {
                    break;
                }
            }

            product *= viewCount;
            if (product == 0)
            {
                return 0;
            }

            // Down.
            viewCount = 0;
            for (int y1 = y + 1; y1 < yMax; y1++)
            {
                viewCount++;
                if (grid[x, y1] >= grid[x, y])
                {
                    break;
                }
            }

            product *= viewCount;
            if (product == 0)
            {
                return 0;
            }

            // Left
            viewCount = 0;
            for (int x1 = x - 1; x1 >= 0; x1--)
            {
                viewCount++;
                if (grid[x1, y] >= grid[x, y])
                {
                    break;
                }
            }

            product *= viewCount;

            return product;
        }

        private static int[,] ReadGrid()
        {
            List<string> lines = PuzzleReader.ReadLines(8).ToList();

            int xMax = lines[0].Length;
            int yMax = lines.Count;
            var grid = new int[xMax, yMax];

            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    grid[x, y] = Int32.Parse(lines[y][x].ToString());
                }
            }

            return grid;
        }
    }
}
