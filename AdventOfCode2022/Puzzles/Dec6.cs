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
            string input = PuzzleReader.ReadLines(6).First();
            int length = isPartTwo ? 14 : 4;

            int pos = Enumerable.Range(0, input.Length - length - 1).Where(i => input.Substring(i, length).Distinct().Count() == length).OrderBy(i => i).First();

            Console.WriteLine($"{pos + length} chars before first start-of-packet marker.");
        }
    }
}
