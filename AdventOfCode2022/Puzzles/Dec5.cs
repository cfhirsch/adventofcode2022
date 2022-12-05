using AdventOfCode2022.Utilities;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec5
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
            var reg = new Regex(@"move (\d+) from (\d+) to (\d+)", RegexOptions.Compiled);

            var stacks = new List<Stack<char>>();

            var stackLines = new List<string>();
            var stackPositions = new List<int>();

            ParseState state = ParseState.ReadStack;

            foreach (string line in PuzzleReader.ReadLines(5))
            {
                switch (state)
                {
                    case ParseState.ReadStack:
                        if (!line.Contains('['))
                        {
                            // read the stack positions.
                            // Stack numbers
                            int stackNo = 1;
                            string stackNoStr = $"{stackNo}";
                            while (line.Contains(stackNoStr))
                            {
                                stackPositions.Add(line.IndexOf(stackNoStr));
                                stackNo++;
                                stackNoStr = $"{stackNo}";
                                stacks.Add(new Stack<char>());
                            }

                            // Now set up the stacks
                            for (int i = 0; i < stacks.Count; i++)
                            {
                                for (int j = stackLines.Count - 1; j >= 0; j--)
                                {
                                    char ch = stackLines[j][stackPositions[i]];
                                    if (char.IsLetter(ch))
                                    {
                                        stacks[i].Push(ch);
                                    }
                                }
                            }

                            state = ParseState.NewLine;
                        }
                        else
                        {
                            stackLines.Add(line);
                        }

                        break;

                    case ParseState.NewLine:
                        state = ParseState.Move;
                        break;

                    case ParseState.Move:
                        Match match = reg.Match(line);
                        int numToMove = Int32.Parse(match.Groups[1].Value);
                        int source = Int32.Parse(match.Groups[2].Value) - 1;
                        int target = Int32.Parse(match.Groups[3].Value) - 1;

                        if (!isPartTwo)
                        {
                            for (int i = 0; i < numToMove; i++)
                            {
                                Char ch = stacks[source].Pop();
                                stacks[target].Push(ch);
                            }
                        }
                        else
                        {
                            var tempStack = new Stack<char>();
                            for (int i = 0; i < numToMove; i++)
                            {
                                Char ch = stacks[source].Pop();
                                tempStack.Push(ch);
                            }

                            while (tempStack.Count > 0)
                            {
                                Char ch = tempStack.Pop();
                                stacks[target].Push(ch);
                            }
                        }

                        break;
                }
            }

            var sb = new StringBuilder();
            foreach (Stack<char> stack in stacks)
            {
                sb.Append(stack.Peek());
            }

            Console.WriteLine(sb);
        }

        public enum ParseState
        {
            ReadStack,
            NewLine,
            Move
        }
    }
}
