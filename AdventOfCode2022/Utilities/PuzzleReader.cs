namespace AdventOfCode2022.Utilities
{
    internal static class PuzzleReader
    {
        public static IEnumerable<string> ReadLines(int day)
        {
            using (var stream = new FileStream(@$"d:\docs\AdventOfCode2022\Dec{day}.txt", FileMode.Open))
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
