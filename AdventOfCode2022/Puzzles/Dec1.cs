using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec1
    {
        // Find the most calories held by any elf.
        public static void SolvePartOne()
        {
            int max = GetElfDictSum(1);
            
            Console.WriteLine($"Part One solution = {max}.");
        }

        // Find the the calories held by an elves with the three biggest calorie amounts.
        public static void SolvePartTwo()
        {
            int max = GetElfDictSum(3);

            Console.WriteLine($"Part Two solution = {max}.");
        }

        private static int GetElfDictSum(int take)
        {
            var elfDict = new Dictionary<int, int>();

            int i = 1;
            foreach (string line in PuzzleReader.ReadLines(1))
            {
                if (string.IsNullOrEmpty(line))
                {
                    i++;
                }
                else
                {
                    if (!elfDict.ContainsKey(i))
                    {
                        elfDict.Add(i, 0);
                    }

                    elfDict[i] += Int32.Parse(line);
                }
            }

            return elfDict.OrderByDescending(s => s.Value).Take(take).Sum(s => s.Value);
        }
    }
}
