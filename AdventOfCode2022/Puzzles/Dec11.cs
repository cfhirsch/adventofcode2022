using AdventOfCode2022.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec11
    {
        public static void SolvePartOne()
        {
            var opReg = new Regex(@"\d+", RegexOptions.Compiled);

            // Parse monkeys.
            List<long> items = null;
            string op = null;
            int divisor = -1;
            int ifTrueMonkey = -1, ifFalseMonkey = -1;
            var monkeys = new List<Monkey>();
            foreach (string line in PuzzleReader.ReadLines(11))
            {
                if (line.StartsWith("Monkey"))
                {
                    if (items != null)
                    {
                        var monkey = new Monkey(op, divisor, ifTrueMonkey, ifFalseMonkey);
                        foreach (long item in items)
                        {
                            monkey.Items.Enqueue(item);
                        }

                        monkeys.Add(monkey);

                        items = null;
                        op = null;
                        divisor = -1;
                        ifTrueMonkey = -1;
                        ifFalseMonkey = -1;
                    }
                }
                else if (line.Contains("Starting items: "))
                {
                    string subStr = line.Substring(line.IndexOf(":") + 2);
                    items = new List<long>();
                    foreach (string strPart in subStr.Split(",", StringSplitOptions.RemoveEmptyEntries))
                    {
                        items.Add(Int64.Parse(strPart));
                    }
                }
                else if (line.Contains("Operation: "))
                {
                    op = line.Substring(line.IndexOf("=") + 2);
                }
                else if (line.Contains("Test: "))
                {
                    divisor = Int32.Parse(opReg.Match(line).Value);
                }
                else if (line.Contains("If true: "))
                {
                    ifTrueMonkey = Int32.Parse(opReg.Match(line).Value);
                }
                else if (line.Contains("If false: "))
                {
                    ifFalseMonkey = Int32.Parse(opReg.Match(line).Value);
                }
            }

            if (items != null)
            {
                var monkey = new Monkey(op, divisor, ifTrueMonkey, ifFalseMonkey);
                foreach (long item in items)
                {
                    monkey.Items.Enqueue(item);
                }

                monkeys.Add(monkey);
            }

            for (int i = 1; i <= 20; i++)
            {
                foreach (Monkey monkey in monkeys)
                {
                    while (monkey.Items.Count > 0)
                    {
                        (long item, int target) = monkey.Process();
                        monkeys[target].Items.Enqueue(item);
                    }
                }

                Console.WriteLine($"Round {i}");
                for (int j = 0; j < monkeys.Count; j++)
                {
                    Console.WriteLine($"Monkey {j}: {monkeys[j]}");
                }

                Console.WriteLine();
                // Console.ReadLine();
            }

            List<int> sorted = monkeys.Select(m => m.InspectCount).OrderByDescending(i => i).ToList();
            int monkeyBusiness = sorted[0] * sorted[1];
            Console.WriteLine($"Monkey business = {monkeyBusiness}.");
        }
    }

    internal class Monkey
    {
        private static Regex reg = new Regex(@"^(.+) ([\+\*]) (.+)$", RegexOptions.Compiled);

        public Monkey(string op, int divisor, int ifTrue, int ifFalse)
        {
            this.Items = new Queue<long>();
            this.Operation = op;
            this.Divisor = divisor;
            this.IfTrueMonkey = ifTrue;
            this.IfFalseMonkey = ifFalse;
        }

        public Queue<long> Items { get; private set; }

        public string Operation { get; private set; }

        public int Divisor { get; private set; }

        public int IfTrueMonkey { get; private set; }

        public int IfFalseMonkey { get; private set; }

        public int InspectCount { get; private set; }

        public (long, int) Process()
        {
            long item = this.Items.Dequeue();

            this.InspectCount++;

            Match match = reg.Match(this.Operation);

            long left = -1, right = -1;
            if (!Int64.TryParse(match.Groups[1].Value, out left))
            {
                // We assume op is "old"
                left = item;
            }

            if (!Int64.TryParse(match.Groups[3].Value, out right))
            {
                // We assume op is "old"
                right = item;
            }

            switch (match.Groups[2].Value)
            {
                case "+":
                    item = left + right;
                    break;

                case "*":
                    item = left * right;
                    break;

                default:
                    throw new Exception($"Unexpeced operand {match.Groups[2].Value}.");
            }

            item /= 3;

            long remainder;
            Math.DivRem(item, this.Divisor, out remainder);

            if (remainder == 0)
            {
                return (item, this.IfTrueMonkey);
            }

            return (item, this.IfFalseMonkey);
        }

        public override string ToString()
        {
            string[] items = this.Items.Select(i => i.ToString()).ToArray();
            return string.Join(",", items);
        }
    }
}
