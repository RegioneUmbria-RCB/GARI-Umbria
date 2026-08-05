using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaDataProvider6.Utils
{
    public static class ListExtension
    {
        /// <summary>
        /// Divide gli elementi di una sequenza in chunk di dimensione massima <paramref name="chunkSize"/>.
        /// </summary>
        public static IEnumerable<IEnumerable<TSource>> ChunkBy<TSource>(
            this IEnumerable<TSource> source, int chunkSize)
        {
            return ChunkByWorker(source, chunkSize);
        }

        private static IEnumerable<IEnumerable<TSource>> ChunkByWorker<TSource>(
       IEnumerable<TSource> source, int chunkSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize), "chunkSize deve essere > 0");

            using var e = source.GetEnumerator();
            while (true)
            {
                var chunk = new List<TSource>(capacity: chunkSize);
                while (chunk.Count < chunkSize && e.MoveNext())
                {
                    chunk.Add(e.Current);
                }

                if (chunk.Count == 0)
                    yield break;

                yield return chunk;
            }
        }
    }
}
