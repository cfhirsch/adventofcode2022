using AdventOfCode2022.Utilities;
using System.Text;

namespace AdventOfCode2022.Puzzles
{
    internal static  class Dec10
    {
        public static void SolvePartOne()
        {
            Solve(isPartTwo: false);
        }

        public static void SolvePartTwo()
        {
            Solve(isPartTwo: true);
        }

        private static void Solve(bool isPartTwo)
        {
            long x = 1;
            int cycle = 0;
            List<string> lines = PuzzleReader.ReadLines(10).ToList();
            int instrPtr = -1;
            
            string currentCommand = null;
            int operand = 0;

            var signalStrengths = new List<long>();
            var crt = new StringBuilder();
            int crtPos = -1;
            
            while (true)
            {
                cycle++;
                crtPos++;

                if (crtPos > 39)
                {
                    if (isPartTwo)
                    {
                        Console.WriteLine(crt);
                    }

                    crt = new StringBuilder();
                    crtPos = 0;
                }

                if (!isPartTwo)
                {
                    // Find the signal strength during the 20th, 60th, 100th, 140th, 180th, and 220th cycles.
                    if (cycle == 20 || cycle == 60 || cycle == 100 || cycle == 140 || cycle == 180 || cycle == 220)
                    {
                        long signalStrength = cycle * x;
                        Console.WriteLine($"cycle = {cycle}, x = {x}, strength = {signalStrength}.");
                        signalStrengths.Add(signalStrength);
                    }
                }

                if (Math.Abs(crtPos - x) <= 1)
                {
                    crt.Append("#");
                }
                else
                {
                    crt.Append(".");
                }

                if (currentCommand == "addx")
                {
                    x += operand;
                    currentCommand = null;
                }
                else
                {
                    instrPtr++;
                    if (instrPtr >= lines.Count)
                    {
                        break;
                    }

                    string[] instructionParts = lines[instrPtr].Split(' ');
                    currentCommand = instructionParts[0];
                    if (currentCommand == "addx")
                    {
                        operand = Int32.Parse(instructionParts[1]);       
                    }
                }
            }

            long sum = signalStrengths.Sum();

            if (!isPartTwo)
            {
                Console.WriteLine($"Sum of signal strengths = {sum}.");
            }
            else
            {
                Console.WriteLine(crt);
            }
        }
    }
}
