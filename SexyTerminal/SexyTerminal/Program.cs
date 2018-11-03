using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SexyTerminal
{
    public enum ColorFormat
    {
        RGB,
        RGBA,
        ARGB
    }

    internal static class Program
    {
        public static List<Color> RecentColors = new[]
        {
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
        }.ToList();

        static string rollTheString(string s, List<int> roll)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            var modulo = 'z' - 'a' + 1;

            foreach (var item in roll)
            {
                var amount = Math.Min(item, s.Length);

                if (amount <= 0)
                {
                    continue;
                }

                var rotated = s.Substring(0, amount).Select(character =>
                {
                    return (char)('a' + ((character - 'a' + 1) % modulo));
                });

                s = string.Join("", rotated) + s.Substring(amount);
            }

            return s;
        }

        private static string[] BalancedBraces(IReadOnlyList<string> values)
        {
            var result = new string[values.Count];
            var opening = new List<char> { '[', '{', '(' };
            var closing = new List<char> { ']', '}', ')' };
            var matching = Enumerable.Range(0, opening.Count).ToDictionary(i => opening[i], i => closing[i]);

            for (var current = 0; current < values.Count; current++)
            {
                var valid = true;
                var sequence = values[current];

                if (sequence.Length % 2 == 0)
                {
                    var index = 0;
                    var stack = new Stack<char>();

                    while (valid && index < sequence.Length)
                    {
                        var symbol = sequence[index++];

                        if (opening.Contains(symbol))
                        {
                            stack.Push(symbol);
                        }
                        else if (closing.Contains(symbol) && (stack.Count == 0 || matching[stack.Pop()] != symbol))
                        {
                            valid = false;
                        }
                    }
                }
                else
                {
                    valid = false;
                }

                result[current] = valid ? "YES" : "NO";
                Console.WriteLine(result[current]);
            }

            return result;
        }


        static void customSort(int[] arr)
        {
            var occurences = arr.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            Array.Sort(arr, (lhs, rhs) =>
            {
                if (occurences[lhs] == occurences[rhs])
                {
                    return lhs.CompareTo(rhs);
                }

                return occurences[lhs].CompareTo(occurences[rhs]);
            });

            foreach (var item in arr)
            {
                Console.WriteLine(item);
            }
        }

        private static string MergeStrings(string a, string b)
        {
            var result = new StringBuilder();

            using (var aEnumerator = a.GetEnumerator())
            using (var bEnumerator = b.GetEnumerator())
            {
                bool firstHasMore;
                bool secondHasMore;

                while ((firstHasMore = aEnumerator.MoveNext()) | (secondHasMore = bEnumerator.MoveNext()))
                {
                    if (firstHasMore)
                    {
                        result.Append(aEnumerator.Current);
                    }

                    if (secondHasMore)
                    {
                        result.Append(bEnumerator.Current);
                    }
                }
            }

            return result.ToString();
        }

        private static string FirstRepeatedWord(string s)
        {
            return s.Split(',', ';', ':', '.', ' ').GroupBy(x => x.ToLower()).FirstOrDefault(x => x.ToArray().Length > 1)?.Key;
        }

        /// <summary>
        /// </summary>
        /// <param name="numbers"></param>
        private static void ClosestNumbers(List<int> numbers)
        {
            var minimumDifference = int.MaxValue;

            for (var index = 0; index < numbers.Count - 1; index++)
            {
                var difference = Math.Min(minimumDifference, Math.Abs(numbers[index + 1] - numbers[index]));

                if (difference > 0)
                {
                    minimumDifference = difference;
                }
            }

            numbers.Sort();

            for (var index = 0; index < numbers.Count - 1; index++)
            {
                if (numbers[index + 1] - numbers[index] == minimumDifference)
                {
                    Console.WriteLine($"{numbers[index]} {numbers[index + 1]}");
                }
            }
        }

        public static sbyte RecentColorsMax { get; set; } = 8;

        /// <summary>
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}