using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec20
    {
        public static void SolvePartOne(bool isTest)
        {
            var numbers = new List<int>();
            foreach (string line in PuzzleReader.ReadLines(20, isTest))
            {
                numbers.Add(Int32.Parse(line));
            }

            var numbersCopy = new List<int>();
            numbersCopy.AddRange(numbers);

            foreach (int i in numbersCopy)
            {
                int j = numbers.IndexOf(i);
                int nextj = MathMod(j + i, numbers.Count);

                numbers.RemoveAt(j);

                int insertAt = nextj;

                if (i < 0)
                {
                    insertAt--;
                }
                else if (j + i >= numbers.Count)
                {
                    insertAt++;
                }

                if (insertAt < 0 || insertAt > numbers.Count)
                {
                    numbers.Add(i);
                }
                else
                {
                    numbers.Insert(insertAt, i);
                }

                if (isTest)
                {
                    Console.WriteLine(string.Join(",", numbers.Select(i => i.ToString()).ToArray()));
                }
            }

            int zeroPos = numbers.IndexOf(0);
            int thousand = numbers[(zeroPos + 1000) % numbers.Count];
            int twoThousand = numbers[(zeroPos + 2000) % numbers.Count];
            int threeThousand = numbers[(zeroPos + 3000) % numbers.Count];

            int sum = thousand + twoThousand + threeThousand;
            Console.WriteLine($"Sum = {sum}.");
        }

        private static int MathMod(int a, int b)
        {
            return (Math.Abs(a * b) + a) % b;
        }
    }
}
