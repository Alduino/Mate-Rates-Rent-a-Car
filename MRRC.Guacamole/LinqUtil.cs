using System.Collections.Generic;
using System.Linq;

namespace MRRC.Guacamole
{
    public static class LinqUtil
    {
        // Originally the program was made in .Net 4.8, but as the QUT VMs only support 4.6.2 we need to add some
        // methods back in
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T next) => source.Concat(new[] {next});
    }
}