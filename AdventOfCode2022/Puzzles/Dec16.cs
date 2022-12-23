using AdventOfCode2022.Utilities;
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

            Dictionary<(Valve, Valve), int> dist = BuildDistDict(valveDict);

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

            Dictionary<(Valve, Valve), int> dist = BuildDistDict(valveDict);

            Valve start = valveDict["AA"];

            HashSet<Valve> humanReachable = Reachable(start, closedValves, dist, 26);
            HashSet<Valve> elephantReachable = Reachable(start, closedValves, dist, 26);

            (Valve, Valve, HashSet<Valve>, HashSet<Valve>, int) key = (start, start, humanReachable, elephantReachable, 26);

            int maxFlow = FindMaximalFlow2(memoized, valveDict, dist, key);

            Console.WriteLine($"Maximal flow is {maxFlow}.");
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

        private static string GetKey2((Valve, Valve, HashSet<Valve>, HashSet<Valve>, int) key)
        {
            (Valve valve1, Valve valve2, HashSet<Valve> set1, HashSet<Valve> set2, int minutesLeft) = key;

            var sb = new StringBuilder();
            sb.Append($"{valve1.Label},{valve2.Label},{{");
            sb.Append(string.Join(",", set1.Select(v => v.Label).OrderBy(v => v).ToArray()));
            sb.Append("},{");
            sb.Append(string.Join(",", set2.Select(v => v.Label).OrderBy(v => v).ToArray()));
            sb.Append($"}},{minutesLeft}");

            return sb.ToString();
        }
        
        private static Dictionary<(Valve, Valve), int> BuildDistDict(Dictionary<string, Valve> valveDict)
        {
            var dict = new Dictionary<(Valve, Valve), int>();

            foreach (Valve source in valveDict.Values)
            {
                var queue = new PriorityQueue<QueueItem>();

                foreach (Valve valve in valveDict.Values)
                {
                    int distance = (source == valve) ? 0 : 100000;

                    dict[(source, valve)] = distance;

                    queue.Enqueue(new QueueItem {  Distance = distance, Valve = valve });
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
            (Valve, Valve, HashSet<Valve>, HashSet<Valve>, int) key)
        {
            (Valve humanValve, Valve elephantValve, HashSet<Valve> humanReachable, HashSet<Valve> elephantReachable, int minutesLeft) = key;

            Console.SetCursorPosition(0, 10);
            Console.WriteLine($"Minutes left = {minutesLeft}.");

            if (minutesLeft <= 1)
            {
                return 0;
            }

            if (minutesLeft == 2 || (!humanReachable.Any() && !elephantReachable.Any()))
            {
                int flow = 0;
                if (humanReachable.Contains(humanValve))
                {
                    flow += humanValve.FlowRate * (minutesLeft - 1);
                }

                if (elephantValve != humanValve && elephantReachable.Contains(elephantValve))
                {
                    flow += elephantValve.FlowRate * (minutesLeft - 1);
                }

                return flow;
            }

            int maxWithoutHumanWithoutElephant = 0;
            int maxWithoutHumanWithElephant = 0;
            int maxWithHumanWithoutElephant = 0;
            int maxWithHumanWithElephant = 0;

            // Calculate what closed valves are reachable on the next turn in the time remaining, if the current valve
            // is closed and the agent spends this turn opening their valve.
            HashSet<Valve> humanReachableWith = Reachable(
                humanValve,
                humanReachable.Except(new[] { humanValve }).ToHashSet(),
                dist,
                minutesLeft - 1);

            HashSet<Valve> elephantReachableWith = Reachable(
                elephantValve,
                elephantReachable.Except(new[] { elephantValve }).ToHashSet(),
                dist,
                minutesLeft - 1);

            int max;

            (Valve, Valve, HashSet<Valve>, HashSet<Valve>, int) subkey;
            string subkeyStr;

            // If the human and elephant are at different valves, and both are closed,
            // comsoider the solution where both human and elephant open their valves on this turn
            if (humanValve != elephantValve && humanReachable.Contains(humanValve) && elephantReachable.Contains(elephantValve))
            {
                subkey = (
                    humanValve,
                    elephantValve,
                    humanReachableWith.Except(new[] { elephantValve }).ToHashSet(),
                    elephantReachableWith.Except(new[] { humanValve }).ToHashSet(),
                    minutesLeft - 1);
                subkeyStr = GetKey2(subkey);
                if (!memoized.ContainsKey(subkeyStr))
                {
                    memoized[subkeyStr] = FindMaximalFlow2(memoized, valveDict, dist, subkey);
                }

                max = (humanValve.FlowRate * (minutesLeft - 1)) +
                      (elephantValve.FlowRate * (minutesLeft - 1)) +
                      memoized[subkeyStr];
                if (max > maxWithHumanWithElephant)
                {
                    maxWithHumanWithElephant = max;
                }
            }

            // If the elephant's valve is closed, consider all the subproblems where the elephant spends this minute
            // opening its valve, while the human spends the turn moving to a neighboring valve.
            if (elephantReachable.Contains(elephantValve))
            {
                foreach(string humanLabel in humanValve.Neighbors)
                {
                    Valve humanNeighbor = valveDict[humanLabel];
                    HashSet<Valve> humanReachableWithout = Reachable(
                        humanNeighbor, 
                        humanReachable.Except(new[] { elephantValve }).ToHashSet(), 
                        dist, 
                        minutesLeft - 1);

                    subkey = (humanNeighbor, elephantValve, humanReachableWithout, elephantReachableWith, minutesLeft - 1);
                    subkeyStr = GetKey2(subkey);
                    if (!memoized.ContainsKey(subkeyStr))
                    {
                        memoized[subkeyStr] = FindMaximalFlow2(memoized, valveDict, dist, subkey);
                    }

                    max = (elephantValve.FlowRate * (minutesLeft - 1)) + memoized[subkeyStr];
                    if (max > maxWithoutHumanWithElephant)
                    {
                        maxWithoutHumanWithElephant = max;
                    }
                }
            }

            // If the humans's valve is closed, consider all the subproblems where the human spends this minute
            // opening its valve, while the elephant spends the turn moving to a neighboring valve.
            if (humanReachable.Contains(humanValve))
            {
                foreach (string elephantLabel in elephantValve.Neighbors)
                {
                    Valve elephantNeighbor = valveDict[elephantLabel];
                    HashSet<Valve> elephantReachableWithout = Reachable(
                        elephantNeighbor,
                        elephantReachable.Except(new[] { humanValve }).ToHashSet(),
                        dist,
                        minutesLeft - 1);

                    subkey = (humanValve, elephantNeighbor, humanReachableWith, elephantReachableWithout, minutesLeft - 1);
                    subkeyStr = GetKey2(subkey);
                    if (!memoized.ContainsKey(subkeyStr))
                    {
                        memoized[subkeyStr] = FindMaximalFlow2(memoized, valveDict, dist, subkey);
                    }

                    max = (humanValve.FlowRate * (minutesLeft - 1)) + memoized[subkeyStr];
                    if (max > maxWithHumanWithoutElephant)
                    {
                        maxWithHumanWithoutElephant = max;
                    }
                }
            }

            // Now consider all the subproblems where both human and elephant spend the current turn moving to a neighbor.
            foreach (string humanLabel in humanValve.Neighbors)
            {
                foreach (string elephantLabel in elephantValve.Neighbors)
                {
                    Valve humanNeighbor = valveDict[humanLabel];
                    Valve elephantNeighbor = valveDict[elephantLabel];

                    HashSet<Valve> humanReachableWithout = Reachable(humanNeighbor, humanReachable, dist, minutesLeft - 1);

                    HashSet<Valve> elephantReachableWithout = Reachable(elephantNeighbor, elephantReachable, dist, minutesLeft - 1);
                    
                    // Consider solution where both human and elephant move to their neighbors 
                    // on this turn.
                    subkey = (humanNeighbor, elephantNeighbor, humanReachableWithout, elephantReachableWithout, minutesLeft - 1);
                    subkeyStr = GetKey2(subkey);
                    if (!memoized.ContainsKey(subkeyStr))
                    {
                        memoized[subkeyStr] = FindMaximalFlow2(memoized, valveDict, dist, subkey);
                    }

                    max = memoized[subkeyStr];
                    if (max > maxWithoutHumanWithoutElephant)
                    {
                        maxWithoutHumanWithoutElephant = max;
                    }
                }
            }

            return (new[] { maxWithoutHumanWithoutElephant, maxWithHumanWithoutElephant, maxWithoutHumanWithElephant, maxWithHumanWithElephant }).Max();
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
