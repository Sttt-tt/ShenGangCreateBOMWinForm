﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsApplication2.Helper;

namespace WindowsApplication2.VO
{
    /// <summary>
    /// 物料清单
    /// </summary>
    class BomVO
    {
        public string itemcode { get; set; }//母件料号
        public string itemdesc { get; set; }//母件描述
        public string unit { get; set; }//单位
        public string qty { get; set; }//用量
        public string formAttribute { get; set; }//料品形态
        public string private2 { get; set; }//私有字段2 工艺路线
        public string private3 { get; set; }//私有字段3 备注
        public List<BomLineVO> rows = new List<BomLineVO>();
    }

    class BomLineVO
    {
        public string itemcode { get; set; }//子件料号
        public string itemdesc { get; set; }//子件描述  名称+材质+规格型号
        public string unit { get; set; }//单位
        public decimal qty { get; set; }//用量
        public string private8 { get; set; }//私有字段8 路线
        public string private9 { get; set; }//私有字段9 备注
        public string formAttribute { get; set; }//料品形态

        public BomLineVO(DataGridViewRow row)
        {
            this.itemcode = DataHelper.getStr(row.Cells["物料编码"].Value);//子件料号
            this.itemdesc = DataHelper.getStr(row.Cells["物料描述"].Value);//名称+材质+规格型号
            this.unit = DataHelper.getStr(row.Cells["基本计量单位"].Value);//基本计量单位   KG=>W013; EA=>PCS
            this.formAttribute = DataHelper.getStr(row.Cells["料品形态属性"].Value);//料品形态属性
            decimal parentQty = DataHelper.getDecimal(row.Cells["母件用量"].Value);//母件用量
            decimal useQty = DataHelper.getDecimal(row.Cells["数量/重量"].Value);//数量

            this.qty = Math.Round(useQty / parentQty, 4);//数量
            //this.qty = useQty;
            this.private8 = DataHelper.getStr(row.Cells["制造路线"].Value);//
            this.private9 = DataHelper.getStr(row.Cells["备注"].Value);//
        }

    }
}
