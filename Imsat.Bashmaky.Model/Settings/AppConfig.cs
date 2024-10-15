using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imsat.Bashmaky.Model.Settings
{
    public class AppConfig
    {
        public int ShortSignalListCount { get; set; }
        public int PageUpdateTimeSec { get; set; }
        public int CheckConnectionTimeMin { get; set; }
        public int MaxConnectionDelayTimeMin { get; set; }
    }
}
