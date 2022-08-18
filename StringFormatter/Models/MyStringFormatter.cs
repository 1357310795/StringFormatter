using StringFormatter.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringFormatter.Models
{
    public class MyStringFormatter
    {
        public IStringFormatter StringFormatter { get; set; }
    }
}
