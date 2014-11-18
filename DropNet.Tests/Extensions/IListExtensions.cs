using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropNet.Tests.Extensions
{
    static class IListExtensions
    {
        public static bool Exists<T>(this IList<T> list, Func<T, bool> predicate)
        {
            return list.First(predicate) != null;
        }
    }
}
