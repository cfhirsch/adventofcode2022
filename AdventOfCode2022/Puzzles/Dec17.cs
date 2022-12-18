using AdventOfCode2022.Utilities;
using System.Drawing;

namespace AdventOfCode2022.Puzzles
{
    internal class Dec17
    {
        public static void SolvePartOne(bool draw)
        {
            string line = PuzzleReader.ReadLines(17).First();
            int numTiles = 2022;
            TileShape[] shapes = Enum.GetValues<TileShape>();

            const int left = 0;
            int top = 0;
            int right = 8;

            var points = new HashSet<Point>();
            int movPos = 0;

            for (int i = 0; i < numTiles; i++)
            {
                int shapePos = i % 5;
                TileShape shape = shapes[shapePos];

                var tile = new Tile(shape, left, top);

                Draw(tile, points, draw);
                bool moved = true;
                while (moved)
                {
                    char mv = line[movPos];
                    switch (mv)
                    {
                        case '<':
                            moved = tile.Move(TileMove.Left, left, right, points);
                            break;

                        case '>':
                            moved = tile.Move(TileMove.Right, left, right, points);
                            break;

                        default:
                            throw new Exception($"Unexpected symbol {mv}.");
                    }

                    if (moved)
                    {
                        Draw(tile, points, draw);
                    }

                    //Console.Read();

                    moved = tile.Move(TileMove.Down, left, right, points);

                    if (moved)
                    {
                        Draw(tile, points, draw);
                    }

                    movPos = (movPos + 1) % line.Length;

                    //Console.Read();
                }

                points = points.Union(tile.Points).ToHashSet();

                top = points.Max(p => p.Y);

                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Tile {i + 1}.");
            }

            Console.SetCursorPosition(0, 1);
            Console.WriteLine($"Tower of rocks is {top} units tall.");
        }

        public static void Draw(Tile tile, HashSet<Point> points, bool draw)
        {
            if (!draw)
            {
                return;
            }

            int minX = 0;
            int maxX = 8;
            int maxY = Int32.MinValue;

            if (!points.Any())
            {
                maxY = 0;
            }
            else
            {
                maxY = points.Max(p => p.Y);
            }

            maxY = Math.Max(maxY, tile.Points.Max(p => p.Y));
            for (int y = 0; y <= maxY; y++)
            {
                int yCoord = maxY - y;
                for (int x = minX; x <= maxX; x++)
                {
                    if (x == minX || x == maxX)
                    {
                        if (yCoord == 0)
                        {
                            Console.Write("+");
                        }
                        else
                        {
                            Console.Write("|");
                        }
                    }
                    else if (yCoord == 0)
                    {
                        Console.Write("-");
                    }
                    else
                    {
                        var pt = new Point(x, yCoord);
                        if (points.Contains(pt))
                        {
                            Console.Write('#');
                        }
                        else if (tile.Points.Contains(pt))
                        {
                            Console.Write("@");
                        }
                        else
                        {
                            Console.Write(".");
                        }
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }

    internal class Tile
    {
        public Tile(TileShape shape, int leftEdge, int highestRock)
        {
            this.Points = new HashSet<Point>();
            int x, y;
            switch (shape)
            {
                case TileShape.Minus:
                    x = leftEdge + 3;
                    y = highestRock + 4;
                    for (int i = 0; i < 4; i++)
                    {
                        this.Points.Add(new Point(x, y));
                        x++;
                    }

                    break;

                case TileShape.Plus:
                    this.Points.Add(new Point(leftEdge + 4, highestRock + 6));
                    this.Points.Add(new Point(leftEdge + 3, highestRock + 5));
                    this.Points.Add(new Point(leftEdge + 4, highestRock + 5));
                    this.Points.Add(new Point(leftEdge + 5, highestRock + 5));
                    this.Points.Add(new Point(leftEdge + 4, highestRock + 4));
                    break;

                case TileShape.BackwardsL:
                    this.Points.Add(new Point(leftEdge + 5, highestRock + 6));
                    this.Points.Add(new Point(leftEdge + 5, highestRock + 5));
                    this.Points.Add(new Point(leftEdge + 3, highestRock + 4));
                    this.Points.Add(new Point(leftEdge + 4, highestRock + 4));
                    this.Points.Add(new Point(leftEdge + 5, highestRock + 4));
                    break;

                case TileShape.Pipe:
                    x = leftEdge + 3;
                    y = highestRock + 4;
                    for (int i = 0; i < 4; i++)
                    {
                        this.Points.Add(new Point(x, y));
                        y++;
                    }

                    break;

                case TileShape.Square:
                    this.Points.Add(new Point(leftEdge + 3, highestRock + 5));
                    this.Points.Add(new Point(leftEdge + 4, highestRock + 5));
                    this.Points.Add(new Point(leftEdge + 3, highestRock + 4));
                    this.Points.Add(new Point(leftEdge + 4, highestRock + 4));

                    break;

                default:
                    throw new Exception($"Unexpected TileShape {shape}.");

            }
        }

        public HashSet<Point> Points { get; private set; }

        public bool Move(TileMove tileMove, int left, int right, HashSet<Point> points)
        {
            HashSet<Point> newPoints = null;
            switch (tileMove)
            {
                case TileMove.Left:
                    int leftMost = this.Points.Select(p => p.X).Min();
                    if (leftMost - 1 == left)
                    {
                        return false;
                    }

                    newPoints = this.Points.Select(p => new Point(p.X - 1, p.Y)).ToHashSet();

                    if (points.Intersect(newPoints).Any())
                    {
                        return false;
                    }

                    this.Points = newPoints;
                    return true;

                case TileMove.Right:
                    int rightMost = this.Points.Select(p => p.X).Max();
                    if (rightMost + 1 == right)
                    {
                        return false;
                    }

                    newPoints = this.Points.Select(p => new Point(p.X + 1, p.Y)).ToHashSet();

                    if (points.Intersect(newPoints).Any())
                    {
                        return false;
                    }

                    this.Points = newPoints;
                    return true;

                case TileMove.Down:
                    int bottomMost = this.Points.Select(p => p.Y).Min();
                    if (bottomMost - 1 == 0)
                    {
                        return false;
                    }

                    newPoints = this.Points.Select(p => new Point(p.X, p.Y - 1)).ToHashSet();

                    if (points.Intersect(newPoints).Any())
                    {
                        return false;
                    }

                    this.Points = newPoints;
                    return true;

                default:
                    throw new ArgumentException($"Unexpected move {tileMove}.");
            }
        }
    }

    internal enum TileShape
    {
        Minus,
        Plus,
        BackwardsL,
        Pipe,
        Square
    }

    internal enum TileMove
    {
        Down,
        Left,
        Right
    }

}
