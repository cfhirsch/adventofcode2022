using AdventOfCode2022.Utilities;
using System.Text;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec22
    {
        public static void SolvePartOne(bool isTest = false)
        {
            int row = 0;

            (char[,] map, string instructions, int col) = GetPuzzleData(isTest);
            
            var dir = Direction.Right;

            var visited = new Dictionary<(int, int), Direction>();
            visited.Add((col, row), dir);

            if (isTest)
            {
                PrintMap(map, visited);
            }

            int maxY = map.GetLength(0);
            int maxX = map.GetLength(1);

            // Now execute the instructions.
            int instrPos = 0;
            while (instrPos < instructions.Length)
            {
                if (instructions[instrPos] == 'R')
                {
                    dir = Turn(dir, TurnDirection.Clockwise);
                    instrPos++;
                }
                else if (instructions[instrPos] == 'L')
                {
                    dir = Turn(dir, TurnDirection.Counterclockwise);
                    instrPos++;
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

                        visited[(col, row)] = dir;

                        if (isTest)
                        {
                            PrintMap(map, visited);
                        }
                    }
                }
            }

            int password = 1000 * (row + 1) + 4 * (col + 1) + DirToNumber(dir);

            Console.WriteLine($"Password = {password}.");
        }

        public static void SolvePartTwo(bool isTest = false)
        {
            int row = 0;

            (char[,] map, string instructions, int col) = GetPuzzleData(isTest);

            // Figure out the cube faces.
            int cubeLength = isTest ? 4 : 50;

            int faceNumber = 1;
            int minFaceNumber = 1;
            
            int maxY = map.GetLength(0);
            int maxX = map.GetLength(1);

            var cubeMap = new char[maxY, maxX];

            var cubeCountDict = new Dictionary<int, int>();
            cubeCountDict[1] = 0;
            cubeCountDict[2] = 0;
            cubeCountDict[3] = 0;
            cubeCountDict[4] = 0;
            cubeCountDict[5] = 0;
            cubeCountDict[6] = 0;

            var cubeFaceLocationDict = new Dictionary<int, (int, int)>();

            for (int y = 0; y < maxY; y++)
            {
                // Find the first nonblank character.
                int x = 0;
                while (map[y, x] == ' ')
                {
                    x++;
                }

                faceNumber = minFaceNumber;

                int count = 0;
                while (x < maxX && map[y, x] != ' ')
                {
                    if (!cubeFaceLocationDict.ContainsKey(faceNumber))
                    {
                        cubeFaceLocationDict.Add(faceNumber, (x, y));
                    }

                    cubeMap[y, x] = $"{faceNumber}"[0];
                    cubeCountDict[faceNumber]++;

                    count++;

                    if (cubeCountDict[faceNumber] == cubeLength * cubeLength)
                    {
                        minFaceNumber++;
                    }

                    if (count >= cubeLength)
                    {
                        count = 0;
                        faceNumber++;
                    }

                    x++;
                }
            }

            //if (isTest)
           // {
                //PrintCubeMap(cubeMap);
                //return;
            //}

            var dir = Direction.Right;

            var visited = new Dictionary<(int, int), Direction>();
            visited.Add((col, row), dir);

            if (isTest)
            {
                PrintMap(map, visited);
            }

            // Now execute the instructions.
            int instrPos = 0;
            while (instrPos < instructions.Length)
            {
                if (instructions[instrPos] == 'R')
                {
                    dir = Turn(dir, TurnDirection.Clockwise);
                    instrPos++;
                }
                else if (instructions[instrPos] == 'L')
                {
                    dir = Turn(dir, TurnDirection.Counterclockwise);
                    instrPos++;
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
                        if (map[y, x] == ' ')
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

                        visited[(col, row)] = dir;

                        if (isTest)
                        {
                            PrintMap(map, visited);
                        }
                    }
                }
            }

            int password = 1000 * (row + 1) + 4 * (col + 1) + DirToNumber(dir);

            Console.WriteLine($"Password = {password}.");
        }

        private static (char[,], string, int) GetPuzzleData(bool isTest = false)
        {
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
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (x > mapLines[y].Length - 1)
                    {
                        map[y, x] = ' ';
                    }
                    else
                    {
                        map[y, x] = mapLines[y][x];
                    }
                }
            }

            return (map, instructions, mapLines[0].IndexOf('.'));
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

        private static (int, int) MoveOnCube(
            (int, int) coords, 
            Direction dir, 
            char[,] map,
            char[,] cubeMap,
            Dictionary<int, (int, int)> cubeFaceDict,
            int cubeFaceLength)
        {
            (int x, int y) = coords;

            int maxX = map.GetLength(1);
            int maxY = map.GetLength(0);
            int faceNumber = cubeMap[y, x];

            switch (dir)
            {
                case Direction.Up:
                    y--;
                    break;

                case Direction.Right:
                    x++;
                    break;

                case Direction.Down:
                    y++;
                    break;

                case Direction.Left:
                    x--;
                    break;

                default:
                    throw new ArgumentException($"Unexpected direction {dir}.");
            }

            if (x < 0)
            {
                
            }
        }

        private static void PrintCubeMap(char[,] map)
        {
            int maxX = map.GetLength(1);
            int maxY = map.GetLength(0);

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Console.Write(map[y, x]);
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static void PrintMap(char[,] map, Dictionary<(int, int), Direction> visited)
        {
            int maxX = map.GetLength(1);
            int maxY = map.GetLength(0);

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (visited.ContainsKey((x, y)))
                    {
                        switch (visited[(x, y)])
                        {
                            case Direction.Up:
                                Console.Write('^');
                                break;

                            case Direction.Right:
                                Console.Write('>');
                                break;

                            case Direction.Down:
                                Console.Write('v');
                                break;

                            case Direction.Left:
                                Console.Write('<');
                                break;

                            default:
                                throw new ArgumentException($"Unexpected direction {visited[(x, y)]}");
                        }
                    }
                    else
                    {
                        Console.Write(map[y, x]);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
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
