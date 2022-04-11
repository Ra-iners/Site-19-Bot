using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Extensions
{
    public static class StringExtensions
    {
        public static string ToRules(this string str)
        {
            foreach(var Key in Globals.Rules.Keys)
                str = str.Replace("{" + Key + "}", Globals.Rules[Key]);
            return str;
        }
    }
}
