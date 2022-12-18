using AdventOfCode2022.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
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

            int maxFlow = GetMaxFlow(
                start,
                valveDict,
                closedValves,
                dist,
                memoized,
                30);

            Console.WriteLine($"Maximal flow is {maxFlow}.");
        }

        public static void SolvePartTwo()
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

            
            //Lets figure out which valves are reachble in 26 minutes - does that reduce the search space?
           
            var openValves = valveDict.Values.Where(
                v => !closedValves.Contains(v) && dist[(start, v)] < 25).ToHashSet();
            // Look at all possible ways of dividing unopened valves between me and elephant.

            int maxFlow = 0;
            
            for (int i = 0; i <= openValves.Count / 2; i++)
            {
                Console.SetCursorPosition(0, 5);
                Console.WriteLine($"Checking all sets of size {i}.");
                int count = 0;

                foreach (HashSet<Valve> subset in GetSubsets(openValves, i))
                {
                    HashSet<Valve> myClosedValves = closedValves.Union(subset).ToHashSet();
                    HashSet<Valve> elephantsClosedValves = closedValves.Union(openValves.Except(subset)).ToHashSet();

                    string myKey = GetKey(myClosedValves);
                    string elephantKey = GetKey(elephantsClosedValves);

                    count++;

                    Console.SetCursorPosition(0, 6);
                    Console.WriteLine($"Count = {count}.");

                    int myMax = GetMaxFlow(
                        start,
                        valveDict,
                        myClosedValves,
                        dist,
                        memoized,
                        26);

                    int elephantMax = GetMaxFlow(
                        start,
                        valveDict,
                        elephantsClosedValves,
                        dist,
                        memoized,
                        26);

                    int max = myMax + elephantMax;

                    if (max > maxFlow)
                    {
                        maxFlow = max;
                    }

                    Console.SetCursorPosition(0, 7);
                    Console.WriteLine($"Max = {maxFlow}.");
                }
            }

            Console.SetCursorPosition(0, 10);
            Console.WriteLine($"Maximal flow is {maxFlow}.");
        }

        private static string GetKey(HashSet<Valve> set)
        {
            return string.Join(",", set.Select(v => v.Label).OrderBy(v => v).ToArray());
        }

        private static int GetMaxFlow(
            Valve start,
            Dictionary<string , Valve> valveDict,
            HashSet<Valve> closedValves,
            Dictionary<(Valve, Valve), int> dist,
            Dictionary<(Valve, HashSet<Valve>, int), int> memoized,
            int minutesLeft)
        {
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
                    minutesLeft - distance);

                if (flow > maxFlow)
                {
                    maxFlow = flow;
                }
            }

            return maxFlow;
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

        private static IEnumerable<HashSet<Valve>> GetSubsets(HashSet<Valve> set, int size)
        {
            if (size == 0)
            {
                yield return new HashSet<Valve>();
            }
            else
            {
                var visited = new HashSet<string>();
                foreach (Valve valve in set)
                {
                    var withoutValve = set.Except(new[] { valve }).ToHashSet();
                    foreach (HashSet<Valve> subset in GetSubsets(withoutValve, size - 1))
                    {
                        HashSet<Valve> newset = subset.Union(new[] { valve }).ToHashSet();
                        string key = GetKey(newset);
                        if (!visited.Contains(key))
                        {
                            visited.Add(key);
                            yield return newset;
                        }
                    }
                }
            }
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
