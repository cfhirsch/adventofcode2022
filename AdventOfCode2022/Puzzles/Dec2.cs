using AdventOfCode2022.Utilities;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec2
    {
        public static void SolvePartOne()
        {
            long score = 0;
            foreach (string line in PuzzleReader.ReadLines(2))
            {
                string[] moveStrings = line.Split(" ");
                Move player1 = StringToMove(moveStrings[0]);
                Move player2 = StringToMove(moveStrings[1]);

                score += GetScore(player1, player2);
            }

            Console.WriteLine("Score = {0}", score);
        }

        public static void SolvePartTwo()
        {
            long score = 0;
            foreach (string line in PuzzleReader.ReadLines(2))
            {
                string[] moveStrings = line.Split(" ");
                Move player1 = StringToMove(moveStrings[0]);
                Move player2 = StringToMove2(player1, moveStrings[1]);

                score += GetScore(player1, player2);
            }

            Console.WriteLine("Score = {0}", score);
        }

        /// The score for a single round is the score for the shape you selected (1 for Rock, 2 for Paper, and 3 for Scissors) 
        /// plus the score for the outcome of the round (0 if you lost, 3 if the round was a draw, and 6 if you won).
        private static long GetScore(Move player1, Move player2)
        {
            switch (player1)
            {
                case Move.Rock:
                    switch (player2)
                    {
                        case Move.Rock:
                            return 1 + 3;

                        case Move.Paper:
                            return 2 + 6;

                        case Move.Scissors:
                            return 3 + 0;

                        default:
                            throw new ArgumentException($"Unexpected move {player2}");
                    }

                case Move.Paper:
                    switch (player2)
                    {
                        case Move.Rock:
                            return 1 + 0;

                        case Move.Paper:
                            return 2 + 3;

                        case Move.Scissors:
                            return 3 + 6;

                        default:
                            throw new ArgumentException($"Unexpected move {player2}");
                    }

                case Move.Scissors:
                    switch (player2)
                    {
                        case Move.Rock:
                            return 1 + 6;

                        case Move.Paper:
                            return 2 + 0;

                        case Move.Scissors:
                            return 3 + 3;

                        default:
                            throw new ArgumentException($"Unexpected move {player2}");
                    }

                default:
                    throw new ArgumentException($"Unexpected move {player1}");
            }
        }

        private static Move StringToMove(string str)
        {
            if (str == "A" || str == "X")
            {
                return Move.Rock;
            }

            if (str == "B" || str == "Y")
            {
                return Move.Paper;
            }

            if (str == "C" || str == "Z")
            {
                return Move.Scissors;
            }

            throw new ArgumentException($"Unexpected input {str}.");
        }

        private static Move StringToMove2(Move player1, string str)
        {
            // X means you need to lose, Y means you need to end the round in a draw, and Z means you need to win.
            
            switch (str)
            {
                case "X":
                    switch (player1)
                    {
                        case Move.Rock:
                            return Move.Scissors;

                        case Move.Paper:
                            return Move.Rock;

                        case Move.Scissors:
                            return Move.Paper;

                        default:
                            throw new ArgumentException($"Unexpected move {player1}.");
                    }

                case "Y":
                    switch (player1)
                    {
                        case Move.Rock:
                            return Move.Rock;

                        case Move.Paper:
                            return Move.Paper;

                        case Move.Scissors:
                            return Move.Scissors;

                        default:
                            throw new ArgumentException($"Unexpected move {player1}.");
                    }

                case "Z":
                    switch (player1)
                    {
                        case Move.Rock:
                            return Move.Paper;

                        case Move.Paper:
                            return Move.Scissors;

                        case Move.Scissors:
                            return Move.Rock;

                        default:
                            throw new ArgumentException($"Unexpected move {player1}.");
                    }
            }

            throw new ArgumentException($"Unexpected input {str}.");
        }
    }

    public enum Move
    {
        Rock,
        Paper,
        Scissors
    }
}
