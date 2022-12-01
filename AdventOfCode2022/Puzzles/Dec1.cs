using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec1
    {
        // Find the most calories held by any elf.
        public static void SolvePartOne()
        {
            var elfDict = new Dictionary<int, int>();

            int i = 1;
            foreach(string line in PuzzleReader.ReadLines(1))
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

            int max = Int32.MinValue;
            foreach ((_, int calories) in elfDict)
            {
                if (calories > max)
                {
                    max = calories;
                }
            }

            Console.WriteLine($"Max calories = {max}.");
        }
    }
}
