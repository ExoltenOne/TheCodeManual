using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksTraceGenerator
{
    public static class ListExtension
    {
        private static readonly Random Rng = new Random();

        /// <summary>
        /// Shuffle any (I)List with an extension method based on the <a href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle">Fisher-Yates shuffle</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
