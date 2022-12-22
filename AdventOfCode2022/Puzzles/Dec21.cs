using AdventOfCode2022.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec21
    {
        public static void SolvePartOne(bool isTest)
        {
            var exprDict = new Dictionary<string, Expression>();
            foreach (string line in PuzzleReader.ReadLines(21, isTest))
            {
                var expr = new Expression(line);
                exprDict.Add(expr.Name, expr);
            }

            Expression root = exprDict["root"];
            long result = root.Evaluate(exprDict);
            Console.WriteLine($"Result = {result}.");
        }
    }

    internal class Expression
    {
        private readonly ExpressionType expressionType;

        private readonly int val;

        private readonly string left, right;

        private static readonly Regex intReg = new Regex(@"(\w+): (\d+)");
        private static readonly Regex reg = new Regex(@"(\w+): (\w+) ([\+\-\*/]) (\w+)", RegexOptions.Compiled);

        public Expression(string line)
        {
            Match match = intReg.Match(line);

            if (match.Success)
            {
                this.expressionType = ExpressionType.Integer;
                this.Name = match.Groups[1].Value;
                this.val = Int32.Parse(match.Groups[2].Value);
            }
            else
            {
                match = reg.Match(line);
                this.Name = match.Groups[1].Value;
                this.left = match.Groups[2].Value;
                this.right = match.Groups[4].Value;

                string op = match.Groups[3].Value;
                switch (op)
                {
                    case "+":
                        this.expressionType = ExpressionType.Plus;
                        break;

                    case "-":
                        this.expressionType = ExpressionType.Minus;
                        break;

                    case "*":
                        this.expressionType = ExpressionType.Times;
                        break;

                    case "/":
                        this.expressionType = ExpressionType.Divide;
                        break;

                    default:
                        throw new Exception($"Unexpected operation {op}.");
                }

            }
        }

        public string Name { get; private set; }

        public long Evaluate(Dictionary<string, Expression> exprDict)
        {
            Expression left, right;

            switch (this.expressionType)
            {
                case ExpressionType.Integer:
                    return this.val;

                case ExpressionType.Plus:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    return left.Evaluate(exprDict) + right.Evaluate(exprDict);

                case ExpressionType.Minus:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    return left.Evaluate(exprDict) - right.Evaluate(exprDict);

                case ExpressionType.Times:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    return left.Evaluate(exprDict) * right.Evaluate(exprDict);

                case ExpressionType.Divide:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    return left.Evaluate(exprDict) / right.Evaluate(exprDict);

                default:
                    throw new Exception($"Unexpected expression type {this.expressionType}.");
            }
        }
    }

    internal enum ExpressionType
    {
        None,
        Integer,
        Plus,
        Minus,
        Times,
        Divide
    }
}
