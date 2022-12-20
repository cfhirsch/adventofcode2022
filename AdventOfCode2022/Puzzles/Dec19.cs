using AdventOfCode2022.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    internal class Dec19
    {
        public static void SolvePartOne()
        {
            var bluePrints = new List<BluePrint>();

            foreach (string line in PuzzleReader.ReadLines(19, isTest: true))
            {
                bluePrints.Add(new BluePrint(line));
            }

            // The state we are interested in is:
            // (1) Number of robots of each type.
            // (2) Number of resources of each type.
            // (3) Number of opened geodes.
            // (4) Number of minutes left.
            var keyReg = new Regex(@"O=(\d+),C=(\d+),Ob=(\d+),G=(\d+),Ore=(\d+),Clay=(\d+),Obs=(\d+),OG=(\d+),M=(\d+)", RegexOptions.Compiled);

            var maxDict = new Dictionary<int, int>();

            foreach (BluePrint bluePrint in bluePrints)
            {
                int maxOpenable = 0;
                var required = bluePrint.ResourceDict[ResourceType.Geode];
                int obsidianRequired = required.First(r => r.Item1 == ResourceType.Obsidian).Item2;
                int oreRequiredForGeode = required.First(r => r.Item1 == ResourceType.Ore).Item2;

                required = bluePrint.ResourceDict[ResourceType.Obsidian];
                int clayRequired = required.First(r => r.Item1 == ResourceType.Clay).Item2;
                int oreRequiredForObsidian = required.First(r => r.Item1 == ResourceType.Ore).Item2;

                required = bluePrint.ResourceDict[ResourceType.Clay];
                int oreRequiredForClay = required.First(r => r.Item1 == ResourceType.Ore).Item2;

                required = bluePrint.ResourceDict[ResourceType.Ore];
                int oreRequiredForOre = required.First(r => r.Item1 == ResourceType.Ore).Item2;

                var queue = new Stack<MiningState>();
                queue.Push(new MiningState(
                    1,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    1));

                int visitHit = 0;
                var visited = new HashSet<MiningState>();

                while (queue.Count() > 0)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine($"Queue size = {queue.Count}.");
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine($"Max found = {maxOpenable}.");
                    Console.SetCursorPosition(0, 2);
                    
                    MiningState miningState = queue.Pop();
                    visited.Add(miningState);
                    Console.WriteLine($"Minute = {miningState.Minute}");

                    Console.SetCursorPosition(0, 3);
                    Console.WriteLine($"Visit hit = {visitHit}.");

                    if (miningState.Minute == 25)
                    {
                        if (miningState.Geodes > maxOpenable)
                        {
                            maxOpenable = miningState.Geodes;
                        }

                        continue;
                    }

                    if (miningState.Geodes == 0)
                    {
                        // We don't have time to create a geode robot to open any geodes.
                        if (miningState.GeodeRobots == 0)
                        {
                            if (miningState.Minute == 24)
                            {
                                continue;
                            }

                            if (miningState.Minute == 23)
                            {
                                // If we don't have any geodes opened yet, we would need to
                                // create a geode robot at time 23.
                                if (miningState.ObsidianRobots == 0 ||
                                    miningState.Obsidian < obsidianRequired)
                                {
                                    continue;
                                }
                            }

                            if (miningState.Minute == 22)
                            {
                                // If we don't have any geodes opened yet, we need to 
                                // create an obsidion robot by time 22, so that we can 
                                // create a geode robot at time 23.
                                // In order to have that, we need a clay robot at time 22.
                                if (miningState.ObsidianRobots == 0)
                                {
                                    if (miningState.ClayRobots == 0 ||
                                        miningState.Clay < clayRequired * obsidianRequired)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    int nextOre = miningState.Ore + miningState.OreRobots;
                    int nextClay = miningState.Clay + miningState.ClayRobots;
                    int nextObsidian = miningState.Obsidian + miningState.ObsidianRobots;
                    int nextGeode = miningState.Geodes + miningState.GeodeRobots;

                    MiningState neighbor;

                    // Do we have enough resources to construct a geode robot.
                    if (miningState.Obsidian >= obsidianRequired && miningState.Ore >= oreRequiredForGeode)
                    {
                        neighbor = new MiningState(
                                miningState.OreRobots,
                                miningState.ClayRobots,
                                miningState.ObsidianRobots,
                                miningState.GeodeRobots + 1,
                                nextOre - oreRequiredForGeode,
                                nextClay,
                                nextObsidian - obsidianRequired,
                                nextGeode,
                                miningState.Minute + 1);

                        if (!visited.Contains(neighbor))
                        {
                            queue.Push(neighbor);
                        }
                        else
                        {
                            visitHit++;
                        }

                        continue;
                    }

                    // Do we have enough resources to construct an obsidian robot.
                    if (miningState.Clay >= clayRequired && miningState.Ore >= oreRequiredForObsidian)
                    {
                        neighbor = new MiningState(
                                miningState.OreRobots,
                                miningState.ClayRobots,
                                miningState.ObsidianRobots + 1,
                                miningState.GeodeRobots,
                                nextOre - oreRequiredForObsidian,
                                nextClay - clayRequired,
                                nextObsidian,
                                nextGeode,
                                miningState.Minute + 1);

                        if (!visited.Contains(neighbor))
                        {
                            queue.Push(neighbor);
                        }
                        else
                        {
                            visitHit++;
                        }
                    }

                    // Do we have enough resources to construct a clay robot.
                    if (miningState.Ore >= oreRequiredForClay)
                    {
                        neighbor = new MiningState(
                                miningState.OreRobots,
                                miningState.ClayRobots + 1,
                                miningState.ObsidianRobots,
                                miningState.GeodeRobots,
                                nextOre - oreRequiredForClay,
                                nextClay,
                                nextObsidian,
                                nextGeode,
                                miningState.Minute + 1);

                        if (!visited.Contains(neighbor))
                        {
                            queue.Push(neighbor);
                        }
                        else
                        {
                            visitHit++;
                        }
                    }

                    // Do we have enough resources to construct an ore robot.
                    if (miningState.Ore >= oreRequiredForOre)
                    {
                        neighbor = new MiningState(
                                miningState.OreRobots + 1,
                                miningState.ClayRobots,
                                miningState.ObsidianRobots,
                                miningState.GeodeRobots,
                                nextOre - oreRequiredForOre,
                                nextClay,
                                nextObsidian,
                                nextGeode,
                                miningState.Minute + 1);

                        if (!visited.Contains(neighbor))
                        {
                            queue.Push(neighbor);
                        }
                        else
                        {
                            visitHit++;
                        }
                    }

                    // Enqueue a state where we don't build any robots, even if we can.
                    neighbor = new MiningState(
                                miningState.OreRobots,
                                miningState.ClayRobots,
                                miningState.ObsidianRobots,
                                miningState.GeodeRobots,
                                nextOre,
                                nextClay,
                                nextObsidian,
                                nextGeode,
                                miningState.Minute + 1);

                    if (!visited.Contains(neighbor))
                    {
                        queue.Push(neighbor);
                    }
                    else
                    {
                        visitHit++;
                    }
                }

                maxDict.Add(bluePrint.Id, maxOpenable);
            }

            Console.ReadLine();
        }
    }

    internal class BluePrint
    {
        private static Regex bluePrintReg = new Regex(@"Blueprint (\d+)", RegexOptions.Compiled);
        private static Regex oreRegex = new Regex(@"(\d+) ore", RegexOptions.Compiled);
        private static Regex oreAndClayRegex = new Regex(@"(\d+) ore and (\d+) clay", RegexOptions.Compiled);
        private static Regex oreAndObsidianRegex = new Regex(@"(\d+) ore and (\d+) obsidian", RegexOptions.Compiled);

        public BluePrint(string blueprint)
        {
            Match match = bluePrintReg.Match(blueprint);
            this.Id = Int32.Parse(match.Groups[1].Value);

            this.ResourceDict = new Dictionary<ResourceType, List<(ResourceType, int)>>();

            string[] lineParts = blueprint.Split(".", StringSplitOptions.RemoveEmptyEntries);
            match = oreRegex.Match(lineParts[0]);
            var resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Ore, Int32.Parse(match.Groups[1].Value))
            };

            this.ResourceDict.Add(ResourceType.Ore, resourceList);

            match = oreRegex.Match(lineParts[1]);
            resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Ore, Int32.Parse(match.Groups[1].Value))
            };

            this.ResourceDict.Add(ResourceType.Clay, resourceList);

            match = oreAndClayRegex.Match(lineParts[2]);
            resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Ore, Int32.Parse(match.Groups[1].Value)),
                (ResourceType.Clay, Int32.Parse(match.Groups[2].Value))
            };

            this.ResourceDict.Add(ResourceType.Obsidian, resourceList);

            match = oreAndObsidianRegex.Match(lineParts[3]);
            resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Ore, Int32.Parse(match.Groups[1].Value)),
                (ResourceType.Obsidian, Int32.Parse(match.Groups[2].Value))
            };

            this.ResourceDict.Add(ResourceType.Geode, resourceList);

            this.Robots = new List<Robot>
            {
                new Robot(ResourceType.Ore)
            };
        }

        public int Id { get; private set; }

        public Dictionary<ResourceType, List<(ResourceType, int)>> ResourceDict { get; private set; }

        public List<Robot> Robots { get; private set; }
    }

    internal struct MiningState
    {
        public MiningState(
            int oreRobots, 
            int clayRobots,
            int obsidianRobots,
            int geodeRobots,
            int ore,
            int clay,
            int obsidian,
            int geodes,
            int minute)
        {
            this.OreRobots = oreRobots;
            this.ClayRobots = clayRobots;
            this.ObsidianRobots = obsidianRobots;
            this.GeodeRobots = geodeRobots;
            this.Ore = ore;
            this.Clay = clay;
            this.Obsidian = obsidian;
            this.Geodes = geodes;
            this.Minute = minute;
        }

        public int OreRobots { get; private set; }

        public int ClayRobots { get; private set; }

        public int ObsidianRobots { get; private set; }

        public int GeodeRobots { get; private set; }

        public int Ore { get; private set; }

        public int Clay { get; private set; }

        public int Obsidian { get; private set; }

        public int Geodes { get; private set; }

        public int Minute { get; set; }
    }

    internal class Robot
    {
        public Robot(ResourceType resourceType)
        {
            this.ResourceType = resourceType;
        }

        public ResourceType ResourceType { get; private set; }
    }

    internal enum ResourceType
    {
        None,
        Ore,
        Clay,
        Obsidian,
        Geode
    }
}
