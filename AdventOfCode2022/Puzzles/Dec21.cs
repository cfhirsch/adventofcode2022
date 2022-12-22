using AdventOfCode2022.Utilities;
using System.Security;
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

        public static void SolvePartTwo(bool isTest)
        {
            var exprDict = new Dictionary<string, Expression>();
            foreach (string line in PuzzleReader.ReadLines(21, isTest))
            {
                var expr = new Expression(line);
                exprDict.Add(expr.Name, expr);
            }

            Expression root = exprDict["root"];
            ExpressionNode tree = root.BuildTree(exprDict);

            ExpressionNode left = tree.Left;
            ExpressionNode right = tree.Right;

            ExpressionNode human = GetHuman(left);

            if (human == null)
            {
                human = GetHuman(right);
                ExpressionNode temp = left;
                left = right;
                right = temp;
            }

            long valToMatch = right.Expression.Evaluate(exprDict);

            ExpressionNode current = left;
            ExpressionNode toEvaluate, next;
            
            while (current != human)
            {
                human = GetHuman(current.Left);
                if (human != null)
                {
                    next = current.Left;
                    toEvaluate = current.Right;
                }
                else
                {
                    next = current.Right;
                    toEvaluate = current.Left;
                }

                long result = toEvaluate.Expression.Evaluate(exprDict);
                switch (current.Expression.ExpressionType)
                {
                    case ExpressionType.Times:
                        valToMatch /= result;
                        break;

                    case ExpressionType.Divide:
                        valToMatch *= result;
                        break;

                    case ExpressionType.Plus:
                        valToMatch -= result;
                        break;

                    case ExpressionType.Minus:
                        // x - [expr with human] = y
                        // - [expr with human] = y - x
                        // [expr with human] = x - y = -(y - x)
                        if (next == current.Right)
                        {
                            valToMatch -= result;
                            valToMatch *= -1;
                        }
                        else
                        {
                            valToMatch += result;
                        }
                        
                        break;

                    default:
                        throw new Exception($"Unexpected expression type {left.Expression.ExpressionType}.");

                }

                current = next;
            }

            Console.WriteLine(root.GetString(exprDict));
            Console.WriteLine($"humn = {valToMatch}.");
        }

        private static ExpressionNode GetHuman(ExpressionNode node)
        {
            if (node.Name == "humn")
            {
                return node;
            }

            if (node.Left == null)
            {
                return null;
            }

            return GetHuman(node.Left) ?? GetHuman(node.Right);
        }
    }

    internal class Expression
    {
        private readonly int val;

        private readonly string left, right;

        private static readonly Regex intReg = new Regex(@"(\w+): (\d+)");
        private static readonly Regex reg = new Regex(@"(\w+): (\w+) ([\+\-\*/]) (\w+)", RegexOptions.Compiled);

        public Expression(string line)
        {
            Match match = intReg.Match(line);

            if (match.Success)
            {
                this.ExpressionType = ExpressionType.Integer;
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
                        this.ExpressionType = ExpressionType.Plus;
                        break;

                    case "-":
                        this.ExpressionType = ExpressionType.Minus;
                        break;

                    case "*":
                        this.ExpressionType = ExpressionType.Times;
                        break;

                    case "/":
                        this.ExpressionType = ExpressionType.Divide;
                        break;

                    default:
                        throw new Exception($"Unexpected operation {op}.");
                }

            }
        }

        public string Name { get; private set; }

        public ExpressionType ExpressionType { get; private set; }

        public long Evaluate(Dictionary<string, Expression> exprDict)
        {
            Expression left, right;

            switch (this.ExpressionType)
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
                    throw new Exception($"Unexpected expression type {this.ExpressionType}.");
            }
        }

        public ExpressionNode BuildTree(
            Dictionary<string, Expression> exprDict)
        {
            Expression left, right;

            if (this.Name == "root")
            {
                left = exprDict[this.left];
                right = exprDict[this.right];

                return new ExpressionNode
                {
                    Name = this.Name,
                    Operation = ExpressionType.Equals,
                    Left = left.BuildTree(exprDict),
                    Right = right.BuildTree(exprDict),
                    Expression = this
                };
            }

            if (this.Name == "humn")
            {
                return new ExpressionNode
                {
                    Name = this.Name,
                    Operation = ExpressionType.Human
                };
            }

            ExpressionNode exprNode;
            switch (this.ExpressionType)
            {
                case ExpressionType.Integer:
                    return new ExpressionNode
                    {
                        Name = this.Name,
                        Operation = ExpressionType.Integer,
                        Value = this.val,
                        Expression = this
                    };

                case ExpressionType.Plus:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    exprNode = new ExpressionNode
                    {
                        Name = this.Name,
                        Operation = ExpressionType.Plus,
                        Left = left.BuildTree(exprDict),
                        Right = right.BuildTree(exprDict),
                        Expression = this
                    };

                    return exprNode;

                case ExpressionType.Minus:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    exprNode = new ExpressionNode
                    {
                        Name = this.Name,
                        Operation = ExpressionType.Minus,
                        Left = left.BuildTree(exprDict),
                        Right = right.BuildTree(exprDict),
                        Expression = this
                    };

                    return exprNode;

                case ExpressionType.Times:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    exprNode = new ExpressionNode
                    {
                        Name = this.Name,
                        Operation = ExpressionType.Times,
                        Left = left.BuildTree(exprDict),
                        Right = right.BuildTree(exprDict),
                        Expression = this
                    };

                    return exprNode;

                case ExpressionType.Divide:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    exprNode = new ExpressionNode
                    {
                        Name = this.Name,
                        Operation = ExpressionType.Divide,
                        Left = left.BuildTree(exprDict),
                        Right = right.BuildTree(exprDict),
                        Expression = this
                    };

                    return exprNode;

                default:
                    throw new Exception($"Unexpected operation {this.ExpressionType}.");
            }
        }

        public string GetString(Dictionary<string, Expression> exprDict)
        {
            Expression left, right;

            if (this.Name == "root")
            {
                left = exprDict[this.left];
                right = exprDict[this.right];
                return $"{left.GetString(exprDict)} = {right.GetString(exprDict)}";
            }

            if (this.Name == "humn")
            {
                return this.Name;
            }

            switch (this.ExpressionType)
            {
                case ExpressionType.Integer:
                    return this.val.ToString();

                case ExpressionType.Plus:
                    left = exprDict[this.left];
                    right = exprDict[this.right];

                    return $"({left.GetString(exprDict)}) + ({right.GetString(exprDict)})";

                case ExpressionType.Minus:
                    left = exprDict[this.left];
                    right = exprDict[this.right];
                    return $"({left.GetString(exprDict)}) - ({right.GetString(exprDict)})";

                case ExpressionType.Times:
                    left = exprDict[this.left];
                    right = exprDict[this.right];
                    return $"({left.GetString(exprDict)}) * ({right.GetString(exprDict)})";

                case ExpressionType.Divide:
                    left = exprDict[this.left];
                    right = exprDict[this.right];
                    return $"({left.GetString(exprDict)}) / ({right.GetString(exprDict)})";

                default:
                    throw new Exception($"Unexpected expression type {this.ExpressionType}.");
            }
        }
    }

    internal class ExpressionNode
    {
        public string Name { get; set; }

        public ExpressionType Operation { get; set; }

        public Expression Expression { get; set; }

        public ExpressionNode Left { get; set; }

        public ExpressionNode Right { get; set; }

        public int Value { get; set; }
    }

    internal enum ExpressionType
    {
        None,
        Integer,
        Plus,
        Minus,
        Times,
        Divide,
        Equals,
        Human
    }
}
