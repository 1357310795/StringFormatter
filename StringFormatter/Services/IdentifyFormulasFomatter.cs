using StringFormatter.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringFormatter.Services
{
    public class IdentifyFormulasFomatter : IStringFormatter
    {
        public string Display { get; } = "识别LaTeX公式、去除多余空格";

        public string Process(string str)
        {
            var lines = str.Split("\r\n");
            StringBuilder sb = new StringBuilder(str.Length);
            bool block = false;

            for(int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "")
                {
                    sb.Append("$$");
                    block = !block;
                }
                else
                {
                    if (block)
                    {
                        sb.Append(lines[i]);
                    }
                    else
                        sb.Append(ProcessLine(lines[i]));
                }
                if (i != lines.Length - 1)
                    sb.Append("\r\n");
            }

            return sb.ToString();
        }

        public string ProcessLine(string str)
        {
            char[] chars = str.ToCharArray();
            StringBuilder sb = new StringBuilder(chars.Length + 20);

            //行内
            bool islatex = false;
            string inlineblank = "";
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (IsChn(c))//中文
                {
                    if (islatex)
                    {
                        sb.Append("$ ");
                        sb.Append(c);
                        islatex = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else if (c == ' ')//空格
                {
                    if (islatex)
                    {
                        inlineblank += c;
                        //sb.Append(c);
                    }
                    else
                    {
                        //忽略其他空格
                    }
                }
                else if (c == '\r' || c == '\n')//换行
                {
                    if (islatex)
                    {
                        islatex = false;
                        sb.Append("$ ");
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else//LaTeX
                {
                    if (islatex)
                    {
                        if (inlineblank != "")
                        {
                            sb.Append(inlineblank);
                            inlineblank = "";
                        }
                        sb.Append(c);
                    }
                    else
                    {
                        if (inlineblank != "")
                        {
                            inlineblank = "";
                        }
                        sb.Append(" $");
                        sb.Append(c);
                        islatex = true;
                    }
                }
            }
            str = sb.ToString();
            Regex reg1 = new Regex(" ?。 ?");
            Regex reg2 = new Regex(" ?， ?");
            Regex reg3 = new Regex(" ?； ?");
            str = reg1.Replace(str, "。");
            str = reg2.Replace(str, "，");
            str = reg3.Replace(str, "；");
            return str;
        }

        public static bool IsChn(char c)
        {
            return c >= (char)128;
        }
    }
}
