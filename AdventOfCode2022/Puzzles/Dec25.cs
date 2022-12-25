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
                long powerOf = 1;
                for (int i = line.Length - 1; i >= 0; i--)
                {
                    int digit = 0;
                    if (Int32.TryParse(line[i].ToString(), out digit))
                    {
                        sum += powerOf * digit;
                    }
                    else if (line[i] == '-')
                    {
                        sum += -1 * powerOf;
                    }
                    else if (line[i] == '=')
                    {
                        sum += -2 * powerOf;
                    }
                    else
                    {
                        throw new ArgumentException($"Unexpected symbol {line[i]}.");
                    }

                    powerOf *= 5;
                }

                string reverse = DecToBase5(sum);
                Console.WriteLine($"Original = {line}, Dec = {sum}, Base5 = {reverse}.");
            }


        }

        private static string DecToBase5(long number)
        {
            var sb = new StringBuilder();

            int pow = 0;
            int divisor = 1;
            while (Math.Round(number / (1.0 * divisor), MidpointRounding.ToEven) > 2)
            {
                pow++;
                divisor *= 5;
            }

            int quotient = (int)Math.Round(number / (1.0 * divisor), MidpointRounding.ToEven);
            sb.Append(quotient);

            long temp = divisor;

            temp *= quotient;

            while (temp != number)
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

            while (divisor > 1)
            {
                sb.Append("0");
                divisor--;
            }

            return sb.ToString();
        }
    }
}
