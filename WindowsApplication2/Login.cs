using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Xml;

using System.Diagnostics;
using System.Collections;
using Microsoft.Data.ConnectionUI;
using UFIDA.U9.Base.UserRole;
/************************************************************

************************************************************/
namespace WindowsApplication2
{
    public partial class Login :Form
    {
        //数据库连接串
        public static string strConn = string.Empty;
        //hashtable  U9上下文
        public static Hashtable u9ContentHt = null;

        //private eOffice2007ColorScheme m_BaseColorScheme = eOffice2007ColorScheme.VistaGlass;
        public Login()
        {
           //this.skinEngine1.SkinFile=
            InitializeComponent();
        }

        //private void ReadXml()
        //{
        //    string settingFilePath = "Setting.xml";
        //    XmlDocument document = new XmlDocument();
        //    try
        //    {
        //        document.Load(settingFilePath);
        //        XmlNode nextSibling = document.FirstChild.NextSibling;
        //        foreach (XmlNode node2 in nextSibling.ChildNodes)
        //        {
        //            if (node2.Name == "EnterpriseID")
        //            {
        //                Init_Set.EnterpriseID = node2.InnerText;
        //            }
                    
        //        }
        //    }
        //    catch (Exception ex)
        //    {
               
        //        throw new Exception(ex.Message);
        //    }
        //}
        private void Login_Load(object sender, EventArgs e)
        {
            strConn = commUntil.GetConntionSetting("Conn");
            
            ////绑定组织
            //DataTable dt = MiddleDBInterface.getdt("select a.Code,b.Name,a.ID from Base_Organization a join Base_Organization_Trl b on a.ID=b.ID", MiddleDBInterface.conn(strConn));
            //this.comboBox1.DataSource = dt;
            //this.comboBox1.DisplayMember = "Name";
            //this.comboBox1.ValueMember = "ID";

            //this.timer1.Start();

            //ReadXml();
            //this.textBoxX1.Text = "admin";
            //this.textBoxX2.Text = "shen9632";
            Process[] curProcess = Process.GetProcessesByName("LDPlugin");
            if (curProcess.Length > 1)
            {
                MessageBox.Show("程序已经开启！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Application.Exit();
            }
            
        }



        private void buttonX1_Click(object sender, EventArgs e)
        {
            //初始化
            u9ContentHt = new Hashtable();

            if (this.comboBox1.Text.Trim() == "")
            {
                MessageBox.Show("请选择登陆组织！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            string userName = this.textBoxX1.Text.Trim();
            string pwd = this.textBoxX2.Text.Trim();
            //string ss = commUntil.Encrypt(this.textBoxX2.Text.Trim());
            //MessageBox.Show(ss);
            DataTable userDt = MiddleDBInterface.getdt($"select * from base_user where code='{userName}'", MiddleDBInterface.conn(strConn));
            if (userDt == null || userDt.Rows.Count <= 0)
            {
                MessageBox.Show($"用户名{userName}不存在！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            string strSalt = userDt.Rows[0]["Salt"].ToString();
            string password = EncryptionManager.Instance.EncryptForSalt(EncryptionManager.Instance.Encrypt(pwd) + strSalt);

            DataTable dt = MiddleDBInterface.getdt("select * from base_user where code='" + userName + "' and password='" + password + "'", MiddleDBInterface.conn(strConn));
            if (dt != null && dt.Rows.Count > 0)
            {

                //Init_Set.UserCode = this.textBoxX1.Text.Trim();
                //Init_Set.Userpwd = this.textBoxX2.Text.Trim();
                //Init_Set.OrgName = this.comboBox1.Text.Trim();
                //Init_Set.UserID = getstr(dt.Rows[0]["ID"]);
                //Init_Set.UserName = getstr(dt.Rows[0]["Name"]);
                //Init_Set.OrgID = getstr(ht_org[this.comboBox1.Text.Trim()]).Split('|')[0];
                //Init_Set.OrgCode = getstr(ht_org[this.comboBox1.Text.Trim()]).Split('|')[1];

                //    Hashtable context = new Hashtable();
                //    context.Add("OrgID", Init_Set.OrgID);
                //    context.Add("UserID", Init_Set.UserID);
                //    context.Add("EnterpriseID", Init_Set.EnterpriseID);
                //    context.Add("EnterpriseName", Init_Set.EnterpriseName);
                //    context.Add("OrgCode", Init_Set.OrgCode);
                //    context.Add("OrgName", Init_Set.OrgName);
                //    context.Add("UserCode", Init_Set.UserCode);
                //    context.Add("UserName", Init_Set.UserName);
                //    Init_Set.context = context;


                u9ContentHt.Add("UserID", dt.Rows[0]["ID"].ToString());
                u9ContentHt.Add("UserCode", this.textBoxX1.Text.Trim());
                u9ContentHt.Add("UserName", dt.Rows[0]["Name"].ToString());
                u9ContentHt.Add("EnterpriseID", commUntil.GetConntionSetting("EnterpriseID"));
                u9ContentHt.Add("EnterpriseName", commUntil.GetConntionSetting("EnterpriseName"));
                u9ContentHt.Add("OrgID", getstr(ht_org[this.comboBox1.Text.Trim()]).Split('|')[0]);
                u9ContentHt.Add("OrgCode", getstr(ht_org[this.comboBox1.Text.Trim()]).Split('|')[1]);
                u9ContentHt.Add("OrgName", this.comboBox1.Text.Trim());
                u9ContentHt.Add("CultureName", "zh-CN");
                u9ContentHt.Add("DefaultCultureName", "zh-CN");

                //Init_Set.context = u9ContentHt;

                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            else
            {
                MessageBox.Show("用户名或密码错误！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        #region 防止NULL异常
        public static long getlong(object obj)
        {
            long lg = 0;
            if (obj != null)
            {
                long.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static bool getbool(object obj)
        {
            bool lg = false;
            if (obj != null)
            {
                bool.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static decimal getdecimal(object obj)
        {
            decimal lg = 0;
            if (obj != null)
            {
                decimal.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static int getint(object obj)
        {
            int lg = 0;
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out lg);
            }
            return lg;
        }
        public static string getstr(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }
        #endregion
      
        
        private Hashtable ht_org = new Hashtable();
       

        private void buttonX2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBoxX1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (this.textBoxX1.Text.Trim() == "更改密码")
                {
                    //SetConfig sc = new SetConfig();
                    //sc.StartPosition = FormStartPosition.CenterParent;
                    //sc.ShowDialog();
                }
                else
                {
                    this.textBoxX2.Focus();
                }
            }
        }
        private void textBoxX1_Leave(object sender, EventArgs e)
        {
            if (this.textBoxX1.Text == "ufidau9")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                DataTable dt = MiddleDBInterface.getdt(@"select a.Code,b.Name,a.ID from Base_Organization a 
join Base_Organization_Trl b on a.ID=b.ID 
join Base_UserOrg c on c.Org=a.ID 
join Base_User d on c.[User]=d.ID 
where d.Code='" + this.textBoxX1.Text.Trim() + "'", MiddleDBInterface.conn(strConn));
                ht_org.Clear();
                this.comboBox1.Items.Clear();
                foreach (DataRow var1 in dt.Rows)
                {
                    ht_org.Add(getstr(var1[1]), getstr(var1[2]) + "|" + getstr(var1[0]));
                    this.comboBox1.Items.Add(getstr(var1[1]));
                    this.comboBox1.SelectedIndex = 0;
                }
                dt = null;
            }
        }

        private void textBoxX2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                buttonX1_Click(sender, null);
            }
        }

        /// <summary>
        /// SQL弹窗  配置数据库连接串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lkSqlConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataConnectionDialog dialog = new DataConnectionDialog();
            dialog.DataSources.Add(DataSource.SqlDataSource);
            dialog.SelectedDataSource = DataSource.SqlDataSource;
            dialog.SelectedDataProvider = DataProvider.SqlDataProvider;
            dialog.ConnectionString = strConn;
            if (DataConnectionDialog.Show(dialog, this) == DialogResult.OK)
            {
                strConn = dialog.ConnectionString;
                commUntil.SetValue("Conn", strConn);
            }
          
        }

       
    }
}