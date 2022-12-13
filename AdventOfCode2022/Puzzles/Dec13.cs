using AdventOfCode2022.Utilities;
using System.Text;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec13
    {
        public static void SolvePartOne()
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

                if (IsInOrder(left, right))
                {
                    sum += index;
                }

                index++;
                i++;
            }

            Console.WriteLine($"Sum of in-order pairs in {sum}.");
        }

        private static bool IsInOrder(string left, string right)
        {
            int leftPos = 1;
            int rightPos = 1;

            while (true)
            {
                (string leftToken, leftPos) = GetNextToken(left, leftPos);
                (string rightToken, rightPos) = GetNextToken(right, rightPos);

                if (leftToken == string.Empty)
                {
                    return true;
                }

                if (rightToken == string.Empty)
                {
                    return false;
                }

                int leftVal, rightVal;
                
                if (Int32.TryParse(leftToken, out leftVal) && Int32.TryParse(rightToken, out rightVal))
                {
                    if (leftVal < rightVal)
                    {
                        return true;
                    }

                    if (leftVal > rightVal)
                    {
                        return false;
                    }
                }
                else if (leftToken.StartsWith("[") && rightToken.StartsWith("["))
                {
                    if (!IsInOrder(leftToken, rightToken))
                    {
                        return false;
                    }
                }
                else if (Int32.TryParse(leftToken, out leftVal) && rightToken.StartsWith("["))
                {
                    leftToken = $"[{leftVal}]";

                    if (!IsInOrder(leftToken, rightToken))
                    {
                        return false;
                    }
                }
                else if (leftToken.StartsWith("[") && Int32.TryParse(rightToken, out rightVal))
                {
                    rightToken = $"[{rightVal}]";
                    if (!IsInOrder(leftToken, rightToken))
                    {
                        return false;
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
    }
}
