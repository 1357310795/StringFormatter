using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringFormatter.Services.Contracts
{
    public interface IStringFormatter
    {
        public string Display { get; }

        public string Process(string str);
    }
}
