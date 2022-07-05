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
        public Form3(string itemvalue, string itemCz)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.dataGridView1.DataSource = getItemMasters(itemvalue, itemCz);
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
            int index = this.dataGridView1.CurrentRow.Index - 1;//由于按回车行索引会自动跳下下一行，所以取当前索引的上一行
            DataGridViewRow row2 = this.dataGridView1.Rows[index];
            this.dataGridView1.Rows[index].Cells[8].Selected = true;
            this.dataGridView1.Rows[index + 1].Cells[8].Selected = false;
            row2.Cells["物料编码"].Value = row.Cells[0].Value;//0是编码，1是描述
            row2.Cells["物料描述"].Value = row.Cells[1].Value;//0是编码，1是描述
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
    }
}
