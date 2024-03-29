﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsApplication2.Helper;

namespace WindowsApplication2.VO
{
    class BomVOZJ
    {
        public string itemcode { get; set; }//母件料号
        public string itemdesc { get; set; }//母件描述
        public string unit { get; set; }//单位
        public string qty { get; set; }//用量
        public string material { get; set; }//材质
        public string gbbm { get; set; }//国标编码
        public string formAttribute { get; set; }//料品形态
        public string private2 { get; set; }//私有字段2 母件路线
        public string private3 { get; set; }//私有字段3 母件备注
        public List<BomLineVOZJ> rows = new List<BomLineVOZJ>();
    }
    

    class BomLineVOZJ
    {
        public string itemcode { get; set; }//子件料号
        public string itemdesc { get; set; }//子件描述  名称+材质+规格型号
        public string unit { get; set; }//单位
        public decimal qty { get; set; }//用量
        public string material { get; set; }//材质
        public decimal weight { get; set; }//单重
        public string gbbm { get; set; }//国标编码  物料编码
        public string private8 { get; set; }//私有字段8 路线
        public string private9 { get; set; }//私有字段9 备注
        public string private10 { get; set; }//私有字段10 总重
        public string formAttribute { get; set; }//料品形态
        public string Bzth { get; set; }//标准图号
        public string Yitemdesc { get; set; }//原子件描述

        public BomLineVOZJ(DataGridViewRow row)
        {
            this.itemcode = DataHelper.getStr(row.Cells["物料编码"].Value);//子件料号
            this.itemdesc = itemcode.StartsWith("S") ? DataHelper.getStr(row.Cells["物料描述"].Value) : DataHelper.getStr(row.Cells["子件描述"].Value);//名称+规格型号
            this.unit =PubHelper.GetUOMCode(DataHelper.getStr(row.Cells["基本计量单位"].Value));
            this.formAttribute = DataHelper.getStr(row.Cells["料品形态属性"].Value);//料品形态属性
            //decimal parentQty = DataHelper.getDecimal(row.Cells["母件用量"].Value);//母件用量
            decimal useQty = DataHelper.getDecimal(row.Cells["子件用量"].Value);//数量
            this.Bzth = DataHelper.getStr(row.Cells["标准图号"].Value);//料品形态属性
            this.Yitemdesc = DataHelper.getStr(row.Cells["子件描述"].Value);//
            this.gbbm = PubHelper.chkIsGB(itemcode) ? itemcode.Split('(')[0] : "";//国标编码
            this.qty = useQty;//数量
            //this.qty = useQty;
            this.private8 = DataHelper.getStr(row.Cells["制造路线"].Value);//
            this.private9 = DataHelper.getStr(row.Cells["备注"].Value);//
            this.private10 = DataHelper.getStr(row.Cells["总重"].Value);//
            this.material = DataHelper.getStr(row.Cells["材料"].Value);
            //this.weight = DataHelper.getDecimal(row.Cells["单重"].Value);
        }

    }
}
