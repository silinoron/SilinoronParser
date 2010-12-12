using System;
using System.Text.RegularExpressions;

namespace SilinoronParser.SQLOutput
{
    public static class Extensions
    {
        public static string ToSQL(this string input)
        {
            //var data = Regex.Replace(input, @"'", @"\'");
            var data = Regex.Replace(input, "\"", "\\\"");
            return data;
        }
    }
}
