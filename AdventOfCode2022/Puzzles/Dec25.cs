using AdventOfCode2022.Utilities;
using System.Text;

namespace AdventOfCode2022.Puzzles
{
    internal static class Dec25
    {
        public static void SolvePartOne(bool isTest = false)
        {
            long sum = 0;
            foreach (string line in PuzzleReader.ReadLines(25, isTest))
            {
                long num = 0;

                long powerOf = 1;
                for (int i = line.Length - 1; i >= 0; i--)
                {
                    int digit = 0;
                    if (Int32.TryParse(line[i].ToString(), out digit))
                    {
                        num += powerOf * digit;
                    }
                    else if (line[i] == '-')
                    {
                        num += -1 * powerOf;
                    }
                    else if (line[i] == '=')
                    {
                        num += -2 * powerOf;
                    }
                    else
                    {
                        throw new ArgumentException($"Unexpected symbol {line[i]}.");
                    }

                    powerOf *= 5;
                }

                sum += num;
                string reverse = DecToBase5(num);
                Console.WriteLine($"Original = {line}, Dec = {num}, Base5 = {reverse}.");
            }

            string snafuNum = DecToBase5(sum);
            Console.WriteLine($"SNAFU num = {snafuNum}.");
        }

        private static string DecToBase5(long number)
        {
            var sb = new StringBuilder();

            int pow = 0;
            long divisor = 1;
            while (Math.Round(number / (1.0 * divisor), MidpointRounding.ToEven) > 2)
            {
                pow++;
                divisor *= 5;
            }

            int quotient = (int)Math.Round(number / (1.0 * divisor), MidpointRounding.ToEven);
            sb.Append(quotient);

            long temp = divisor;

            temp *= quotient;

            while (divisor > 1)
            {
                divisor /= 5;
                if (temp > number)
                {
                    quotient = (int)Math.Round((temp - number)/ (1.0 * divisor), MidpointRounding.ToEven);
                    switch (quotient)
                    {
                        case 0:
                            sb.Append("0");
                            break;

                        case 1:
                            sb.Append("-");
                            temp -= divisor;
                            break;

                        case 2:
                            sb.Append("=");
                            temp -= (2 * divisor);
                            break;

                        default:
                            throw new ArgumentException($"Unexpected quotient {quotient}.");
                    }
                }
                else
                {
                    quotient = (int)Math.Round((number - temp) / (1.0 * divisor), MidpointRounding.ToEven);
                    switch (quotient)
                    {
                        case 0:
                            sb.Append("0");
                            break;

                        case 1:
                            sb.Append("1");
                            temp += divisor;
                            break;

                        case 2:
                            sb.Append("2");
                            temp += (2 * divisor);
                            break;

                        default:
                            throw new ArgumentException($"Unexpected quotient {quotient}.");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
