﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec16
    {
        public static void SolvePartOne()
        {
            var valveDict = new Dictionary<string, Valve>();

        }

        // We memoize the solution for each combination of:
        // (1) the valve that we are currently in front of
        // (2) the set of other valves that haven't been opened yet
        // (3) the number of minutes left
        private static int FindMaximalFlow(
            Dictionary<Tuple<Valve, HashSet<Valve>, int>, int> memoized, 
            Dictionary<string, Valve> valveDict,
            Valve currentValve, 
            HashSet<Valve> closedValves, 
            int minutesLeft)
        {
            if (closedValves.Count == 0)
            {
                return 0;
            }

            if (minutesLeft < 2)
            {
                // Not enough time to open the valve and get any flow.
                return 0;
            }

            bool isCurrentOpen = closedValves.Contains(currentValve);

            // 3 minutes left - move to other valve
            // 2 minutes left - open valve
            // 1 minute left - valve flows
            if (minutesLeft < 3)
            {
                // No other closed valves to visit and open,
                // or there isn't enough time to move to the next valve
                // and open it for it to make any difference.
                return isCurrentOpen ? currentValve.FlowRate * (minutesLeft - 1) : 0;
            }

            int maxWithoutCurrent = Int32.MinValue;
            int maxWithCurrent = Int32.MinValue;

            HashSet<Valve> closedPlusCurrent = closedValves.Union(new[] { currentValve }).ToHashSet();

            foreach (string neighborLabel in currentValve.Neighbors)
            {
                Valve neighbor = valveDict[neighborLabel];
                if (!closedValves.Contains(neighbor))
                {
                    var key = new Tuple<Valve, HashSet<Valve>, int>(neighbor, closedValves, minutesLeft - 1);
                    if (!memoized.ContainsKey(key))
                    {
                        memoized[key] = FindMaximalFlow(memoized, valveDict, neighbor, closedValves, minutesLeft - 1);
                    }

                    int withoutCurrent = memoized[key];
                    if (withoutCurrent > maxWithoutCurrent)
                    {
                        maxWithoutCurrent = withoutCurrent;
                    }

                    // If current valve is open
                    // Consider solution where we open current valve then move to neighbor.
                    if (isCurrentOpen)
                    {
                        key = new Tuple<Valve, HashSet<Valve>, int>(neighbor, closedPlusCurrent, minutesLeft - 2);
                        if (!memoized.ContainsKey(key))
                        {
                            memoized[key] = FindMaximalFlow(memoized, valveDict, neighbor, closedPlusCurrent, minutesLeft - 2);
                        }

                        int withCurrent = memoized[key] + currentValve.FlowRate * (minutesLeft - 1);
                        if (withCurrent > maxWithCurrent)
                        {
                            maxWithCurrent = withCurrent;
                        }
                    }

                }
            }

            return Math.Max(maxWithoutCurrent, maxWithCurrent);
        }
    }

    public class Valve
    {
        public Valve(int flowRate, List<string> neighbors)
        {
            this.FlowRate = flowRate;
            this.Neighbors = neighbors;
        }

        public int FlowRate { get; private set; }

        public List<string> Neighbors { get; private set; }
    }
}
