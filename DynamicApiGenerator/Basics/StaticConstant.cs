using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicApiGenerator
{
    internal static class StaticConstant
    {
       public static ConcurrentDictionary<string, object> dic = new ConcurrentDictionary<string, object>();
    }
}
