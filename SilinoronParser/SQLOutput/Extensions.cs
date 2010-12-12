using System;
using System.Text.RegularExpressions;

namespace SilinoronParser.SQLOutput
{
    public static class Extensions
    {
        public static string ToSQL(this string input)
        {
            var str = input.Replace("\\", "\\\\");
            str = str.Replace("'", "\\'");
            str = str.Replace("\"", "\\\"");
            str = str.Replace("\r", "\\r");
            str = str.Replace("\n", "\\n");
            return str;
        }
    }
}
