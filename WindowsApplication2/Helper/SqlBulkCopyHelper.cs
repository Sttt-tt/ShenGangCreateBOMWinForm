using ERP8.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApplication2.Helper
{
    public class SqlBulkCopyHelper
    {
        static string connectionString = ConfigurationManager.AppSettings["Conn"];
        public static void SqlBulkCopyByDatatable(string TableName, DataTable dt)
        {

            string delstr = "delete from " + TableName + " where wbs='" + dt.Rows[0]["WBS"].ToString() + "'";
            SqlHelper.ExecuteNonQuerys(delstr);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (System.Data.SqlClient.SqlBulkCopy sqlbulkcopy = new System.Data.SqlClient.SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
