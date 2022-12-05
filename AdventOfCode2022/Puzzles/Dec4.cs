using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec4
    {
        public static void SolvePartOne()
        {
            int count = 0;
            foreach (string line in PuzzleReader.ReadLines(4))
            {
                string[] parts = line.Split(",");
                Tuple<int, int> first = StringToInterval(parts[0]);
                Tuple<int, int> second = StringToInterval(parts[1]);

                if (Contains(first, second) || Contains(second, first))
                {
                    count++;
                }
            }

            Console.WriteLine($"There are {count} pairs where one interval contains the other.");
        }

        public static void SolvePartTwo()
        {
            int count = 0;
            foreach (string line in PuzzleReader.ReadLines(4))
            {
                string[] parts = line.Split(",");
                Tuple<int, int> first = StringToInterval(parts[0]);
                Tuple<int, int> second = StringToInterval(parts[1]);

                if (Overlaps(first, second) || Overlaps(second, first))
                {
                    count++;
                }
            }

            Console.WriteLine($"There are {count} pairs where one interval overlaps the other.");
        }

        private static bool Contains(Tuple<int, int> first, Tuple<int, int> second)
        {
            return (first.Item1 <= second.Item1 && first.Item2 >= second.Item2);
        }

        private static bool Overlaps(Tuple<int, int> first, Tuple<int, int> second)
        {
            return (first.Item1 >= second.Item1 && first.Item1 <= second.Item2) ||
                   (first.Item2 >= second.Item1 && first.Item2 <= second.Item2);
        }

        private static Tuple<int, int> StringToInterval(string str)
        {
            string[] parts = str.Split("-");
            return new Tuple<int, int>(Int32.Parse(parts[0]), Int32.Parse(parts[1]));
        }
    }
}
