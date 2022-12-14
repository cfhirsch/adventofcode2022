using AdventOfCode2022.Utilities;
using System.Text;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec13
    {
        public static void SolvePartOne(bool diagnostic)
        {
            List<string> inputLines = PuzzleReader.ReadLines(13).ToList();

            int i = 0;
            int index = 1;
            int sum = 0;
            while (i < inputLines.Count - 1)
            {
                string left = inputLines[i];
                i++;

                string right = inputLines[i];
                i++;
                if (diagnostic)
                {
                    Console.WriteLine($"== Pair {index}");

                    Console.WriteLine($"- Compare {left} vs {right}");
                }

                CompareResult result = IsInOrder(left, right, diagnostic, 0);

                if (result == CompareResult.Inconclusive)
                {
                    throw new Exception("Did not get conclusive result.");
                }

                if (result == CompareResult.InOrder)
                {
                    sum += index;
                }

                index++;
                i++;
            }

            Console.WriteLine($"Sum of in-order pairs is {sum}.");
        }

        public static void SolvePartTwo()
        {
            var inputLines = new List<string>();

            foreach (string line in PuzzleReader.ReadLines(13))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    inputLines.Add(line);
                }
            }

            const string firstDividerPacket = "[[2]]";
            const string secondDividerPacket = "[[6]]";
            inputLines.Add(firstDividerPacket);
            inputLines.Add(secondDividerPacket);

            QuickSort(inputLines, 0, inputLines.Count - 1);

            int product = 1;
            for (int i = 0; i < inputLines.Count; i++)
            {
                if (inputLines[i] == firstDividerPacket || inputLines[i] == secondDividerPacket)
                {
                    product *= i + 1;
                }
            }

            Console.WriteLine($"Decoder key = {product}.");
        }

        private static CompareResult IsInOrder(string left, string right, bool diagnostic, int level)
        {
            int leftPos = 1;
            int rightPos = 1;

            while (true)
            {
                (string leftToken, leftPos) = GetNextToken(left, leftPos);
                (string rightToken, rightPos) = GetNextToken(right, rightPos);

                if (leftToken == string.Empty && rightToken == string.Empty)
                {
                    return CompareResult.Inconclusive;
                }

                if (leftToken == string.Empty)
                {
                    if (diagnostic)
                    {
                        for (int i = 0; i < level; i++)
                        {
                            Console.Write("\t");
                        }

                        Console.WriteLine($"- Left side ran out of items, so inputs are in the right order");
                    }

                    return CompareResult.InOrder;
                }

                if (rightToken == string.Empty)
                {
                    if (diagnostic)
                    {
                        for (int i = 0; i < level; i++)
                        {
                            Console.Write("\t");
                        }

                        Console.WriteLine($"- Right side ran out of items, so inputs are NOT in the right order");
                    }

                    return CompareResult.OutOfOrder;
                }

                if (diagnostic)
                {
                    for (int i = 0; i < level; i++)
                    {
                        Console.Write("\t");
                    }

                    Console.WriteLine($"- Compare {leftToken} vs {rightToken}");
                }

                int leftVal, rightVal;
                
                if (Int32.TryParse(leftToken, out leftVal) && Int32.TryParse(rightToken, out rightVal))
                {
                    if (leftVal < rightVal)
                    {
                        if (diagnostic)
                        {
                            for (int i = 0; i < level; i++)
                            {
                                Console.Write("\t");
                            }

                            Console.WriteLine($"- Left side is smaller, so inputs are in the right order");
                        }

                        return CompareResult.InOrder;
                    }

                    if (leftVal > rightVal)
                    {
                        if (diagnostic)
                        {
                            for (int i = 0; i < level; i++)
                            {
                                Console.Write("\t");
                            }

                            Console.WriteLine($"- Right side is smaller, so inputs are NOT in the right order");
                        }

                        return CompareResult.OutOfOrder;
                    }
                }
                else if (leftToken.StartsWith("[") && rightToken.StartsWith("["))
                {
                    CompareResult result = IsInOrder(leftToken, rightToken, diagnostic, level + 1);
                    if (result != CompareResult.Inconclusive)
                    {
                        return result;
                    }
                }
                else if (Int32.TryParse(leftToken, out leftVal) && rightToken.StartsWith("["))
                {
                    leftToken = $"[{leftVal}]";

                    if (diagnostic)
                    {
                        for (int i = 0; i < level; i++)
                        {
                            Console.Write("\t");
                        }

                        Console.WriteLine($"- Mixed types; convert left to {leftToken} and retry comparison.");
                    }

                    CompareResult result = IsInOrder(leftToken, rightToken, diagnostic, level + 1);
                    if (result != CompareResult.Inconclusive)
                    {
                        return result;
                    }
                }
                else if (leftToken.StartsWith("[") && Int32.TryParse(rightToken, out rightVal))
                {
                    rightToken = $"[{rightVal}]";
                    if (diagnostic)
                    {
                        for (int i = 0; i < level; i++)
                        {
                            Console.Write("\t");
                        }

                        Console.WriteLine($"- Mixed types; convert right to {rightToken} and retry comparison.");
                    }

                    CompareResult result = IsInOrder(leftToken, rightToken, diagnostic, level + 1);
                    if (result != CompareResult.Inconclusive)
                    {
                        return result;
                    }
                }
                else
                {
                    throw new Exception("Unexpected case.");
                }
            }
        }

        private static (string, int) GetNextToken(string str, int startPos)
        {
            if (startPos >= str.Length - 1)
            {
                return (string.Empty, startPos);
            }

            var sb = new StringBuilder();
            var stack = new Stack<char>();
            int pos = startPos; 

            while (pos < str.Length)
            {
                if (str[pos] == ',' && stack.Count() == 0)
                {
                    pos++;
                    break;
                }

                if (str[pos] == ']' && stack.Count() == 0)
                {
                    pos++;
                    break;
                }

                sb.Append(str[pos]);
                if (str[pos] == '[')
                {
                    stack.Push('[');
                }

                if (str[pos] == ']')
                {
                    stack.Pop();
                }

                pos++;
            }

            return (sb.ToString(), pos);
        }

        private static void QuickSort(List<string> lines, int lo, int hi)
        {
            if (lo >= hi || lo < 0)
            {
                return;
            }

            int p = Partition(lines, lo, hi);

            QuickSort(lines, lo, p - 1);
            QuickSort(lines, p + 1, hi);
        }

        private static int Partition(List<string> lines, int lo, int hi)
        {
            string pivot = lines[hi];
            int i = lo - 1;

            string temp;
            for (int j = lo; j < hi; j++)
            {
                CompareResult result = IsInOrder(lines[j], pivot, false, 0);
                if (result == CompareResult.Inconclusive)
                {
                    throw new Exception("Did not get conclusive comparison");
                }

                if (IsInOrder(lines[j], pivot, false, 0) == CompareResult.InOrder)
                {
                    i++;
                    temp = lines[i];
                    lines[i] = lines[j];
                    lines[j] = temp;
                }
            }

            i++;
            temp = lines[i];
            lines[i] = lines[hi];
            lines[hi] = temp;

            return i;
        }
    }

    internal enum CompareResult
    {
        Inconclusive,
        OutOfOrder,
        InOrder
    }
}
