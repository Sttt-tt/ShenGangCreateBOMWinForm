using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsApplication2
{
    public class InitSet
    {
        public static string connstr = "Provider=SQLOLEDB;User Id=sa;Password=123456;Data Source=192.168.0.11;Initial Catalog=UFDATA_003_2012;packet size=4096;Max Pool size=100;Connection Timeout=900;persist security info=True;MultipleActiveResultSets=true;";
        public static string sqlconnstr = "";
        public static string srcdata = "";
        public static string accid = "";
        public static string mbconnstr = "";
        public static string mbconnstr_sql = "";
        public static string loginuser = "";
    }
}
