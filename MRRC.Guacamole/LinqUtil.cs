using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole
{
    /// <summary>
    /// Utilities for Linq
    /// </summary>
    public static class LinqUtil
    {
        // Originally the program was made in .Net 4.8, but as the QUT VMs only support 4.6.2 we need to add some
        // methods back in
        /// <summary>
        /// Appends an item to the end of the enumerator
        /// </summary>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T next) => source.Concat(new[] {next});

        /// <summary>
        /// Adds the specified items to the end of the collection
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items) collection.Add(item);
        }
    }
}