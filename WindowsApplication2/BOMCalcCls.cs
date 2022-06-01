using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data;

namespace WindowsApplication2
{
    public class BOMCalcCls
    {
        public static decimal getimQty(string str)
        {
            decimal i = 9999999m;
            Hashtable nt_tmp=new Hashtable();
            Hashtable imht = getimht(str, nt_tmp);
            string invcodes = "'" + str + "',";
            foreach (string cinvcode in imht.Keys)
            {
                invcodes += "'" + cinvcode + "',";
            }
            invcodes = invcodes.Trim(',');
            if (invcodes == "") return 0;
            Hashtable htqty = new Hashtable();
            //œ÷¥Ê¡ø
            DataTable dt = SQLHelper.getdt("select a.cInvCode,sum(a.iQuantity) as iQuantity from currentstock a where a.cInvCode in (" + invcodes + ") group by a.cInvCode", SQLHelper.conn(InitSet.connstr));
            foreach (DataRow dr in dt.Rows)
            {
                htqty.Add(dr["cInvCode"].ToString(), Convert.ToDecimal(dr["iQuantity"]));
            }
            nt_tmp = new Hashtable();
            Hashtable midht = getmidht(str, nt_tmp);
            foreach (string midinvcode in midht.Keys)
            {
                nt_tmp = new Hashtable();
                Hashtable ht = getht(midinvcode, nt_tmp);
                foreach (string imstr in ht.Keys)
                {
                    if (htqty.ContainsKey(imstr))
                    {
                        htqty[imstr] = Convert.ToDecimal(htqty[imstr]) + Convert.ToDecimal(ht[imstr]) * Convert.ToDecimal(htqty[midinvcode]);
                    }
                }
            }
            nt_tmp = new Hashtable();
            Hashtable htfinnalny = getht(str, nt_tmp);
            foreach (string imfinnal in htfinnalny.Keys)
            {
                if (Math.Floor(Convert.ToDecimal(htqty[imfinnal]) / Convert.ToDecimal(htfinnalny[imfinnal])) < i)
                {
                    i = Math.Floor(Convert.ToDecimal(htqty[imfinnal]) / Convert.ToDecimal(htfinnalny[imfinnal]));
                }
            }
            if (i == 9999999m)
            {
                i = 0m;
            }
            return i + Convert.ToDecimal(htqty[str]);
        }
        private static Hashtable getht(string  cinvcode, Hashtable hs)
        {
            DataTable dt;
            DataTable dt_sub;
            dt = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + cinvcode + "'", SQLHelper.conn(InitSet.connstr));
            
