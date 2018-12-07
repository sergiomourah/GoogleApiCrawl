using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoogleApiCrawl.Converters
{
    public class Utils
    {
        public static string GetOnlyNumber(string value)
        {
            if (value == null) return "";

            Regex r = new Regex(@"\d+");
            string result = "";
            foreach (Match m in r.Matches(value))
                result += m.Value;

            return result;
        }
    }
}
