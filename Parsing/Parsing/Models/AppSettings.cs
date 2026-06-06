using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Models
{
    public class AppSettings
    {
        public int Id  { get; set; }

        public int PageSize { get; set; }

        public bool AskPageSize { get; set; }
    }
}
