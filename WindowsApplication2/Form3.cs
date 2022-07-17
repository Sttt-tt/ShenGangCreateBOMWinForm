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

namespace WindowsApplication2
{
    public partial class Form3 : Form
    {
        public delegate void form3UserControlValue(string controlname, string code, string name, string cl);
        public form3UserControlValue form3UserControls;
        private string form1contorlname3 = "";
        public Form3(string contorlname)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.dataGridView1.DataSource = getItemMasters(itemvalue, itemCz);
            form1contorlname3 = contorlname;
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
            form3UserControls(form1contorlname3, row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());
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

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripTextBox1.Text))
            {
                MessageBox.Show("请先输入物料规格");
            }

            if (string.IsNullOrEmpty(toolStripTextBox2.Text))
            {
                MessageBox.Show("请先输入物料材质");
            }
            string sql = string.Format(@"select Code 料号,Name+SPECS 品名,DescFlexField_PrivateDescSeg1 材料 from CBO_ItemMaster where DescFlexField_PrivateDescSeg1 = '{0}' 
                                        and SPECS='{1}' group by Code,name,SPECS,DescFlexField_PrivateDescSeg1
                                        ", toolStripTextBox2.Text, toolStripTextBox1.Text);
            DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            this.dataGridView1.DataSource = dt;
        }
    }
}
