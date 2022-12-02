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
    }

    public enum Move
    {
        Rock,
        Paper,
        Scissors
    }
}
