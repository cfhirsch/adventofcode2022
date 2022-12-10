using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static  class Dec10
    {
        public static void SolvePartOne()
        {
            long x = 1;
            int cycle = 0;
            List<string> lines = PuzzleReader.ReadLines(10).ToList();
            int instrPtr = -1;
            
            string currentCommand = null;
            int operand = 0;

            var signalStrengths = new List<long>();
            
            while (cycle <= 220)
            {
                cycle++;

                // Find the signal strength during the 20th, 60th, 100th, 140th, 180th, and 220th cycles.
                if (cycle == 20 || cycle == 60 || cycle == 100 || cycle == 140 || cycle == 180 || cycle == 220)
                {
                    long signalStrength = cycle * x;
                    Console.WriteLine($"cycle = {cycle}, x = {x}, strength = {signalStrength}.");
                    signalStrengths.Add(signalStrength);
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
            Console.WriteLine($"Sum of signal strengths = {sum}.");
        }
    }
}
