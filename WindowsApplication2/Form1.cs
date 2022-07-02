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

namespace WindowsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //this.tabControl1.Visible = false;
            //this.tabControl1.TabPages.Remove(this.tabPage2);
            this.comboBox1.Visible = false;
            //this.tabControl1.TabPages.AddRange(this.tabPage1);
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

                if ((getint(dt.Rows[i][0]) <= 0 || getstr(dt.Rows[i][1]).Trim() == "") && (getstr(dt.Rows[i][0]) != "���" && getstr(dt.Rows[i][1]) != "�㼶"))
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
                if ((this.getint(dt.Rows[i][0]) <= 0 || getstr(dt.Rows[i][1]) == "") && getstr(dt.Rows[i][0]) != "���" && getstr(dt.Rows[i][1]) != "�������")
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


        #region <<��ť�¼�����>>
        /// <summary>
        /// �Ϲ������嵥���밴ť�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //����tabpage text
            if (this.tabPage1.Text != "�����嵥����")
            {
                this.tabPage1.Text = "�����嵥����";
                this.tabPage1.Refresh();
            }
            //���ļ�

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl�ļ� (*.xls)|*.xls|�����ļ� (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "�����ļ�����·��";//ΪExcel
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
                MessageBox.Show("�ļ� " + openFileDialog.FileName + " ������һ���ʹ�ã�\r\n���ȹرոý��̣�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //DataTable dt = ReadExcelToDataSet.ImportDataTableFromExcel(myStream, 0, 3, true);
            ExcelHelper excelHelper = new ExcelHelper(openFileDialog.FileName);
            DataTable dt = excelHelper.ExcelToDataTable2(0);
            //Form1.dataGridView4.DataSource = changedt_bom(dt);

            dataGridView1.DataSource = dt;

            for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
            {
                string bitem = getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value);
                string item = getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value);
                if (iRow >= 1)
                {
                    string sbitem = getstr(dataGridView1.Rows[iRow - 1].Cells["ĸ����Ʒ"].Value);
                    string sitem = getstr(dataGridView1.Rows[iRow - 1].Cells["���ϱ���"].Value);
                    if (bitem == sbitem && item == sitem)
                    {
                        dataGridView1.Rows.Remove(dataGridView1.Rows[iRow]);
                        iRow = iRow - 1;
                    }
                }
            }
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
            //{
            //    string zkc = getstr(dataGridView1.Rows[iRow].Cells["չ����"].Value);
            //    if (Convert.ToInt32(zkc) <= 2) continue;
            //    string bitem = getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value);
            //    string item = getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value);
            //    if (dic.Count > 0)
            //    {
            //        if (ContainsKeyValue(dic, bitem, item))
            //        {
            //            dataGridView1.Rows.Remove(dataGridView1.Rows[iRow]);
            //        }
            //        else
            //        {
            //            dic.Add(bitem, item);
            //        }
            //    }
            //    else
            //    {
            //        dic.Add(bitem, item);
            //    }
            //}
            //Dictionary<string, List<BomKey>> dic = new Dictionary<string, List<BomKey>>();
            //for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
            //{
            //    string zkc = getstr(dataGridView1.Rows[iRow].Cells["չ����"].Value);
            //    string xuhao = getstr(dataGridView1.Rows[iRow].Cells["���"].Value);
            //    if (xuhao == "92")
            //    {
            //        continue;
            //    }
            //    if (Convert.ToInt32(zkc) <= 2) continue;
            //    string bitem = getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value);
            //    string item = getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value);
            //    if (dic.Count > 0)
            //    {
            //        if (ContainsKeyValue(dic, bitem, item))
            //        {
            //            dataGridView1.Rows.Remove(dataGridView1.Rows[iRow]);
            //        }
            //        else
            //        {
            //            BomKey newkey = new BomKey();
            //            newkey.BomMaster = bitem;
            //            newkey.BomComponent = item;
            //            List<BomKey> bomKey =  new List<BomKey>();
            //            dic.TryGetValue(bitem,out bomKey);
            //            if (bomKey==null)
            //            {
            //                List<BomKey> newlistkey = new List<BomKey>();
            //                newlistkey.Add(newkey);
            //                dic.Add(bitem, newlistkey);
            //            }
            //            else
            //            {
            //                dic[bitem].Add(newkey);
            //            }
            //            //dic.Add(bitem, item);
            //        }
            //    }
            //    else
            //    {
            //        BomKey newkey = new BomKey();
            //        newkey.BomMaster = bitem;
            //        newkey.BomComponent = item;
            //        List<BomKey> newlistkey = new List<BomKey>();
            //        newlistkey.Add(newkey);
            //        dic.Add(bitem, newlistkey);
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
            ArrayList lst1 = new ArrayList();//����·�߼���



        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.tabPage1.Text != "�����嵥����")
            {
                MessageBox.Show("����������Ч");
                return;
            }
            DataGridView dg = dataGridView1;
            if (dg == null || dg.Rows.Count <= 0) return;

            //��һ��  DataTalbeתBOM�ṹ
            string strJson = GetBOMJson(dg);

            string CommandType = "CreateBOM";
            List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> dictdtos = new List<UFIDAU9CustCommonAPISVDocDTOData>();
            Dictionary<string, Hashtable> dic = new Dictionary<string, Hashtable>();
            string key = string.Empty;
            Hashtable damicht = new Hashtable();
            List<string> MasterItemMasters = new List<string>(); ///������Ҫ������bomĸ������
            string rtnmsg = "";
            #region �����嵥
            //Hashtable bomHt = new Hashtable();
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)
            //{
            //    string strBomJC = Form1.dataGridView4.Rows[iRow].Cells[1].ToString();


            //}


            #endregion

            #region BOM���⣺���빤�ߴ������⣬�ڱ�ʾ���εķ��ų�������ʱ��ϵͳ������Ҳ�����롣

            //StringBuilder errSb = new StringBuilder();
            //errSb.Insert(0, "");
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)//ѭ��dt,��ӡ����š����ӽڵ�
            //{
            //    int rowNum = iRow + 1;

            //    key = getstr(Form1.dataGridView4.Rows[iRow].Cells[0].Value);
            //    if (key == "")
            //    {
            //        errSb.AppendLine(rowNum + "�в㼶Ϊ�գ�");
            //    }
            //    else
            //    {
            //        //У��������һλ���������֣���ֻ�������ֺ�"."���

            //        Regex reg = new Regex(@"^[1-9](\.{0,1}[1-9][0-9]{0,2})*$");
            //        if (!reg.IsMatch(key))
            //        {
            //            errSb.AppendLine(key);
            //        }
            //    }
            //}


            //if (!string.IsNullOrEmpty(errSb.ToString()))
            //{
            //    errSb.Insert(0, "�������������\r\n");
            //    MessageBox.Show(errSb.ToString());
            //    return;
            //}

            #endregion




            ////�����������ĸ��
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)
            //{
            //   string Itemmaster = getstr(Form1.dataGridView4.Rows[iRow].Cells[10].Value);
            //    if (!MasterItemMasters.Contains(Itemmaster)) MasterItemMasters.Add(Itemmaster);
            //}
            ////��������ĸ��  ����ĸ���������Ӽ���ӵ�str�д���bom
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
            //            str += "'LineNum':'" + (iRow+1)*10 + "'" + ",";//�к�
            //            str += "'BOM':'" + MasterItemMaster + "'" + ",";//ĸ����Ʒ
            //            str += "'BOMC':'" + ComponentItem + "'" + ",";//��Ʒ�Ӽ�
            //            str += "'ComponentItemName':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[4].Value) + "'" + ",";//��Ʒ����
            //            str += "'ItemAttribute':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[11].Value) + "'" + ",";//��Ʒ��̬����
            //            str += "'BomUse':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[5].Value) + "'" + ",";//bom��;
            //            str += "'ItemCatagory':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[6].Value) + "'" + ",";//���Ϸ���
            //            str += "'UMO':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[7].Value) + "'" + ",";//������λ
            //            str += "'Qty':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[8].Value) + "'" + ",";//����
            //            str += "'Router':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[12].Value) + "'" + ",";//����·��
            //            str += "'Size':'" + getstr(Form1.dataGridView4.Rows[iRow].Cells[9].Value) + "'";//�ߴ�
            //            str += "},";
            //        }
            //    }
            //    str = str.Substring(0, str.Length - 1);
            //    str = "[" + str + "]";
            rtnmsg = PostCreatBom(strJson);
            //    //���ַ�������u9   ��һ���Ե��룬ֻ��һ�νӿ�
            //    //rtnmsg =PostCreatBom(str);
            //    //if (rtnmsg!="{\"d\":\"\"}")
            //    //{
            //    //    msg(rtnmsg);
            //    //}
            //}

            if (rtnmsg != "{\"d\":\"\"}")
            {
                if (rtnmsg.Contains("��Ŀ�Ѵ���"))
                {
                    return;
                }
                {
                    msg("����ʧ�ܣ�" + rtnmsg);
                }
            }
            else
            {
                int cgqty = 0, zzqty = 0, xnqty = 0, gyqty = 0;
                List<string> listcg = new List<string>();
                List<string> listZz = new List<string>();
                for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
                {
                    string attribute = getstr(dataGridView1.Rows[iRow].Cells["��Ʒ��̬����"].Value);
                    if (attribute == "�ɹ���")
                    {
                        if (!listcg.Contains(getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value)))
                        {
                            listcg.Add(getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value));
                        }
                        if (!listZz.Contains(getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value)))
                        {
                            listZz.Add(getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value));
                        }
                    }
                    else if (attribute == "�����")
                    {
                        if (!listZz.Contains(getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value)))
                        {
                            listZz.Add(getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value));
                        }
                        if (!listZz.Contains(getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value)))
                        {
                            listZz.Add(getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value));
                        }
                    }
                    else if (attribute == "����")
                    {
                        xnqty++;
                    }
                    else if (attribute == "����")
                    {
                        gyqty++;
                    }
                }
                cgqty = listcg.Count;
                zzqty = listZz.Count;
                msg("����bom�ɹ�,�����������" + zzqty + "��,�ɹ���" + cgqty + "��,�����" + xnqty + "��,���ռ�" + gyqty + "��");
            }
            #region<<demo>>
            //for (int iRow = 0; iRow < Form1.dataGridView4.Rows.Count; iRow++)//ѭ��dt,��ӡ����š����ӽڵ�
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
            //        msg("�������Ϊ���������ظ���" + key);
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
            //    //ȥ���ظ���Bom
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

            #region BOM�������⣺ϵͳ���д����ϵ�BOM�����뵼��İ汾�Ų�һ��ʱ����Ҫ��ʾ��߰汾�Ƕ���

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
            //            bomSb.AppendFormat("'{0}'�汾��{1}��ϵͳ���Ѵ���,�����Ƽ���ϵͳ�е���߰�Ϊ'{2}'", itemCode, versionCode, dt.Rows[0][0].ToString()).AppendLine();
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
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl�ļ� (*.xls)|*.xls|�����ļ� (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "�����ļ�����·��";//ΪExcel
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
        /// ���ļ���ȡ Stream 
        /// </summary> 
        public Stream FileToStream(string fileName)
        {
            // ���ļ� 
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // ��ȡ�ļ��� byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // �� byte[] ת���� Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// ��Ԫ����ʽ����
        /// </summary>
        /// <param name="dg"></param>
        private void initDataGrid(DataGridView dg)
        {
            dg.Columns["���"].Width = 50;
            dg.Columns["չ����"].Width = 60;
            dg.Columns["��������"].Width = 200;
            dg.Columns["����������λ"].Width = 80;
            dg.Columns["BOM��;"].Visible = false;
            dg.Columns["��������"].Visible = false;
            //dg.Columns["ĸ������"].Visible = false;
        }

        private void msg(string message)
        {
            MessageBox.Show(message);
        }


        #region <<ƴ�����ݷ����͵���U9����BOM�ӿ�>>
        /// <summary>
        /// ��ȡBOM�ṹ����
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
                string pInvCode = DataHelper.getStr(row.Cells["ĸ����Ʒ"].Value);
                string pInvDesc = DataHelper.getStr(row.Cells["ĸ����������"].Value);
                string pInvUnit = DataHelper.getStr(row.Cells["ĸ������������λ"].Value);
                string pInvQty = DataHelper.getStr(row.Cells["ĸ������"].Value);
                BomVO dto = dtos.Find(t => t.itemcode.Equals(pInvCode));
                if (dto == null)
                {
                    dto = new BomVO();
                    dto.itemcode = pInvCode;
                    dto.itemdesc = pInvDesc;
                    dto.unit = pInvUnit;
                    dto.qty = pInvQty;
                    dto.private2 = DataHelper.getStr(row.Cells["����·��"].Value);
                    dto.rows.Add(new BomLineVO(row));
                    dtos.Add(dto);
                }
                else
                {
                    dto.rows.Add(new BomLineVO(row));
                }
            }
            strJson = Newtonsoft.Json.JsonConvert.SerializeObject(dtos);//תjson�ַ���
            return strJson;
        }


        /// <summary>
        ///�Խӻ�ȡBOM�ṹ����
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        private string GetZJBOMJson(DataGridView dg)
        {
            string strJson = string.Empty;
            List<BomVO> dtos = new List<BomVO>();
            foreach (DataGridViewRow row in dg.Rows)
            {
                string pInvCode = DataHelper.getStr(row.Cells["ĸ����Ʒ"].Value);
                string pInvDesc = DataHelper.getStr(row.Cells["ĸ����������"].Value);
                string pInvUnit = DataHelper.getStr(row.Cells["ĸ������������λ"].Value);
                string pInvQty = DataHelper.getStr(row.Cells["ĸ������"].Value);
                BomVO dto = dtos.Find(t => t.itemcode.Equals(pInvCode));
                if (dto == null)
                {
                    dto = new BomVO();
                    dto.itemcode = pInvCode;
                    dto.itemdesc = pInvDesc;
                    dto.unit = pInvUnit;
                    dto.qty = pInvQty;
                    //dto.private2 = DataHelper.getStr(row.Cells["����·��"].Value);
                    dto.rows.Add(new BomLineVO(row));
                    dtos.Add(dto);
                }
                else
                {
                    dto.rows.Add(new BomLineVO(row));
                }
            }
            strJson = Newtonsoft.Json.JsonConvert.SerializeObject(dtos);//תjson�ַ���
            return strJson;
        }


        /// <summary>
        /// ����U9Bom����ӿ�
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
            string EntCode = getstr(Login.u9ContentHt["OrgCode"]);//��������֯����
            string UserCode = getstr(Login.u9ContentHt["UserCode"]);//�������û�����
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"01\",\"OrgCode\":\"" + EntCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"CreateBom\"}";
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
            str = "" + str.Replace("\"", "\\\"") + "";
            string EntCode = getstr(Login.u9ContentHt["OrgCode"]);//��������֯����
            string UserCode = getstr(Login.u9ContentHt["UserCode"]);//�������û�����
            string body = "{\"context\":{\"CultureName\":\"zh-CN\",\"EntCode\":\"01\",\"OrgCode\":\"" + EntCode + "\",\"UserCode\":\"" + UserCode + "\"},\"args\":\"" + str + "\",\"action\":\"ZJCreateBom\"}";
            //body.Replace("strorg", getstr(Login.u9ContentHt["OrgCode"]));
            //body.Replace("struser", getstr(Login.u9ContentHt["UserCode"]));
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
        #endregion

        #region <<comboBox1�¼�����>>
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
                if (zjItemCode == getstr(dataGridView1.Rows[iRow].Cells["���ϱ���"].Value))
                {
                    dataGridView1.Rows[iRow].Cells["��Ʒ��̬����"].Value = dataGridView1.CurrentCell.Value;
                }
            }
        }
        #endregion



        public static DataTable DataTable2(DataTable dt)
        {
            DataView dataView = new DataView(dt);
            string[] columnNames = new string[] { "ĸ����Ʒ", "���ϱ���" };
            DataTable dt2 = dataView.ToTable(true, "ĸ����Ʒ");
            return dt2;
        }

        /// <summary>
        /// datatableȥ��
        /// </summary>
        /// <param name="dtSource">��Ҫȥ�ص�datatable</param>
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
        /// �жϽ�֧���Ƿ����
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

        #region <<dataGridView1�¼�����>>

        /// <summary>
        /// �޸����������ֶ��޸�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.ColumnIndex;
            string value = dataGridView1[e.ColumnIndex, e.RowIndex].Value?.ToString();

            string mjItemCode = dataGridView1[3, e.RowIndex].Value?.ToString();

            //����������������޸�  ������µ�form2
            if (e.ColumnIndex == 8)
            {
                Form2 form2 = new Form2(value);
                form2.Show();
            }
            else if (e.ColumnIndex == 15)
            {
                for (int iRow = 0; iRow < dataGridView1.Rows.Count; iRow++)
                {
                    if (mjItemCode == getstr(dataGridView1.Rows[iRow].Cells["ĸ����Ʒ"].Value))
                    {
                        dataGridView1.Rows[iRow].Cells["����·��"].Value = value;
                    }
                }
            }
        }

        /// <summary>
        /// ��Ʒ��̬�����ֶ��޸�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {

            DataGridViewCell cell = dataGridView1.CurrentCell;
            if (cell == null) return;

            DataGridViewColumn column = cell.OwningColumn;

            //�����Ҫ��ʾ�����б���еĻ�
            if (column.Name.Equals("��Ʒ��̬����"))
            {

                int columnIndex = dataGridView1.CurrentCell.ColumnIndex;
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                Point p = dataGridView1.Location;
                Rectangle rect = dataGridView1.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                comboBox1.Left = rect.Left + p.X + 3;
                comboBox1.Top = rect.Top + p.Y + dataGridView1.ColumnHeadersHeight + rect.Height;
                comboBox1.Width = rect.Width;
                comboBox1.Height = rect.Height;
                //����Ԫ���������ʾΪ�����б�ĵ�ǰ��
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

        //    //�����Ҫ��ʾ�����б���еĻ�
        //    if (column.Name.Equals("��������"))
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

        #region <<��ֹNULL�쳣>>

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

        #endregion

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //����tabpage text
            this.tabPage1.Text = "���ϵ�������";
            this.tabPage1.Refresh();
            //���ļ�
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl�ļ� (*.xls)|*.xls|�����ļ� (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "�����ļ�����·��";//ΪExcel
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
            if (this.tabPage1.Text != "���ϵ�������")
            {
                MessageBox.Show("����������Ч");
                return;
            }
            Thread TD = new Thread(ShowProgressForm);
            TD.Start();
            try
            {
                DataTable itemDt = this.dataGridView1.DataSource as DataTable;
                if (itemDt == null || itemDt.Rows.Count <= 0)
                    return;
                string strJson = JsonConvert.SerializeObject(itemDt);
                string strResult = HttpClientHelper.DoPost(strJson, "SG_BatchCreatItemMaster");
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(strResult);
                int k = 0;//������
                Int32 records = 0;//�ɹ������¼��
                indexLst = new List<int>();
                if (!itemDt.Columns.Contains("�����¼"))
                    itemDt.Columns.Add("�����¼");
                foreach (DataRow row in itemDt.Rows)
                {
                    DataRow[] newRow = dt.Select($"ID={k}");
                    if (newRow.Length > 0)
                    {
                        if (Convert.ToBoolean(newRow[0]["IsSuccess"]))
                        {
                            //���ϴ���ʱ���ռ�
                            if (newRow[0]["Error"].Equals("�Ϻ��Ѵ���"))
                            {
                                indexLst.Add(k);
                            }
                            else
                                records++;
                            row["���ϱ���"] = newRow[0]["code"].ToString();
                        }
                        row["�����¼"] = newRow[0]["Error"];
                    }

                    k++;
                }

                this.dataGridView1.DataSource = itemDt;


                TD.Abort();

                MessageBox.Show($"�ܼƵ���{itemDt.Rows.Count},�ɹ�{records}��");
            }
            catch (Exception ex)
            {
                TD.Abort();
            }
            //������ֵ
            //MessageBox.Show(strResult);
        }
        private List<Int32> indexLst = new List<int>();//������ϵͳ���Ѵ��ڵ���

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (this.tabPage1.Text == "���ϵ�������")
            {
                if (e.ColumnIndex > -1)
                {
                    DataGridViewColumn ThisCL = dataGridView1.Columns[e.ColumnIndex];
                    if (ThisCL.Name.Equals("���ϱ���") && indexLst.Contains(e.RowIndex))
                        e.CellStyle.ForeColor = Color.Red;
                }
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            //����tabpage text
            this.tabPage1.Text = "�Խ������嵥����";
            this.tabPage1.Refresh();
            //���ļ�
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Execl�ļ� (*.xls)|*.xls|�����ļ� (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            //openFileDialog.CreatePrompt = true;
            openFileDialog.Title = "�����ļ�����·��";//ΪExcel
            openFileDialog.FileName = null;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string strFile = openFileDialog.FileName;
            if (string.IsNullOrEmpty(strFile))
                return;
            ExcelHelper excelHelper = new ExcelHelper(openFileDialog.FileName);
            DataTable excelDt = excelHelper.ZjExcelToDataTable(1);
            //ƴ��BOM��ʽ����
            DataTable bomexcelDt = excelHelper.ZjExcelToBOMDataTable(excelDt);
            this.dataGridView1.DataSource = bomexcelDt;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (this.tabPage1.Text != "�Խ������嵥����")
            {
                MessageBox.Show("����������Ч");
                return;
            }
            DataGridView dg = dataGridView1;
            if (dg == null || dg.Rows.Count <= 0) return;

            //��һ��  DataTalbeתBOM�ṹ
            string strJson = GetZJBOMJson(dg);

            string CommandType = "CreateBOM";
            List<www.ufida.org.EntityData.UFIDAU9CustCommonAPISVDocDTOData> dictdtos = new List<UFIDAU9CustCommonAPISVDocDTOData>();
            Dictionary<string, Hashtable> dic = new Dictionary<string, Hashtable>();
            string key = string.Empty;
            Hashtable damicht = new Hashtable();
            List<string> MasterItemMasters = new List<string>(); ///������Ҫ������bomĸ������
            string rtnmsg = "";
            rtnmsg = ZJPostCreatBom(strJson);

            if (rtnmsg != "{\"d\":\"\"}")
            {
                if (rtnmsg.Contains("��Ŀ�Ѵ���"))
                {
                    return;
                }
                {
                    msg("����ʧ�ܣ�" + rtnmsg);
                }
            }
            else
            {
                msg("����bom�ɹ�");
            }
        }
    }
}