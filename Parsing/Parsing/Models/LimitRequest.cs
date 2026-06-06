using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Models
{
    public class LimitRequest
    {
        public int Id { get; set; }

        public DateTimeOffset? Time { get; set; }
    }
}
