using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using FlexCell;
using System.Threading;
using www.ufida.org.EntityData;
using System.Text.RegularExpressions;
using RestSharp;
using WindowsApplication2.VO;
using WindowsApplication2.Helper;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;
using ERP8.Common;

namespace WindowsApplication2
{
    public partial class Form1 : Form
    {
        string EntCode = getstr(Login.u9ContentHt["OrgCode"]);//上下文组织编码
        private static string connectionString = ConfigurationManager.AppSettings["Conn"];

        public delegate void AsynUpdateUI(string controlname, string valuse);
        public static string controlname = "";

        public delegate void AsynUpdateUI2(string controlname, string valuse);
        public static string controlname3 = "";
        public Form1()
        {

            InitializeComponent();
            //this.tabControl1.Visible = false;
            //this.tabControl1.TabPages.Remove(this.tabPage2);
            this.comboBox1.Visible = false;
            //this.tabControl1.TabPages.AddRange(this.tabPage1);
            toolStripComboBox1.ComboBox.Text = "";
            toolStripComboBox2.ComboBox.Text = "";
            //string str = "select wbs from cust_bomsg_data where wbs not in(select A1.Code from CBO_BOMMaster  A left join CBO_ItemMaster  A1 on A.ItemMaster=A1.ID left join Base_Organization A2 on A.Org=A2.ID where A2.Code='" + EntCode + "') group by wbs                                        ";
            string str = "select wbs from cust_bomsg_data group by wbs";
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, str);

            if (!JudgeDs(ds))
            {
                toolStripComboBox1.ComboBox.Items.Add("上锅物料清单查询");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    toolStripComboBox1.ComboBox.Items.Add(dr["wbs"].ToString());
                }
            }
            else
            {
                toolStripComboBox1.ComboBox.Items.Add("上锅物料清单查询");
            }

            //string str2 = "select wbs from cust_bomzj_data where wbs not in(select A1.Code from CBO_BOMMaster  A left join CBO_ItemMaster  A1 on A.ItemMaster=A1.ID left join Base_Organization A2 on A.Org=A2.ID where A2.Code='" + EntCode + "') group by wbs                                        ";

