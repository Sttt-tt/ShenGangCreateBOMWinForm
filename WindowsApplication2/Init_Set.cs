using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace WindowsApplication2
{
    class Init_Set
    {
        public static string File_path = AppDomain.CurrentDomain.BaseDirectory;

        public static string portal_URL = "192.168.110.117";//U9 产品目录
        public static long Istatus_id = 1000703050000119;
        public static string OrgID = "1001403070394967";//组织ID
        public static string OrgCode = "01";//组织ID
        public static string UserID = "1001403070395189";//用户ID
        public static string UserCode = "admin";//用户ID
        public static string UserName = "";//用户ID
        public static string OrgName = "";//用户ID
        public static string EnterpriseID = "01";      //企业编码
        public static string EnterpriseName = "正式数据还原";//企业名称
      

        public static string Userpwd = "";//企业名称
        public static string ls_conn = "user id=sa;data source=10.1.3.10;Connect Timeout=30;initial catalog=v28std;password=ufida@123";//u8
        //public static string ls_conn = "user id=sa;data source=10.0.30.20;Connect Timeout=30;initial catalog=zshy;password=0512Boamax";//u8

        public static string middledb_conn = "Provider=SQLOLEDB; user id=sa;data source=127.0.0.1;Connect Timeout=30;initial catalog=u9test;password=123456";//u8
        public static string parent_code = "";  //母件编码
        public static string parent_name = "";  //母件名称
        public static long parent_qty = 0;
        public static string parent_name_en = "";  //母件英文名称
        public static string parent_unit = "S002";//母件计量单位

        public static string parent_describe = "";//母件描述
        public static string parent_describe_en = "";  //母件英文名称

        public static long parent_unit_keyid = 1000703050000119;
        public static int qtybyexec = 1;
        public static string Version_code = "";
        public static string itemsql = "select distinct  st_buy as 料品形态 , 1 as MRP计划方法,a.cInvCode as 料品编号,a.Object_name as 料品名称,a.st_clgg as 规格,c.Code as 库存主单位 from INTERFACE_PART  a left join base_uom_trl b on a.st_cldw=b.name left join base_uom c on b.id=c.id where a.Status=0 and c.Code is not null";
        public static string bomsql = "select  'A01' as BOM版本号,ParentU9Code as 母件编码,ComponentU9Code as 子件编码,BaseQtyN as 子件用量 from INTERFACE_BOM where Status=0  order by ParentCode,SortSeq";
        public static long child_unit_keyid = 1000703050000119;
        public static string cus_code = "";//客户
        public static Hashtable context;

    }
}
