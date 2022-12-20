namespace AdventOfCode2022.Utilities
{
    internal static class PuzzleReader
    {
        public static IEnumerable<string> ReadLines(int day, bool isTest = false)
        {
            string fileName = isTest ? $"Dec{day}_test.txt" : $"Dec{day}.txt";
            using (var stream = new FileStream(@$"c:\docs\AdventOfCode2022\{fileName}", FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
        }
    }
}
