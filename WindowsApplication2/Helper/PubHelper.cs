using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsApplication2.Helper
{
    class PubHelper
    {
        /// <summary>
        /// 检查是否是国标料号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool chkIsGB(string code)
        {
            bool isGB = false;
            string strGB = System.Configuration.ConfigurationManager.AppSettings["BeginningDrawNums"];
            string[] dtos = strGB.Split(',');
            foreach(string dto in dtos)
            {
                if (code.StartsWith(dto)) return true;
            }


            return isGB;

        }
        /// <summary>
        /// 校验主分类是否双单位
        /// </summary>
        /// <param name="categroyName"></param>
        /// <returns></returns>
        public static bool chkIsDualUOM(string categroyName)
        {
            bool isDualUOM = false;
            string strMainCategoryNames = System.Configuration.ConfigurationManager.AppSettings["DoubleUOM"];
            string[] dtos = strMainCategoryNames.Split(',');
            if (strMainCategoryNames.Contains(categroyName))
                isDualUOM = true;
            return isDualUOM;

        }
        /// <summary>
        /// 判断当前序号是否是末级
        /// </summary>
        /// <param name="curXh"></param>
        /// <param name="xhLst"></param>
        /// <returns></returns>
        public static bool chkIsEnd(string curXh,List<string> xhLst)
        {
            //一级   1
            //二级   1-1
            //三级   1-1/1
            //四级   1-1/1-1
            string[] curXh1S = curXh.Split('-');
            string[] curXh2S = curXh.Split('/');
            string nextXh = string.Empty;//下一级序号
            if (!curXh.Contains("-"))
            {
                //一级   1
                nextXh = curXh + "-1";//下级序号
                if (!xhLst.Contains(nextXh))
                    return true;
            } else if (curXh1S.Length==2 && !curXh.Contains("/"))
            {
                //二级   1-1
                nextXh = curXh + "/1";//下级序号
                if (!xhLst.Contains(nextXh))
                    return true;
            } else if (curXh1S.Length==2 && curXh2S.Length == 2)
            {
                //三级   1-1/1
                nextXh = curXh + "-1";//下级序号
                if (!xhLst.Contains(nextXh))
                    return true;
            } else if (curXh1S.Length == 3 && curXh2S.Length == 2)
            {
                //四级   1-1/1-1
                nextXh = curXh + "/1";//下级序号
                if (!xhLst.Contains(nextXh))
                    return true;
            }

            return false;

        }

        /// <summary>
        /// 获取单位编码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetUOMCode(string name)
        {
            string code = string.Empty;//单位编码
            switch (name)
            {
                case "公斤":
                    code = "W013";
                    break;
                case "米":
                    code = "L007";
                    break;
                case "立方米":
                    code = "B030";
                    break;
                case "立方分米":
                    code = "B031";
                    break;
                default:
                    code = "PCS";
                    break;
            }
            return code;
        }

        public static List<DataGridViewRow> GetSelRows(DataGridView dg,string xh)
        {
            List<DataGridViewRow> GetDelRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dg.Rows)
            {
                string strXH = DataHelper.getStr(row.Cells["序号"].Value);
                if (!strXH.StartsWith(xh))
                    continue;
                GetDelRows.Add(row);
            }
            return GetDelRows;
        }
        /// <summary>
        /// 按分类名称取分类编码
        /// </summary>
        /// <param name="strConn"></param>
        /// <param name="name"></param>
        /// <param name="orgid"></param>
        /// <returns></returns>
        public static string getMainCategroyCodeByName(string strConn,string name,string orgid)
        {
            string sql = $@"select A.Code from  CBO_Category A 
left join cbo_category_trl A1 on A.ID=A1.ID and A1.SysMLFlag='zh-CN' 
            LEFT JOIN CBO_CategoryType A2 ON A2.ID = A.CategorySystem
            where a1.Name = '{name}' and a.org='{orgid}' AND A2.Code = '01' ";
            object obj = SQLHelper.ExecuteScalar(strConn, sql);
            return DataHelper.getStr(obj);
        }
    }
}
