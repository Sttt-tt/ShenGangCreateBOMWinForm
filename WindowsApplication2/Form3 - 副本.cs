using Newtonsoft.Json;
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
using WindowsApplication2.Helper;
using WindowsApplication2.VO;

namespace WindowsApplication2
{

    public partial class Form3 : Form
    {
        public static string EntID = getstr(Login.u9ContentHt["OrgID"]);//登陆组织ID
        public delegate void form3UserControlValue(string ItemDesc, string CaiZhi, string controlname, string code, string name, string cl, string unit);
        public form3UserControlValue form3UserControls;
        private string form1contorlname3 = "";
        private string itemAttribute = string.Empty;//料品形态属性
        //public string unit = "";
        public string ItemDesc = string.Empty;
        public string CaiZhi = string.Empty;
        //private string pItemCode = string.Empty;//料号
        public Form3(string _pItemCode, string contorlname, string pItemDesc, string caizhi, string _itemAttribute)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.dataGridView1.DataSource = getItemMasters(itemvalue, itemCz);
            form1contorlname3 = contorlname;
            //unit = itemUnit;
            itemAttribute = _itemAttribute;
            initGrid(_pItemCode, pItemDesc, caizhi);
            ItemDesc = pItemDesc;
            CaiZhi = caizhi;
        }


