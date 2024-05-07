using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbinds
{
    internal class Helpers
    {
    }

    public static class IEnumerableExtension
    {
        public static string Stringify<T>(this IEnumerable<T> enumerable)
        {
            return $"[ {string.Join(", ", enumerable)} ]";
        }
    } 
}
