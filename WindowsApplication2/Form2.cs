using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsApplication2
{
    /// <summary>
    /// 物料修改窗口
    /// </summary>
    public partial class Form2 : Form
    {

        public delegate void form2UserControlValue(string controlname, string code, string name);
        public form2UserControlValue form2UserControls;
        private string form1contorlname = "";
        public Form2(string contorlname)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.dataGridView1.DataSource = getItemMasters(itemvalue);
            form1contorlname = contorlname;

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
            form2UserControls(form1contorlname, row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
            this.Close();
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
            string sql = string.Format(@"select '0000000000'+Code 料号,Name +'_'+DescFlexField_PrivateDescSeg1+'_'+SPECS 品名 from CBO_ItemMaster where DescFlexField_PrivateDescSeg1 = '{0}' 
                                        and SPECS='{1}' group by Code,name,SPECS,DescFlexField_PrivateDescSeg1
                                        ", toolStripTextBox2.Text, toolStripTextBox1.Text);
            DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            this.dataGridView1.DataSource = dt;

        }
    }
}
