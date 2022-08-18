using StringFormatter.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringFormatter.Services
{
    public class UnderlineEscapeFormatter : IStringFormatter
    {
        public string Display { get; } = "下划线转义";

        public string Process(string str)
        {
            StringBuilder sb = new StringBuilder();
            bool inline = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '$') inline = !inline;
                if (inline && c == '_')
                {
                    sb.Append("\\");
                    sb.Append(c);
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
