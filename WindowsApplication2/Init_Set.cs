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

        public static string portal_URL = "192.168.110.117";//U9 ��ƷĿ¼
        public static long Istatus_id = 1000703050000119;
        public static string OrgID = "1001403070394967";//��֯ID
        public static string OrgCode = "01";//��֯ID
        public static string UserID = "1001403070395189";//�û�ID
        public static string UserCode = "admin";//�û�ID
        public static string UserName = "";//�û�ID
        public static string OrgName = "";//�û�ID
        public static string EnterpriseID = "01";      //��ҵ����
        public static string EnterpriseName = "��ʽ���ݻ�ԭ";//��ҵ����
      

        public static string Userpwd = "";//��ҵ����
        public static string ls_conn = "user id=sa;data source=10.1.3.10;Connect Timeout=30;initial catalog=v28std;password=ufida@123";//u8
        //public static string ls_conn = "user id=sa;data source=10.0.30.20;Connect Timeout=30;initial catalog=zshy;password=0512Boamax";//u8

        public static string middledb_conn = "Provider=SQLOLEDB; user id=sa;data source=127.0.0.1;Connect Timeout=30;initial catalog=u9test;password=123456";//u8
        public static string parent_code = "";  //ĸ������
        public static string parent_name = "";  //ĸ������
        public static long parent_qty = 0;
        public static string parent_name_en = "";  //ĸ��Ӣ������
        public static string parent_unit = "S002";//ĸ��������λ

        public static string parent_describe = "";//ĸ������
        public static string parent_describe_en = "";  //ĸ��Ӣ������

        public static long parent_unit_keyid = 1000703050000119;
        public static int qtybyexec = 1;
        public static string Version_code = "";
        public static string itemsql = "select distinct  st_buy as ��Ʒ��̬ , 1 as MRP�ƻ�����,a.cInvCode as ��Ʒ���,a.Object_name as ��Ʒ����,a.st_clgg as ���,c.Code as �������λ from INTERFACE_PART  a left join base_uom_trl b on a.st_cldw=b.name left join base_uom c on b.id=c.id where a.Status=0 and c.Code is not null";
        public static string bomsql = "select  'A01' as BOM�汾��,ParentU9Code as ĸ������,ComponentU9Code as �Ӽ�����,BaseQtyN as �Ӽ����� from INTERFACE_BOM where Status=0  order by ParentCode,SortSeq";
        public static long child_unit_keyid = 1000703050000119;
        public static string cus_code = "";//�ͻ�
        public static Hashtable context;

    }
}
