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

            for (int i = 0; i <= 30; i++)
            {
                foreach (Valve valve in valveDict.Values)
                {
                    if (i == 30 && valve.Label != "AA")
                    {
                        // For i = 30, we only care about the solution for "AA".
                        continue;
                    }

                    // Find all non-broken valves that could be opened in time remaining (including current one).
                    HashSet<Valve> reachable = valveDict.Values.Where(v => !closedValves.Contains(v) && dist[(valve, v)] <= i - 2).ToHashSet();

                    // Now we need to consider every permutation of opened/closed for these reachable valves.
                    foreach (HashSet<Valve> subset in GetSubsets(reachable))
                    {
                        if (i <= 1)
                        {
                            memoized[(valve, subset, i)] = 0;
                        }
                        else if (i == 2)
                        {
                            memoized[(valve, subset, i)] = subset.Contains(valve) ? 0 : valve.FlowRate;
                        }
                        else
                        {
                            int maxWith = Int32.MinValue;
                            int maxWithout = Int32.MinValue;
                            foreach (Valve neighbor in subset)
                            {
                                int distance = dist[(valve, neighbor)];
                                int without = memoized[(neighbor, subset, i - distance)];

                                if (without > maxWithout)
                                {
                                    maxWithout = without;
                                }

                                if (!subset.Contains(valve))
                                {
                                    int with = valve.FlowRate * (i - 1) + memoized[(neighbor, subset, i - distance - 1)];
                                    if (with > maxWith)
                                    {
                                        maxWith = with;
                                    }
                                }
                            }

                            memoized[(valve, subset, i)] = Math.Max(maxWith, maxWithout)];
                        }
                    }
                }
            }

            Valve start = valveDict["AA"];
            int maxFlow = memoized[(start, closedValves, 30)];
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

        private static IEnumerable<HashSet<Valve>> GetSubsets(HashSet<Valve> valves)
        {
            int count = valves.Count;
            for (int i = 0; i <= count; i++)
            {
                foreach (HashSet<Valve> subset in GetSubsets(valves, i))
                {
                    yield return subset;
                }
            }

        }

        private static IEnumerable<HashSet<Valve>> GetSubsets(HashSet<Valve> valves, int size)
        {
            if (size == 0)
            {
                yield return new HashSet<Valve>();
            }

            foreach (Valve valve in valves)
            {
                foreach (HashSet<Valve> subsets in GetSubsets(valves.Except(new[] { valve }).ToHashSet(), size - 1))
                {
                    yield return subsets.Union(new[] { valve }).ToHashSet();
                }
            }
        }

        // We memoize the solution for each combination of:
        // (1) the valve that we are currently in front of
        // (2) the set of other valves that haven't been opened yet
        // (3) the number of minutes left
        private static int FindMaximalFlow(
            Dictionary<(Valve, HashSet<Valve>, int), int> memoized, 
            Dictionary<string, Valve> valveDict,
            Valve currentValve, 
            HashSet<Valve> openedValves,
            Dictionary<(Valve, Valve), int> dist,
            int minutesLeft)
        {
            if (minutesLeft <= 1)
            {
                return 0;
            }

            if (minutesLeft == 2)
            {
                return openedValves.Contains(currentValve) ? 0 : currentValve.FlowRate;
            }

            int maxScoreWithCurrentValve = Int32.MinValue;
            int maxScoreWithoutCurrentValve = Int32.MinValue;
            
            foreach (Valve valve in valveDict.Values.Where(v => v != currentValve && !openedValves.Contains(v)))
            {
                int distance = dist[(currentValve, valve)];
                if (!memoized.ContainsKey((valve, openedValves, minutesLeft - distance)))
                {
                    memoized[(valve, openedValves, minutesLeft - distance)] = FindMaximalFlow(memoized, valveDict, valve, openedValves, dist, minutesLeft - distance);
                }

                int scoreWithout = memoized[(valve, openedValves, minutesLeft - distance)];
                if (scoreWithout > maxScoreWithoutCurrentValve)
                {
                    maxScoreWithoutCurrentValve = scoreWithout;
                }

                if (!openedValves.Contains(valve))
                {
                    if (!memoized.ContainsKey((valve, openedValves, minutesLeft - distance - 1)))
                    {
                        memoized[(valve, openedValves, minutesLeft - distance)] = FindMaximalFlow(memoized, valveDict, valve, openedValves, dist, minutesLeft - distance - 1);
                    }

                    int scoreWith = currentValve.FlowRate * (minutesLeft - 1) + memoized[(valve, openedValves, minutesLeft - distance)];
                    if (scoreWith > maxScoreWithCurrentValve)
                    {
                        maxScoreWithCurrentValve = scoreWith;
                    }
                }
            }

            return Math.Max(maxScoreWithoutCurrentValve, maxScoreWithCurrentValve);
        }
    }

    public class Valve
    {
        public Valve(string label,int flowRate, List<string> neighbors)
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
