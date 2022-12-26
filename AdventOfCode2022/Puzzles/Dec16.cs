using AdventOfCode2022.Utilities;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec16
    {
        public static void SolvePartOne(bool isTest = false)
        {
            var reg = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnels lead to valves ([\w,\s]+)", RegexOptions.Compiled);
            var reg2 = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnel leads to valve ([\w,\s]+)", RegexOptions.Compiled);

            // Key is current valve, set of neighbors that are open, minutes left.
            var memoized = new Dictionary<string, int>();
            var valveDict = new Dictionary<string, Valve>();
            var closedValves = new HashSet<Valve>();

            foreach (string line in PuzzleReader.ReadLines(16, isTest))
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
                if (valve.FlowRate > 0)
                {
                    closedValves.Add(valve);
                }
            }

            Dictionary<(Valve, Valve), int> dist = BuildDistDict(valveDict, out _);

            /*foreach (KeyValuePair<(Valve, Valve), int> kvp in dist)
            {
                Console.WriteLine($"d({kvp.Key.Item1.Label}, {kvp.Key.Item2.Label}) = {kvp.Value}");
            }

            Console.ReadLine();*/

            Valve start = valveDict["AA"];

            HashSet<Valve> reachable = Reachable(start, closedValves, dist, 30);

            (Valve, HashSet<Valve>, int) key = (start, reachable, 30);

            int maxFlow = FindMaximalFlow(memoized, valveDict, dist, key);

            Console.WriteLine($"Maximal flow is {maxFlow}.");
        }

        public static void SolvePartTwo(bool isTest = false)
        {
            var reg = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnels lead to valves ([\w,\s]+)", RegexOptions.Compiled);
            var reg2 = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnel leads to valve ([\w,\s]+)", RegexOptions.Compiled);

            // Key is current valve, set of neighbors that are open, minutes left.
            var memoized = new Dictionary<string, int>();
            var valveDict = new Dictionary<string, Valve>();
            var closedValves = new HashSet<Valve>();

            foreach (string line in PuzzleReader.ReadLines(16, isTest))
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
                if (valve.FlowRate > 0)
                {
                    closedValves.Add(valve);
                }
            }

            var nextInPath = new Dictionary<(Valve, Valve), List<Valve>>();

            Dictionary<(Valve, Valve), int> dist = BuildDistDict2(valveDict, out nextInPath);

            Valve start = valveDict["AA"];

            var reachableList = Reachable(start, closedValves, dist, 26).ToList();

            int max = 0;
            for (int i = 0; i < reachableList.Count - 1; i++)
            {
                for (int j = i + 1; j < reachableList.Count; j++)
                {
                    Valve humanTarget = reachableList[i];
                    Valve elephantTarget = reachableList[j];

                    var key = (
                        start,
                        start,
                        humanTarget,
                        elephantTarget,
                        closedValves.Except(new[] { humanTarget, elephantTarget }).ToHashSet(),
                        26);

                    int result = FindMaximalFlow2(
                        memoized,
                        valveDict,
                        dist,
                        nextInPath,
                        key);

                    if (result > max)
                    {
                        max = result;
                    }
                }
            }

            Console.WriteLine($"Maximal flow is {max}.");
        }

        private static string GetKey((Valve, HashSet<Valve>, int) key)
        {
            (Valve valve, HashSet<Valve> set, int minutesLeft) = key;

            var sb = new StringBuilder();
            sb.Append($"{valve.Label},{{");
            sb.Append(string.Join(",", set.Select(v => v.Label).OrderBy(v => v).ToArray()));
            sb.Append($"}},{minutesLeft}");

            return sb.ToString();
        }

        private static string GetKey2((Valve, Valve, Valve, Valve, HashSet<Valve>, int) key)
        {
            (Valve valve1, Valve valve2, Valve target1, Valve target2, HashSet<Valve> set, int minutesLeft) = key;

            var sb = new StringBuilder();
            sb.Append($"{valve1.Label},{valve2.Label},{target1?.Label},{target2?.Label},{{");
            sb.Append(string.Join(",", set.Select(v => v.Label).OrderBy(v => v).ToArray()));
            sb.Append($"}},{minutesLeft}");

            return sb.ToString();
        }

        private static Dictionary<(Valve, Valve), int> BuildDistDict(
            Dictionary<string, Valve> valveDict,
            out Dictionary<(Valve, Valve), List<Valve>> shortestPaths)
        {
            var dict = new Dictionary<(Valve, Valve), int>();
            shortestPaths = new Dictionary<(Valve, Valve), List<Valve>>();

            foreach (Valve source in valveDict.Values)
            {
                var queue = new PriorityQueue<QueueItem>();
                var pred = new Dictionary<(Valve, Valve), Valve>();

                foreach (Valve valve in valveDict.Values)
                {
                    int distance = (source == valve) ? 0 : 100000;

                    dict[(source, valve)] = distance;

                    queue.Enqueue(new QueueItem { Distance = distance, Valve = valve });
                }

                while (queue.Count() > 0)
                {
                    QueueItem current = queue.Dequeue();
                    Valve currentValve = current.Valve;
                    foreach (string neighborLabel in currentValve.Neighbors)
                    {
                        Valve neighbor = valveDict[neighborLabel];
                        int alt = dict[(source, currentValve)] + 1;
                        if (alt < dict[(source, neighbor)])
                        {
                            dict[(source, neighbor)] = alt;

                            // pred[(s, t)] is the predecessor of t in the 
                            // shortest path from s to t.
                            pred[(source, neighbor)] = currentValve;

                            if (queue.Data.Any(q => q.Valve == neighbor))
                            {
                                QueueItem queueItem = queue.Data.First(q => q.Valve == neighbor);
                                queue.Data.Remove(queueItem);
                            }

                            queue.Enqueue(new QueueItem { Valve = neighbor, Distance = alt });
                        }
                    }
                }
            }

            return dict;
        }

        private static Dictionary<(Valve, Valve), int> BuildDistDict2(
           Dictionary<string, Valve> valveDict,
           out Dictionary<(Valve, Valve), List<Valve>> shortestPaths)
        {
            var dict = new Dictionary<(Valve, Valve), int>();
            shortestPaths = new Dictionary<(Valve, Valve), List<Valve>>();

            foreach (Valve source in valveDict.Values)
            {
                var queue = new Queue<(Valve, List<Valve>)>();
                var visited = new HashSet<Valve>();

                queue.Enqueue((source, new List<Valve>()));
                visited.Add(source);

                dict[(source, source)] = 0;

                while (queue.Count() > 0)
                {
                    (Valve currentValve, List<Valve> path) = queue.Dequeue();
                    foreach (string neighborLabel in currentValve.Neighbors)
                    {
                        Valve neighbor = valveDict[neighborLabel];
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);

                            List<Valve> newPath = new List<Valve>(path);
                            newPath.Add(neighbor);

                            shortestPaths.Add((source, neighbor), newPath);

                            dict[(source, neighbor)] = newPath.Count;

                            queue.Enqueue((neighbor, newPath));
                        }
                    }
                }
            }

            return dict;
        }

        // We memoize the solution for each combination of:
        // (1) The valve that we are currently in front of.
        // (2) The set of valves that can be usefully opened (i.e., they are close enough
        //     that we can get to them in time to turn them on and get some flow before
        //     the clock runs out.
        // (3) The number of minutes left
        private static int FindMaximalFlow(
            Dictionary<string, int> memoized,
            Dictionary<string, Valve> valveDict,
            Dictionary<(Valve, Valve), int> dist,
            (Valve, HashSet<Valve>, int) key)
        {
            (Valve currentValve, HashSet<Valve> closedValves, int minutesLeft) = key;

            //Console.SetCursorPosition(0, 5);
            //Console.WriteLine($"Minutes Left = {minutesLeft}.");

            if (minutesLeft <= 1)
            {
                return 0;
            }

            if (minutesLeft == 2 || !closedValves.Any())
            {
                return closedValves.Contains(currentValve) ? currentValve.FlowRate * (minutesLeft - 1) : 0;
            }

            int maxScoreWithCurrentValve = 0;
            int maxScoreWithoutCurrentValve = 0;

            foreach (string neighborLabel in currentValve.Neighbors)
            {
                Valve neighbor = valveDict[neighborLabel];

                /*if (neighbor == previousValve)
                {
                    continue;
                }  */

                // Calculate the maximal value of the subproblem that does not include the current valve
                HashSet<Valve> reachableFromNeighborWithout = Reachable(neighbor, closedValves, dist, minutesLeft - 1);
                var subkey = (neighbor, reachableFromNeighborWithout, minutesLeft - 1);
                var subKeyStr = GetKey(subkey);

                if (!memoized.ContainsKey(subKeyStr))
                {
                    memoized[subKeyStr] = FindMaximalFlow(memoized, valveDict, dist, subkey);
                }
                /*else
                {
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine("Cache hit.");
                }*/

                int maxWithout = memoized[subKeyStr];

                if (maxWithout > maxScoreWithoutCurrentValve)
                {
                    maxScoreWithoutCurrentValve = maxWithout;
                }

                if (closedValves.Contains(currentValve))
                {
                    HashSet<Valve> reachableFromNeighborWith = Reachable(
                        neighbor,
                        closedValves.Except(new[] { currentValve }).ToHashSet(),
                        dist,
                        minutesLeft - 2);

                    subkey = (neighbor, reachableFromNeighborWith, minutesLeft - 2);
                    subKeyStr = GetKey(subkey);
                    if (!memoized.ContainsKey(subKeyStr))
                    {
                        memoized[subKeyStr] = FindMaximalFlow(memoized, valveDict, dist, subkey);
                    }
                    /*else
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("Cache hit.");
                    }*/

                    int maxWith = (currentValve.FlowRate * (minutesLeft - 1)) + memoized[subKeyStr];

                    if (maxWith > maxScoreWithCurrentValve)
                    {
                        maxScoreWithCurrentValve = maxWith;
                    }
                }
            }

            return Math.Max(maxScoreWithoutCurrentValve, maxScoreWithCurrentValve);
        }

        // We memoize the solution for each combination of:
        // (1) The valve that each actor is currently in front of.
        // (2) The set of valves that can be usefully opened by each actor (i.e., they are close enough
        //     that we can get to them in time to turn them on and get some flow before
        //     the clock runs out.
        // (3) The number of minutes left
        private static int FindMaximalFlow2(
            Dictionary<string, int> memoized,
            Dictionary<string, Valve> valveDict,
            Dictionary<(Valve, Valve), int> dist,
            Dictionary<(Valve, Valve), List<Valve>> nextInPath,
            (Valve, Valve, Valve, Valve, HashSet<Valve>, int) key)
        {
            (Valve humanCurrent,
                Valve elephantCurrent,
                Valve humanTarget,
                Valve elephantTarget,
                HashSet<Valve> remainingValves, int minutesLeft) = key;

            Console.WriteLine($"{minutesLeft} minutes left.");

            if (minutesLeft < 2)
            {
                return 0;
            }

            int flow = 0;
            if (minutesLeft == 2)
            {
                if (humanCurrent == humanTarget)
                {
                    Console.WriteLine($"Human opens valve {humanTarget.Label}.");
                    flow += humanCurrent.FlowRate;
                }

                if (elephantCurrent == elephantTarget)
                {
                    Console.WriteLine($"Elephant opens valve {elephantTarget.Label}.");
                    flow += elephantCurrent.FlowRate;
                }

                return flow;
            }

            var nextHumanValves = new List<(Valve?, Valve?)>();
            var nextElephantValves = new List<(Valve?, Valve?)>();

            bool humanOpenedValve = false, elephantOpenedValve = false;

            if (humanCurrent == humanTarget)
            {
                flow += humanTarget.FlowRate * (minutesLeft - 1);
                Console.WriteLine($"Human opens valve {humanTarget.Label}.");
                humanOpenedValve = true;
            }

            if (elephantCurrent == elephantTarget)
            {
                flow += elephantTarget.FlowRate * (minutesLeft - 1);
                Console.WriteLine($"Elephant opens valve {elephantTarget.Label}.");
                elephantOpenedValve = true;
            }

            if (humanOpenedValve)
            {
                var humanReachable = Reachable(humanCurrent, remainingValves, dist, minutesLeft - 1);
                foreach (Valve nextHuman in humanReachable)
                {
                    nextHumanValves.Add((nextInPath[(humanCurrent, nextHuman)].First(), nextHuman));
                }
            }
            else if (humanTarget != null)
            {
                Valve next = nextInPath[(humanCurrent, humanTarget)].First();
                Console.WriteLine($"Human moves to {next.Label}.");
                nextHumanValves.Add((next, humanTarget));
            }

            if (elephantOpenedValve)
            {
                var elephantReachable = Reachable(elephantCurrent, remainingValves, dist, minutesLeft - 1);
                foreach (Valve nextElephant in elephantReachable)
                {
                    nextElephantValves.Add((nextInPath[(elephantCurrent, nextElephant)].First(), nextElephant));
                }
            }
            else if (elephantTarget != null)
            {
                Valve next = nextInPath[(elephantCurrent, elephantTarget)].First();
                Console.WriteLine($"Elephant moves to {next.Label}.");
                nextElephantValves.Add((next, elephantTarget));
            }

            if (!nextHumanValves.Any() && !nextElephantValves.Any())
            {
                Console.WriteLine("==  DONE ==");
                return flow;
            }

            if (!nextHumanValves.Any())
            {
                nextHumanValves.Add((humanCurrent, null));
            }

            if (!nextElephantValves.Any())
            {
                nextElephantValves.Add((elephantCurrent, null));
            }

            if (nextHumanValves.Count == 1 && nextElephantValves.Count == 1)
            {
                var (h1, h2) = nextHumanValves.First();
                var (e1, e2) = nextElephantValves.First();

                // If there's only one reachable valve left for each agent,
                // and it's the same valve, let the human take care of it.
                if (h2 != null && e2 != null && h2 == e2)
                {
                    (Valve, Valve, Valve, Valve, HashSet<Valve>, int) subKey = (
                        h1,
                        e1,
                        h2,
                        null,
                        new HashSet<Valve>(),
                        minutesLeft - 1);
                    var subKeyStr = GetKey2(subKey);

                    if (!memoized.ContainsKey(subKeyStr))
                    {
                        memoized[subKeyStr] = FindMaximalFlow2(
                            memoized,
                            valveDict,
                            dist,
                            nextInPath,
                            subKey);

                        (Valve, Valve, Valve, Valve, HashSet<Valve>, int) subKey2 = (
                           e1,
                           h1,
                           null,
                           h2,
                           new HashSet<Valve>(),
                           minutesLeft - 1);
                        var subKeyStr2 = GetKey2(subKey);

                        memoized[subKeyStr2] = memoized[subKeyStr];
                    }

                    return flow + memoized[subKeyStr];
                }
            }

            int max = 0;
            foreach (var (h1, h2) in nextHumanValves)
            {
                foreach (var (e1, e2) in nextElephantValves)
                {
                    if (h2 != null && e2 != null && h2 == e2)
                    {
                        continue;
                    }

                    var nextRemaining = remainingValves.Except(new[] { h2, e2 }).ToHashSet();
                    var subKey = (
                        h1,
                        e1,
                        h2,
                        e2,
                        nextRemaining,
                        minutesLeft - 1);
                    var subKeyStr = GetKey2(subKey);

                    if (!memoized.ContainsKey(subKeyStr))
                    {
                        memoized[subKeyStr] = FindMaximalFlow2(
                            memoized,
                            valveDict,
                            dist,
                            nextInPath,
                            subKey);

                        var subKey2 = (
                            e1,
                            h1,
                            e2,
                            h2,
                            nextRemaining,
                            minutesLeft - 1);
                        var subKeyStr2 = GetKey2(subKey2);

                        memoized[subKeyStr2] = memoized[subKeyStr];
                    }

                    int result = memoized[subKeyStr];

                    if (result > max)
                    {
                        max = result;
                    }
                }
            }

            return flow + max;
        }

        private static HashSet<Valve> Reachable(
            Valve currentValve,
            HashSet<Valve> valves,
            Dictionary<(Valve, Valve), int> dist,
            int minutesLeft)
        {
            return valves.Where(v => dist[(currentValve, v)] < minutesLeft - 1).ToHashSet();
        }
    }

    public struct QueueItem : IComparable<QueueItem>
    {
        public Valve Valve { get; set; }

        public int Distance { get; set; }

        public int CompareTo(QueueItem other)
        {
            if (this.Distance < other.Distance)
            {
                return -1;
            }
            else if (this.Distance == other.Distance)
            {
                return 0;
            }

            return 1;
        }
    }

    public class Valve
    {
        private int openCount;

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
