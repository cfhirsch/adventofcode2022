using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec18
    {
        public static void Solve()
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

            Console.WriteLine("Part One:");

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

            Console.WriteLine("Part Two:");

            int minX = points.Min(pt => pt.X);
            int maxX = points.Max(pt => pt.X);

            int minY = points.Min(pt => pt.Y);
            int maxY = points.Max(pt => pt.Y);

            int minZ = points.Min(pt => pt.Z);
            int maxZ = points.Max(pt => pt.Z);

            var visited = new HashSet<Point3D>();
            var airpocketPoints = new HashSet<Point3D>();
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        var pt = new Point3D(x, y, z);

                        // Continue if current point is in the lava droplet,
                        // or if we've already visited it.
                        if (points.Contains(pt) || visited.Contains(pt))
                        {
                            continue;
                        }

                        visited.Add(pt);

                        // Explore neighborhood of this point to see if we are in an
                        // air pocket.
                        var queue = new Queue<Point3D>();
                        queue.Enqueue(pt);

                        var searchVisited = new HashSet<Point3D>
                        {
                            pt
                        };

                        bool isAirPocket = true;
                        while (queue.Count > 0)
                        {
                            Point3D current = queue.Dequeue();
                            foreach (Point3D neighbor in GetNeighbors(current, points))
                            {
                                if (!visited.Contains(neighbor))
                                {
                                    visited.Add(neighbor);
                                    searchVisited.Add(neighbor);
                                    // If we've past the boundaries of the lava droplet,
                                    // then we know this wasn't an air pocket.
                                    if (neighbor.X < minX || neighbor.X > maxX ||
                                        neighbor.Y < minY || neighbor.Y > maxY ||
                                        neighbor.Z < minZ || neighbor.Z > maxZ)
                                    {
                                        isAirPocket = false;
                                    }
                                    else
                                    {
                                        queue.Enqueue(neighbor);
                                    }
                                }
                            }
                        }

                        if (isAirPocket)
                        {
                            airpocketPoints = airpocketPoints.Union(searchVisited).ToHashSet();
                        }
                    }
                }
            }

            // Now we have to account for all the airpockets we have found.
            foreach (Point3D pt in airpocketPoints)
            {
                // Is there a cube above?
                if (points.Any(p => p.X == pt.X && p.Y == pt.Y && p.Z == pt.Z + 1))
                {
                    surfaceArea--;
                }

                // Is there a cube above?
                if (points.Any(p => p.X == pt.X && p.Y == pt.Y && p.Z == pt.Z - 1))
                {
                    surfaceArea--;
                }

                // Is there a cube behind?
                if (points.Any(p => p.X == pt.X && p.Z == pt.Z && p.Y == pt.Y + 1))
                {
                    surfaceArea--;
                }

                // Is there a cube in front?
                if (points.Any(p => p.X == pt.X && p.Z == pt.Z && p.Y == pt.Y - 1))
                {
                    surfaceArea--;
                }

                // Is there a cube to the right?
                if (points.Any(p => p.Y == pt.Y && p.Z == pt.Z && p.X == pt.X + 1))
                {
                    surfaceArea--;
                }

                // Is there a cube to the left?
                if (points.Any(p => p.Y == pt.Y && p.Z == pt.Z && p.X == pt.X - 1))
                {
                    surfaceArea--;
                }
            }

            Console.WriteLine($"Acounting for air pockets, surface area = {surfaceArea}.");
        }

        private static IEnumerable<Point3D> GetNeighbors(
            Point3D current, 
            List<Point3D> points)
        {
            int x = current.X;
            int y = current.Y;
            int z = current.Z;

            Point3D neighbor;

            // Look above.
            neighbor = new Point3D(x, y, z + 1);
            if (!points.Contains(neighbor))
            {
                yield return neighbor;
            }

            // Look below.
            neighbor = new Point3D(x, y, z - 1);
            if (!points.Contains(neighbor))
            {
                yield return neighbor;
            }

            // Look behind.
            neighbor = new Point3D(x, y + 1, z);
            if (!points.Contains(neighbor))
            {
                yield return neighbor;
            }

            // Look in front.
            neighbor = new Point3D(x, y - 1, z);
            if (!points.Contains(neighbor))
            {
                yield return neighbor;
            }

            // Look to the left.
            neighbor = new Point3D(x - 1, y, z);
            if (!points.Contains(neighbor))
            {
                yield return neighbor;
            }

            // Look to the right.
            neighbor = new Point3D(x + 1, y, z);
            if (!points.Contains(neighbor))
            {
                yield return neighbor;
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
}