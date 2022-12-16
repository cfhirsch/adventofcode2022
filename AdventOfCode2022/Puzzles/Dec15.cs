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

            var ranges = new Ranges(y);
            foreach (Sensor sensor in sensors)
            {
                sensor.UpdateRanges(ranges);
            }

            int count = ranges.GetCount();

            Console.WriteLine($"{count} points cannot contain a beacon.");
        }
    }

    public class Range
    {
        public int Lower { get; set; }

        public int Upper { get; set; }
    }

    public class Ranges
    {
        private List<Range> ranges;

        public Ranges(int y)
        {
            this.ranges = new List<Range>();
            this.Y = y;
        }

        public int Y { get; set; }

        public void AddRange(Range range)
        {
            List<Range> intersected = this.ranges.Where(r => (range.Lower >= r.Lower && range.Lower <= r.Upper) || (range.Upper >= r.Lower && range.Upper <= r.Upper) ||
                                                             (r.Lower >= range.Lower && r.Lower <= range.Upper) || (r.Upper >= range.Lower && r.Upper <= range.Upper)).ToList();
            while (intersected.Any())
            {
                Range range2 = intersected.First();
                this.ranges.Remove(range2);

                range = Merge(range, range2);

                intersected = this.ranges.Where(r => (range.Lower >= r.Lower && range.Lower <= r.Upper) || (range.Upper >= r.Lower && range.Upper <= r.Upper)).ToList();
            }

            this.ranges.Add(range);
        }

        public int GetCount()
        {
            return this.ranges.Select(r => r.Upper - r.Lower + 1).Sum();
        }

        public void Remove(int x)
        {
            Range range = this.ranges.FirstOrDefault(r => r.Lower <= x && r.Upper <= x);
            if (range != null)
            {
                this.ranges.Remove(range);

                // Case 1 - x is the lower endpoint of range.
                if (range.Lower == x)
                {
                    // If range.Upper = x then we just remove the range.
                    if (range.Upper > x)
                    {
                        this.ranges.Add(new Range { Lower = x + 1, Upper = range.Upper });
                    }
                }
                // Case 2 - x is the upper endpoint of range
                else if (range.Upper == x)
                {
                    // If range.Lower = x then we just remove the range.
                    if (range.Lower < x)
                    {
                        this.ranges.Add(new Range { Lower = x, Upper = range.Upper - 1 });
                    }
                }
                // Case 3 - range.Lower < x < range.Upper
                else
                {
                    var range1 = new Range { Lower = range.Lower, Upper = x - 1 };
                    var range2 = new Range { Lower = x + 1, Upper = range.Upper };
                    this.ranges.AddRange(new[] { range1, range2 });
                }
            }
        }

        public static Range Merge(Range range1, Range range2)
        {
            return new Range { Lower = Math.Min(range1.Lower, range2.Lower), Upper = Math.Max(range1.Upper, range2.Upper) };
        }
    }

    public class Sensor
    {
        private readonly int distanceToNearestBeacon;
        
        public Sensor(Point location, Point nearestBeacon)
        {
            this.Location = location;
            this.NearestBeacon = nearestBeacon;
            distanceToNearestBeacon = Distance(this.NearestBeacon);
        }

        public Point Location { get; set; }

        public Point NearestBeacon { get; set; }

        public void UpdateRanges(Ranges ranges)
        {
            int yDist = Math.Abs(ranges.Y - this.Location.Y);
            if (yDist > this.distanceToNearestBeacon)
            {
                return;
            }

            int xRange = this.distanceToNearestBeacon - yDist;
            int xLow = this.Location.X - xRange;
            int xHigh = this.Location.X + xRange;

            if (this.NearestBeacon.Y == ranges.Y)
            {
                if (this.NearestBeacon.X == xLow)
                {
                    ranges.Remove(xLow);
                    xLow++;
                }

                if (this.NearestBeacon.X == xHigh)
                {
                    ranges.Remove(xHigh);
                    xHigh--;
                }
            }

            if (xLow <= xHigh)
            {
                ranges.AddRange(new Range { Lower = xLow, Upper = xHigh });
            }
        }

        private int Distance(Point pt)
        {
            return Math.Abs(pt.X - this.Location.X) + Math.Abs(pt.Y - this.Location.Y);
        }
    }
}