        /// <summary>
        /// 查询数据库里的料品数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DataTable getItemMasters(string item, string itemCz)
        {
            string ItemName = string.IsNullOrEmpty(KeepChinese(item)) ? Regex.Replace(item, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase) : KeepChinese(item);//物料名称
            string ItemSPECS = Regex.Replace(item, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase); //物料规格型号
            DataTable dt = new DataTable();
            string sql = string.Empty;
            sql = string.Format(@"select Code 料号,Name+SPECS 品名,DescFlexField_PrivateDescSeg1 材料 from CBO_ItemMaster where Name like'{0}' and DescFlexField_PrivateDescSeg1 like '{1}' 
                                        and SPECS like'{2}' 
                                        ", ItemName, itemCz, ItemSPECS);
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
            form3UserControls(ItemDesc, CaiZhi, form1contorlname3, row.Cells[0].Value.ToString(), row.Cells["品名"].Value.ToString() + row.Cells["规格"].Value.ToString(), row.Cells["材质"].Value.ToString(), row.Cells["单位"].Value.ToString());
            this.Close();
        }


        /// <summary>
        /// 保留中文字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string KeepChinese(string str)
        {
            //声明存储结果的字符串
            string chineseString = "";


            //将传入参数中的中文字符添加到结果字符串中
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] >= 0x4E00 && str[i] <= 0x9FA5) //汉字
                {
                    chineseString += str[i];
                }
            }


            //返回保留中文的处理结果
            return chineseString;
        }


        private void initGrid(string pItemCode, string pItemDesc, string caizhi)
        {
            string ItemName = string.Empty;
            //国标处理
            if (pItemDesc.StartsWith(pItemCode))
            {
                pItemDesc = pItemDesc.Replace(pItemCode, "");
                ItemName = string.IsNullOrEmpty(KeepChinese(pItemDesc)) ? Regex.Replace(pItemDesc, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase) : KeepChinese(pItemDesc);//物料名称
                ItemName = pItemCode + ItemName;

            }
            else
            {
                ItemName = string.IsNullOrEmpty(KeepChinese(pItemDesc)) ? Regex.Replace(pItemDesc, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase) : KeepChinese(pItemDesc);//物料名称

            }
            string itemSpecs = Regex.Replace(pItemDesc, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase).Trim(); //物料规格型号
            string sql = @"select A.Code 料号,A.Name 品名,A.DescFlexField_PrivateDescSeg1 材质,A.SPECS 规格,A3.Code 单位
            from CBO_ItemMaster A
            left join Base_UOM A3  ON A3.ID=A.InventoryUOM
            where 1=1 and A.Code like 'S%' and A.Name not like '%失效%' and  A.Effective_IsEffective=1 and A.Org=" + EntID + " ";

            sql = sql + " and A.Name='" + ItemName + "'";

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


        private void toolStripButton10_Click(object sender, EventArgs e)
        {

            //if (string.IsNullOrEmpty(toolStripTextBox3.Text))
            //{
            //    MessageBox.Show("请先输入品名");
            //}
            //if (string.IsNullOrEmpty(toolStripTextBox1.Text))
            //{
            //    MessageBox.Show("请先输入物料规格");
            //}

            //if (string.IsNullOrEmpty(toolStripTextBox2.Text))
            //{
            //    MessageBox.Show("请先输入物料材质");
            //}
            string strName = toolStripTextBox3.Text.Trim();
            if (string.IsNullOrEmpty(strName) && string.IsNullOrEmpty(toolStripTextBox1.Text) && string.IsNullOrEmpty(toolStripTextBox2.Text))
            {
                MessageBox.Show("请先输入查询条件");
                return;
            }

            //string sql = string.Format(@"select A.Code 料号,A.Name+A.SPECS 品名,A.DescFlexField_PrivateDescSeg1 材质,A3.Code 单位
            //from CBO_ItemMaster A
            //left join CBO_Category A1 on A.MainItemCategory=A1.ID
            //left join CBO_Category_Trl A2 on A1.ID=A2.ID
            //left join Base_UOM A3  ON A3.ID=A.InventoryUOM
            //where 1=1 and A.Org=" + EntID + " ");


            string sql = string.Format(@"select A.Code 料号,A.Name 品名,A.DescFlexField_PrivateDescSeg1 材质,A.SPECS 规格,A3.Code 单位
            from CBO_ItemMaster A
            left join Base_UOM A3  ON A3.ID=A.InventoryUOM
            where A.Code like 'S%' and A.Name not like '%失效%' and A.Effective_IsEffective=1  and A.Org=" + EntID + " ");

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


        /// <summary>
        /// 添加料品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string itemName = toolStripTextBox3.Text.Trim();//品名
            if (string.IsNullOrEmpty(itemName))
            {
                MessageBox.Show("品名必须输入");
                return;
            }
            //双单位集合 配置文件里设置
            string[] mainCategoryCodeS = System.Configuration.ConfigurationManager.AppSettings["DoubleUOM"].Split(',');


            //公斤是W013  米是L007  PCS就是PCS 
            string strUOMCode = string.Empty;//单位编码
            string strUOMName = this.txtUOMcbx.Text;//单位名称
            switch (strUOMName)
            {
                case "公斤":
                    strUOMCode = "W013";
                    break;
                case "米":
                    strUOMCode = "L007";
                    break;
                default:
                    strUOMCode = "PCS";
                    break;
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

            //自接导入U9的制造件料号，主分类都是上锅产成品，自接部分制造件的分类改为主分类编码为S1112的自接成品
            string specs = toolStripTextBox1.Text.Trim();//规格
            string cz = toolStripTextBox2.Text.Trim();//材质
            decimal zhl = 0;//转换率
                            //首先判断料品对应的主分类是否唯一
            string mainCategoryCode = string.Empty;
            int count = itemAttribute == "制造件" ? 1 : itemMainItemCategoryCount(itemName, ref mainCategoryCode);

            if (count == 0)
            {
                MessageBox.Show("系统中不存在品名" + itemName + "对应的主分类");
                return;
            }
             if (count == 1)
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
                itemInfo.ItemName = itemName;
                itemInfo.Specs = specs;
                itemInfo.CaiZ = cz;
                itemInfo.Zhl = zhl.ToString();
                itemInfo.Unit = itemAttribute == "制造件" ? "PCS" : strUOMCode;
                itemInfo.MainItemCategoryCode = itemAttribute == "制造件" ? "S1112" : mainCategoryCode;
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
                        string sql = string.Format(@"select A.Code 料号,A.Name+A.SPECS 品名,A.DescFlexField_PrivateDescSeg1 材质,A1.Code 单位
                        from CBO_ItemMaster A
						left join Base_UOM  A1 on A.InventoryUOM=A1.ID
						where A.ID={0} and A.Effective_IsEffective=1 and A.Org={1}", itemid, EntID);
                        DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
                        //this.dataGridView1.DataSource = dt;
                        form3UserControls(ItemDesc, CaiZhi, form1contorlname3, Convert.ToString(dt.Rows[0]["料号"]), Convert.ToString(dt.Rows[0]["品名"]), Convert.ToString(dt.Rows[0]["材质"]), Convert.ToString(dt.Rows[0]["单位"]));
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
                        string sql = string.Format(@"select A.Code 料号,A.Name+A.SPECS 品名,A.DescFlexField_PrivateDescSeg1 材质,A1.Code 单位
                        from CBO_ItemMaster A
						left join Base_UOM  A1 on A.InventoryUOM=A1.ID
						where A.ID={0} and A.Effective_IsEffective=1 and A.Org={1}", itemid, EntID);
                        DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
                        //this.dataGridView1.DataSource = dt;
                        form3UserControls(ItemDesc, CaiZhi, form1contorlname3, Convert.ToString(dt.Rows[0]["料号"]), Convert.ToString(dt.Rows[0]["品名"]), Convert.ToString(dt.Rows[0]["材质"]), Convert.ToString(dt.Rows[0]["单位"]));
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("系统中品名" + itemName + "对应的主分类不唯一");
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
            where a.Name='{0}' and A.Effective_IsEffective=1 and a.Org='{1}' AND A2.Code='01' and A1.DescFlexField_PrivateDescSeg2!='true' 
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
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"" + EntCode + "\",\"OrgCode\":\"" + OrgCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"ZJAddForSgcgWinform\"}";
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
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"" + EntCode + "\",\"OrgCode\":\"" + OrgCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"ZJAddForSgcgWinformByZhl\"}";
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


    public class ItemInfo
    {

        public string ItemName { get; set; }
        public string Specs { get; set; }
        public string CaiZ { get; set; }
        public string Unit { get; set; }
        public string Zhl { get; set; }
        public string MainItemCategoryCode { get; set; }
    }
}
