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

namespace WindowsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //this.tabControl1.Visible = false;
            this.tabControl1.TabPages.Remove(this.tabPage2);
            //this.tabControl1.TabPages.AddRange(this.tabPage1);
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

        /// <summary>
        /// 上锅物料清单导入按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl文件 (*.xls)|*.xls|所有文件 (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "导出文件保存路径";//为Excel
            openFileDialog.FileName = null;
            openFileDialog.ShowDialog();


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
            //Form1.dataGridView4.DataSource = changedt_bom(dt);
            Form1.dataGridView4.DataSource = dt;
            initDataGrid(Form1.dataGridView4);
        }

        private void initDataGrid(DataGridView dg)
        {
            dg.Columns["序号"].Width = 50;
            dg.Columns["展开层"].Width = 60;
            dg.Columns["物料描述"].Width = 200;
            dg.Columns["基本计量单位"].Width = 80;
            dg.Columns["BOM用途"].Visible = false;
            dg.Columns["物料类型"].Visible = false;
            //dg.Columns["母件用量"].Visible = false;
        }

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

        #region 防止NULL异常
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

        #endregion
        private FlexCell.Grid grid1 = new FlexCell.Grid();
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string CommandType = "Routing";
            List<string> strs = new List<string>();

            #region 校验母件+工序行号是否重复


            string strKey = string.Empty;
            ArrayList lst1 = new ArrayList();//工艺路线集合
            ArrayList lst2 = new ArrayList();//Excel中工艺路线重复项
            for (int iRow = 0; iRow < this.dataGridView2.Rows.Count; iRow++)
            {
                strKey = this.dataGridView2.Rows[iRow].Cells[0].Value.ToString() + " / " + this.dataGridView2.Rows[iRow].Cells[5].Value.ToString();
                if (lst1.Contains(strKey))
                    lst2.Add(strKey);
                else
                    lst1.Add(strKey);

            }
            if (lst2.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lst2.Count; i++)
                {
                    sb.Append("\n").Append((lst2[i].ToString()));
                }
                msg("Excel中存在重复的工艺母件和工序行号: " + sb.ToString());
                return;
            }

            #endregion

            foreach (DataGridViewRow dgvr in dataGridView2.Rows)
            {

                string str = "";
                foreach (DataGridViewColumn col in dataGridView2.Columns)
                {
                    str += getstr(dgvr.Cells[col.Name].Value) + "|";
                }
                str = str.Substring(0, str.Length - 1);
                strs.Add(str);
            }
            string rtnmsg = "";
            List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> rtnlst = UFIDA.U9.Cust.U9CommonAPISv.CommonAPI.DOU9Commonsv(Login.u9ContentHt, CommandType, strs, "", 0, null, null, out rtnmsg);

            if (rtnmsg == "OK")
            {
                msg(rtnlst[0].m_rtnStr);
            }
            else
            {
                msg(rtnmsg);
            }
        }
        private void msg(string message)
        {
            MessageBox.Show(message);
        }

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
                    dto.private2 = DataHelper.getStr(row.Cells["工艺路线"].Value);
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

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            DataGridView dg = Form1.dataGridView4;
            if (dg == null || dg.Rows.Count <= 0) return;

            //第一步  DataTalbe转BOM结构
            string strJson = GetBOMJson(dg);

            string CommandType = "CreateBOM";
            List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> dictdtos = new List<UFIDAU9CustCommonAPISVDocDTOData>();
            Dictionary<string, Hashtable> dic = new Dictionary<string, Hashtable>();
            string key = string.Empty;
            Hashtable damicht = new Hashtable();
            List<string> MasterItemMasters = new List<string>(); ///所有需要创建的bom母件集合
            string rtnmsg = "";
            #region 物料清单
            //Hashtable bomHt = new Hashtable();
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)
            //{
            //    string strBomJC = Form1.dataGridView4.Rows[iRow].Cells[1].ToString();


            //}


            #endregion

            #region BOM问题：导入工具存在问题，在表示级次的符号出现问题时，系统不报错，也不导入。

            //StringBuilder errSb = new StringBuilder();
            //errSb.Insert(0, "");
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)//循环dt,添加“部门”的子节点
            //{
            //    int rowNum = iRow + 1;

            //    key = getstr(Form1.dataGridView4.Rows[iRow].Cells[0].Value);
            //    if (key == "")
            //    {
            //        errSb.AppendLine(rowNum + "行层级为空！");
            //    }
            //    else
            //    {
            //        //校验序号最后一位必须是数字，且只能由数字和"."组成

            //        Regex reg = new Regex(@"^[1-9](\.{0,1}[1-9][0-9]{0,2})*$");
            //        if (!reg.IsMatch(key))
            //        {
            //            errSb.AppendLine(key);
            //        }
            //    }
            //}


            //if (!string.IsNullOrEmpty(errSb.ToString()))
            //{
            //    errSb.Insert(0, "以下序号有问题\r\n");
            //    MessageBox.Show(errSb.ToString());
            //    return;
            //}

            #endregion




            ////便利获得所有母件
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)
            //{
            //   string Itemmaster = getstr(Form1.dataGridView4.Rows[iRow].Cells[10].Value);
            //    if (!MasterItemMasters.Contains(Itemmaster)) MasterItemMasters.Add(Itemmaster);
            //}
            ////便利所有母件  将该母件的所有子键添加到str中创建bom
            //foreach (string MasterItemMaster in MasterItemMasters)
            //{
            //    string str = string.Empty;
            //    for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)
            //    {
            //        string Itemmaster = getstr(Form1.dataGridView4.Rows[iRow].Cells[10].Value);
            //        string ComponentItem = getstr(Form1.dataGridView4.Rows[iRow].Cells[3].Value);
            //        if (MasterItemMaster == Itemmaster)
            //        {
            //            str += "{";
            //            str += "'LineNum':'" + (iRow+1)*10 + "'" + ",";//行号
            //            str += "'BOM':'" + MasterItemMaster + "'" + ",";//母件料品
            //            str += "'BOMC':'" + ComponentItem + "'" + ",";//料品子件
            //            str += "'ComponentItemName':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[4].Value) + "'" + ",";//料品名称
            //            str += "'ItemAttribute':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[11].Value) + "'" + ",";//料品形态属性
            //            str += "'BomUse':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[5].Value) + "'" + ",";//bom用途
            //            str += "'ItemCatagory':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[6].Value) + "'" + ",";//物料分类
            //            str += "'UMO':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[7].Value) + "'" + ",";//计量单位
            //            str += "'Qty':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[8].Value) + "'" + ",";//数量
            //            str += "'Router':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[12].Value) + "'" + ",";//工艺路线
            //            str += "'Size':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[9].Value) + "'";//尺寸
            //            str += "},";
            //        }
            //    }
            //    str = str.Substring(0, str.Length - 1);
            //    str = "[" + str + "]";
            rtnmsg = PostCreatBom(strJson);
            //    //将字符串传给u9   需一次性导入，只调一次接口
            //    //rtnmsg =PostCreatBom(str);
            //    //if (rtnmsg!="{\"d\":\"\"}")
            //    //{
            //    //    msg(rtnmsg);
            //    //}
            //}
            int cqqty = 0, zzqty = 0, xnqty = 0, gyqty = 0;
            for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)
            {
                string attribute = getstr(Form1.dataGridView4.Rows[iRow].Cells["料品形态属性"].Value);
                if (attribute == "采购件")
                {
                    cqqty++;
                }
                else if (attribute == "制造件")
                {
                    zzqty++;
                }
                else if (attribute == "虚拟")
                {
                    xnqty++;
                }
                else if (attribute == "工艺")
                {
                    gyqty++;
                }
            }
            if (rtnmsg != "{\"d\":\"\"}")
            {
                msg("创建失败：" + rtnmsg);
            }
            else
            {
                msg("创建bom成功,共导入采购件" + cqqty + "件,制造件" + zzqty + "件,虚拟件" + xnqty + "件,工艺件" + gyqty + "件");
            }
            #region<<demo>>
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)//循环dt,添加“部门”的子节点
            //{

            //    int rowNum = iRow + 1;
            //    string damicstr = "";
            //    key = getstr(Form1.dataGridView4.Rows[iRow].Cells[0].Value);


            //    string cellContent = string.Empty;
            //    for (int iCol = 0; iCol < Form1.dataGridView4.Columns.Count; iCol++)
            //    {


            //        cellContent = getstr(Form1.dataGridView4.Rows[iRow].Cells[iCol].Value);
            //        damicstr += cellContent + "|";
            //    }
            //    if (!damicht.ContainsKey(key))
            //    {
            //        damicht.Add(key, damicstr.Trim('|'));
            //    }
            //    else
            //    {
            //        msg("序号列作为主键不能重复！" + key);
            //        return;
            //    }
            //    //Hashtable ht = new Hashtable();
            //    Hashtable ht = new NoSortHashtable();


            //    string dickey = key;
            //    for (int iRowc = 0; iRowc < Form1.dataGridView4.Rows.Count; iRowc++)
            //    {

            //        if (getstr(Form1.dataGridView4.Rows[iRowc].Cells[0].Value) == "1")
            //            continue;
            //        int t = getstr(Form1.dataGridView4.Rows[iRowc].Cells[0].Value).LastIndexOf('.');
            //        t = getstr(Form1.dataGridView4.Rows[iRowc].Cells[0].Value).Length - t;
            //        string hashkey = "1";
            //        //string hashkey = getstr(Form1.dataGridView4.Rows[iRowc].Cells[0].Value).Substring(0, getstr(Form1.dataGridView4.Rows[iRowc].Cells[0].Value).Length - t);

            //        if (hashkey == dickey)
            //        {
            //            string str = "";
            //            string htkey = getstr(Form1.dataGridView4.Rows[iRowc].Cells[0].Value).ToString();
            //            for (int iCol = 0; iCol < Form1.dataGridView4.ColumnCount; iCol++)
            //            {
            //                cellContent = getstr(Form1.dataGridView4.Rows[iRowc].Cells[iCol].Value);
            //                str += cellContent + "|";
            //            }
            //            if (!ht.ContainsKey(htkey))
            //            {
            //                ht.Add(htkey, str);
            //            }
            //        }
            //    }
            //    if (ht.Count > 0)
            //    {
            //        if (!dic.ContainsKey(dickey))
            //            dic.Add(dickey, ht);
            //    }

            //}
            //dic.Add("Head", damicht);
            //ArrayList lst = new ArrayList();


            //foreach (string lastkey in dic.Keys)
            //{
            //    if (lastkey == "Head") continue;

            //    UFIDAU9CustCommonAPISVDocDTOData dtodata = new UFIDAU9CustCommonAPISVDocDTOData();
            //    dtodata.m_str = dic["Head"][lastkey].ToString();
            //    //去除重复的Bom
            //    if (lst.Contains(dtodata.m_str.Split('|')[1]))
            //        continue;
            //    lst.Add(dtodata.m_str.Split('|')[1]);

            //    List<UFIDAU9CustCommonAPISVDocLineDTOData> lines = new List<UFIDAU9CustCommonAPISVDocLineDTOData>();


            //    foreach (string subkey in dic[lastkey].Keys)
            //    {
            //        UFIDAU9CustCommonAPISVDocLineDTOData line = new UFIDAU9CustCommonAPISVDocLineDTOData();
            //        line.m_str = dic[lastkey][subkey].ToString();
            //        lines.Add(line);


            //    }
            //    //lines.Reverse();
            //    dtodata.m_docLineDTOs = lines.ToArray();
            //    dictdtos.Add(dtodata);
            //}
            #endregion
            //return;

            #region BOM导入问题：系统中有此物料的BOM，但与导入的版本号不一致时，需要提示最高版本是多少

            //StringBuilder bomSb = new StringBuilder();
            //foreach (UFIDAU9CustCommonAPISVDocDTOData DocDTO in dictdtos)
            //{
            //    string strs = DocDTO.m_str;
            //    if (!string.IsNullOrEmpty(strs))
            //    {
            //        string itemCode = strs.Split('|')[1];
            //        string versionCode = strs.Split('|')[5] == "" ? "X0" : strs.Split('|')[5];
            //        StringBuilder sb = new StringBuilder();
            //        sb.Append(" select bomversioncode from cbo_bommaster A ");
            //        sb.Append(" left join cbo_itemmaster A1 on A.ItemMaster=A1.ID ");
            //        sb.AppendFormat(" where A1.code='{0}' order by bomversioncode desc", itemCode);

            //        DataTable dt = MiddleDBInterface.getdt(sb.ToString(), SQLHelper.sqlconn(Login.strConn));
            //        if (dt.Select("bomversioncode='" + versionCode + "'").Length > 0)
            //        {
            //            bomSb.AppendFormat("'{0}'版本号{1}在系统中已存在,该自制件在系统中的最高版为'{2}'", itemCode, versionCode, dt.Rows[0][0].ToString()).AppendLine();
            //        }


            //    }
            //}
            //if (!string.IsNullOrEmpty(bomSb.ToString()))
            //{
            //    MessageBox.Show(bomSb.ToString());
            //    return;
            //}

            #endregion




            //List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> rtnlst = UFIDA.U9.Cust.U9CommonAPISv.CommonAPI.DOU9Commonsv(Login.u9ContentHt, CommandType, null, "", 0, null, dictdtos, out rtnmsg);


        }

        /// <summary>
        /// 调用U9Bom导入接口
        /// </summary>
        /// <param name="str"></param>
        public static string PostCreatBom(string str)
        {
            string url = "http://localhost/U9/RestServices/YY.U9.Cust.APISV.IMainSV.svc/DO";
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            str = "" + str.Replace("\"", "\\\"") + "";
            string EntCode = getstr(Login.u9ContentHt["OrgCode"]);//组织编码
            string UserCode = getstr(Login.u9ContentHt["UserCode"]);//用户编码
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"01\",\"OrgCode\":\"" + EntCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"CreateBom\"}";
            //body.Replace("strorg", getstr(Login.u9ContentHt["OrgCode"]));
            //body.Replace("struser", getstr(Login.u9ContentHt["UserCode"]));
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        private static string getstr(object obj)
        {
            return obj == null ? "" : obj.ToString();
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
                this.dataGridView2.DataSource = changedt_routing(dt);
            }
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(comboBox1.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView4.CurrentCell != null)
                dataGridView4.CurrentCell.Value = comboBox1.Items[comboBox1.SelectedIndex];
        }

        private void dataGridView4_CurrentCellChanged(object sender, EventArgs e)
        {

            DataGridViewCell cell = dataGridView4.CurrentCell;
            if (cell == null) return;

            DataGridViewColumn column = cell.OwningColumn;

            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("料品形态属性"))
            {

                int columnIndex = dataGridView4.CurrentCell.ColumnIndex;
                int rowIndex = dataGridView4.CurrentCell.RowIndex;
                Point p = Form1.dataGridView4.Location;
                Rectangle rect = dataGridView4.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                comboBox1.Left = rect.Left + p.X + 3;
                comboBox1.Top = rect.Top + p.Y + dataGridView4.ColumnHeadersHeight + rect.Height;
                comboBox1.Width = rect.Width;
                comboBox1.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = dataGridView4.Rows[rowIndex].Cells[columnIndex].Value.ToString();
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
        private void dataGridView4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.ColumnIndex;
            string value = dataGridView4[e.ColumnIndex, e.RowIndex].Value?.ToString();
            //如果是物料描述列修改  则进入新的form2
            if (e.ColumnIndex != 8) return;
            Form2 form2 = new Form2(value);
            form2.Show();
        }

        private void dataGridView4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                //e.SuppressKeyPress = true;
            }
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

        }
    }
}