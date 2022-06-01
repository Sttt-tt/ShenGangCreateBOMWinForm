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

namespace WindowsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            if (dt.Columns.Count == 17)
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
                    if (j == 0 || j >= 17) dt.Columns.RemoveAt(j);
                }
            }
            return dt;
        }
        private DataTable changedt_routing(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {

                if ((getint(dt.Rows[i][0]) <= 0 || getstr(dt.Rows[i][1]) == "") && (getstr(dt.Rows[i][0]) != "序号" && getstr(dt.Rows[i][1]) != "存货编码"))
                {
                    dt.Rows.RemoveAt(i);
                }

            }
            if (dt.Columns.Count == 19)
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
                    if (j == 0 || j > 17) dt.Columns.RemoveAt(j);
                }
            }
            return dt;
        }
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

            DataTable dt = ReadExcelToDataSet.ImportDataTableFromExcel(myStream, 0, 3,true);
            this.dataGridView1.DataSource = changedt_bom(dt);
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
        private string getstr(object obj)
        {
            return obj == null ? "" : obj.ToString();
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
                if(lst1.Contains(strKey))
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


        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            string CommandType = "CreateBOM";
            List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> dictdtos = new List<UFIDAU9CustCommonAPISVDocDTOData>();
            Dictionary<string, Hashtable> dic = new Dictionary<string, Hashtable>();
            string key = string.Empty;
            Hashtable damicht = new Hashtable();
            for (int iRow = 0; iRow < this.dataGridView1.Rows.Count; iRow++)//循环dt,添加“部门”的子节点
            {
                string damicstr = "";
                if (getstr(this.dataGridView1.Rows[iRow].Cells[0].Value) == "")
                {
                    MessageBox.Show("层级不能为空！");
                    return;
                }
                key = getstr(this.dataGridView1.Rows[iRow].Cells[0].Value);
                string cellContent = string.Empty;
                for (int iCol = 0; iCol < this.dataGridView1.Columns.Count; iCol++)
                {


                    cellContent = getstr(this.dataGridView1.Rows[iRow].Cells[iCol].Value);
                    damicstr += cellContent + "|";
                }
                if (!damicht.ContainsKey(key))
                {
                    damicht.Add(key, damicstr.Trim('|'));
                }
                else
                {
                    msg("序号列作为主键不能重复！"+key);
                    return;
                }
                Hashtable ht = new Hashtable();

                string dickey = key;
                for (int iRowc = 0; iRowc < this.dataGridView1.Rows.Count; iRowc++)
                {

                    if (getstr(this.dataGridView1.Rows[iRowc].Cells[0].Value) == "1")
                        continue;
                    int t = getstr(this.dataGridView1.Rows[iRowc].Cells[0].Value).LastIndexOf('.');
                    t = getstr(this.dataGridView1.Rows[iRowc].Cells[0].Value).Length - t;
                    string hashkey = getstr(this.dataGridView1.Rows[iRowc].Cells[0].Value).Substring(0, getstr(this.dataGridView1.Rows[iRowc].Cells[0].Value).Length - t);

                    if (hashkey == dickey)
                    {
                        string str = "";
                        string htkey = getstr(this.dataGridView1.Rows[iRowc].Cells[0].Value).ToString();
                        for (int iCol = 0; iCol < this.dataGridView1.ColumnCount; iCol++)
                        {


                            cellContent = getstr(this.dataGridView1.Rows[iRowc].Cells[iCol].Value);
                            str += cellContent + "|";
                        }
                        if (!ht.ContainsKey(htkey))
                        {
                            ht.Add(htkey, str);
                        }
                    }
                }
                if (ht.Count > 0)
                {
                    if (!dic.ContainsKey(dickey))
                        dic.Add(dickey, ht);
                }

            }
            dic.Add("Head", damicht);
            foreach (string lastkey in dic.Keys)
            {
                if (lastkey == "Head") continue;
                UFIDAU9CustCommonAPISVDocDTOData dtodata = new UFIDAU9CustCommonAPISVDocDTOData();
                dtodata.m_str = dic["Head"][lastkey].ToString();
                List<UFIDAU9CustCommonAPISVDocLineDTOData> lines = new List<UFIDAU9CustCommonAPISVDocLineDTOData>();
                foreach (string subkey in dic[lastkey].Keys)
                {
                    UFIDAU9CustCommonAPISVDocLineDTOData line = new UFIDAU9CustCommonAPISVDocLineDTOData();
                    line.m_str = dic[lastkey][subkey].ToString();
                    lines.Add(line);

                }
                dtodata.m_docLineDTOs = lines.ToArray();
                dictdtos.Add(dtodata);
            }

            string rtnmsg = "";
            List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> rtnlst = UFIDA.U9.Cust.U9CommonAPISv.CommonAPI.DOU9Commonsv(Login.u9ContentHt, CommandType, null, "", 0, null, dictdtos, out rtnmsg);

            if (rtnmsg == "OK")
            {
                msg(rtnlst[0].m_rtnStr);
            }
            else
            {
                msg(rtnmsg);
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
                this.dataGridView2.DataSource = changedt_routing(dt);
            }
        }
    }
}