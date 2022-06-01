using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WindowsApplication2
{
    public class SQLHelper
    {
        public static OleDbConnection conn(string connstr)
        {
            if (connstr == "")
            {
                throw new System.Exception("执行帐套间数据导入操作请先选择目标帐套！");
            }
            OleDbConnection oledb = new OleDbConnection(connstr);
                if (oledb != null)
                {
                    if (oledb.State != ConnectionState.Open)
                        oledb.Open();
                }
                return oledb;
            

        }
        public static SqlConnection sqlconn(string connstr)
        {
            if (connstr == "")
            {
                throw new System.Exception("执行帐套间数据导入操作请先选择目标帐套！");
            }
            SqlConnection oledb = new SqlConnection(connstr);
            if (oledb != null)
            {
                if (oledb.State != ConnectionState.Open)
                    oledb.Open();
            }
            return oledb;


        }
        #region 防止NULL异常
        public static long getlong(object obj)
        {
            long lg = 0;
            if (obj != null)
            {
                long.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static bool getbool(object obj)
        {
            bool lg = false;
            if (obj != null)
            {
                bool.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static decimal getdecimal(object obj)
        {
            decimal lg = 0;
            if (obj != null)
            {
                decimal.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static DateTime getdatetime(object obj)
        {
            DateTime lg = DateTime.MinValue;
            if (obj != null)
            {
                DateTime.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static int getint(object obj)
        {
            int lg = 0;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }

        public static string getstr(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }
        #endregion
        public static object ExecuteScalar(OleDbConnection connection,CommandType dd,string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            OleDbCommand odc = new OleDbCommand(commandText, connection);
            return odc.ExecuteScalar();
        }
        public static DataTable getdt_new(string sql,Object obj)
        {
            try
            {


                SqlConnection conn = new SqlConnection("user id=sa;data source=localhost;Connect Timeout=30;initial catalog=UFDATA_003_2013;password=123456");
                DataTable dt = new DataTable();
                if (conn != null)
                {
                    SqlDataAdapter odda = new SqlDataAdapter(sql, conn);

                    odda.Fill(dt);
                    conn.Dispose();
                }
                else
                {
                    throw new System.Exception("数据库连接失败，请检查配置！");
                }
                return dt;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new DataTable();
            }
        }
        public static DataTable getdt(string sql, OleDbConnection conn)
        {
            try
            {


                if (false)
                {
                    //byte[][] rtnbytess = null;
                    //List<byte[]> test = new List<byte[]>();

                    //test.Add(PubMethod.ObjectToBytes(sql));
                    //bool flag = U9CommonSv.U9CommonSv.DOU9CommonSv("SQLGetDT", test, Init_Set.context, Init_Set.portal_url, out rtnbytess);
                    //if (flag)
                    //{
                    //    DataTable u9dt = PubMethod.BytesToObject(rtnbytess[0]) as DataTable;
                    //    return u9dt;
                    //}
                }
                DataTable dt = new DataTable();
                if (conn != null)
                {
                    OleDbDataAdapter odda = new OleDbDataAdapter(sql, conn);

                    odda.Fill(dt);
                    conn.Dispose();
                }
                else
                {
                    throw new System.Exception("数据库连接失败，请检查配置！");
                }
                return dt;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new DataTable();
            }
        }
        public static DataTable getdt(string sql, SqlConnection conn)
        {
            try
            {


                if (false)
                {
                    //byte[][] rtnbytess = null;
                    //List<byte[]> test = new List<byte[]>();

                    //test.Add(PubMethod.ObjectToBytes(sql));
                    //bool flag = U9CommonSv.U9CommonSv.DOU9CommonSv("SQLGetDT", test, Init_Set.context, Init_Set.portal_url, out rtnbytess);
                    //if (flag)
                    //{
                    //    DataTable u9dt = PubMethod.BytesToObject(rtnbytess[0]) as DataTable;
                    //    return u9dt;
                    //}
                }
                DataTable dt = new DataTable();
                if (conn != null)
                {
                    SqlDataAdapter odda = new SqlDataAdapter(sql, conn);

                    odda.Fill(dt);
                    conn.Dispose();
                }
                else
                {
                    throw new System.Exception("数据库连接失败，请检查配置！");
                }
                return dt;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new DataTable();
            }
        }
        public static DataTable getdtbystore(string storename, OleDbConnection conn, string WhereItem, string WhereDept, string VersionCode, int type)
        {
            //OleDbConnection sqlconn = conn;
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = storename;
            cmd.CommandType = CommandType.StoredProcedure;
            // 创建参数
            
            IDataParameter[] parameters = {
                new OleDbParameter("@WhereItem", OleDbType.VarChar,2000) ,
                new OleDbParameter("@WhereDept", OleDbType.VarChar,2000) ,
                new OleDbParameter("@VersionCode", OleDbType.VarChar,200), 
                new OleDbParameter("@type", OleDbType.Integer,4)   // 返回值
            };
            // 设置参数类型
            parameters[0].Value = WhereItem;        // 设置为输出参数
            parameters[1].Value = WhereDept;                   // 给输入参数赋值
            parameters[2].Value = VersionCode;   // 设置为返回值
            parameters[3].Value = type;   // 设置为返回值
            // 添加参数
            cmd.Parameters.Add(parameters[0]);
            cmd.Parameters.Add(parameters[1]);
            cmd.Parameters.Add(parameters[2]);
            cmd.Parameters.Add(parameters[3]);

            OleDbDataAdapter dp = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();
            // 填充dataset
            dp.Fill(ds);
            return ds.Tables[0];
        }
        public static int execsql(string sql, OleDbConnection conn)
        {
            DataTable dt = new DataTable();
            OleDbCommand odda = new OleDbCommand(sql, conn);
            int d = odda.ExecuteNonQuery();
            return d;
        }
        public static Dictionary<string, int> execsqls(ArrayList sqls, OleDbConnection conn,out ArrayList arr_rtn)
        {
            arr_rtn = new ArrayList();
            Dictionary<string, int> rtnvalue = new Dictionary<string, int>();
            OleDbCommand odda = new OleDbCommand();
            odda.CommandTimeout = 2400;
            odda.Connection = conn;
            int d = 0;
            foreach (string sql in sqls)
            {
                odda.CommandText = sql;
                try
                {
                    d = odda.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    
                    

                }
                if (!rtnvalue.ContainsKey(sql))
                    rtnvalue.Add(sql, d);
            }

            return rtnvalue;
        }
        public static void executeFunc(ArrayList arr_sqls, OleDbConnection conn, ref string sReturnCode)
        {
                string sErrorMessage = "OK";
                sReturnCode = "OK";
                //int bReturn = 0;
                try
                {
                    OleDbTransaction st = conn.BeginTransaction(); ;
                    //st.Connection = conn;
                    foreach (string sql in arr_sqls)
                    {
                        OleDbCommand odda = new OleDbCommand(sql, conn);

                        odda.Transaction = st;
                        //odda.BeginExecuteNonQuery();
                        odda.ExecuteNonQuery();
                    }

                    st.Commit();
                    //log.SQLLog(sSchemaName, sSQLs[i].ToString(), sTerminalID, sTerminalID, sUserID, sFunctionID, "ACTION");                
                }
                catch (System.Exception ex)
                {
                    sReturnCode = "Error";
                    sErrorMessage = ex.Message + ex.StackTrace;
                    throw new System.Exception(sErrorMessage);
                }
                finally
                {
                    conn.Dispose();
                }
        }
        public static Dictionary<string, Dictionary<long, Dictionary<int, string>>> getdictss(Hashtable ht_sql, OleDbConnection conntmp)
        {

            Dictionary<string, Dictionary<long, Dictionary<int, string>>> dictss = new Dictionary<string, Dictionary<long, Dictionary<int, string>>>();
           
            if (conntmp.State != ConnectionState.Open)
            {
                conntmp.Open();
            }
            OleDbCommand sc = new OleDbCommand();
            sc.Connection = conntmp;
            foreach (string strsql in ht_sql.Keys)
            {
                Dictionary<long, Dictionary<int, string>> dicts = new Dictionary<long, Dictionary<int, string>>();
                sc.CommandText = getstr(ht_sql[strsql]);
                OleDbDataReader sdr=sc.ExecuteReader();
                while (sdr.Read())
                {
                    Dictionary<int, string> dict = new Dictionary<int, string>();
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        dict.Add(i, getstr(sdr[i]));
                    }
                    if (strsql == "SaleOrder" || strsql == "ShipRMA" || strsql == "ARBill" || strsql == "RecMoney")
                    {
                        if (!dicts.ContainsKey(getlong(sdr["LineID"])))
                            dicts.Add(getlong(sdr["LineID"]), dict);
                    }
                    else
                        dicts.Add(getlong(sdr["ID"]), dict);
                }
                sdr.Close();
                dictss.Add(strsql, dicts);
            }
            sc.Dispose();
            conntmp.Dispose();
            return dictss;
        }      
    }
}