            if (dt.Rows.Count <= 0)
            {
            }
            else
            {
                foreach (DataRow var in dt.Rows)
                {
                    dt_sub = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + var["S_cInvCode"].ToString() + "'", SQLHelper.conn(InitSet.connstr));
                    if (dt_sub.Rows.Count > 0)
                    {

                        getht(Convert.ToString(var["S_cInvCode"]), hs);
                    }
                    else
                    {
                        //if (Convert.ToInt32(var["ItemFormAttribute"]) != 9) continue;

                        if (!hs.ContainsKey(Convert.ToString(var["S_cInvCode"])))
                        {
                            hs.Add(Convert.ToString(var["S_cInvCode"]), Convert.ToDecimal(var["Qty"]));
                        }
                        else
                        {
                            hs[Convert.ToString(var["S_cInvCode"])] = Convert.ToDecimal(hs[Convert.ToString(var["S_cInvCode"])]) + Convert.ToDecimal(var["Qty"]);
                        }
                    }
                }
            }
            return hs;
        }
        private static Hashtable getimht(string cinvcode, Hashtable hs)
        {
            DataTable dt;
            DataTable dt_sub;
            dt = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + cinvcode + "'", SQLHelper.conn(InitSet.connstr));

            if (dt.Rows.Count <= 0)
            {
            }
            else
            {
                foreach (DataRow var in dt.Rows)
                {
                    dt_sub = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + var["S_cInvCode"].ToString() + "'", SQLHelper.conn(InitSet.connstr));
                    if (dt_sub.Rows.Count > 0)
                    {
                        if (!hs.ContainsKey(Convert.ToString(var["S_cInvCode"])))
                        {
                            hs.Add(Convert.ToString(var["S_cInvCode"]), Convert.ToDecimal(var["Qty"]));
                        }
                        else
                        {
                            hs[Convert.ToString(var["S_cInvCode"])] = Convert.ToDecimal(hs[Convert.ToString(var["S_cInvCode"])]) + Convert.ToDecimal(var["Qty"]);
                        }
                        getht(Convert.ToString(var["S_cInvCode"]), hs);
                    }
                    else
                    {
                        //if (Convert.ToInt32(var["ItemFormAttribute"]) != 9) continue;

                        if (!hs.ContainsKey(Convert.ToString(var["S_cInvCode"])))
                        {
                            hs.Add(Convert.ToString(var["S_cInvCode"]), Convert.ToDecimal(var["Qty"]));
                        }
                        else
                        {
                            hs[Convert.ToString(var["S_cInvCode"])] = Convert.ToDecimal(hs[Convert.ToString(var["S_cInvCode"])]) + Convert.ToDecimal(var["Qty"]);
                        }
                    }
                }
            }
            return hs;
        }

        private static Hashtable getmidht(string cinvcode, Hashtable hs)
        {
            DataTable dt;
            DataTable dt_sub;
            dt = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + cinvcode + "'", SQLHelper.conn(InitSet.connstr));

            if (dt.Rows.Count <= 0)
            {
            }
            else
            {
                foreach (DataRow var in dt.Rows)
                {
                    dt_sub = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + var["S_cInvCode"].ToString() + "'", SQLHelper.conn(InitSet.connstr));
                    if (dt_sub.Rows.Count > 0)
                    {
                        if (!hs.ContainsKey(Convert.ToString(var["S_cInvCode"])))
                        {
                            hs.Add(Convert.ToString(var["S_cInvCode"]), Convert.ToDecimal(var["Qty"]));
                        }
                        else
                        {
                            hs[Convert.ToString(var["S_cInvCode"])] = Convert.ToDecimal(hs[Convert.ToString(var["S_cInvCode"])]) + Convert.ToDecimal(var["Qty"]);
                        }
                        getmidht(Convert.ToString(var["S_cInvCode"]), hs);
                    }
                    else
                    {
                        //if (Convert.ToInt32(var["ItemFormAttribute"]) != 9) continue;

                        //if (!hs.ContainsKey(Convert.ToString(var["S_cInvCode"])))
                        //{
                        //    hs.Add(Convert.ToString(var["S_cInvCode"]), Convert.ToDecimal(var["Qty"]));
                        //}
                        //else
                        //{
                        //    hs[Convert.ToString(var["S_cInvCode"])] = Convert.ToDecimal(hs[Convert.ToString(var["S_cInvCode"])]) + Convert.ToDecimal(var["Qty"]);
                        //}
                    }
                }
            }
            return hs;
        }

        public static ArrayList getarrlst(string cinvcode, ArrayList arr_lst, int level, decimal priovsqty)
        {
            DataTable dt;
            DataTable dt_sub;
            dt = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty,e.cInvName,isnull((select SUM(stock.iQuantity) from currentstock stock where stock.cInvCode=d.InvCode),0) as StoreQty,(select top 1 suba.Description from sfc_proutingdetail  suba join v_sfc_proutingpart_rpt subb on suba.PRoutingId=subb.PRoutingId where suba.OpSeq=b.OpSeq  and subb.cInvCode=c.InvCode) as OpDescription,b.OpSeq from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId join Inventory e on d.InvCode=e.cInvCode   where c.InvCode='" + cinvcode + "'", SQLHelper.conn(InitSet.connstr));

            if (dt.Rows.Count <= 0)
            {
            }
            else
            {
                foreach (DataRow var in dt.Rows)
                {
                    dt_sub = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + var["S_cInvCode"].ToString() + "'", SQLHelper.conn(InitSet.connstr));
                    if (dt_sub.Rows.Count > 0 && var["S_cInvCode"].ToString().Substring(0,3)!="202")
                    {
                        //Hashtable ht = new Hashtable();
                        //ht.Add("Level", level);
                        //ht.Add("S_cInvCode",Convert.ToString(var["S_cInvCode"]));
                        //ht.Add("StoreQty", Convert.ToString(var["StoreQty"]));
                        //ht.Add("cInvName", Convert.ToString(var["cInvName"]));
                        //ht.Add("BaseQty", Convert.ToDecimal(var["Qty"])*priovsqty);
                        //arr_lst.Add(ht);
                        getarrlst(Convert.ToString(var["S_cInvCode"]), arr_lst, level + 1, Convert.ToDecimal(var["Qty"]) * priovsqty);
                    }
                    else
                    {
                        Hashtable ht = new Hashtable();
                        ht.Add("Level", level);
                        ht.Add("S_cInvCode", Convert.ToString(var["S_cInvCode"]));
                        ht.Add("BaseQty", Convert.ToDecimal(var["Qty"]) * priovsqty);
                        ht.Add("StoreQty", Convert.ToString(var["StoreQty"]));
                        ht.Add("OpDescription", Convert.ToString(var["OpDescription"]));
                        ht.Add("OpSeq", Convert.ToString(var["OpSeq"]));
                        ht.Add("cInvName", Convert.ToString(var["cInvName"]));
                        arr_lst.Add(ht);
                    }
                }
            }
            return arr_lst;
        }
        public static ArrayList getarrlst(string cinvcode, ArrayList arr_lst, int level, decimal priovsqty,ArrayList arr_tmp)
        {
            DataTable dt;
            DataTable dt_sub;
            dt = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty,e.cInvName from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId join Inventory e on d.InvCode=e.cInvCode   where c.InvCode='" + cinvcode + "'", SQLHelper.conn(InitSet.connstr));

            if (dt.Rows.Count <= 0)
            {
            }
            else
            {
                foreach (DataRow var in dt.Rows)
                {
                    dt_sub = SQLHelper.getdt("select a.ParentId,c.InvCode as P_cInvCode,b.ComponentId,d.InvCode as S_cInvCode,b.BaseQtyN/b.BaseQtyD as Qty from bom_parent a join bom_opcomponent b on a.BomId=b.BomId join bas_part c on a.ParentId=c.PartId join bas_part d on b.ComponentId=d.PartId  where c.InvCode='" + var["S_cInvCode"].ToString() + "'", SQLHelper.conn(InitSet.connstr));
                    if (dt_sub.Rows.Count > 0 && var["S_cInvCode"].ToString().Substring(0, 3) != "202")
                    {
                        //Hashtable ht = new Hashtable();
                        //ht.Add("Level", level);
                        //ht.Add("S_cInvCode",Convert.ToString(var["S_cInvCode"]));
                        //ht.Add("StoreQty", Convert.ToString(var["StoreQty"]));
                        //ht.Add("cInvName", Convert.ToString(var["cInvName"]));
                        //ht.Add("BaseQty", Convert.ToDecimal(var["Qty"])*priovsqty);
                        //arr_lst.Add(ht);
                        getarrlst(Convert.ToString(var["S_cInvCode"]), arr_lst, level + 1, Convert.ToDecimal(var["Qty"]) * priovsqty, arr_tmp);
                    }
                    else
                    {
                        Hashtable ht = new Hashtable();
                        ht.Add("Level", level);
                        ht.Add("S_cInvCode", Convert.ToString(var["S_cInvCode"]));
                        ht.Add("BaseQty", Convert.ToDecimal(var["Qty"]) * priovsqty);
                        //ht.Add("StoreQty", Convert.ToString(var["StoreQty"]));
                        //ht.Add("OpDescription", Convert.ToString(var["OpDescription"]));
                        //ht.Add("OpSeq", Convert.ToString(var["OpSeq"]));
                        ht.Add("cInvName", Convert.ToString(var["cInvName"]));
                        arr_lst.Add(ht);
                        //if(Convert.ToString(var["S_cInvCode"])=="10201000022")
                        //{
                        //arr_tmp.Add(Convert.ToString(var["S_cInvCode"])+" "+cinvcode+" "+SQLHelper.getstr(Convert.ToDecimal(var["Qty"]) * priovsqty));
                        //}
                    }
                }
            }
            return arr_lst;
        }

    }
}
