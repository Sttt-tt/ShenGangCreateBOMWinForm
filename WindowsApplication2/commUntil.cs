using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace WindowsApplication2
{
    class commUntil
    {
        //获得数据库字符串
        public static string GetConntionSetting(string strSouce)
        {
            string strConnSetting = "";
            try
            {
                XmlDocument xDoc = new XmlDocument();
                string strPath = Application.StartupPath + "\\导入工具.exe.config";
                xDoc.Load(strPath);
                XmlNode xNode = xDoc.SelectSingleNode("//appSettings");
                XmlElement xElement = (XmlElement)xNode.SelectSingleNode("//add[@key='" + strSouce + "']");
                strConnSetting = xElement.Attributes["value"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            return strConnSetting;
        }

        /// <summary>
        /// 设置Config中KEY VALUE的值
        /// </summary>
        /// <param name="AppKey">key</param>
        /// <param name="AppValue">value</param>
        public static void SetValue(string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            string strPath = Application.StartupPath + "\\导入工具.exe.config";
            xDoc.Load(strPath);
            XmlNode xNode;
            XmlElement xElement;
            XmlElement xElement1;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElement = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElement != null)
            {
                //string[] arr = AppValue.Split(';');
                //string str = AppValue.Substring(AppValue.LastIndexOf("Password=") + 9, AppValue.Length - AppValue.LastIndexOf("Password=") - 9);
                //DES des = new DES();
                //string s1 = des.Md5Encrypt(str);
                //AppValue = arr[0] + ";" + arr[1] + ";" + arr[2] + ";" + "Password=" + s1 + " ";
                xElement.SetAttribute("value", AppValue);
            }
            else
            {
                xElement1 = xDoc.CreateElement("add");
                xElement1.SetAttribute("key", AppKey);
                xElement1.SetAttribute("value", AppValue);
                xNode.AppendChild(xElement1);
            }
            xDoc.Save(strPath);
        }

        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encrypt(string source)
        {
            MD5 md = MD5.Create();
            byte[] bytes = new UnicodeEncoding().GetBytes(source);
            return Convert.ToBase64String(md.ComputeHash(bytes));
        }
    }
}
