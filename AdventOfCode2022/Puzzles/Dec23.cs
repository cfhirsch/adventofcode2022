using AdventOfCode2022.Utilities;
using System.Drawing;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec23
    {
        public static void Solve(bool isTest = false, bool isPartTwo = false)
        {
            var elfLocations = new HashSet<Point>();
            var elfProposals = new List<ElfProposal>();

            int y = 0;
            foreach (string line in PuzzleReader.ReadLines(23, isTest))
            {
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        elfLocations.Add(new Point(x, y));
                    }
                }

                y++;
            }

            /*If there is no Elf in the N, NE, or NW adjacent positions, the Elf proposes moving north one step.
If there is no Elf in the S, SE, or SW adjacent positions, the Elf proposes moving south one step.
If there is no Elf in the W, NW, or SW adjacent positions, the Elf proposes moving west one step.
If there is no Elf in the E, NE, or SE adjacent positions, the Elf proposes moving east one step.*/
            elfProposals.Add(
                new ElfProposal(
                    new List<ElfDirection>(new[] { ElfDirection.North, ElfDirection.NorthEast, ElfDirection.NorthWest }),
                    ElfDirection.North));

            elfProposals.Add(
                new ElfProposal(
                    new List<ElfDirection>(new[] { ElfDirection.South, ElfDirection.SouthEast, ElfDirection.SouthWest }),
                    ElfDirection.South));

            elfProposals.Add(
                new ElfProposal(
                    new List<ElfDirection>(new[] { ElfDirection.West, ElfDirection.NorthWest, ElfDirection.SouthWest }),
                    ElfDirection.West));

            elfProposals.Add(
                new ElfProposal(
                    new List<ElfDirection>(new[] { ElfDirection.East, ElfDirection.NorthEast, ElfDirection.SouthEast }),
                    ElfDirection.East));

            int numRounds = isPartTwo ? 100000 : 10;

            PrintMap(elfLocations, "Initial State", isTest);

            for (int i = 0; i < numRounds; i++)
            {
                Console.SetCursorPosition(0, 5);
                Console.WriteLine($"Round: {i + 1}");

                // First Half - each elf comes up with a proposed move.
                var proposalDict = new Dictionary<Point, Point>();
                bool elfMoved = false;

                foreach (Point pt in elfLocations)
                {
                    // If there are no neighboring elves, current elf does nothing this round.
                    if (!Enum.GetValues<ElfDirection>().Except(
                        new[] { ElfDirection.None }).Select(
                            d => ElfProposal.GetNeighbor(pt, d)).Any(p => elfLocations.Contains(p)))
                    {
                        continue;
                    }

                    foreach (ElfProposal proposal in elfProposals)
                    {
                        if (proposal.Evaluate(elfLocations, pt))
                        {
                            Point proposed = ElfProposal.GetNeighbor(pt, proposal.Direction);
                            proposalDict[pt] = proposed;
                            break;
                        }
                    }
                }

                // Second Half - for each elf that proposes a move, move if they are the only elf 
                // proposing to move to proposed location.
                foreach (KeyValuePair<Point, Point> kvp in proposalDict)
                {
                    if (proposalDict.Count(k => k.Value == kvp.Value) == 1)
                    {
                        elfLocations.Remove(kvp.Key);
                        elfLocations.Add(kvp.Value);
                        elfMoved = true;
                    }
                }

                // Finally rotate the proposals.
                ElfProposal first = elfProposals.First();
                elfProposals = elfProposals.Skip(1).ToList();
                elfProposals.Add(first);

                PrintMap(elfLocations, $"End of Round {i + 1}", isTest);

                if (isPartTwo && !elfMoved)
                {
                    Console.WriteLine($"First round where no elf moved: {i + 1}.");
                    break;
                }
            }

            if (!isPartTwo)
            {
                // Now find the smallest rectangle that contains all the elves, and count the number of empty tiles.
                int minX = elfLocations.Min(p => p.X);
                int maxX = elfLocations.Max(p => p.X);
                int minY = elfLocations.Min(p => p.Y);
                int maxY = elfLocations.Max(p => p.Y);

                int count = 0;
                for (y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        if (!elfLocations.Contains(new Point(x, y)))
                        {
                            count++;
                        }
                    }
                }

                Console.WriteLine($"There are {count} empty ground tiles.");
            }
        }

        public static void PrintMap(HashSet<Point> elfLocations, string title, bool isTest)
        {
            if (!isTest)
            {
                return;
            }

            Console.WriteLine($"== {title} ==");

            int minX = elfLocations.Min(p => p.X);
            int maxX = elfLocations.Max(p => p.X);
            int minY = elfLocations.Min(p => p.Y);
            int maxY = elfLocations.Max(p => p.Y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (elfLocations.Contains(new Point(x, y)))
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
    }

    public struct ElfProposal
    {
        public ElfProposal(List<ElfDirection> elfDirection, ElfDirection direction)
        {
            this.ElfDirections = elfDirection;
            this.Direction = direction;
        }

        public List<ElfDirection> ElfDirections { get; private set; }

        public ElfDirection Direction { get; private set; }

        public bool Evaluate(HashSet<Point> elfLocations, Point currentLocation)
        {
            foreach (ElfDirection dir in this.ElfDirections)
            {
                Point neighbor = GetNeighbor(currentLocation, dir);
                if (elfLocations.Contains(neighbor))
                {
                    return false;
                }
            }

            return true;
        }

        public static Point GetNeighbor(Point current, ElfDirection dir)
        {
            switch (dir)
            {
                case ElfDirection.North:
                    return new Point(current.X, current.Y - 1);

                case ElfDirection.NorthEast:
                    return new Point(current.X + 1, current.Y - 1);

                case ElfDirection.East:
                    return new Point(current.X + 1, current.Y);

                case ElfDirection.SouthEast:
                    return new Point(current.X + 1, current.Y + 1);

                case ElfDirection.South:
                    return new Point(current.X, current.Y + 1);

                case ElfDirection.SouthWest:
                    return new Point(current.X - 1, current.Y + 1);

                case ElfDirection.West:
                    return new Point(current.X - 1, current.Y);

                case ElfDirection.NorthWest:
                    return new Point(current.X - 1, current.Y - 1);

                default:
                    throw new ArgumentException($"Unexpected elf direction {dir}.");
            }
        }
    }

    public enum ElfDirection
    {
        None,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
}
