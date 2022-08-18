using StringFormatter.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringFormatter.Services
{
    internal class MathrmFormatter : IStringFormatter
    {
        public string Display { get; } = "\\text变\\mathrm";

        public string Process(string str)
        {
            var reg1 = new Regex(@"\\text\ ?{ *(.+?) *\}");
            str = reg1.Replace(str, x => $"\\mathrm{{{x.Groups[1].Value}}}");
            return str;
        }
    }
}
