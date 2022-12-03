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
                    int pri = GetPriority(ch);
                    Console.WriteLine($"{ch} is in common, has priority {pri}.");
                    sum += pri;
                }
            }

            Console.WriteLine($"Sum = {sum}.");
        }

        public static void SolvePartTwo()
        {
            int sum = 0;

            List<string> lines = PuzzleReader.ReadLines(3).ToList();

            for (int i = 0; i <= lines.Count - 3; i+=3)
            {   
                foreach (char ch in lines[i])
                {
                    if (lines[i + 1].Contains(ch) && lines[i + 2].Contains(ch))
                    {
                        int pri = GetPriority(ch);
                        Console.WriteLine($"Badge = {ch}, pri = {pri}.");
                        sum += pri;
                        break;
                    }
                }
            }

            Console.WriteLine($"Sum = {sum}.");
        }

        private static int GetPriority(char ch)
        {
            int lowerA = (int)'a';
            int upperA = (int)'A';
            if (char.IsUpper(ch))
            {
                return ((int)ch) - upperA + 27;
            }

            return ((int)ch) - lowerA + 1;
        }
    }
}
