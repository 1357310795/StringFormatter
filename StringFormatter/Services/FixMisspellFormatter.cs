using StringFormatter.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringFormatter.Services
{
    public class FixMisspellFormatter : IStringFormatter
    {
        public string Display { get; } = "修复OCR的各种错别字";

        public string Process(string str)
        {
            var reg1 = new Regex("人(?!们)");
            str = reg1.Replace(str, "入");
            return str;
        }
    }
}
