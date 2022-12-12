using AdventOfCode2022.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    /* I got the solution to part two after thinking of the Chinese Remainder Theorem (CRT)
     * https://en.wikipedia.org/wiki/Chinese_remainder_theorem.
     * My undestanding is really vague here (and not 100% sure reasoning is correct, 
     * but consider n monkeys, where monkey i has divisor d_i
     * for 1 <= i <= n. Note that each divisor is a prime, which is critical for the CRT to apply. 
     * For a given item with worry level w, we have the following system of equations
     * (where "=" means "is equivalent to"):
     * 
     * w = a_1 mod d_1
     * w = a_2 mod d_2
     * ...
     * w = a_n mod d_n
     * 
     * For some integers a_1, a_2, ... , a_n.
     * 
     * The Chinese Remainder Theorem tells us that if w' is any other integer that satifies the above 
     * system of equations, then w = w' mod N, where N = d_1 * d_2 * ... * d_n.
     * So after we update the worry level of an item based on a monkey's operation to w, we can
     * update w to w % N to keep the numbers manageable. That is, we update worry levels using 
     * modulo N arithmetic.
     * 
     * What I'm not so clear on here is - does the CRT explicitly imply that, if w is a solution to
     * the above system of equations, and w' = w mod N, then w' is also a solution. All I really know
     * for sure is that this approach generated the correct answer for parts one and two.
     */
    internal static class Dec11
    {
        public static void SolvePartOne()
        {
            Solve(isPartTwo: false);
        }

        public static void SolvePartTwo()
        {
            Solve(isPartTwo: true);
        }

        public static void Solve(bool isPartTwo)
        {
            var opReg = new Regex(@"\d+", RegexOptions.Compiled);

            // Parse monkeys.
            List<long> items = null;
            string op = null;
            int divisor = -1;
            int ifTrueMonkey = -1, ifFalseMonkey = -1;
            var monkeys = new List<Monkey>();
            long N = 1;
            foreach (string line in PuzzleReader.ReadLines(11))
            {
                if (line.StartsWith("Monkey"))
                {
                    if (items != null)
                    {
                        var monkey = new Monkey(op, divisor, ifTrueMonkey, ifFalseMonkey, isPartTwo);
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
                    N *= divisor;
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
                var monkey = new Monkey(op, divisor, ifTrueMonkey, ifFalseMonkey, isPartTwo);
                foreach (long item in items)
                {
                    monkey.Items.Enqueue(item);
                }

                monkeys.Add(monkey);
            }

            foreach (Monkey monkey in monkeys)
            {
                monkey.N = N;
            }

            int numRounds = isPartTwo ? 10000 : 20;
            for (int i = 1; i <= numRounds; i++)
            {
                foreach (Monkey monkey in monkeys)
                {
                    while (monkey.Items.Count > 0)
                    {
                        (long item, int target) = monkey.Process();
                        monkeys[target].Items.Enqueue(item);
                    }
                }

                /*Console.WriteLine($"Round {i}");
                for (int j = 0; j < monkeys.Count; j++)
                {
                    Console.WriteLine($"Monkey {j}: {monkeys[j]}");
                }

                Console.WriteLine();*/
    // Console.ReadLine();
}

List<long> sorted = monkeys.Select(m => m.InspectCount).OrderByDescending(i => i).ToList();
            long monkeyBusiness = sorted[0] * sorted[1];
            Console.WriteLine($"Monkey business = {monkeyBusiness}.");
        }
    }

    internal class Monkey
    {
        private static Regex reg = new Regex(@"^(.+) ([\+\*]) (.+)$", RegexOptions.Compiled);

        private readonly bool isPartTwo;
        
        public Monkey(string op, int divisor, int ifTrue, int ifFalse, bool isPartTwo)
        {
            this.Items = new Queue<long>();
            this.Operation = op;
            this.Divisor = divisor;
            this.IfTrueMonkey = ifTrue;
            this.IfFalseMonkey = ifFalse;
            this.isPartTwo = isPartTwo;
        }

        public Queue<long> Items { get; private set; }

        public string Operation { get; private set; }

        public int Divisor { get; private set; }

        public int IfTrueMonkey { get; private set; }

        public int IfFalseMonkey { get; private set; }

        public long InspectCount { get; private set; }

        public long N { get; set; }

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
                    item = (left + right) % this.N;
                    break;

                case "*":
                    item = (left * right) % this.N;
                    break;

                default:
                    throw new Exception($"Unexpeced operand {match.Groups[2].Value}.");
            }

            if (!this.isPartTwo)
            {
                item /= 3;
            }

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
