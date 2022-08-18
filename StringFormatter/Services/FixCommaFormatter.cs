using StringFormatter.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringFormatter.Services
{
    internal class FixCommaFormatter : IStringFormatter
    {
        public string Display { get; } = "英文符号转中文";

        public string Process(string str)
        {
            str = str.Replace(", ", "，");
            str = str.Replace(",", "，");
            Regex reg1 = new Regex(" ?; ?");
            Regex reg2 = new Regex(" ?: ?");
            str = reg1.Replace(str, "；");
            str = reg2.Replace(str, "：");
            return str;
        }
    }
}
