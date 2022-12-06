using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec6
    {
        public static void SolvePartOne()
        {
            Solve(isPartTwo: false);
        }

        public static void SolvePartTwo()
        {
            Solve(isPartTwo: true);
        }

        public static void Solve(bool isPartTwo)
        {
            string input = PuzzleReader.ReadLines(6).First();// xxxx
            int i = 0;
            int length = isPartTwo ? 14 : 4;

            while (i < input.Length - length - 1)
            {
                string subStr = input.Substring(i, length);
                int distinct = subStr.Select(c => c).Distinct().Count();
                if (distinct == length)
                {
                    Console.WriteLine($"{i + length} chars before first start-of-packet marker.");
                    break;
                }

                i++;
            }
        }
    }
}
