using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApplication2.Helper
{
    class DataHelper
    {
        public static string getStr(object obj)
        {
            return obj == null ? "" : obj.ToString().Replace("\t","");
        }

        public static decimal getDecimal(object obj)
        {
            return obj == null || string.IsNullOrEmpty(obj.ToString()) ? 0 : Convert.ToDecimal(obj.ToString());
        }
    }
}
