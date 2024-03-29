﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsApplication2.VO;

namespace WindowsApplication2
{
    /// <summary>
    /// 物料修改窗口
    /// </summary>
    public partial class Form2 : Form
    {
        public static string EntID = getstr(Login.u9ContentHt["OrgID"]);//登陆组织ID
        public delegate void form2UserControlValue(string controlname, string code, string name, string unit);
        public form2UserControlValue form2UserControls;
        private string form1contorlname = "";

        public string unit = "";
        public Form2(string contorlname, string itemUnit,string pItemDesc)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.dataGridView1.DataSource = getItemMasters(itemvalue);
            form1contorlname = contorlname;
            unit = itemUnit;
            initGrid(pItemDesc);
        }

        private void initGrid(string pItemDesc)
        {
            string[] temps = pItemDesc.Split('_');
            string caizhi = string.Empty;//材质
            string itemSpecs = string.Empty;//规格
            string name = string.Empty;//品名
            caizhi = temps[1].ToString();
            name = temps[0].ToString();
            if (temps.Length >= 3)
            {
            
                itemSpecs = temps[2].ToString();
            } 
   
            string sql = string.Format(@"select A.Code 料号,A.Name 品名,A.DescFlexField_PrivateDescSeg1 材质,A.SPECS 规格,A3.Code 单位
            from CBO_ItemMaster A
            left join Base_UOM A3  ON A3.ID=A.InventoryUOM
            where 1=1 and A.Effective_IsEffective=1 and A.Org=" + EntID + " ");


            sql = sql + " and A.name='" + name + "'";
            sql = sql + " and A.DescFlexField_PrivateDescSeg1='" + caizhi + "'";



            sql = sql + " and A.SPECS='" + itemSpecs + "'";


            sql = sql + " order by A.Code,A.name,A.SPECS,A.DescFlexField_PrivateDescSeg1,A3.Code";

            //string sql = string.Format(@"select Code 料号,Name+SPECS 品名,DescFlexField_PrivateDescSeg1 材料 from CBO_ItemMaster where Name like '%{0}%' and DescFlexField_PrivateDescSeg1 = '{1}' 
            //                            and SPECS='{2}' group by Code,name,SPECS,DescFlexField_PrivateDescSeg1
            //                            ", toolStripTextBox3.Text, toolStripTextBox2.Text, toolStripTextBox1.Text);

            DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns["单位"].Visible = false;
        }

        /// <summary>
        /// 查询数据库里的料品数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DataTable getItemMasters(string item)
        {
            DataTable dt = new DataTable();
            string sql = string.Empty;
            if (!item.Contains("_") && !string.IsNullOrEmpty(item))
            {
                sql = string.Format(@"select '0000000000'+Code 料号,Name +'_'+DescFlexField_PrivateDescSeg1+'_'+SPECS 品名 from CBO_ItemMaster where Name like '%{0}%' 
                                        ", item);
                dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
                return dt;
            }
            string[] temps = item.Split('_');
            //如果是3段  精确查找
            if (temps.Length >= 3)
            {
                sql = string.Format(@"select '0000000000'+Code 料号,Name +'_'+DescFlexField_PrivateDescSeg1+'_'+SPECS 品名 from CBO_ItemMaster where Name='{0}' and DescFlexField_PrivateDescSeg1 = '{1}' 
                                        and SPECS='{2}'
                                        ", temps[0], temps[1], temps[2]);
            }
            else if (temps.Length == 2)//如果是2段  模糊匹配
            {
                sql = string.Format(@"select '0000000000'+Code 料号,Name +'_'+DescFlexField_PrivateDescSeg1+'_'+SPECS 品名 from CBO_ItemMaster where Name like '%{0}%' and DescFlexField_PrivateDescSeg1 like '%{1}%' 
                                        ", temps[0], temps[1]);
            }
            else
            {
                sql = string.Format(@"select '0000000000'+Code 料号,Name +'_'+DescFlexField_PrivateDescSeg1+'_'+SPECS 品名 from CBO_ItemMaster where Name like '%{0}%' 
                                        ", temps[0]);
            }


            dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            return dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        /// <summary>
        /// 行双击事件,绑定选中值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Currentduoubleclick(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            if (row.Cells[0].Value == null) return;
            form2UserControls(form1contorlname, row.Cells[0].Value.ToString(), row.Cells["品名"].Value.ToString() + '_' + row.Cells["材质"].Value.ToString() + '_' + row.Cells["规格"].Value.ToString(), row.Cells["单位"].Value.ToString());
            this.Close();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            string strName = toolStripTextBox3.Text.Trim();//品名

            if (string.IsNullOrEmpty(strName) && string.IsNullOrEmpty(toolStripTextBox1.Text) && string.IsNullOrEmpty(toolStripTextBox2.Text))
            {
                MessageBox.Show("请先输入查询条件");
                return;
            }

            //string sql = string.Format(@"select '0000000000'+A.Code 料号,A.Name +'_'+A.DescFlexField_PrivateDescSeg1+'_'+A.SPECS 品名,A3.Code 单位
            //from CBO_ItemMaster A
            //left join CBO_Category A1 on A.MainItemCategory=A1.ID
            //left join CBO_Category_Trl A2 on A1.ID=A2.ID
            //left join Base_UOM A3  ON A3.ID=A.InventoryUOM
            //where 1=1 and A.Org=" + EntID + " ");

            string sql = string.Format(@"select A.Code 料号,A.Name 品名,A.DescFlexField_PrivateDescSeg1 材质,A.SPECS 规格,A3.Code 单位
            from CBO_ItemMaster A
            left join Base_UOM A3  ON A3.ID=A.InventoryUOM
            where  A.Code like 'S%' and A.Name not like '%失效%'  and A.Effective_IsEffective=1 and A.Org=" + EntID + " ");

            if (!string.IsNullOrEmpty(strName))
            {
                //sql = sql + " and A.Name='" + toolStripTextBox3.Text + "' and A2.Name='" + toolStripTextBox3.Text + "'";
                sql = sql + " and A.Name like '%" + strName + "%'";
            }
            if (!string.IsNullOrEmpty(toolStripTextBox2.Text))
            {
                sql = sql + " and A.DescFlexField_PrivateDescSeg1 like '%" + toolStripTextBox2.Text + "%'";
            }

            if (!string.IsNullOrEmpty(toolStripTextBox1.Text))
            {
                sql = sql + " and A.SPECS like '%" + toolStripTextBox1.Text + "%'";
            }

            sql = sql + " order by A.Code,A.name,A.SPECS,A.DescFlexField_PrivateDescSeg1,A3.Code";

            //string sql = string.Format(@"select Code 料号,Name+SPECS 品名,DescFlexField_PrivateDescSeg1 材料 from CBO_ItemMaster where Name like '%{0}%' and DescFlexField_PrivateDescSeg1 = '{1}' 
            //                            and SPECS='{2}' group by Code,name,SPECS,DescFlexField_PrivateDescSeg1
            //                            ", toolStripTextBox3.Text, toolStripTextBox2.Text, toolStripTextBox1.Text);

            DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns["单位"].Visible = false;

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string itemcode = toolStripTextBox3.Text.Trim();//品名
            if (string.IsNullOrEmpty(itemcode))
            {
                MessageBox.Show("品名必须输入");
                return;
            }

            //1、钢管转化率：M = 0.0246615 * (D - t) * t
            //M: 质量(Kg / m)
            //D: 外径(mm)
            //t: 壁厚(mm)
            //例如钢管φ51 * 5转化率为：0.0246615 *（51 - 5）*5 = 5.672145 Kg / m

            //2、扁钢转化率：M = t * B * 7.85 * 10 ^ (-3)
            //M: 质量(Kg / m)
            //t: 厚度(mm)
            //B：宽度(mm)
            //例如扁钢t6 * 20转化率为：6 * 20 * 7.85 * 10 ^ (-3) = 0.942Kg / m


            string specs = toolStripTextBox1.Text.Trim();//规格
                string cz = toolStripTextBox2.Text.Trim();//材质
                decimal zhl = 0;//转换率
                //首先判断料品对应的主分类是否唯一
                string mainItemCategoryCode = string.Empty;
                int count = itemMainItemCategoryCount(itemcode, ref mainItemCategoryCode);

                if (count == 0)
                {
                    MessageBox.Show("系统中不存在品名" + itemcode + "对应的主分类");
                }
                else if (count == 1)
                {
                    //if (itemcode == "钢管" && specs.Contains("φ"))
                    //{
                    //    string[] s = specs.Trim().Split('φ');
                    //    string[] s1 = s[1].Split('*');
                    //    if (s1.Count() == 2)
                    //    {
                    //        zhl = Convert.ToDecimal(0.0246615) * (Convert.ToDecimal(s1[0]) - Convert.ToDecimal(s1[1])) * Convert.ToDecimal(s1[1]);
                    //    }
                    //}
                    //if (itemcode == "扁钢" && specs.Contains("*"))
                    //{
                    //    specs = specs.Replace("t", "");
                    //    string[] s = specs.Split('*');
                    //    zhl = Convert.ToDecimal(s[0]) * Convert.ToDecimal(s[1]) * Convert.ToDecimal(7.85) * Convert.ToDecimal(0.001);
                    //}
                    if (!string.IsNullOrEmpty(toolStripTextBox4.Text))
                    {
                        zhl = Convert.ToDecimal(toolStripTextBox4.Text);
                    }
                    ItemInfo itemInfo = new ItemInfo();
                    itemInfo.ItemName = itemcode;
                    itemInfo.Specs = specs;
                    itemInfo.CaiZ = cz;
                    itemInfo.Zhl = zhl.ToString();
                    itemInfo.Unit = unit;
                    itemInfo.MainItemCategoryCode = mainItemCategoryCode;
                    string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(itemInfo);
                    if (zhl > 0)
                    {
                        string msg = ZJPostCreatItemByZhl(strJson);
                        msg = msg.Substring(7);
                        msg = msg.Remove(msg.Length - 3, 3);
                        msg = msg.Replace(@"\", string.Empty);
                        ResultModelForZJ resultModelForZJ = JsonConvert.DeserializeObject<ResultModelForZJ>(msg);
                        if (resultModelForZJ.Msg == "fail")
                        {
                            MessageBox.Show(resultModelForZJ.Error);
                        }
                        else
                        {
                            string itemid = resultModelForZJ.ItemCode;
                            string sql = string.Format(@"select A.Code 料号,A.Name +'_'+A.DescFlexField_PrivateDescSeg1+'_'+A.SPECS 品名,A1.Code 单位
                            from CBO_ItemMaster A
						    left join Base_UOM  A1 on A.InventoryUOM=A1.ID
						    where A.ID={0} and A.Org={1}", itemid, EntID);
                            DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
                            //this.dataGridView1.DataSource = dt;
                            form2UserControls(form1contorlname, Convert.ToString(dt.Rows[0]["料号"]), Convert.ToString(dt.Rows[0]["品名"]), Convert.ToString(dt.Rows[0]["单位"]));
                            this.Close();
                        }
                    }
                    else
                    {
                        string msg = ZJPostCreatItem(strJson);
                        //{"d":"[{\"Error\":\"主分类名称不等边角钢不存在\",\"Result\":0,\"Method\":null,\"Msg\":\"fail\",\"ItemCode\":\"\"}]"}
                        msg = msg.Substring(7);
                        msg = msg.Remove(msg.Length - 3, 3);
                        msg = msg.Replace(@"\", string.Empty);
                        ResultModelForZJ resultModelForZJ = JsonConvert.DeserializeObject<ResultModelForZJ>(msg);
                        if (resultModelForZJ.Msg == "fail")
                        {
                            MessageBox.Show(resultModelForZJ.Error);
                        }
                        else
                        {
                            string itemid = resultModelForZJ.ItemCode;
                            string sql = string.Format(@"select '0000000000'+A.Code 料号,A.Name +'_'+A.DescFlexField_PrivateDescSeg1+'_'+A.SPECS 品名,A1.Code 单位
                        from CBO_ItemMaster A
						left join Base_UOM  A1 on A.InventoryUOM=A1.ID
						where A.ID={0} and A.Org={1}", itemid, EntID);
                            DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
                            //this.dataGridView1.DataSource = dt;
                            form2UserControls(form1contorlname, Convert.ToString(dt.Rows[0]["料号"]), Convert.ToString(dt.Rows[0]["品名"]), Convert.ToString(dt.Rows[0]["单位"]));
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("系统中品名" + itemcode + "对应的主分类不唯一");
                }
           
        }


        /// <summary>
        /// 获取当前料品的主分类是否唯一
        /// </summary>
        /// <returns></returns>
        public static int itemMainItemCategoryCount(string name, ref string Code)
        {
            int count = 0;
            string str = string.Format(@"select distinct a.MainItemCategory,A1.Code from  CBO_ItemMaster  A
            left join CBO_Category A1 on a1.ID=a.MainItemCategory
            LEFT JOIN CBO_CategoryType A2 ON A2.ID=A1.CategorySystem
            where a.Name='{0}' and a.Org='{1}' AND A2.Code='01' and A1.DescFlexField_PrivateDescSeg2!='true' 
            order by a.MainItemCategory,A1.Code", name, EntID);
            DataTable dt = MiddleDBInterface.getdt(str, SQLHelper.sqlconn(Login.strConn));
            if (dt.Rows.Count == 0)
            {
                count = 0;
            }
            else if (dt.Rows.Count == 1)
            {
                count = 1;
                Code = Convert.ToString(dt.Rows[0]["Code"]);
            }
            else
            {
                count = dt.Rows.Count;
            }
            return count;
        }


        /// <summary>
        /// 创建料品
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ZJPostCreatItem(string str)
        {
            string url = commUntil.GetConntionSetting("U9API");
            //string url = "http://localhost/U9/RestServices/YY.U9.Cust.APISV.IMainSV.svc/DO";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //str = "" + str.Replace("\"", "\\\"") + "";
            str = ReplaceString(str);
            string OrgCode = getstr(Login.u9ContentHt["OrgCode"]);//上下文组织编码
            string UserCode = getstr(Login.u9ContentHt["UserCode"]);//上下文用户编码
            string EntCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseID"];//企业编码
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\""+EntCode+"\",\"OrgCode\":\"" + OrgCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"ZJAddForSgcgWinform\"}";
            //body.Replace("strorg", getstr(Login.u9ContentHt["OrgCode"]));
            //body.Replace("struser", getstr(Login.u9ContentHt["UserCode"]));
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        ///转换率不为空的料品创建,双单位
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ZJPostCreatItemByZhl(string str)
        {
            string url = commUntil.GetConntionSetting("U9API");
            //string url = "http://localhost/U9/RestServices/YY.U9.Cust.APISV.IMainSV.svc/DO";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //str = "" + str.Replace("\"", "\\\"") + "";
            str = ReplaceString(str);
            string OrgCode = getstr(Login.u9ContentHt["OrgCode"]);//上下文组织编码
            string UserCode = getstr(Login.u9ContentHt["UserCode"]);//上下文用户编码
            string EntCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseID"];//企业编码
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\""+EntCode+"\",\"OrgCode\":\"" + OrgCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"ZJAddForSgcgWinformByZhl\"}";
            //body.Replace("strorg", getstr(Login.u9ContentHt["OrgCode"]));
            //body.Replace("struser", getstr(Login.u9ContentHt["UserCode"]));
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }


        /// <summary>
        ///   替换部分字符串
        /// </summary>
        /// <param name="sPassed">需要替换的字符串</param>
        /// <returns></returns>
        public static string ReplaceString(string JsonString)
        {
            if (JsonString == null) { return JsonString; }
            if (JsonString.Contains("\\"))
            {
                JsonString = JsonString.Replace("\\", "\\\\");
            }
            if (JsonString.Contains("\'"))
            {
                JsonString = JsonString.Replace("\'", "\\\'");
            }
            if (JsonString.Contains("\""))
            {
                JsonString = JsonString.Replace("\"", "\\\"");
            }
            //去掉字符串的回车换行符
            JsonString = Regex.Replace(JsonString, @"[\n\r]", "");
            JsonString = JsonString.Trim();
            return JsonString;
        }


        #region <<防止NULL异常>>

        private static string getstr(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }


        private long getlong(object obj)
        {
            long lg = 0;
            if (obj != null)
            {
                long.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        private bool getbool(object obj)
        {
            bool lg = false;
            if (obj != null)
            {
                bool.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        private decimal getdecimal(object obj)
        {
            decimal lg = 0;
            if (obj != null)
            {
                decimal.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        private int getint(object obj)
        {
            int lg = 0;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        private int getgiftint(object obj)
        {
            int lg = 0;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out lg);
            }
            else
            {
                lg = -1;
            }
            return lg;
        }


        /// <summary>  
        /// 判断DS是否为空  
        /// </summary>  
        /// <param name="ds">需要判断的ds</param>  
        /// <returns>如果ds为空，返回true</returns>  
        private bool JudgeDs(DataSet ds)
        {
            bool Flag = false;
            if ((ds == null) || (ds.Tables.Count == 0) || (ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 0))
            {
                Flag = true;
            }
            return Flag;
        }

        #endregion


        /// <summary>
        /// 品名改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox3_TextChanged(object sender, EventArgs e)
        {
            string itemcode = toolStripTextBox3.Text.Trim();//品名
            string specs = toolStripTextBox1.Text.Trim().Replace("M", "").Replace("W", "");//规格
            string strWidth = string.Empty;//宽度
            if (itemcode == "钢管" && !string.IsNullOrEmpty(specs))
            {
                if (specs.Contains("φ"))
                {
                    string[] s = specs.Trim().Split('φ');
                    string[] s1 = s[1].Split('*');
                    if (s1.Count() == 2)
                    {
                        strWidth = s1[1].ToString().TrimEnd('.');
                        toolStripTextBox4.Text = Convert.ToString(Math.Round(Convert.ToDecimal(0.0246615) * (Convert.ToDecimal(s1[0]) - Convert.ToDecimal(strWidth)) * Convert.ToDecimal(strWidth), 4));
                    }
                }
            }
            if (itemcode == "扁钢" && !string.IsNullOrEmpty(specs))
            {
                if (specs.Contains("*"))
                {
                    specs = specs.Replace("t", "");
                    string[] s = specs.Split('*');
                    if (s.Count() == 2)
                    {
                        strWidth = s[1].ToString().TrimEnd('.');
                        toolStripTextBox4.Text = Convert.ToString(Math.Round(Convert.ToDecimal(s[0]) * Convert.ToDecimal(strWidth) * Convert.ToDecimal(7.85) * Convert.ToDecimal(0.001), 4));
                    }
                }
            }

        }

        /// <summary>
        /// 规格改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            string itemcode = toolStripTextBox3.Text.Trim();//品名
            string specs = toolStripTextBox1.Text.Trim().Replace("M", "").Replace("W", "");//规格
            string strWidth = string.Empty;//宽度
            if (specs.Contains("φ") && !string.IsNullOrEmpty(itemcode))
            {
                if (itemcode == "钢管")
                {
                    string[] s = specs.Trim().Split('φ');
                    string[] s1 = s[1].Split('*');
                    if (s1.Count() == 2)
                    {
                        strWidth = s1[1].ToString().TrimEnd('.');
                        toolStripTextBox4.Text = Convert.ToString(Math.Round(Convert.ToDecimal(0.0246615) * (Convert.ToDecimal(s1[0]) - Convert.ToDecimal(strWidth)) * Convert.ToDecimal(strWidth), 4));
                    }
                }
            }
            if (specs.Contains("*") && !string.IsNullOrEmpty(itemcode))
            {
                if (itemcode == "扁钢")
                {
                    specs = specs.Replace("t", "");
                    string[] s = specs.Split('*');
                    if (s.Count() == 2)
                    {
                        strWidth = s[1].ToString().TrimEnd('.');
                        toolStripTextBox4.Text = Convert.ToString(Math.Round(Convert.ToDecimal(s[0]) * Convert.ToDecimal(strWidth) * Convert.ToDecimal(7.85) * Convert.ToDecimal(0.001), 4));
                    }
                }
            }
        }

        
    }


    //public class ItemInfo
    //{

    //    public string ItemName { get; set; }
    //    public string Specs { get; set; }
    //    public string CaiZ { get; set; }
    //    public string Unit { get; set; }
    //    public string Zhl { get; set; }
    //    public string MainItemCategoryCode { get; set; }
    //}
}
