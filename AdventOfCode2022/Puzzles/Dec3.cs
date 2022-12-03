using AdventOfCode2022.Utilities;
using System.Reflection;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec3
    {
        public static void SolvePartOne()
        {
            int lowerA = (int)'a';
            int upperA = (int)'A';
            int sum = 0;

            foreach (string line in PuzzleReader.ReadLines(3))
            {
                int compartmentLength = line.Length / 2;
                string compOne = line.Substring(0, compartmentLength);
                string compTwo = line.Substring(compartmentLength, line.Length - compartmentLength);
                var common = new HashSet<char>();

                foreach (char ch in compOne)
                {
                    if (compTwo.Contains(ch))
                    {
                        common.Add(ch);
                    }
                }

                foreach (char ch in common)
                {
                    int pri;
                    if (char.IsUpper(ch))
                    {
                        pri = ((int)ch) - upperA + 27;
                    }
                    else
                    {
                        pri = ((int)ch) - lowerA + 1;
                    }

                    Console.WriteLine($"{ch} is in common, has priority {pri}.");
                    sum += pri;
                }
            }

            Console.WriteLine($"Sum = {sum}.");
        }
    }
}
