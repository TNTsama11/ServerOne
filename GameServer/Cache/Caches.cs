using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache
{
    public class Caches
    {
        public static AccCache acc { get; set; }
        static Caches()
        {
            acc = new AccCache();
        }
    }
}
