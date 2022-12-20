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

                int oreRobots = 1;
                int clayRobots = 0;
                int obsidianRobots = 0;
                int geodeRobots = 0;
                int ore = 0;
                int clay = 0;
                int obsidian = 0;
                int openedGeodes = 0;
                int minute = 0;

                while (minute < 24)
                {
                    minute++;
                    Console.WriteLine($"== Minute {minute} ==");

                    int nextOre = ore + oreRobots;
                    int nextClay = clay + clayRobots;
                    int nextObsidian = obsidian + obsidianRobots;
                    int nextGeodes = openedGeodes + geodeRobots;
                   
                    // Build a geode robot if we have the resources.
                    if (obsidian >= obsidianRequired &&
                        ore >= oreRequiredForGeode)
                    {
                        obsidian -= obsidianRequired;
                        nextObsidian -= obsidianRequired;
                        ore -= oreRequiredForGeode;
                        nextOre -= oreRequiredForGeode;
                        geodeRobots++;
                        Console.WriteLine("Build a Geode robot.");
                    }

                    // Build an obsidian robot if we have the resources, unless
                    // we'll have enough for a geode robot in the next minute.
                    if ((nextObsidian + obsidianRobots < obsidianRequired ||
                         nextOre + oreRobots < oreRequiredForGeode) &&
                         clay >= clayRequired &&
                         ore >= oreRequiredForObsidian)
                    {
                        clay -= clayRequired;
                        nextClay -= clayRequired;
                        ore -= oreRequiredForObsidian;
                        nextOre -= oreRequiredForObsidian;
                        obsidianRobots++;
                        Console.WriteLine("Build an Obsidian robot.");
                    }

                    // Build a clay robot if we have the resources, unless
                    // we'll have enough for an obsidian robot in the next minute.
                    if ((nextClay + clayRobots < clayRequired ||
                         nextOre + oreRequiredForOre < oreRequiredForObsidian) &&
                         ore >= oreRequiredForClay)
                    {
                        ore -= oreRequiredForClay;
                        nextOre -= oreRequiredForClay;
                        clayRobots++;
                        Console.WriteLine("Build a clay robot.");
                    }

                    // Build an ore robot if we have the resources, unless
                    // we'll have enough for a clay robot in the next minute.
                    if (nextOre + oreRobots < oreRequiredForClay && ore >= oreRequiredForOre)
                    {
                        ore -= oreRequiredForOre;
                        nextOre -= oreRequiredForOre;
                        oreRobots++;
                        Console.WriteLine("Build an ore robot.");
                    }

                    ore = nextOre;
                    clay = nextClay;
                    obsidian = nextObsidian;
                    openedGeodes = nextGeodes;

                    Console.WriteLine($"{oreRobots} ore robots, {clayRobots} clay robots, {obsidianRobots} obsidian robots, {geodeRobots} geode robots.");
                    Console.WriteLine($"{ore} ore, {clay} clay, {obsidian} obsidian, {openedGeodes} geodes.");
                }

                maxDict.Add(bluePrint.Id, openedGeodes);
            }
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
