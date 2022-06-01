using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UFSoft.UBF.Util.Context;
using System.ServiceModel;
using UFSoft.UBF.Service;
using www.ufida.org.EntityData;
using UFSoft.UBF.Exceptions;
using System.Windows.Forms;
using System.Data;

namespace UFIDA.U9.Cust.U9CommonAPISv
{
    public class CommonAPI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandtype">操作类型  OperateReceivement：收货 OperateIssueDoc：领料 OperateCompleteRpt：完工 OperateShip：出货 OperatePrdEndChkBill：盘点</param>
        /// <param name="bytes"> 条码各种数据转换后的字节数组</param>
        /// <param name="context"> U9上下文</param>
        /// <param name="IP"> U9服务器IP</param>
        /// <param name="message"> 返回是否成功的提示</param>
        /// <returns>  U9返回的信息转换后的字节数组</returns>
        public static List<UFIDAU9CustCommonAPISVDocDTOData> DOU9Commonsv(Hashtable htcontext,string commandtype,List<string> strs,string str,long lg,List<long> lgs,List<UFIDAU9CustCommonAPISVDocDTOData> docdtos,out string rtnmsg)
        {
            rtnmsg="OK"; 
            List<UFIDAU9CustCommonAPISVDocDTOData> rtndtos = new List<UFIDAU9CustCommonAPISVDocDTOData>();
            try
            {

                
            MessageBase[] message;
            ThreadContext context=CreateContextObj(htcontext);

            UFIDAU9CustCommonAPISVICommonAPISvClient client = new UFIDAU9CustCommonAPISVICommonAPISvClient();
            if (strs == null) strs = new List<string>();
            if (lgs == null) lgs = new List<long>();
            if (docdtos == null) docdtos = new List<UFIDAU9CustCommonAPISVDocDTOData>();
            UFIDAU9CustCommonAPISVDocDTOData[] rtnitems=client.Do(out message,context,commandtype,strs.ToArray(),str,lg,lgs.ToArray(),docdtos.ToArray());
            foreach (UFIDAU9CustCommonAPISVDocDTOData var in rtnitems)
            {
                rtndtos.Add(var);
            }
            return rtndtos;
        }
        catch (Exception ex)
        {

            rtnmsg=GetExceptionMessage(ex);
        }
        return rtndtos;
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
        public static DateTime getdatetime(object obj)
        {
            DateTime lg = DateTime.Now;
            if (obj != null)
            {
                DateTime.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static string getstr(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }
        #endregion
        #region 给上下文信息赋值
        /// <summary>
        /// 给上下文信息赋值
        /// </summary>
        /// <returns></returns>
        private static ThreadContext CreateContextObj(Hashtable htcontext)
        {
            // 实例化应用上下文对象
            ThreadContext thContext = new ThreadContext();

            System.Collections.Generic.Dictionary<object, object> ns = new Dictionary<object, object>();
            ns.Add("OrgID", getstr(htcontext["OrgID"]));  //组织
            ns.Add("OrgCode", getstr(htcontext["OrgCode"]));  //组织
            ns.Add("OrgName", getstr(htcontext["OrgName"]));  //组织
            ns.Add("UserID", getstr(htcontext["UserID"])); //用户
            ns.Add("UserCode", getstr(htcontext["UserCode"])); //用户
            ns.Add("UserName", getstr(htcontext["UserName"])); //用户
            ns.Add("CultureName", "zh-CN");         //语言
            ns.Add("DefaultCultureName", "zh-CN");
            ns.Add("EnterpriseID", getstr(htcontext["EnterpriseID"]));          //企业
            thContext.nameValueHas = ns;

            return thContext;
        }
        #endregion
        public static DataTable ListStrToDataTable(List<string> strs)
        {
            DataTable dt = new DataTable();
            dt.TableName = "RtnDt";
            if (strs.Count > 0)
            {
                foreach (string col in strs[0].Split('|'))
                {
                    dt.Columns.Add(col);
                }
                for (int i = 1; i < strs.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dr[j] = strs[i].Split('|')[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        #region 提取异常信息
        /// <summary>
        /// 提取异常信息
        /// </summary>
        /// <param name="ex"></param>
        private static string GetExceptionMessage(Exception ex)
        {
            string faultMessage = "未知错误，请查看ERP日志！";

            System.TimeoutException timeoutEx = ex as System.TimeoutException;
            if (timeoutEx != null)
            {
                faultMessage = "访问服务超时，请修改配置信息！";
            }
            else
            {
                FaultException<ServiceException> faultEx = ex as FaultException<ServiceException>;
                if (faultEx == null)
                {
                    faultMessage = ex.Message;
                }
                else
                {
                    ServiceException serviceEx = faultEx.Detail;
                    if (serviceEx != null && !string.IsNullOrEmpty(serviceEx.Message)
                        && !serviceEx.Message.Equals("fault", StringComparison.OrdinalIgnoreCase))
                    {
                        // 错误信息在faultEx.Message中，请提取，
                        // 格式为"Fault:料品不能为空，请录入\n 在....."
                        int startIndex = serviceEx.Message.IndexOf(":");
                        int endIndex = serviceEx.Message.IndexOf("\n");
                        if (endIndex == -1)
                            endIndex = serviceEx.Message.Length;
                        if (endIndex > 0 && endIndex > startIndex + 1)
                        {
                            faultMessage = serviceEx.Message.Substring(startIndex + 1, endIndex - startIndex - 1);
                        }
                        else
                        {
                            faultMessage = serviceEx.Message;
                        }
                    }
                }
            }
            return faultMessage;
        }
        #endregion
    }
}
