using AdventOfCode2022.Utilities;
using System.Text;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec7
    {
        public static void SolvePartOne()
        {
            Directory root = BuildTree();

            Console.WriteLine(root);

            int sum = GetDirectories(root).Select(d => d.Size).Where(s => s <= 100000).Sum();
            Console.WriteLine($"Sum = {sum}");
        }

        public static void SolvePartTwo()
        {
            const int totalDiskSpace = 70000000;
            const int freeDiskSpaceNeeded = 30000000;

            Directory root = BuildTree();
            int freeDiskSpace = totalDiskSpace - root.Size;

            int spaceToFree = GetDirectories(root).Where(d => freeDiskSpace + d.Size >= freeDiskSpaceNeeded).Select(d => d.Size).OrderBy(s => s).First();
            Console.WriteLine($"Space To Free = {spaceToFree}");
        }

        public static Directory BuildTree()
        {
            var root = new Directory(null, "/");
            Node current = root;

            Mode mode;

            foreach (string line in PuzzleReader.ReadLines(7))
            {
                mode = line.StartsWith("$") ? Mode.Command : Mode.List;
                string[] commandParts = line.Split(' ');
                switch (mode)
                {
                    case Mode.Command:

                        if (commandParts[1] == "cd")
                        {
                            string dir = commandParts[2];
                            if (dir == "/")
                            {
                                current = root;
                            }
                            else if (dir == "..")
                            {
                                current = current.Parent;
                            }
                            else
                            {
                                var child = GetChild(current, dir) as Directory;
                                if (child == null)
                                {
                                    child = new Directory(current, dir);
                                    ((Directory)current).Children.Add(child);
                                }

                                current = child;
                            }
                        }

                        break;

                    case Mode.List:
                        int size;
                        string name = commandParts[1];
                        if (Int32.TryParse(commandParts[0], out size))
                        {
                            var child = GetChild(current, name) as File;
                            if (child == null)
                            {
                                child = new File(current, name, size);
                                ((Directory)current).Children.Add(child);
                            }
                        }
                        else
                        {
                            var child = GetChild(current, name) as Directory;
                            if (child == null)
                            {
                                child = new Directory(current, name);
                                ((Directory)current).Children.Add(child);
                            }
                        }

                        break;
                }
            }

            return root;
        }

        private static Node GetChild(Node node, string name)
        {
            return ((Directory)node).Children.Where(n => n.Name == name).FirstOrDefault();
        }

        private static IEnumerable<Directory> GetDirectories(Directory node)
        {
            foreach (Node child in node.Children)
            {
                var dir = child as Directory;
                if (dir != null)
                {
                    foreach (Directory childDir in GetDirectories(dir))
                    {
                        yield return childDir;
                    }
                }
            }

            yield return node;
        }

    }

    internal enum Mode
    {
        Command,
        List
    }

    internal abstract class Node
    {
        public abstract int Size { get; }

        public string Name { get; protected set; }

        public Node Parent { get; protected set; }

        public int Level
        {
            get
            {
                int i = 0;
                var current = this;
                while (current.Parent != null)
                {
                    i++;
                    current = current.Parent;
                }

                return i;
            }
        }
    }

    internal class File : Node
    {
        private int size;

        public File(Node parent, string name, int size)
        {
            this.Parent = parent;
            this.Name = name;
            this.size = size;
        }

        public override int Size => this.size;

        public override string ToString()
        {
            var sb = new StringBuilder();
            int level = this.Level;
            for (int i = 0; i < level; i++)
            {
                sb.Append("\t");
            }

            sb.Append($"- {this.Name} (file, size={this.Size})");
            return sb.ToString();
        }
    }

    internal class Directory : Node
    {
        public Directory(Node parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
            this.Children = new List<Node>();
        }

        public List<Node> Children { get; private set; }

        public override int Size
        {
            get
            {
                return this.Children.Sum(c => c.Size);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int level = this.Level;
            for (int i = 0; i < level; i++)
            {
                sb.Append("\t");
            }

            sb.AppendLine($"- {this.Name} (dir)");
            foreach (Node child in this.Children)
            {
                sb.AppendLine($"{child}");
            }

            return sb.ToString();
        }
    }
}
