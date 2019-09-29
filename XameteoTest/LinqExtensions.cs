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
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static T MinimumBy<T>(this IEnumerable<T> source, Func<T, IComparable> comparer)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext() == false)
                {
                    return default;
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