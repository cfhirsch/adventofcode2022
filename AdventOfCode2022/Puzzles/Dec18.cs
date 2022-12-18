using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec18
    {
        public static void SolvePartOne()
        {
            var points = new List<Point3D>();

            foreach (string line in PuzzleReader.ReadLines(18))
            {
                string[] lineParts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                points.Add(new Point3D(
                    Int32.Parse(lineParts[0]),
                    Int32.Parse(lineParts[1]),
                    Int32.Parse(lineParts[2])));
            }

            var openFaces = Enumerable.Repeat(6, points.Count).ToList();

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].Y == points[j].Y && 
                        points[i].Z == points[j].Z &&
                        Math.Abs(points[i].X - points[j].X) == 1)
                    {
                        openFaces[i]--;
                        openFaces[j]--;
                    }

                    if (points[i].X == points[j].X &&
                        points[i].Z == points[j].Z &&
                        Math.Abs(points[i].Y - points[j].Y) == 1)
                    {
                        openFaces[i]--;
                        openFaces[j]--;
                    }

                    if (points[i].X == points[j].X &&
                        points[i].Y == points[j].Y &&
                        Math.Abs(points[i].Z - points[j].Z) == 1)
                    {
                        openFaces[i]--;
                        openFaces[j]--;
                    }
                }
            }

            int surfaceArea = openFaces.Sum();

            Console.WriteLine($"Surface area = {surfaceArea}.");
        }
    }

    internal struct Point3D
    {
        public Point3D(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Z { get; private set; }
    }
}
