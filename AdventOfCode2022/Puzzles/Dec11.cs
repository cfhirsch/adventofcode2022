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
            int operand = 1;
            int divisor = -1;
            int ifTrueMonkey = -1, ifFalseMonkey = -1;
            var monkeys = new List<Monkey>();
            foreach (string line in PuzzleReader.ReadLines(11))
            {
                if (line.StartsWith("Monkey"))
                {
                    if (items != null)
                    {
                        var monkey = new Monkey(op, operand, divisor, ifTrueMonkey, ifFalseMonkey);
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
                else if (line.StartsWith("Starting items: "))
                {
                    string subStr = line.Substring(line.IndexOf("Starting items: "));
                    items = new List<long>();
                    foreach (string strPart in subStr.Split(",", StringSplitOptions.RemoveEmptyEntries))
                    {
                        items.Add(Int64.Parse(strPart));
                    }
                }
                else if (line.StartsWith("Operation: "))
                {
                    if (line.Contains("+"))
                    {
                        op = "+";
                    }
                    else if (line.Contains("*"))
                    {
                        op = "*";
                    }
                    else
                    {
                        throw new Exception("Did not find expected operator.");
                    }
                }
                else if (line.StartsWith("Test: "))
                {
                    divisor = Int32.Parse(opReg.Match(line).Value);
                }
                else if (line.StartsWith("If true: "))
                {
                    ifTrueMonkey = Int32.Parse(opReg.Match(line).Value);
                }
                else if (line.StartsWith("If false: "))
                {
                    ifFalseMonkey = Int32.Parse(opReg.Match(line).Value);
                }
            }

            if (items != null)
            {
                var monkey = new Monkey(op, operand, divisor, ifTrueMonkey, ifFalseMonkey);
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
            }
        }
    }

    internal class Monkey
    {
        public Monkey(string op, int operand,  int divisor, int ifTrue, int ifFalse)
        {
            this.Items = new Queue<long>();
            this.Operation = op;
            this.Operand = operand;
            this.Divisor = divisor;
            this.IfTrueMonkey = ifTrue;
            this.IfFalseMonkey = ifFalse;
        }

        public Queue<long> Items { get; private set; }

        public string Operation { get; private set; }

        public int Operand { get; private set; }

        public int Divisor { get; private set; }

        public int IfTrueMonkey { get; private set; }

        public int IfFalseMonkey { get; private set; }

        public (long, int) Process()
        {
            long item = this.Items.Dequeue();

            switch (this.Operation)
            {
                case "+":
                    item += this.Operand;
                    break;

                case "*":
                    item *= this.Operand;
                    break;

                default:
                    throw new Exception($"Unexpeced operand {this.Operand}.");
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