            string str2 = "select wbs from cust_bomzj_data group by wbs";
            DataSet ds2 = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, str2);

            if (!JudgeDs(ds2))
            {
                toolStripComboBox2.ComboBox.Items.Add("自接物料清单查询");
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    toolStripComboBox2.ComboBox.Items.Add(dr["wbs"].ToString());
                }
            }
            else
            {
                toolStripComboBox2.ComboBox.Items.Add("自接物料清单查询");
            }

            toolStripComboBox1.ComboBox.SelectedIndex = 0;
            toolStripComboBox2.ComboBox.SelectedIndex = 0;
        }
        private void ShowProgressForm()
        {
            string val = "";
            ProgressFrom p = new ProgressFrom(val);
            p.ShowDialog();
        }
        private DataTable changedt_bom(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {

                if ((getint(dt.Rows[i][0]) <= 0 || getstr(dt.Rows[i][1]).Trim() == "") && (getstr(dt.Rows[i][0]) != "序号" && getstr(dt.Rows[i][1]) != "层级"))
                {
                    dt.Rows.RemoveAt(i);
                }

            }
            if (dt.Columns.Count == 18)
            {
                for (int j = dt.Columns.Count - 1; j >= 0; j--)
                {
                    if (j == 0) dt.Columns.RemoveAt(j);
                }
            }
            else
            {
                for (int j = dt.Columns.Count - 1; j >= 0; j--)
                {
                    if (j == 0 || j >= 18) dt.Columns.RemoveAt(j);
                }
            }
            return dt;
        }
        private DataTable changedt_routing(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if ((this.getint(dt.Rows[i][0]) <= 0 || getstr(dt.Rows[i][1]) == "") && getstr(dt.Rows[i][0]) != "序号" && getstr(dt.Rows[i][1]) != "存货编码")
                {
                    dt.Rows.RemoveAt(i);
                }
            }
            if (dt.Columns.Count == 22)
            {
                for (int j = dt.Columns.Count - 1; j >= 0; j--)
                {
                    if (j == 0)
                    {
                        dt.Columns.RemoveAt(j);
                    }
                }
            }
            else
            {
                for (int j = dt.Columns.Count - 1; j >= 0; j--)
                {
                    if (j == 0 || j > 17)
                    {
                        dt.Columns.RemoveAt(j);
                    }
                }
            }
            return dt;
        }
        private FlexCell.Grid grid1 = new FlexCell.Grid();

        DataTable dtsg = new DataTable();//上锅物料清单datatable


        #region <<按钮事件集合>>
        /// <summary>
        /// 上锅物料清单导入按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //设置tabpage text
            if (this.tabPage1.Text != "物料清单数据")
            {
                this.tabPage1.Text = "物料清单数据";
                this.tabPage1.Refresh();
            }
            //打开文件

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl文件 (*.xls)|*.xls|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "导出文件保存路径";//为Excel
            openFileDialog.FileName = null;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;


            Stream myStream;
            try
            {
                myStream = FileToStream(openFileDialog.FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("文件 " + openFileDialog.FileName + " 正由另一起程使用，\r\n请先关闭该进程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //DataTable dt = ReadExcelToDataSet.ImportDataTableFromExcel(myStream, 0, 3, true);
            ExcelHelper excelHelper = new ExcelHelper(openFileDialog.FileName);
            DataTable dt = excelHelper.ExcelToDataTable2(0);
          
            dataGridView1.DataSource = dt;
            //for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
            //{
            //    string bitem = getstr(dataGridView1.Rows[iRow].Cells["母件料品"].Value);
            //    string item = getstr(dataGridView1.Rows[iRow].Cells["物料编码"].Value);
            //    if (iRow >= 1)
            //    {
            //        string sbitem = getstr(dataGridView1.Rows[iRow - 1].Cells["母件料品"].Value);
            //        string sitem = getstr(dataGridView1.Rows[iRow - 1].Cells["物料编码"].Value);
            //        if (bitem == sbitem && item == sitem)
            //        {
            //            dataGridView1.Rows.Remove(dataGridView1.Rows[iRow]);
            //            iRow = iRow - 1;
            //        }
            //    }
            //}
           
            initDataGrid(dataGridView1);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string rtnmsg = "";
            List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> rtnlst = UFIDA.U9.Cust.U9CommonAPISv.CommonAPI.DOU9Commonsv(Login.u9ContentHt, "Test", null, "", 0, null, null, out rtnmsg);
            if (rtnmsg == "OK")
            {
                msg(rtnlst[0].m_rtnStr);
            }
            else
            {
                msg(rtnmsg);
            }

            //dataGridView2.DataSource = dt;
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string CommandType = "Routing";
            List<string> strs = new List<string>();




            string strKey = string.Empty;
            ArrayList lst1 = new ArrayList();//制造路线集合



        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.tabPage1.Text != "物料清单数据")
            {
                MessageBox.Show("导入数据无效");
                return;
            }


            Thread TD = new Thread(ShowProgressForm);
            TD.Start();

            try
            {
                DataGridView dg = dataGridView1;
                if (dg == null || dg.Rows.Count <= 0) return;

                //第一步  DataTalbe转BOM结构
                string strJson = GetBOMJson(dg);

                Hashtable damicht = new Hashtable();
                List<string> MasterItemMasters = new List<string>(); ///所有需要创建的bom母件集合


                string rtnmsg = PostCreatBom(strJson);

                TD.Abort();
                if (rtnmsg != "{\"d\":\"\"}")
                {
                   
                    if (rtnmsg.Contains("项目已存在"))
                    {
                        return;
                    }
                    {
                        msg("创建失败：" + rtnmsg);
                    }
                }
                else
                {
                    List<string> pInvCodeLst = new List<string>();//母件集合
                    Dictionary<string, long> dict = new Dictionary<string, long>();
                    dict.Add("采购件", 0);
                    dict.Add("制造件", 0);
                    dict.Add("虚拟", 0);
                    dict.Add("工艺", 0);
                    List<string> itemCodeLst = new List<string>();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {

                        string itemCode = DataHelper.getStr(row.Cells["物料编码"].Value);
                        if (itemCodeLst.Contains(itemCode)) continue;
                        itemCodeLst.Add(itemCode);


                        string itemAttribute = DataHelper.getStr(row.Cells["料品形态属性"].Value);
                        if (dict.ContainsKey(itemAttribute))
                            dict[itemAttribute] = dict[itemAttribute] + 1;
                        
                        string strGrade = DataHelper.getStr(row.Cells["展开层"].Value);
                        if (strGrade == "2")
                        {
                            string strPInvCode = DataHelper.getStr(row.Cells["母件料品"].Value);
                            if (pInvCodeLst.Contains(strPInvCode)) continue;
                            pInvCodeLst.Add(strPInvCode);
                        }

                    }
                    long zhizaoTotalNum = dict["制造件"] + pInvCodeLst.Count;//制造总数
                    msg("创建bom成功,共导入制造件" + zhizaoTotalNum + ",采购件" + dict["采购件"] + ",虚拟件" + dict["虚拟"] + ",工艺件" + dict["工艺"]);

                }
            }
            catch (Exception ex)
            {
                msg(ex.Message);
                TD.Abort();
            }


        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl文件 (*.xls)|*.xls|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "导出文件保存路径";//为Excel
            openFileDialog.FileName = null;
            //openFileDialog.ShowDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = FileToStream(openFileDialog.FileName);

                DataTable dt = ReadExcelToDataSet.ImportDataTableFromExcel(myStream, 0, 3, false);
                if (dt == null || dt.Rows.Count == 0) return;

            }
        }
        #endregion





        /// <summary> 
        /// 从文件读取 Stream 
        /// </summary> 
        public Stream FileToStream(string fileName)
        {
            // 打开文件 
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// 单元格样式设置
        /// </summary>
        /// <param name="dg"></param>
        private void initDataGrid(DataGridView dg)
        {
            dg.Columns["序号"].Width = 50;
            dg.Columns["展开层"].Width = 60;
            dg.Columns["物料描述"].Width = 200;
            dg.Columns["基本计量单位"].Width = 80;
            dg.Columns["母件物料描述"].Visible = false;
            dg.Columns["BOM用途"].Visible = false;
            dg.Columns["物料类型"].Visible = false;
            dg.Columns["WBS"].Visible = false;
            //dg.Columns["母件用量"].Visible = false;
        }

        private void msg(string message)
        {
            MessageBox.Show(message);
        }



        #region <<拼接数据方法和调用U9创建BOM接口>>
        /// <summary>
        /// 获取BOM结构数据
        /// author yfj,on 2022-06-02
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        private string GetBOMJson(DataGridView dg)
        {
            string strJson = string.Empty;
            List<BomVO> dtos = new List<BomVO>();
            DataTable dt = dg.DataSource as DataTable;
            foreach (DataGridViewRow row in dg.Rows)
            {
                string pInvCode = DataHelper.getStr(row.Cells["母件料品"].Value);
                string pInvDesc = DataHelper.getStr(row.Cells["母件物料描述"].Value);
                string pInvUnit = DataHelper.getStr(row.Cells["母件基本计量单位"].Value);
                string pInvQty = DataHelper.getStr(row.Cells["母件用量"].Value);
                BomVO dto = dtos.Find(t => t.itemcode.Equals(pInvCode));
                if (dto == null)
                {
                    dto = new BomVO();
                    dto.itemcode = pInvCode;
                    dto.itemdesc = pInvDesc;
                    dto.unit = pInvUnit;
                    dto.qty = pInvQty;
                    DataRow[] selRows = dt.Select("物料编码='" + pInvCode + "'");
                    dto.formAttribute = selRows.Length <= 0 ? "制造件" : DataHelper.getStr(selRows[0]["料品形态属性"]);
                    dto.private2 = selRows.Length <= 0 ? "" : DataHelper.getStr(selRows[0]["制造路线"]);
                    dto.private3 = selRows.Length <= 0 ? "" : DataHelper.getStr(selRows[0]["备注"]);
                    dto.rows.Add(new BomLineVO(row));
                    dtos.Add(dto);
                }
                else
                {
                    dto.rows.Add(new BomLineVO(row));
                }
            }
            strJson = Newtonsoft.Json.JsonConvert.SerializeObject(dtos);//转json字符串
            return strJson;
        }


        /// <summary>
        ///自接获取BOM结构数据
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        private string GetZJBOMJson(DataGridView dg)
        {
            string strJson = string.Empty;
            List<BomVOZJ> dtos = new List<BomVOZJ>();
            DataTable dt = dg.DataSource as DataTable;

            foreach (DataGridViewRow row in dg.Rows)
            {
                string pInvCode = DataHelper.getStr(row.Cells["母件料品"].Value);
               // string pInvDesc = DataHelper.getStr(row.Cells["母件物料描述"].Value);
                string pInvDesc = DataHelper.getStr(row.Cells["母件描述"].Value);
                string pInvUnit = DataHelper.getStr(row.Cells["母件基本计量单位"].Value);
                string pInvQty = DataHelper.getStr(row.Cells["母件用量"].Value);
                string material = DataHelper.getStr(row.Cells["母件材料"].Value);
                BomVOZJ dto = dtos.Find(t => t.itemcode.Equals(pInvCode));
                if (dto == null)
                {
                    dto = new BomVOZJ();
                    dto.itemcode = pInvCode;
                    dto.gbbm= PubHelper.chkIsGB(pInvCode) ? pInvCode.Split('(')[0] : "";//国标编码
                    dto.unit = pInvUnit;
                    dto.material = material;
                    
                    dto.qty = pInvQty;
                    DataRow[] selRows = dt.Select("物料编码='" + pInvCode + "'");
                    dto.itemdesc = selRows.Length <= 0 ? pInvDesc : DataHelper.getStr(selRows[0]["子件描述"]);;
                    dto.formAttribute = selRows.Length <= 0 ? "制造件" : DataHelper.getStr(selRows[0]["料品形态属性"]);

                    //dto.private2 = DataHelper.getStr(row.Cells["工艺路线"].Value);
                    dto.private2 = selRows.Length <= 0 ? "" : DataHelper.getStr(selRows[0]["制造路线"]);
                    dto.private3 = selRows.Length <= 0 ? "" : DataHelper.getStr(selRows[0]["备注"]);
                    dto.rows.Add(new BomLineVOZJ(row));
                    dtos.Add(dto);
                }
                else
                {
                    dto.rows.Add(new BomLineVOZJ(row));
                }
            }
            strJson = Newtonsoft.Json.JsonConvert.SerializeObject(dtos);//转json字符串
            return strJson;
        }


        /// <summary>
        /// 调用U9Bom导入接口
        /// </summary>
        /// <param name="str"></param>
        public static string PostCreatBom(string str)
        {
            string url = commUntil.GetConntionSetting("U9API");
            //string url = "http://localhost/U9/RestServices/YY.U9.Cust.APISV.IMainSV.svc/DO";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            str = "" + str.Replace("\"", "\\\"") + "";
            string OrgCode = getstr(Login.u9ContentHt["OrgCode"]);//上下文组织编码
            string UserCode = getstr(Login.u9ContentHt["UserCode"]);//上下文用户编码
            string EntCode = System.Configuration.ConfigurationManager.AppSettings["EnterpriseID"];//企业编码
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"" + EntCode + "\",\"OrgCode\":\"" + OrgCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"CreateBom\"}";
            //body.Replace("strorg", getstr(Login.u9ContentHt["OrgCode"]));
            //body.Replace("struser", getstr(Login.u9ContentHt["UserCode"]));
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }


        public static string ZJPostCreatBom(string str)
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
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"" + EntCode + "\",\"OrgCode\":\"" + OrgCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"ZJCreateBom\"}";
            //body.Replace("strorg", getstr(Login.u9ContentHt["OrgCode"]));
            //body.Replace("struser", getstr(Login.u9ContentHt["UserCode"]));
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        #endregion



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
        #region <<comboBox1事件集合>>
        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(comboBox1.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
                dataGridView1.CurrentCell.Value = comboBox1.Items[comboBox1.SelectedIndex];
            string zjItemCode = dataGridView1[7, dataGridView1.CurrentCell.RowIndex].Value?.ToString();
            for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
            {
                if (zjItemCode == getstr(dataGridView1.Rows[iRow].Cells["物料编码"].Value))
                {
                    dataGridView1.Rows[iRow].Cells["料品形态属性"].Value = dataGridView1.CurrentCell.Value;
                }
            }
        }
        #endregion



        public static DataTable DataTable2(DataTable dt)
        {
            DataView dataView = new DataView(dt);
            string[] columnNames = new string[] { "母件料品", "物料编码" };
            DataTable dt2 = dataView.ToTable(true, "母件料品");
            return dt2;
        }

        /// <summary>
        /// datatable去重
        /// </summary>
        /// <param name="dtSource">需要去重的datatable</param>
        /// <returns></returns>
        public static DataTable GetDistinctTable(DataTable dtSource)
        {
            DataTable distinctTable = null;
            try
            {
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    string[] columnNames = GetTableColumnName(dtSource);
                    DataView dv = new DataView(dtSource);
                    distinctTable = dv.ToTable(true, columnNames);
                }
            }
            catch (Exception ee)
            {
            }
            return distinctTable;
        }



        public static string[] GetTableColumnName(DataTable dt)
        {
            string cols = string.Empty;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                cols += (dt.Columns[i].ColumnName + ",");
            }
            cols = cols.TrimEnd(',');
            return cols.Split(',');
        }


        /// <summary>
        /// 判断舰支队是否存在
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="expectedKey"></param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        //public bool ContainsKeyValue(Dictionary<string, string> dictionary, string expectedKey, string expectedValue)
        //{
        //    string actualValue;
        //    return dictionary.TryGetValue(expectedKey, out actualValue) &&
        //           actualValue == expectedValue;
        //}

        public bool ContainsKeyValue(Dictionary<string, List<BomKey>> dictionary, string expectedKey, string expectedValue)
        {
            List<BomKey> bomKeys = new List<BomKey>();
            bool ishave = dictionary.TryGetValue(expectedKey, out bomKeys);
            if (!ishave) return false;
            bool result = false;
            foreach (BomKey item in bomKeys)
            {
                if (item.BomComponent == expectedValue) result = true;
            }
            return result;
        }

        #region <<dataGridView1事件集合>>

        /// <summary>
        /// 修改物料描述字段修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {



        }


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

        public void UpdataUIValue(string controlname, string code, string name, string unit)
        {
            switch (controlname)
            {
                case "dataGridView1":
                    int index = this.dataGridView1.CurrentRow.Index;//由于按回车行索引会自动跳下下一行，所以取当前索引的上一行
                    DataGridViewRow row2 = this.dataGridView1.Rows[index];
                    this.dataGridView1.Rows[index].Cells[8].Selected = true;
                    row2.Cells["物料编码"].Value = code;//0是编码，1是描述
                    row2.Cells["物料描述"].Value = name;//0是编码，1是描述
                    if (unit == "W013")
                    {
                        unit = "KG";
                    }
                    else if (unit == "PCS")
                    {
                        unit = "EA";
                    }
                    else if (unit == "L007")
                    {
                        unit = "M";
                    }
                    row2.Cells["基本计量单位"].Value = unit;
                    break;
            }
        }


        public void UpdataUIValue3(string ItemDesc, string CaiZhi, string controlname, string code, string name, string cl, string unit)
        {
            switch (controlname)
            {
                case "dataGridView1":
                    int index = this.dataGridView1.CurrentRow.Index;//由于按回车行索引会自动跳下下一行，所以取当前索引的上一行
                    DataGridViewRow row2 = this.dataGridView1.Rows[index];
                    this.dataGridView1.Rows[index].Cells[8].Selected = true;
                    //this.dataGridView1.Rows[index + 1].Cells[8].Selected = false;
                    row2.Cells["物料编码"].Value = code;//0是编码，1是描述
                    row2.Cells["物料描述"].Value = name;//0是编码，1是描述
                    row2.Cells["材料"].Value = cl;
                    row2.Cells["子件描述"].Value = name;//0是编码，1是描述
                    row2.Cells["基本计量单位"].Value = unit;

                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (Convert.ToString(dataGridView1.Rows[i].Cells["母件物料描述"].Value) == ItemDesc && Convert.ToString(dataGridView1.Rows[i].Cells["母件材料"].Value) == CaiZhi)
                        {

                            CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                            cm.SuspendBinding(); //挂起数据绑定
                            //dataGridView1.ReadOnly = true; //继续，这行可选，如果你的datagridview是可编辑的就加上
                            cm.ResumeBinding(); //继续数据绑定
                            this.dataGridView1.Rows[i].Cells["物料编码"].Value = code;
                            this.dataGridView1.Rows[i].Cells["物料描述"].Value = name;
                            this.dataGridView1.Rows[i].Cells["子件描述"].Value = name;
                            this.dataGridView1.Rows[i].Cells["材料"].Value = cl;
                            this.dataGridView1.Rows[i].Cells["基本计量单位"].Value = unit;

                        }

                    }
                    break;
            }
        }

        /// <summary>
        /// 料品形态属性字段修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {

            DataGridViewCell cell = dataGridView1.CurrentCell;
            if (cell == null) return;

            DataGridViewColumn column = cell.OwningColumn;

            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("料品形态属性"))
            {

                int columnIndex = dataGridView1.CurrentCell.ColumnIndex;
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                Point p = dataGridView1.Location;
                Rectangle rect = dataGridView1.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                comboBox1.Left = rect.Left + p.X + 3;
                comboBox1.Top = rect.Top + p.Y + dataGridView1.ColumnHeadersHeight + rect.Height;
                comboBox1.Width = rect.Width;
                comboBox1.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = dataGridView1.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = comboBox1.Items.IndexOf(consultingRoom);

                comboBox1.SelectedIndex = index;
                comboBox1.Visible = true;
            }
            else
            {
                comboBox1.Visible = false;
            }
        }


        //DataGridViewCell cell = dataGridView4.CurrentCell;
        //    if (cell == null) return;

        //    DataGridViewColumn column = cell.OwningColumn;

        //    //如果是要显示下拉列表的列的话
        //    if (column.Name.Equals("物料描述"))
        //    {

        //    }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                //e.SuppressKeyPress = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

        }
        #endregion

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

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //设置tabpage text
            this.tabPage1.Text = "物料档案数据";
            this.tabPage1.Refresh();
            //打开文件
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl文件 (*.xls)|*.xls|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "导出文件保存路径";//为Excel
            openFileDialog.FileName = null;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string strFile = openFileDialog.FileName;
            if (string.IsNullOrEmpty(strFile))
                return;

            DataTable excelDt = ExcelHelper.GetData(strFile);
            this.dataGridView1.DataSource = excelDt;

        }

        private void toolStripButton6_Click_1(object sender, EventArgs e)
        {
            if (this.tabPage1.Text != "物料档案数据")
            {
                MessageBox.Show("导入数据无效");
                return;
            }
            Thread TD = new Thread(ShowProgressForm);
            TD.Start();
            try
            {
                DataTable itemDt = this.dataGridView1.DataSource as DataTable;
                if (itemDt == null || itemDt.Rows.Count <= 0)
                    return;
                foreach (DataRow row in itemDt.Rows)
                {
                    if (row["规格"].ToString().Contains("\""))
                        row["规格"] = row["规格"].ToString().Replace("\"", "$");
                }
                string strJson = JsonConvert.SerializeObject(itemDt);
                string strResult = HttpClientHelper.DoPost(strJson, "SG_BatchCreatItemMaster");
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(strResult);
                int k = 0;//索引号
                Int32 records = 0;//成功导入记录数
                indexLst = new List<int>();
                if (!itemDt.Columns.Contains("错误记录"))
                    itemDt.Columns.Add("错误记录");
                foreach (DataRow row in itemDt.Rows)
                {
                    DataRow[] newRow = dt.Select($"ID='{k}'");
                    if (newRow.Length > 0)
                    {
                        if (Convert.ToBoolean(newRow[0]["IsSuccess"]))
                        {
                            //物料存在时，收集
                            if (newRow[0]["Error"].Equals("料号已存在"))
                            {
                                indexLst.Add(k);
                            }
                            else
                                records++;
                            row["物料编码"] = newRow[0]["code"].ToString();
                        }
                        if (row["规格"].ToString().Contains("$"))
                            row["规格"] = row["规格"].ToString().Replace("$", "\"");
                        row["错误记录"] = newRow[0]["Error"];
                    }

                    k++;
                }

                this.dataGridView1.DataSource = itemDt;


                TD.Abort();

                MessageBox.Show($"总计导入{itemDt.Rows.Count},成功{records}个");
            }
            catch (Exception ex)
            {
                TD.Abort();
            }
            //处理返回值
            //MessageBox.Show(strResult);
        }
        private List<Int32> indexLst = new List<int>();//物料在系统中已存在的行

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (this.tabPage1.Text == "物料档案数据")
            {
                if (e.ColumnIndex > -1)
                {
                    DataGridViewColumn ThisCL = dataGridView1.Columns[e.ColumnIndex];
                    if (ThisCL.Name.Equals("物料编码") && indexLst.Contains(e.RowIndex))
                        e.CellStyle.ForeColor = Color.Red;
                }
            }
        }

        DataTable dtzj = new DataTable();//上锅物料清单datatable
        DataRow DataRowone;
        /// <summary>
        /// 自接物料清单导入
        /// 创建人：lvhe
        /// 创建时间：2022-07-08 23:09:59
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            //设置tabpage text
            this.tabPage1.Text = "自接物料清单数据";
            this.tabPage1.Refresh();
            //打开文件
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl文件 (*.xls)|*.xls|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "导出文件保存路径";//为Excel
            openFileDialog.FileName = null;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string strFile = openFileDialog.FileName;
            if (string.IsNullOrEmpty(strFile))
                return;
            ExcelHelper excelHelper = new ExcelHelper(openFileDialog.FileName);
            //获取excel原始数据
            DataTable excelDt = excelHelper.ZjExcelToDataTable(1);
            //根据原始数据拼接DataGrid数据
            DataTable bomexcelDt = excelHelper.ZjExcelToBOMDataTable(excelDt);
            //DataRowone = bomexcelDt.Rows[0];
            this.dataGridView1.DataSource = bomexcelDt;

            //this.dataGridView1.Columns["母件料品"].Visible = false;
            //this.dataGridView1.Columns["母件物料描述"].Visible = false;
            //this.dataGridView1.Columns["母件材料"].Visible = false;
            //this.dataGridView1.Columns["母件基本计量单位"].Visible = false;
            //this.dataGridView1.Columns["母件用量"].Visible = false;
            this.dataGridView1.Columns["是否虚拟"].Visible = false;
            this.dataGridView1.Columns["是否末阶"].Visible = false;
            this.dataGridView1.Columns["wbs"].Visible = false;
            this.dataGridView1.Columns["标准图号"].Visible = false;
            this.dataGridView1.Columns["母件物料描述"].Visible = false;
            this.dataGridView1.Columns["母件基本计量单位"].Visible = false;
            this.dataGridView1.Columns["序号"].Width =60;
            this.dataGridView1.Columns["母件用量"].Width = 70;
            this.dataGridView1.Columns["子件用量"].Width = 70;
            this.dataGridView1.Columns["物料描述"].Visible = false;
            this.dataGridView1.Columns["基本计量单位"].Width = 90;
            //隐藏1.2.3.4.5.6.7.8.9.10......
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('-') == -1)
                {
                    if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('/') == -1)
                    {
                        CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                        cm.SuspendBinding(); //挂起数据绑定
                        //dataGridView1.ReadOnly = true; //继续，这行可选，如果你的datagridview是可编辑的就加上
                        cm.ResumeBinding(); //继续数据绑定
                        this.dataGridView1.Rows[i].Visible = false;
                    }
                }

            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {

            #region  校验
            if (this.tabPage1.Text != "自接物料清单数据")
            {
                MessageBox.Show("导入数据无效");
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string xh = DataHelper.getStr(row.Cells["序号"].Value);
                string itemCode = DataHelper.getStr(row.Cells["物料编码"].Value);
                decimal qty = DataHelper.getDecimal(row.Cells["子件用量"].Value);
                if (string.IsNullOrEmpty(itemCode) || qty<=0)
                {
                    sb.AppendLine($"序号{xh} 物料编码 和 子件用量不能空值!");
                }
            }
            if(!string.IsNullOrEmpty(sb.ToString()))
            {
                MessageBox.Show(sb.ToString());
                return;
            }
            #endregion

            Thread TD = new Thread(ShowProgressForm);
            TD.Start();

            try
            {
                DataGridView dg = dataGridView1;
                if (dg == null || dg.Rows.Count <= 0) return;

                //第一步  DataTalbe转BOM结构
                string strJson = GetZJBOMJson(dg);

                //string CommandType = "CreateBOM";
                //List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> dictdtos = new List<UFIDAU9CustCommonAPISVDocDTOData>();
                //Dictionary<string, Hashtable> dic = new Dictionary<string, Hashtable>();
                //string key = string.Empty;
                //Hashtable damicht = new Hashtable();
                List<string> MasterItemMasters = new List<string>(); ///所有需要创建的bom母件集合
                string rtnmsg = "";
                rtnmsg = ZJPostCreatBom(strJson);
                TD.Abort();
                if (!string.IsNullOrEmpty(rtnmsg) && rtnmsg != "{\"d\":\"\"}")
                {
                   
                    if (rtnmsg.Contains("项目已存在"))
                    {
                        return;
                    }
                    {

                        msg("创建失败：" + rtnmsg);
                    }
                }
                else
                {
                    Dictionary<string, long> dict = new Dictionary<string, long>();
                    dict.Add("采购件", 0);
                    dict.Add("制造件", 1);
                    dict.Add("虚拟", 0);
                    dict.Add("工艺", 0);
                    List<string> itemCodeLst = new List<string>();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string itemAttribute = DataHelper.getStr(row.Cells["料品形态属性"].Value);
                        string itemCode = DataHelper.getStr(row.Cells["物料编码"].Value);
                        if (itemCodeLst.Contains(itemCode)) continue;
                        itemCodeLst.Add(itemCode);
                        if (dict.ContainsKey(itemAttribute))
                            dict[itemAttribute] = dict[itemAttribute] + 1;

                      

                    }
                    msg("创建bom成功,共导入制造件" + dict["制造件"] + ",采购件" + dict["采购件"] + ",虚拟件" + dict["虚拟"] + ",工艺件" + dict["工艺"]);

                }
            }
            catch (Exception)
            {
                TD.Abort();
            }

        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (this.tabPage1.Text == "自接物料清单数据")
            {
                DataGridViewRow dr = (sender as DataGridView).Rows[e.RowIndex];

                if (dr.Cells["是否虚拟"].Value.ToString().Trim().Equals("是"))
                {
                    // 设置单元格的背景色
                    dr.Cells["物料编码"].Style.ForeColor = Color.Red;
                }
            }
        }



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null) return;

            if (this.tabPage1.Text == "物料清单数据")
            {
                DataTable dt = (dataGridView1.DataSource as DataTable);
                SqlBulkCopyHelper.SqlBulkCopyByDatatable("cust_bomsg_data", dt);
            }
            if (this.tabPage1.Text == "自接物料清单数据")
            {
                this.dataGridView1.Columns["是否虚拟"].Visible = true;
                this.dataGridView1.Columns["是否末阶"].Visible = true;
                this.dataGridView1.Columns["wbs"].Visible = true;
                this.dataGridView1.Columns["标准图号"].Visible = true;
                //his.dataGridView1.Columns["原物料描述"].Visible = true;
                //隐藏1.2.3.4.5.6.7.8.9.10......
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('-') == -1)
                    {
                        if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('/') == -1)
                        {
                            CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                            cm.SuspendBinding(); //挂起数据绑定
                                                 //dataGridView1.ReadOnly = true; //继续，这行可选，如果你的datagridview是可编辑的就加上
                            cm.ResumeBinding(); //继续数据绑定
                            this.dataGridView1.Rows[i].Visible = true;
                        }
                    }

                }
                DataTable dtt = (dataGridView1.DataSource as DataTable);
                //if (DataRowone != null)
                //{
                //    dtt.Rows.InsertAt(DataRowone, 0);
                //}
                SqlBulkCopyHelper.SqlBulkCopyByDatatable("Cust_BomZj_Data", dtt);
                this.dataGridView1.Columns["是否虚拟"].Visible = false;
                this.dataGridView1.Columns["是否末阶"].Visible = false;
                this.dataGridView1.Columns["wbs"].Visible = false;
                this.dataGridView1.Columns["标准图号"].Visible = false;
                //this.dataGridView1.Columns["原物料描述"].Visible = false;
                //隐藏1.2.3.4.5.6.7.8.9.10......
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('-') == -1)
                    {
                        if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('/') == -1)
                        {
                            CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                            cm.SuspendBinding(); //挂起数据绑定
                                                 //dataGridView1.ReadOnly = true; //继续，这行可选，如果你的datagridview是可编辑的就加上
                            cm.ResumeBinding(); //继续数据绑定
                            this.dataGridView1.Rows[i].Visible = false;
                        }
                    }

                }
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string code = string.Empty;
            string str = string.Empty;
            code = ((System.Windows.Forms.ToolStripComboBox)sender).SelectedItem.ToString();
            if (code == "上锅物料清单查询") return;
            //设置tabpage text
            if (this.tabPage1.Text != "物料清单数据")
            {
                this.tabPage1.Text = "物料清单数据";
                this.tabPage1.Refresh();
            }

            str = "select 序号,WBS,展开层,母件料品,母件物料描述,母件基本计量单位,母件用量,物料编码,物料描述,BOM用途,物料类型,基本计量单位,[数量/重量],尺寸,料品形态属性,制造路线,备注 from Cust_BomSG_Data where wbs='" + code + "' order by 母件料品 asc,物料描述 asc";
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, str);
            this.dataGridView1.DataSource = ds.Tables[0];
            initDataGrid(dataGridView1);
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string code = string.Empty;
            string str = string.Empty;
            code = ((System.Windows.Forms.ToolStripComboBox)sender).SelectedItem.ToString();
            if (code == "自接物料清单查询") return;
            //设置tabpage text
            if (this.tabPage1.Text != "自接物料清单数据")
            {
                this.tabPage1.Text = "自接物料清单数据";
                this.tabPage1.Refresh();
            }
            str = "select 序号,母件料品,母件描述,母件物料描述,母件材料,母件基本计量单位,母件用量,物料编码,物料描述,子件描述,基本计量单位,子件用量,材料,制造路线,是否末阶,是否虚拟,wbs,总重,料品形态属性,备注,标准图号 from Cust_BomZJ_Data where wbs='" + code + "'";
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, str);
            DataRowone = ds.Tables[0].Rows[0];
            this.dataGridView1.DataSource = ds.Tables[0];
            this.dataGridView1.Columns["是否虚拟"].Visible = false;
            this.dataGridView1.Columns["是否末阶"].Visible = false;
            this.dataGridView1.Columns["wbs"].Visible = false;
            this.dataGridView1.Columns["标准图号"].Visible = false;
            this.dataGridView1.Columns["母件物料描述"].Visible = false;
            this.dataGridView1.Columns["物料描述"].Visible = false;

            this.dataGridView1.Columns["序号"].Width = 60;
            this.dataGridView1.Columns["母件用量"].Width = 70;
            this.dataGridView1.Columns["子件用量"].Width = 70;
            this.dataGridView1.Columns["基本计量单位"].Width = 90;

            //隐藏1.2.3.4.5.6.7.8.9.10......
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('-') == -1)
                {
                    if (Convert.ToString(dataGridView1.Rows[i].Cells["序号"].Value).IndexOf('/') == -1)
                    {
                        CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                        cm.SuspendBinding(); //挂起数据绑定
                        //dataGridView1.ReadOnly = true; //继续，这行可选，如果你的datagridview是可编辑的就加上
                        cm.ResumeBinding(); //继续数据绑定
                        this.dataGridView1.Rows[i].Visible = false;
                    }
                }

            }
            //initDataGrid(dataGridView1);
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string pItemDesc = string.Empty;
            //自接修改物料清单
            if (tabPage1.Text == "自接物料清单数据")
            {

                //是否是末级物料
                //string sfmj = dataGridView1[13, e.RowIndex].Value?.ToString();
                //if (sfmj != "是") return;


                int index = e.ColumnIndex;
                string value = dataGridView1[e.ColumnIndex, e.RowIndex].Value?.ToString();

                //单位
                //string itemUnit = dataGridView1[4, e.RowIndex].Value?.ToString();
                //如果是物料描述列修改  则进入新的form2
                if (e.ColumnIndex == 9)
                {
                    //料品形态属性
                    string itemAttribute = DataHelper.getStr(dataGridView1["料品形态属性", e.RowIndex].Value);
                    //Form3 form = new Form3(value, itemCz);
                    //form.Show();
                    //母件物料描述
                    pItemDesc = dataGridView1["母件物料描述", e.RowIndex].Value?.ToString();
                    string pItemCode = dataGridView1["母件料品", e.RowIndex].Value?.ToString().Split('(')[0];
                    string caizhi = dataGridView1["母件材料", e.RowIndex].Value?.ToString();
                    controlname3 = ((DataGridView)sender).Name;
                    Form3 f = new Form3(pItemCode, controlname3, pItemDesc, caizhi, itemAttribute);
                    f.Show();
                    f.form3UserControls += UpdataUIValue3;
                }
            }
            else
            {
                int index = e.ColumnIndex;
                string value = dataGridView1[e.ColumnIndex, e.RowIndex].Value?.ToString();
                string mjItemCode = dataGridView1[3, e.RowIndex].Value?.ToString();
                //单位
                string itemUnit = dataGridView1[11, e.RowIndex].Value?.ToString();
                //如果是物料描述列修改  则进入新的form2
                if (e.ColumnIndex == 8)
                {
                    controlname = ((DataGridView)sender).Name;
                    pItemDesc = dataGridView1["物料描述", e.RowIndex].Value?.ToString();
                    Form2 f = new Form2(controlname, itemUnit, pItemDesc);
                    f.Show();
                    f.form2UserControls += UpdataUIValue;
                    //string[] temps = value.Split('_');
                    ////如果是3段  精确查找
                    //if (temps.Length >= 3)
                    //{
                    //    string sql = string.Format(@"select '0000000000'+Code 料号,Name +'_'+DescFlexField_PrivateDescSeg1+'_'+SPECS 品名 from CBO_ItemMaster where DescFlexField_PrivateDescSeg1 = '{0}' 
                    //                    and SPECS='{1}' group by Code,name,SPECS,DescFlexField_PrivateDescSeg1", temps[1], temps[2]);
                    //    DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
                    //    if (dt.Rows.Count == 1)
                    //    {
                    //        int index1 = this.dataGridView1.CurrentRow.Index - 1;//由于按回车行索引会自动跳下下一行，所以取当前索引的上一行
                    //        DataGridViewRow row2 = this.dataGridView1.Rows[index1];
                    //        this.dataGridView1.Rows[index1].Cells[8].Selected = true;
                    //        this.dataGridView1.Rows[index1 + 1].Cells[8].Selected = false;
                    //        row2.Cells["物料编码"].Value = dt.Rows[0]["料号"];//0是编码，1是描述
                    //        row2.Cells["物料描述"].Value = dt.Rows[0]["品名"]; ;//0是编码，1是描述
                    //    }
                    //    else
                    //    {
                    //        controlname = ((DataGridView)sender).Name;
                    //        Form2 f = new Form2(controlname);
                    //        f.Show();
                    //        f.form2UserControls += UpdataUIValue;
                    //    }
                    //}
                    //else
                    //{
                    //    controlname = ((DataGridView)sender).Name;
                    //    Form2 f = new Form2(controlname);
                    //    f.Show();
                    //    f.form2UserControls += UpdataUIValue;
                    //}

                }
                else if (e.ColumnIndex == 15)
                {
                    for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
                    {
                        if (mjItemCode == getstr(dataGridView1.Rows[iRow].Cells["母件料品"].Value))
                        {
                            dataGridView1.Rows[iRow].Cells["制造路线"].Value = value;
                        }
                    }
                }
            }
        }
        private string strDelXH = string.Empty;//待删除的序号
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string strTitle = this.tabPage1.Text;
            switch (strTitle)
            {
                case "自接物料清单数据":
                case "物料清单数据":
                    DataGridViewRow row = e.Row;//当前要删除的行

                    //删除前确认
                    if (MessageBox.Show("确认要删除选中的行吗?", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        //如果不是Ok，则删除
                        e.Cancel = true;
                        return;
                    }
                    strDelXH= DataHelper.getStr(row.Cells["序号"].Value);
                    
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            string strTitle = this.tabPage1.Text;
            switch (strTitle)
            {
                case "自接物料清单数据":
                case "物料清单数据":
                    List<DataGridViewRow> delRows = PubHelper.GetSelRows(this.dataGridView1, strDelXH);
                    foreach (DataGridViewRow dgRow in delRows)
                    {
                        this.dataGridView1.Rows.Remove(dgRow);
                    }
                    break;
                default:
                    break;
            }

        }
    }
}