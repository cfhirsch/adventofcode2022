using AdventOfCode2022.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec16
    {
        public static void SolvePartOne()
        {
            var reg = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnels lead to valves ([\w,\s]+)", RegexOptions.Compiled);
            var reg2 = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnel leads to valve ([\w,\s]+)", RegexOptions.Compiled);

            // Key is current valve, set of neighbors that are open, minutes left.
            var memoized = new Dictionary<(Valve, HashSet<Valve>, int), int>();
            var valveDict = new Dictionary<string, Valve>();
            var closedValves = new HashSet<Valve>();

            foreach (string line in PuzzleReader.ReadLines(16))
            {
                Match match = reg.Match(line);
                if (!match.Success)
                {
                    match = reg2.Match(line);
                }
                string label = match.Groups[1].Value;
                int flow = Int32.Parse(match.Groups[2].Value);
                string neighbors = match.Groups[3].Value;
                List<string> neighborParts = neighbors.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList();
                var valve = new Valve(label, flow, neighborParts);

                valveDict.Add(label, valve);

                // We consider any "broken" valves to already be open.
                if (valve.FlowRate == 0)
                {
                    closedValves.Add(valve);
                }
            }

            Dictionary<(Valve, Valve), int> dist = BuildDistDict(valveDict);

            Valve start = valveDict["AA"];

            int maxFlow = 0;

            foreach (Valve valve in valveDict.Values)
            {
                if (closedValves.Contains(valve))
                {
                    continue;
                }

                int distance = dist[(start, valve)];
                int flow = FindMaximalFlow(
                    memoized,
                    valveDict,
                    valve,
                    closedValves,
                    dist,
                    30 - distance);

                if (flow > maxFlow)
                {
                    maxFlow = flow;
                }
            }

            Console.WriteLine($"Maximal flow is {maxFlow}.");
        }

        private static Dictionary<(Valve, Valve), int> BuildDistDict(Dictionary<string, Valve> valveDict)
        {
            var dict = new Dictionary<(Valve, Valve), int>();

            foreach (Valve valve1 in valveDict.Values)
            {
                foreach (Valve valve2 in valveDict.Values)
                {
                    int dist = (valve1 == valve2) ? 0 : 1000000;
                    dict.Add((valve1, valve2), dist);
                }
            }

            foreach (Valve valve in valveDict.Values)
            {
                foreach (string neighborLabel in valve.Neighbors)
                {
                    Valve neighbor = valveDict[neighborLabel];
                    dict[(valve, neighbor)] = 1;
                }
            }

            foreach (Valve valve1 in valveDict.Values)
            {
                foreach (Valve valve2 in valveDict.Values)
                {
                    foreach (Valve valve3 in valveDict.Values)
                    {
                        if (dict[(valve1, valve3)] > dict[(valve1, valve2)] + dict[(valve2, valve3)])
                        {
                            dict[(valve1, valve3)] = dict[(valve1, valve2)] + dict[(valve2, valve3)];
                        }
                    }
                }
            }

            return dict;
        }

        // We memoize the solution for each combination of:
        // (1) the valve that we are currently in front of (we assume current valve is open).
        // (2) the set of other valves that haven't been opened yet
        // (3) the number of minutes left
        private static int FindMaximalFlow(
            Dictionary<(Valve, HashSet<Valve>, int), int> memoized,
            Dictionary<string, Valve> valveDict,
            Valve currentValve,
            HashSet<Valve> closedValves,
            Dictionary<(Valve, Valve), int> dist,
            int minutesLeft)
        {
            if (minutesLeft <= 1)
            {
                return 0;
            }

            if (minutesLeft == 2)
            {
                return currentValve.FlowRate;
            }

            int maxScoreWithCurrentValve = 0;
            int maxScoreWithoutCurrentValve = 0;

            // Find all the remaining unopened valves that could be reached in time 
            // to usefully open them.
            IEnumerable<Valve> reachable = valveDict.Values.Where(
                v => v != currentValve && !closedValves.Contains(v) && dist[(currentValve, v)] < minutesLeft - 1);

            if (!reachable.Any())
            {
                return currentValve.FlowRate * (minutesLeft - 1);
            }

            var closedWith = closedValves.Union(new[] { currentValve }).ToHashSet();

            foreach (Valve valve in reachable)
            {
                int distance = dist[(currentValve, valve)];
                if (!memoized.ContainsKey((valve, closedValves, minutesLeft - distance)))
                {
                    memoized[(valve, closedValves, minutesLeft - distance)] =
                        FindMaximalFlow(memoized, valveDict, valve, closedValves, dist, minutesLeft - distance);
                }

                int maxWithout = memoized[(valve, closedValves, minutesLeft - distance)];

                if (maxWithout > maxScoreWithoutCurrentValve)
                {
                    maxScoreWithoutCurrentValve = maxWithout;
                }

                if (!memoized.ContainsKey((valve, closedWith, minutesLeft - distance - 1)))
                {
                    memoized[(valve, closedWith, minutesLeft - distance - 1)] =
                        FindMaximalFlow(memoized, valveDict, valve, closedWith, dist, minutesLeft - distance - 1);
                }

                int maxWith = (currentValve.FlowRate * (minutesLeft - 1)) + memoized[(valve, closedWith, minutesLeft - distance - 1)];

                if (maxWith > maxScoreWithCurrentValve)
                {
                    maxScoreWithCurrentValve = maxWith;
                }
            }

            return Math.Max(maxScoreWithoutCurrentValve, maxScoreWithCurrentValve);
        }
    }

    public class Valve
    {
        public Valve(string label, int flowRate, List<string> neighbors)
        {
            this.Label = label;
            this.FlowRate = flowRate;
            this.Neighbors = neighbors;
        }

        public string Label { get; private set; }

        public int FlowRate { get; private set; }

        public List<string> Neighbors { get; private set; }
    }
}
