using AdventOfCode2022.Utilities;
using System.Text;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec22
    {
        public static void SolvePartOne(bool isTest = false)
        {
            // Read in the map.
            var mapLines = new List<string>();
            bool parsingMap = true;
            string instructions = string.Empty;

            foreach (string line in PuzzleReader.ReadLines(22, isTest))
            {
                if (string.IsNullOrEmpty(line))
                {
                    parsingMap = false;
                    continue;
                }

                if (parsingMap)
                {
                    mapLines.Add(line);
                }
                else
                {
                    instructions = line;
                }
            }

            int maxX = mapLines.Max(l => l.Length);
            int maxY = mapLines.Count;

            var map = new char[maxY, maxX];
            for (int i = 0; i < maxY; i++)
            {
                for (int j = 0; j < maxX; j++)
                {
                    map[i, j] = mapLines[i][j];
                }
            }

            int row = 0;
            int col = mapLines[row].IndexOf('.');
            var dir = Direction.Right;

            // Now execute the instructions.
            int instrPos = 0;
            while (instrPos < instructions.Length)
            {
                if (instructions[instrPos] == 'R')
                {
                    dir = Turn(dir, TurnDirection.Clockwise);
                }
                else if (instructions[instrPos] == 'L')
                {
                    dir = Turn(dir, TurnDirection.Counterclockwise);
                }
                else
                {
                    var sb = new StringBuilder();
                    int num;
                    while (instrPos < instructions.Length && Int32.TryParse(instructions[instrPos].ToString(), out num))
                    {
                        sb.Append(instructions[instrPos]);
                        instrPos++;
                    }

                    num = Int32.Parse(sb.ToString());

                    for (int i = 0; i < num; i++)
                    {
                        (int x, int y) = Move((col, row), dir);

                        // Bound checks.
                        if (x < 0)
                        {
                            x = maxX - 1;
                        }

                        if (x >= maxX)
                        {
                            x = 0;
                        }

                        if (y < 0)
                        {
                            y = maxY - 1;
                        }

                        if (y >= maxY)
                        {
                            y = 0;
                        }

                        // Wrap around the board if needed.
                        if (map[y,x] == ' ')
                        {
                            switch (dir)
                            {
                                case Direction.Up:
                                    y = maxY - 1;
                                    while (map[y, x] == ' ')
                                    {
                                        y--;
                                    }

                                    break;

                                case Direction.Right:
                                    x = 0;
                                    while (map[y, x] == ' ')
                                    {
                                        x++;
                                    }

                                    break;

                                case Direction.Down:
                                    y = 0;
                                    while (map[y, x] == ' ')
                                    {
                                        y++;
                                    }

                                    break;

                                case Direction.Left:
                                    x = maxX - 1;
                                    while (map[y, x] == ' ')
                                    {
                                        x--;
                                    }

                                    break;

                                default:
                                    throw new ArgumentException($"Unexpected direction {dir}.");
                            }
                        }

                        if (map[y, x] == '#')
                        {
                            break;
                        }
                        
                        row = y;
                        col = x;
                    }
                }

                instrPos++;
            }

            int password = 6 * (row + 1) + 4 * (col + 1) + DirToNumber(dir);

            Console.WriteLine($"Password = {password}.");
        }

        private static int DirToNumber(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right:
                    return 0;

                case Direction.Down:
                    return 1;

                case Direction.Left:
                    return 2;

                case Direction.Up:
                    return 3;

                default:
                    throw new ArgumentException($"Unexpected direction {dir}.");
            }
        }

        private static (int, int) Move((int, int) coords, Direction dir)
        {
            (int x, int y) = coords;

            switch (dir)
            {
                case Direction.Up:
                    return (x, y - 1);

                case Direction.Right:
                    return (x + 1, y);

                case Direction.Down:
                    return (x, y + 1);

                case Direction.Left:
                    return (x - 1, y);

                default:
                    throw new ArgumentException($"Unexpected direction {dir}.");
            }
        }

        private static Direction Turn(Direction dir, TurnDirection turnDir)
        {
            switch (turnDir)
            {
                case TurnDirection.Clockwise:
                    switch (dir)
                    {
                        case Direction.Up:
                            return Direction.Right;

                        case Direction.Right:
                            return Direction.Down;

                        case Direction.Down:
                            return Direction.Left;

                        case Direction.Left:
                            return Direction.Up;

                        default:
                            throw new ArgumentException($"Unexpected direction {dir}.");
                    }

                case TurnDirection.Counterclockwise:
                    switch (dir)
                    {
                        case Direction.Up:
                            return Direction.Left;

                        case Direction.Right:
                            return Direction.Up;

                        case Direction.Down:
                            return Direction.Right;

                        case Direction.Left:
                            return Direction.Down;

                        default:
                            throw new ArgumentException($"Unexpected direction {dir}.");
                    }

                default:
                    throw new ArgumentException($"Unexpected turn direction {turnDir}.");
            }
        }
    }

    internal enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    internal enum TurnDirection
    {
        None,
        Clockwise,
        Counterclockwise
    }
}
