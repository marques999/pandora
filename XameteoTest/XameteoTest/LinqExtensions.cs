using System;
using System.Collections.Generic;

namespace XameteoTest
{
    /// <summary>
    /// </summary>
    internal static class LinqExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static TSource MinimumBy<TSource>(this IEnumerable<TSource> source, Func<TSource, IComparable> comparer)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext() == false)
                {
                    throw new InvalidOperationException();
                }

                var minimum = enumerator.Current;
                var minimumProjection = comparer(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    var currentProjection = comparer(enumerator.Current);

                    if (currentProjection.CompareTo(minimumProjection) >= 0)
                    {
                        continue;
                    }

                    minimum = enumerator.Current;
                    minimumProjection = currentProjection;
                }

                return minimum;
            }
        }
    }
}