using AdventOfCode2022.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec6
    {
        public static void SolvePartOne()
        {
            string input = PuzzleReader.ReadLines(6).First();// xxxx
            int i = 0;
            while (i < input.Length - 3)
            {
                string subStr = input.Substring(i, 4);
                int distinct = subStr.Select(c => c).Distinct().Count();
                if (distinct == 4)
                {
                    Console.WriteLine($"{i + 4} chars before first start-of-packet marker.");
                    break;
                }

                i++;
            }
        }
    }
}
