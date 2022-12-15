using AdventOfCode2022.Utilities;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec15
    {
        public static void SolvePartOne()
        {
            var reg = new Regex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");

            int y = 2000000;
            var sensors = new List<Sensor>();

            foreach (string line in PuzzleReader.ReadLines(15))
            {
                Match match = reg.Match(line);

                var sensorLocation = new Point(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
                var beaconLocation = new Point(Int32.Parse(match.Groups[3].Value), Int32.Parse(match.Groups[4].Value));

                sensors.Add(new Sensor(sensorLocation, beaconLocation));
            }

            HashSet<Point> union = null;
            int setCount = 0;
            foreach (Sensor sensor in sensors)
            {
                if (union == null)
                {
                    union = sensor.PointsThatCannotContainBeacon(y);
                }
                else
                {
                    union = union.Union(sensor.PointsThatCannotContainBeacon(y)).ToHashSet();
                }

                setCount++;
                Console.WriteLine($"Processed {setCount} sets.");
            }

            int count = union.Count(p => p.Y == y);

            Console.WriteLine($"{count} points cannot contain a beacon.");
        }
    }

    public class Sensor
    {
        private readonly int distanceToNearestBeacon;
        private HashSet<Point> pointsThatCannotContainBeacon;

        public Sensor(Point location, Point nearestBeacon)
        {
            this.Location = location;
            this.NearestBeacon = nearestBeacon;
            distanceToNearestBeacon = Distance(this.NearestBeacon);
        }

        public Point Location { get; set; }

        public Point NearestBeacon { get; set; }

        public int Distance(Point pt)
        {
            return Math.Abs(pt.X - this.Location.X) + Math.Abs(pt.Y - this.Location.Y);
        }

        public HashSet<Point> PointsThatCannotContainBeacon(int y)
        {
            var set = new HashSet<Point>();
            int x = this.Location.X;
                    

            for (int i = x - distanceToNearestBeacon; i <= x + distanceToNearestBeacon; i++)
            {   
                var pt = new Point(i, y);
                if (pt != this.NearestBeacon && this.Distance(pt) <= distanceToNearestBeacon)
                {
                    set.Add(pt);
                }
            }

            return set;
        }
    }
}
