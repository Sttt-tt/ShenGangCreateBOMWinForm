using System;
using System.Collections.Generic;
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
            string sql = $@"select A1.Code from  CBO_Category A 
left join cbo_category_trl A1 on A.ID=A1.ID and A1.SysMLFlag='zh-CN' 
            LEFT JOIN CBO_CategoryType A2 ON A2.ID = A1.CategorySystem
            where a.Name = '{name}' and a.org='{orgid}' and A.Effective_IsEffective = 1 AND A2.Code = '01' and A1.DescFlexField_PrivateDescSeg2 != 'true'
            order by a.MainItemCategory,A1.Code";
            object obj = SQLHelper.ExecuteScalar(strConn, sql);
            return DataHelper.getStr(obj);
        }
    }
}
