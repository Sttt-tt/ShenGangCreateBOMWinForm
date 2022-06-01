using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace WindowsApplication2
{
    public class MiddleDBInterface
    {
        public static string strconn = Init_Set.ls_conn;//"user id=chaxun;data source=172.16.100.64;Connect Timeout=300;initial catalog=SANHUADATA;password=sanhua219";//u8
        //public static string strconn = "user id=sa;data source=172.16.100.20;Connect Timeout=300;initial catalog=20130411;password=8004";//u8
        //public static string strconn = "user id=sa;data source=.;Connect Timeout=300;initial catalog=BMTest;password=123456";//u8
        public static SqlConnection conn()
        {
            SqlConnection oledb = new SqlConnection(strconn);
            try
            {
                if (oledb != null)
                {
                    if (oledb.State != ConnectionState.Open)
                        oledb.Open();
                }
                return oledb;
            }
            catch (System.Exception ex)
            {

                //MessageBox.Show(ex.Message);
                return null;
            }

        }

        public static SqlConnection conn(string strConn)
        {
            SqlConnection oledb = new SqlConnection(strConn);
            try
            {
                if (oledb != null)
                {
                    if (oledb.State != ConnectionState.Open)
                        oledb.Open();
                }
                return oledb;
            }
            catch (System.Exception ex)
            {

                //MessageBox.Show(ex.Message);
                return null;
            }

        }
        public static SqlConnection conn_mid()
        {

            //string conn_str="";
            SqlConnection oledb = new SqlConnection(strconn);
            try
            {
                if (oledb != null)
                {
                    if (oledb.State != ConnectionState.Open)
                        oledb.Open();
                }
                return oledb;
            }
            catch (System.Exception ex)
            {

                //MessageBox.Show(ex.Message);
                return null;
            }

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
        public static DataTable getdt(string sql, SqlConnection conn)
        {
            DataTable dt = new DataTable();
            if (conn != null)
            {
                SqlDataAdapter odda = new SqlDataAdapter(sql, conn);
                odda.Fill(dt);
                conn.Dispose();
            }
            else
            {
                MessageBox.Show("数据库连接失败，请检查配置！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            return dt;
        }

        public static DataTable getitemdata()
        {
            return getdt("", conn());
        }
        public static int execsql(string sql, SqlConnection conn)
        {
            DataTable dt = new DataTable();
            SqlCommand odda = new SqlCommand(sql, conn);
            int d = odda.ExecuteNonQuery();
            return d;
        }
        public static void msg(string message)
        {
            
        }
        public static void executeFunc(ArrayList arr_sqls, SqlConnection conn, ref string sReturnCode)
        {
            string sErrorMessage = "OK";
            sReturnCode = "OK";
            int bReturn = 0;
            try
            {
                SqlTransaction st = conn.BeginTransaction(); ;
                //st.Connection = conn;
                foreach (string sql in arr_sqls)
                {
                    SqlCommand odda = new SqlCommand(sql, conn);

                    odda.Transaction = st;
                    //odda.BeginExecuteNonQuery();
                    odda.ExecuteNonQuery();
                }

                st.Commit();
                bReturn = 1;
                //log.SQLLog(sSchemaName, sSQLs[i].ToString(), sTerminalID, sTerminalID, sUserID, sFunctionID, "ACTION");                
            }
            catch (System.Exception ex)
            {
                sReturnCode = "Error";
                sErrorMessage = ex.Message + ex.StackTrace;
                bReturn = -1;
                throw new System.Exception(sErrorMessage);
            }
            finally
            {
                
            }
        }
        public static void executeFuncbyhtsqls(Hashtable ht_sqls, SqlConnection conn, ref string sReturnCode)
        {
            string sErrorMessage = "OK";
            sReturnCode = "OK";
            int bReturn = 0;
            try
            {
                SqlTransaction st = conn.BeginTransaction(); ;
                //st.Connection = conn;
                SqlCommand oddacheck;
                SqlCommand odda;
                foreach (string key in ht_sqls.Keys)
                {
                    string excelname = key.Split('|')[0];
                    string colum1 = key.Split('|')[1];
                    oddacheck = new SqlCommand("select ExcelName from SG_Data where ExcelName='" + excelname + "' and Column1='" + colum1 + "'", conn);
                    oddacheck.Transaction = st;
                    if (getstr(oddacheck.ExecuteScalar()) != "")
                    {
                        odda = new SqlCommand("delete from SG_Data where ExcelName='"+excelname+"' and Column1='"+colum1+"'", conn);
                        odda.Transaction = st;
                        //odda.BeginExecuteNonQuery();
                        odda.ExecuteNonQuery();
                    }
                    odda = new SqlCommand(getstr(ht_sqls[key]), conn);

                    odda.Transaction = st;
                    //odda.BeginExecuteNonQuery();
                    odda.ExecuteNonQuery();
                }

                st.Commit();
                bReturn = 1;
                //log.SQLLog(sSchemaName, sSQLs[i].ToString(), sTerminalID, sTerminalID, sUserID, sFunctionID, "ACTION");                
            }
            catch (System.Exception ex)
            {
                sReturnCode = "Error";
                sErrorMessage = ex.Message + ex.StackTrace;
                bReturn = -1;
                throw new System.Exception(sErrorMessage);
            }
            finally
            {

            }
        }
        public static void executeFunc_spc(ArrayList arr_sqls, SqlConnection conn,Dictionary<string,Hashtable> spc, ref string sReturnCode)
        {
            string sErrorMessage = "OK";
            sReturnCode = "OK";
            int bReturn = 0;
            SqlTransaction st=conn.BeginTransaction();
            try
            {
                 //st = conn.BeginTransaction(); ;
                //st.Connection = conn;
                foreach (string sqltype in spc.Keys)
                {
                    Hashtable ht_spc = spc[sqltype] as Hashtable;
                    if (sqltype == "BOMDoc")
                    {
                        #region sql
                        string sql= @"INSERT INTO [dbo].[InterFace_BOMDoc]
           ([ID]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           ,[SysVersion]
           ,[Code]
           ,[Rev]
           ,[Name]
           ,[Style]
           ,[Description]
           ,[ItemForm]
           ,[Unit]
           ,[Project_No]
           ,[Schema_No]
           ,[EffectiveDate]
           ,[DisableDate]
           ,[MRPPlan]
           ,[Preid]
           ,[Department]
           ,[DrawingNo]
           ,[Category])
     VALUES(@ID
           ,@CreatedOn
           ,@CreatedBy
           ,@ModifiedOn
           ,@ModifiedBy
           ,@SysVersion
           ,@Code
           ,@Rev
           ,@Name
           ,@Style
           ,@Description
           ,@ItemForm
           ,@Unit
           ,@Project_No
           ,@Schema_No
           ,@EffectiveDate
           ,@DisableDate
           ,@MRPPlan
           ,@Preid
           ,@Department
           ,@DrawingNo
           ,@Category)";

                        #endregion
                        SqlCommand odda = new SqlCommand(sql, conn);
                        //odda.Parameters.
                            odda.Parameters.Add(new SqlParameter("@ID", SqlDbType.BigInt));
                            odda.Parameters["@ID"].Value = getlong(ht_spc["ID"]);
                        odda.Parameters.Add(new SqlParameter("@CreatedOn", SqlDbType.DateTime));
                        odda.Parameters["@CreatedOn"].Value = Convert.ToDateTime(ht_spc["CreatedOn"]);
                        odda.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar));
                        odda.Parameters["@CreatedBy"].Value = getstr(ht_spc["CreatedBy"]);
                        odda.Parameters.Add(new SqlParameter("@ModifiedOn", SqlDbType.DateTime));
                        odda.Parameters["@ModifiedOn"].Value = Convert.ToDateTime(ht_spc["ModifiedOn"]);
                        odda.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.NVarChar));
                        odda.Parameters["@ModifiedBy"].Value = getstr(ht_spc["ModifiedBy"]);
                        odda.Parameters.Add(new SqlParameter("@SysVersion", SqlDbType.BigInt));
                        odda.Parameters["@SysVersion"].Value = getint(ht_spc["SysVersion"]);
                        odda.Parameters.Add(new SqlParameter("@Code", SqlDbType.NVarChar));
                        odda.Parameters["@Code"].Value = getstr(ht_spc["Code"]);
                        odda.Parameters.Add(new SqlParameter("@Rev", SqlDbType.NVarChar));
                        odda.Parameters["@Rev"].Value = getstr(ht_spc["Rev"]);
                        odda.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                        odda.Parameters["@Name"].Value = getstr(ht_spc["Name"]);
                        odda.Parameters.Add(new SqlParameter("@Style", SqlDbType.NVarChar));
                        odda.Parameters["@Style"].Value = getstr(ht_spc["Style"]);
                        odda.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar));
                        odda.Parameters["@Description"].Value = getstr(ht_spc["Description"]);
                        odda.Parameters.Add(new SqlParameter("@ItemForm", SqlDbType.NVarChar));
                        odda.Parameters["@ItemForm"].Value = getstr(ht_spc["ItemForm"]);
                        odda.Parameters.Add(new SqlParameter("@Unit", SqlDbType.NVarChar));
                        odda.Parameters["@Unit"].Value = getstr(ht_spc["Unit"]);
                        odda.Parameters.Add(new SqlParameter("@Project_No", SqlDbType.NVarChar));
                        odda.Parameters["@Project_No"].Value = getstr(ht_spc["Project_No"]);
                        odda.Parameters.Add(new SqlParameter("@Schema_No", SqlDbType.NVarChar));
                        odda.Parameters["@Schema_No"].Value = getstr(ht_spc["Schema_No"]);
                        odda.Parameters.Add(new SqlParameter("@EffectiveDate", SqlDbType.DateTime));
                        odda.Parameters["@EffectiveDate"].Value = Convert.ToDateTime(ht_spc["EffectiveDate"]);
                        odda.Parameters.Add(new SqlParameter("@DisableDate", SqlDbType.DateTime));
                        odda.Parameters["@DisableDate"].Value = Convert.ToDateTime(ht_spc["DisableDate"]);
                        odda.Parameters.Add(new SqlParameter("@MRPPlan", SqlDbType.Int));
                        odda.Parameters["@MRPPlan"].Value = getint(ht_spc["MRPPlan"]);
                        odda.Parameters.Add(new SqlParameter("@Preid", SqlDbType.Decimal));
                        odda.Parameters["@Preid"].Value = getdecimal(ht_spc["Preid"]);
                        odda.Parameters.Add(new SqlParameter("@Department", SqlDbType.NVarChar));
                        odda.Parameters["@Department"].Value = getstr(ht_spc["Department"]);
                        odda.Parameters.Add(new SqlParameter("@DrawingNo", SqlDbType.NVarChar));
                        odda.Parameters["@DrawingNo"].Value = getstr(ht_spc["DrawingNo"]);
                        odda.Parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar));
                        odda.Parameters["@Category"].Value = getstr(ht_spc["Category"]);
                        odda.Transaction = st;
                        //odda.BeginExecuteNonQuery();
                        odda.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = @"INSERT INTO [dbo].[InterFace_BOMLine]
           ([ID]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           ,[SysVersion]
           ,[BOMDoc]
           ,[OrderNo]
           ,[Code]
           ,[Name]
           ,[Style]
           ,[Description]
           ,[ItemForm]
           ,[Material]
           ,[Quantity]
           ,[Unit]
           ,[SigWeight]
           ,[TotWeight]
           ,[Factory]
           ,[Remark]
           ,[MRPPlan]
           ,[Preid]
           ,[Department]
           ,[UsingOrg]
           ,[IssueStyle]
           ,[DrawingNo],[Category])
     VALUES(@ID
           ,@CreatedOn
           ,@CreatedBy
           ,@ModifiedOn
           ,@ModifiedBy
           ,@SysVersion
           ,@BOMDoc
           ,@OrderNo
           ,@Code
           ,@Name
           ,@Style
           ,@Description
           ,@ItemForm
           ,@Material
           ,@Quantity
           ,@Unit
           ,@SigWeight
           ,@TotWeight
           ,@Factory
           ,@Remark
           ,@MRPPlan
           ,@Preid
           ,@Department
           ,@UsingOrg
           ,@IssueStyle
,@DrawingNo,@Category)";
                        SqlCommand odda = new SqlCommand(sql, conn);
                        
                        odda.Parameters.Add(new SqlParameter("@ID", SqlDbType.BigInt));
                        odda.Parameters["@ID"].Value = getlong(ht_spc["ID"]);
                        odda.Parameters.Add(new SqlParameter("@CreatedOn", SqlDbType.DateTime));
                        odda.Parameters["@CreatedOn"].Value = DateTime.Now;
                        odda.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar));
                        odda.Parameters["@CreatedBy"].Value = "CAD";
                        odda.Parameters.Add(new SqlParameter("@ModifiedOn", SqlDbType.DateTime));
                        odda.Parameters["@ModifiedOn"].Value = DateTime.Now;
                        odda.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.NVarChar));
                        odda.Parameters["@ModifiedBy"].Value = "CAD";
                        odda.Parameters.Add(new SqlParameter("@SysVersion", SqlDbType.BigInt));
                        odda.Parameters["@SysVersion"].Value = 0;
                        odda.Parameters.Add(new SqlParameter("@BOMDoc", SqlDbType.BigInt));
                        odda.Parameters["@BOMDoc"].Value = getlong(ht_spc["BOMDoc"]);
                        odda.Parameters.Add(new SqlParameter("@OrderNo", SqlDbType.NVarChar));
                        odda.Parameters["@OrderNo"].Value = getstr(ht_spc["OrderNo"]);
                        odda.Parameters.Add(new SqlParameter("@Code", SqlDbType.NVarChar));
                        odda.Parameters["@Code"].Value = getstr(ht_spc["Code"]);
                        //odda.Parameters.Add(new SqlParameter("@Material", SqlDbType.NVarChar));
                        //odda.Parameters["@Rev"].Value = this.txt_Rec.Text;
                        odda.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                        odda.Parameters["@Name"].Value = getstr(ht_spc["Name"]);
                        odda.Parameters.Add(new SqlParameter("@Style", SqlDbType.NVarChar));
                        odda.Parameters["@Style"].Value = getstr(ht_spc["Style"]);
                        odda.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar));
                        odda.Parameters["@Description"].Value = getstr(ht_spc["Description"]);
                        odda.Parameters.Add(new SqlParameter("@ItemForm", SqlDbType.NVarChar));
                        odda.Parameters["@ItemForm"].Value = getstr(ht_spc["ItemForm"]);
                        odda.Parameters.Add(new SqlParameter("@Material", SqlDbType.NVarChar));
                        odda.Parameters["@Material"].Value = getstr(ht_spc["Material"]);
                        odda.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Decimal));
                        odda.Parameters["@Quantity"].Value = getdecimal(ht_spc["Quantity"]);
                        odda.Parameters.Add(new SqlParameter("@Unit", SqlDbType.NVarChar));
                        odda.Parameters["@Unit"].Value = getstr(ht_spc["Unit"]);
                        odda.Parameters.Add(new SqlParameter("@SigWeight", SqlDbType.Decimal));
                        odda.Parameters["@SigWeight"].Value = getdecimal(ht_spc["SigWeight"]);
                        odda.Parameters.Add(new SqlParameter("@TotWeight", SqlDbType.Decimal));
                        odda.Parameters["@TotWeight"].Value = getdecimal(ht_spc["TotWeight"]);
                        odda.Parameters.Add(new SqlParameter("@Factory", SqlDbType.NVarChar));
                        odda.Parameters["@Factory"].Value = getstr(ht_spc["Factory"]);
                        odda.Parameters.Add(new SqlParameter("@Remark", SqlDbType.NVarChar));
                        odda.Parameters["@Remark"].Value = getstr(ht_spc["Remark"]);
                        odda.Parameters.Add(new SqlParameter("@MRPPlan", SqlDbType.Int));
                        odda.Parameters["@MRPPlan"].Value = getint(ht_spc["MRPPlan"]);
                        odda.Parameters.Add(new SqlParameter("@Preid", SqlDbType.Decimal));
                        odda.Parameters["@Preid"].Value = getdecimal(ht_spc["Preid"]);
                        odda.Parameters.Add(new SqlParameter("@Department", SqlDbType.NVarChar));
                        odda.Parameters["@Department"].Value = getstr(ht_spc["Department"]);
                        odda.Parameters.Add(new SqlParameter("@UsingOrg", SqlDbType.NVarChar));
                        odda.Parameters["@UsingOrg"].Value = getstr(ht_spc["UsingOrg"]);
                        odda.Parameters.Add(new SqlParameter("@IssueStyle", SqlDbType.NVarChar));
                        odda.Parameters["@IssueStyle"].Value = getstr(ht_spc["IssueStyle"]);
                        odda.Parameters.Add(new SqlParameter("@DrawingNo", SqlDbType.Int));
                        odda.Parameters["@DrawingNo"].Value = getint(ht_spc["DrawingNo"]);
                        odda.Parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar));
                        odda.Parameters["@Category"].Value = getstr(ht_spc["zhufenlei"]);
                        //odda.Parameters = htsql["spc"] as SqlParameterCollection;
                        odda.Transaction = st;
                        //odda.BeginExecuteNonQuery();
                        odda.ExecuteNonQuery();

                    }
                   
                }

                st.Commit();
                bReturn = 1;
                //log.SQLLog(sSchemaName, sSQLs[i].ToString(), sTerminalID, sTerminalID, sUserID, sFunctionID, "ACTION");                
            }
            catch (System.Exception ex)
            {
                st.Rollback();
                conn.Dispose();
                sReturnCode = "Error";
                sErrorMessage = ex.Message + ex.StackTrace;
                bReturn = -1;
                MessageBox.Show(sErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                
            }
            finally
            {

            }
        }
        public static void updatedtFunc_spc(ArrayList arr_sqls, SqlConnection conn, Dictionary<string, Hashtable> spc, ref string sReturnCode)
        {
            string sErrorMessage = "OK";
            sReturnCode = "OK";
            int bReturn = 0;
            SqlTransaction st = conn.BeginTransaction();
            try
            {
                //st = conn.BeginTransaction(); ;
                //st.Connection = conn;
                foreach (string sqltype in spc.Keys)
                {
                    Hashtable ht_spc = spc[sqltype] as Hashtable;
                    if (sqltype == "BOMDoc")
                    {
                        #region sql
                        string sql = @"update [dbo].[InterFace_BOMDoc]
           set [CreatedOn]=@CreatedOn
           ,[CreatedBy]=@CreatedBy
           ,[ModifiedOn]=@ModifiedOn
           ,[ModifiedBy]=@ModifiedBy
           ,[SysVersion]=@SysVersion
           ,[Rev]=@Rev
           ,[Name]=@Name
           ,[Style]=@Style
           ,[Description]=@Description
           ,[ItemForm]=@ItemForm
           ,[Unit]=@Unit
           ,[Schema_No]=@Schema_No
           ,[EffectiveDate]=@EffectiveDate
           ,[DisableDate]=@DisableDate
           ,[MRPPlan]=@MRPPlan
           ,[Preid]=@Preid
           ,[Department]=@Department
           ,[DrawingNo]=@DrawingNo
           ,[Category]=@Category where ID=@ID";

                        #endregion
                        SqlCommand odda = new SqlCommand(sql, conn);
                        //odda.Parameters.
                        odda.Parameters.Add(new SqlParameter("@ID", SqlDbType.BigInt));
                        odda.Parameters["@ID"].Value = getlong(ht_spc["ID"]);
                        odda.Parameters.Add(new SqlParameter("@CreatedOn", SqlDbType.DateTime));
                        odda.Parameters["@CreatedOn"].Value = Convert.ToDateTime(ht_spc["CreatedOn"]);
                        odda.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar));
                        odda.Parameters["@CreatedBy"].Value = getstr(ht_spc["CreatedBy"]);
                        odda.Parameters.Add(new SqlParameter("@ModifiedOn", SqlDbType.DateTime));
                        odda.Parameters["@ModifiedOn"].Value = Convert.ToDateTime(ht_spc["ModifiedOn"]);
                        odda.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.NVarChar));
                        odda.Parameters["@ModifiedBy"].Value = getstr(ht_spc["ModifiedBy"]);
                        odda.Parameters.Add(new SqlParameter("@SysVersion", SqlDbType.BigInt));
                        odda.Parameters["@SysVersion"].Value = getint(ht_spc["SysVersion"]);
                        //odda.Parameters.Add(new SqlParameter("@Code", SqlDbType.NVarChar));
                        //odda.Parameters["@Code"].Value = getstr(ht_spc["Code"]);
                        odda.Parameters.Add(new SqlParameter("@Rev", SqlDbType.NVarChar));
                        odda.Parameters["@Rev"].Value = getstr(ht_spc["Rev"]);
                        odda.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                        odda.Parameters["@Name"].Value = getstr(ht_spc["Name"]);
                        odda.Parameters.Add(new SqlParameter("@Style", SqlDbType.NVarChar));
                        odda.Parameters["@Style"].Value = getstr(ht_spc["Style"]);
                        odda.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar));
                        odda.Parameters["@Description"].Value = getstr(ht_spc["Description"]);
                        odda.Parameters.Add(new SqlParameter("@ItemForm", SqlDbType.NVarChar));
                        odda.Parameters["@ItemForm"].Value = getstr(ht_spc["ItemForm"]);
                        odda.Parameters.Add(new SqlParameter("@Unit", SqlDbType.NVarChar));
                        odda.Parameters["@Unit"].Value = getstr(ht_spc["Unit"]);
                        //odda.Parameters.Add(new SqlParameter("@Project_No", SqlDbType.NVarChar));
                        //odda.Parameters["@Project_No"].Value = getstr(ht_spc["Project_No"]);
                        odda.Parameters.Add(new SqlParameter("@Schema_No", SqlDbType.NVarChar));
                        odda.Parameters["@Schema_No"].Value = getstr(ht_spc["Schema_No"]);
                        odda.Parameters.Add(new SqlParameter("@EffectiveDate", SqlDbType.DateTime));
                        odda.Parameters["@EffectiveDate"].Value = Convert.ToDateTime(ht_spc["EffectiveDate"]);
                        odda.Parameters.Add(new SqlParameter("@DisableDate", SqlDbType.DateTime));
                        odda.Parameters["@DisableDate"].Value = Convert.ToDateTime(ht_spc["DisableDate"]);
                        odda.Parameters.Add(new SqlParameter("@MRPPlan", SqlDbType.Int));
                        odda.Parameters["@MRPPlan"].Value = getint(ht_spc["MRPPlan"]);
                        odda.Parameters.Add(new SqlParameter("@Preid", SqlDbType.Decimal));
                        odda.Parameters["@Preid"].Value = getdecimal(ht_spc["Preid"]);
                        odda.Parameters.Add(new SqlParameter("@Department", SqlDbType.NVarChar));
                        odda.Parameters["@Department"].Value = getstr(ht_spc["Department"]);
                        odda.Parameters.Add(new SqlParameter("@DrawingNo", SqlDbType.NVarChar));
                        odda.Parameters["@DrawingNo"].Value = getstr(ht_spc["DrawingNo"]);
                        odda.Parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar));
                        odda.Parameters["@Category"].Value = getstr(ht_spc["Category"]);
                        odda.Transaction = st;
                        //odda.BeginExecuteNonQuery();
                        odda.ExecuteNonQuery();
                    }
                    else
                    {
                        DataTable dt_validate = MiddleDBInterface.getdt("select a.id from InterFace_BOMLine a where a.BOMDoc=" + getstr(ht_spc["BOMDoc"]) + " and a.OrderNo='" + getstr(ht_spc["OrderNo"]) + "'", MiddleDBInterface.conn());

                        if (dt_validate.Rows.Count>0)
                        {
                            #region 修改
                            string sql = @"UPDATE [dbo].[InterFace_BOMLine]
           set 
          [CreatedOn]=@CreatedOn
           ,[CreatedBy]=@CreatedBy
           ,[ModifiedOn]=@ModifiedOn
           ,[ModifiedBy]=@ModifiedBy
           ,[SysVersion]=@SysVersion
           ,[Code]=@Code
           ,[Name]=@Name
           ,[Style]=@Style
           ,[Description]=@Description
           ,[ItemForm]=@ItemForm
           ,[Material]=@Material
           ,[Quantity]=@Quantity
           ,[Unit]=@Unit
           ,[SigWeight]=@SigWeight
           ,[TotWeight]=@TotWeight
           ,[Factory]=@Factory
           ,[Remark]=@Remark
           ,[MRPPlan]=@MRPPlan
           ,[Preid]=@Preid
           ,[Department]=@Department
           ,[UsingOrg]=@UsingOrg
           ,[IssueStyle]=@IssueStyle
           ,[DrawingNo]=@DrawingNo,[Category]=@Category where BOMDoc=@BOMDoc and OrderNo=@OrderNo";
                            SqlCommand odda = new SqlCommand(sql, conn);


                            odda.Parameters.Add(new SqlParameter("@CreatedOn", SqlDbType.DateTime));
                            odda.Parameters["@CreatedOn"].Value = DateTime.Now;
                            odda.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar));
                            odda.Parameters["@CreatedBy"].Value = "CAD";
                            odda.Parameters.Add(new SqlParameter("@ModifiedOn", SqlDbType.DateTime));
                            odda.Parameters["@ModifiedOn"].Value = DateTime.Now;
                            odda.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.NVarChar));
                            odda.Parameters["@ModifiedBy"].Value = "CAD";
                            odda.Parameters.Add(new SqlParameter("@SysVersion", SqlDbType.BigInt));
                            odda.Parameters["@SysVersion"].Value = 0;
                            odda.Parameters.Add(new SqlParameter("@BOMDoc", SqlDbType.BigInt));
                            odda.Parameters["@BOMDoc"].Value = getlong(ht_spc["BOMDoc"]);
                            odda.Parameters.Add(new SqlParameter("@OrderNo", SqlDbType.NVarChar));
                            odda.Parameters["@OrderNo"].Value = getstr(ht_spc["OrderNo"]);
                            odda.Parameters.Add(new SqlParameter("@Code", SqlDbType.NVarChar));
                            odda.Parameters["@Code"].Value = getstr(ht_spc["Code"]);
                            //odda.Parameters.Add(new SqlParameter("@Material", SqlDbType.NVarChar));
                            //odda.Parameters["@Rev"].Value = this.txt_Rec.Text;
                            odda.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                            odda.Parameters["@Name"].Value = getstr(ht_spc["Name"]);
                            odda.Parameters.Add(new SqlParameter("@Style", SqlDbType.NVarChar));
                            odda.Parameters["@Style"].Value = getstr(ht_spc["Style"]);
                            odda.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar));
                            odda.Parameters["@Description"].Value = getstr(ht_spc["Description"]);
                            odda.Parameters.Add(new SqlParameter("@ItemForm", SqlDbType.NVarChar));
                            odda.Parameters["@ItemForm"].Value = getstr(ht_spc["ItemForm"]);
                            odda.Parameters.Add(new SqlParameter("@Material", SqlDbType.NVarChar));
                            odda.Parameters["@Material"].Value = getstr(ht_spc["Material"]);
                            odda.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Decimal));
                            odda.Parameters["@Quantity"].Value = getdecimal(ht_spc["Quantity"]);
                            odda.Parameters.Add(new SqlParameter("@Unit", SqlDbType.NVarChar));
                            odda.Parameters["@Unit"].Value = getstr(ht_spc["Unit"]);
                            odda.Parameters.Add(new SqlParameter("@SigWeight", SqlDbType.Decimal));
                            odda.Parameters["@SigWeight"].Value = getdecimal(ht_spc["SigWeight"]);
                            odda.Parameters.Add(new SqlParameter("@TotWeight", SqlDbType.Decimal));
                            odda.Parameters["@TotWeight"].Value = getdecimal(ht_spc["TotWeight"]);
                            odda.Parameters.Add(new SqlParameter("@Factory", SqlDbType.NVarChar));
                            odda.Parameters["@Factory"].Value = getstr(ht_spc["Factory"]);
                            odda.Parameters.Add(new SqlParameter("@Remark", SqlDbType.NVarChar));
                            odda.Parameters["@Remark"].Value = getstr(ht_spc["Remark"]);
                            odda.Parameters.Add(new SqlParameter("@MRPPlan", SqlDbType.Int));
                            odda.Parameters["@MRPPlan"].Value = getint(ht_spc["MRPPlan"]);
                            odda.Parameters.Add(new SqlParameter("@Preid", SqlDbType.Decimal));
                            odda.Parameters["@Preid"].Value = getdecimal(ht_spc["Preid"]);
                            odda.Parameters.Add(new SqlParameter("@Department", SqlDbType.NVarChar));
                            odda.Parameters["@Department"].Value = getstr(ht_spc["Department"]);
                            odda.Parameters.Add(new SqlParameter("@UsingOrg", SqlDbType.NVarChar));
                            odda.Parameters["@UsingOrg"].Value = getstr(ht_spc["UsingOrg"]);
                            odda.Parameters.Add(new SqlParameter("@IssueStyle", SqlDbType.NVarChar));
                            odda.Parameters["@IssueStyle"].Value = getstr(ht_spc["IssueStyle"]);
                            odda.Parameters.Add(new SqlParameter("@DrawingNo", SqlDbType.Int));
                            odda.Parameters["@DrawingNo"].Value = getint(ht_spc["DrawingNo"]);
                            odda.Parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar));
                            odda.Parameters["@Category"].Value = getstr(ht_spc["zhufenlei"]);
                            //odda.Parameters = htsql["spc"] as SqlParameterCollection;
                            odda.Transaction = st;
                            //odda.BeginExecuteNonQuery();
                            odda.ExecuteNonQuery();
                            #endregion
                        }
                        else
                        {
                            #region 创建
                            string sql = @"INSERT INTO [dbo].[InterFace_BOMLine]
           ([ID]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           ,[SysVersion]
           ,[BOMDoc]
           ,[OrderNo]
           ,[Code]
           ,[Name]
           ,[Style]
           ,[Description]
           ,[ItemForm]
           ,[Material]
           ,[Quantity]
           ,[Unit]
           ,[SigWeight]
           ,[TotWeight]
           ,[Factory]
           ,[Remark]
           ,[MRPPlan]
           ,[Preid]
           ,[Department]
           ,[UsingOrg]
           ,[IssueStyle]
           ,[DrawingNo],[Category])
     VALUES(@ID
           ,@CreatedOn
           ,@CreatedBy
           ,@ModifiedOn
           ,@ModifiedBy
           ,@SysVersion
           ,@BOMDoc
           ,@OrderNo
           ,@Code
           ,@Name
           ,@Style
           ,@Description
           ,@ItemForm
           ,@Material
           ,@Quantity
           ,@Unit
           ,@SigWeight
           ,@TotWeight
           ,@Factory
           ,@Remark
           ,@MRPPlan
           ,@Preid
           ,@Department
           ,@UsingOrg
           ,@IssueStyle
,@DrawingNo,@Category)";
                            SqlCommand odda = new SqlCommand(sql, conn);

                            odda.Parameters.Add(new SqlParameter("@ID", SqlDbType.BigInt));
                            odda.Parameters["@ID"].Value = getlong(ht_spc["ID"]);
                            odda.Parameters.Add(new SqlParameter("@CreatedOn", SqlDbType.DateTime));
                            odda.Parameters["@CreatedOn"].Value = DateTime.Now;
                            odda.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar));
                            odda.Parameters["@CreatedBy"].Value = "CAD";
                            odda.Parameters.Add(new SqlParameter("@ModifiedOn", SqlDbType.DateTime));
                            odda.Parameters["@ModifiedOn"].Value = DateTime.Now;
                            odda.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.NVarChar));
                            odda.Parameters["@ModifiedBy"].Value = "CAD";
                            odda.Parameters.Add(new SqlParameter("@SysVersion", SqlDbType.BigInt));
                            odda.Parameters["@SysVersion"].Value = 0;
                            odda.Parameters.Add(new SqlParameter("@BOMDoc", SqlDbType.BigInt));
                            odda.Parameters["@BOMDoc"].Value = getlong(ht_spc["BOMDoc"]);
                            odda.Parameters.Add(new SqlParameter("@OrderNo", SqlDbType.NVarChar));
                            odda.Parameters["@OrderNo"].Value = getstr(ht_spc["OrderNo"]);
                            odda.Parameters.Add(new SqlParameter("@Code", SqlDbType.NVarChar));
                            odda.Parameters["@Code"].Value = getstr(ht_spc["Code"]);
                            //odda.Parameters.Add(new SqlParameter("@Material", SqlDbType.NVarChar));
                            //odda.Parameters["@Rev"].Value = this.txt_Rec.Text;
                            odda.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                            odda.Parameters["@Name"].Value = getstr(ht_spc["Name"]);
                            odda.Parameters.Add(new SqlParameter("@Style", SqlDbType.NVarChar));
                            odda.Parameters["@Style"].Value = getstr(ht_spc["Style"]);
                            odda.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar));
                            odda.Parameters["@Description"].Value = getstr(ht_spc["Description"]);
                            odda.Parameters.Add(new SqlParameter("@ItemForm", SqlDbType.NVarChar));
                            odda.Parameters["@ItemForm"].Value = getstr(ht_spc["ItemForm"]);
                            odda.Parameters.Add(new SqlParameter("@Material", SqlDbType.NVarChar));
                            odda.Parameters["@Material"].Value = getstr(ht_spc["Material"]);
                            odda.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Decimal));
                            odda.Parameters["@Quantity"].Value = getdecimal(ht_spc["Quantity"]);
                            odda.Parameters.Add(new SqlParameter("@Unit", SqlDbType.NVarChar));
                            odda.Parameters["@Unit"].Value = getstr(ht_spc["Unit"]);
                            odda.Parameters.Add(new SqlParameter("@SigWeight", SqlDbType.Decimal));
                            odda.Parameters["@SigWeight"].Value = getdecimal(ht_spc["SigWeight"]);
                            odda.Parameters.Add(new SqlParameter("@TotWeight", SqlDbType.Decimal));
                            odda.Parameters["@TotWeight"].Value = getdecimal(ht_spc["TotWeight"]);
                            odda.Parameters.Add(new SqlParameter("@Factory", SqlDbType.NVarChar));
                            odda.Parameters["@Factory"].Value = getstr(ht_spc["Factory"]);
                            odda.Parameters.Add(new SqlParameter("@Remark", SqlDbType.NVarChar));
                            odda.Parameters["@Remark"].Value = getstr(ht_spc["Remark"]);
                            odda.Parameters.Add(new SqlParameter("@MRPPlan", SqlDbType.Int));
                            odda.Parameters["@MRPPlan"].Value = getint(ht_spc["MRPPlan"]);
                            odda.Parameters.Add(new SqlParameter("@Preid", SqlDbType.Decimal));
                            odda.Parameters["@Preid"].Value = getdecimal(ht_spc["Preid"]);
                            odda.Parameters.Add(new SqlParameter("@Department", SqlDbType.NVarChar));
                            odda.Parameters["@Department"].Value = getstr(ht_spc["Department"]);
                            odda.Parameters.Add(new SqlParameter("@UsingOrg", SqlDbType.NVarChar));
                            odda.Parameters["@UsingOrg"].Value = getstr(ht_spc["UsingOrg"]);
                            odda.Parameters.Add(new SqlParameter("@IssueStyle", SqlDbType.NVarChar));
                            odda.Parameters["@IssueStyle"].Value = getstr(ht_spc["IssueStyle"]);
                            odda.Parameters.Add(new SqlParameter("@DrawingNo", SqlDbType.Int));
                            odda.Parameters["@DrawingNo"].Value = getint(ht_spc["DrawingNo"]);
                            odda.Parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar));
                            odda.Parameters["@Category"].Value = getstr(ht_spc["zhufenlei"]);
                            //odda.Parameters = htsql["spc"] as SqlParameterCollection;
                            odda.Transaction = st;
                            //odda.BeginExecuteNonQuery();
                            odda.ExecuteNonQuery();
                            #endregion
                        }

                    }

                }

                st.Commit();
                bReturn = 1;
                //log.SQLLog(sSchemaName, sSQLs[i].ToString(), sTerminalID, sTerminalID, sUserID, sFunctionID, "ACTION");                
            }
            catch (System.Exception ex)
            {
                st.Rollback();
                conn.Dispose();
                sReturnCode = "Error";
                sErrorMessage = ex.Message + ex.StackTrace;
                bReturn = -1;
                MessageBox.Show(sErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {

            }
        }
        public static void delFunc_spc(ArrayList arr_sqls, SqlConnection conn, Dictionary<string, Hashtable> spc, ref string sReturnCode)
        {
            string sErrorMessage = "OK";
            sReturnCode = "OK";
            int bReturn = 0;
            SqlTransaction st = conn.BeginTransaction();
            try
            {
                //st = conn.BeginTransaction(); ;
                //st.Connection = conn;
                foreach (string sqltype in spc.Keys)
                {
                    Hashtable ht_spc = spc[sqltype] as Hashtable;
                    if (sqltype == "BOMDoc")
                    {
                        #region sql
                        string sql = @"delete [dbo].[InterFace_BOMDoc]
           where ID=@ID";

                        #endregion
                        SqlCommand odda = new SqlCommand(sql, conn);
                        //odda.Parameters.
                        odda.Parameters.Add(new SqlParameter("@ID", SqlDbType.BigInt));
                        odda.Parameters["@ID"].Value = getlong(ht_spc["ID"]);
                        
                        odda.Transaction = st;
                        //odda.BeginExecuteNonQuery();
                        odda.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = @"delete [dbo].[InterFace_BOMLine]
            where BOMDoc=@BOMDoc and OrderNo=@OrderNo";
                        SqlCommand odda = new SqlCommand(sql, conn);


                        
                        odda.Parameters.Add(new SqlParameter("@BOMDoc", SqlDbType.BigInt));
                        odda.Parameters["@BOMDoc"].Value = getlong(ht_spc["BOMDoc"]);
                        odda.Parameters.Add(new SqlParameter("@OrderNo", SqlDbType.NVarChar));
                        odda.Parameters["@OrderNo"].Value = getstr(ht_spc["OrderNo"]);
                        
                        //odda.Parameters = htsql["spc"] as SqlParameterCollection;
                        odda.Transaction = st;
                        //odda.BeginExecuteNonQuery();
                        odda.ExecuteNonQuery();

                    }

                }

                st.Commit();
                bReturn = 1;
                //log.SQLLog(sSchemaName, sSQLs[i].ToString(), sTerminalID, sTerminalID, sUserID, sFunctionID, "ACTION");                
            }
            catch (System.Exception ex)
            {
                st.Rollback();
                conn.Dispose();
                sReturnCode = "Error";
                sErrorMessage = ex.Message + ex.StackTrace;
                bReturn = -1;
                MessageBox.Show(sErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
            finally
            {

            }
        }
    }
}
