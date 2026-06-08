using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Models
{
    public class MonoBankApi
    {
        public string? id { get; set; }

        public string? description { get; set; }

        public long amount { get; set; }

        public string? counterName { get; set; }

        public long time { get; set; }

        public long balance { get; set; } 
    }
}
