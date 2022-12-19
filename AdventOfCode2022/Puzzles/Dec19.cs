using AdventOfCode2022.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    internal class Dec19
    {
        public static void SolvePartOne()
        {
            var bluePrints = new List<BluePrint>();

            foreach (string line in PuzzleReader.ReadLines(19))
            {
                bluePrints.Add(new BluePrint(line));
            }

            // The state we are interested in is:
            // (1) Number of robots of each type.
            // (2) Number of resources of each type.
            // (3) Number of opened geodes.
            // (4) Number of minutes left.
            var keyReg = new Regex(@"O=(\d+),C=(\d+),Ob=(\d+),G=(\d+),Ore=(\d+),Clay=(\d+),Obs=(\d+),OG=(\d+),M=(\d+)", RegexOptions.Compiled);

            
            var queue = new Queue<string>();
            queue.Enqueue("O=1,C=0,Ob=0,G=0,OG=0,M=24");
            int maxOpenable = 0;
            while (queue.Count > 0)
            {
                string key = queue.Dequeue();
                Match match = keyReg.Match(key);

                int oreRobots = Int32.Parse(match.Groups[1].Value);
                int clayRobots = Int32.Parse(match.Groups[2].Value);
                int obsidianRobots = Int32.Parse(match.Groups[3].Value);
                int geodeRobots = Int32.Parse(match.Groups[4].Value);
                int ore = Int32.Parse(match.Groups[5].Value);
                int clay = Int32.Parse(match.Groups[6].Value);
                int obsidian = Int32.Parse(match.Groups[7].Value);
                int openedGeodes = Int32.Parse(match.Groups[8].Value);
                int minutesLeft = Int32.Parse(match.Groups[9].Value);

                if (minutesLeft == 0)
                {
                    if (openedGeodes > maxOpenable)
                    {
                        maxOpenable = openedGeodes;
                    }

                    continue;
                }
                
                if (geodeRobots > 0)
                {
                    openedGeodes += geodeRobots;
                }


            }
        }
    }

    internal class BluePrint
    {
        private static Regex bluePrintReg = new Regex(@"Blueprint (\d+)", RegexOptions.Compiled);
        private static Regex oreRegex = new Regex(@"(\d+) ore", RegexOptions.Compiled);
        private static Regex oreAndClayRegex = new Regex(@"(\d+) ore and (\d+) clay", RegexOptions.Compiled);
        private static Regex oreAndObsidianRegex = new Regex(@"(\d+) ore and (\d+) obsidian", RegexOptions.Compiled);

        private readonly Dictionary<ResourceType, List<(ResourceType, int)>> resourceDict;

        public BluePrint(string blueprint)
        {
            Match match = bluePrintReg.Match(blueprint);
            this.Id = Int32.Parse(match.Groups[1].Value);

            this.resourceDict = new Dictionary<ResourceType, List<(ResourceType, int)>>();

            string[] lineParts = blueprint.Split(".", StringSplitOptions.RemoveEmptyEntries);
            match = oreRegex.Match(lineParts[0]);
            var resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Ore, Int32.Parse(match.Groups[1].Value))
            };

            match = oreRegex.Match(lineParts[0]);
            resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Clay, Int32.Parse(match.Groups[1].Value))
            };

            this.resourceDict.Add(ResourceType.Clay, resourceList);

            match = oreAndClayRegex.Match(lineParts[1]);
            resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Ore, Int32.Parse(match.Groups[1].Value)),
                (ResourceType.Clay, Int32.Parse(match.Groups[2].Value))
            };

            this.resourceDict.Add(ResourceType.Obsidian, resourceList);

            match = oreAndObsidianRegex.Match(lineParts[2]);
            resourceList = new List<(ResourceType, int)>
            {
                (ResourceType.Ore, Int32.Parse(match.Groups[1].Value)),
                (ResourceType.Obsidian, Int32.Parse(match.Groups[2].Value))
            };

            this.resourceDict.Add(ResourceType.Geode, resourceList);

            this.Robots = new List<Robot>
            {
                new Robot(ResourceType.Ore)
            };
        }

        public int Id { get; private set; }

        public List<Robot> Robots { get; private set; }

        public override 

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
