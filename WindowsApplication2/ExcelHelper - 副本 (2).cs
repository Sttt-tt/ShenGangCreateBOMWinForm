using Aspose.Cells;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsApplication2.Helper;

namespace WindowsApplication2
{
    public class ExcelHelper : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public string beginningDrawNums = ConfigurationManager.AppSettings["BeginningDrawNums"];

        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        ///
        public Dictionary<int, string> ReturnSheetList()
        {
            Dictionary<int, string> t = new Dictionary<int, string>();
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);
                int count = workbook.NumberOfSheets; //获取所有SheetName
                for (int i = 0; i < count; i++)
                {
                    sheet = workbook.GetSheetAt(i);
                    if (sheet.LastRowNum > 0)
                    {
                        t.Add(i, workbook.GetSheetAt(i).SheetName);
                    }
                }
                return t;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
        public DataTable ExcelToDataTable(int index)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);
                //int coutnts = workbook.NumberOfSheets;

                sheet = workbook.GetSheetAt(index);
                //string names= sheet.SheetName;
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数


                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        ICell cell = firstRow.GetCell(i);
                        CellType c = cell.CellType;
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue;
                            if (cellValue != null)
                            {
                                DataColumn column = new DataColumn(cellValue);
                                data.Columns.Add(column);
                            }
                        }
                    }
                    startRow = sheet.FirstRowNum + 1;


                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                return null;
                throw new Exception(ex.Message);

            }
        }


        public DataTable ExcelToDataTable2(int index)
        {
            //实例化DataTable来存放数据
            DataTable dt = new DataTable();
            //string fileName = file;
            string sheetName = "Details";//Excel的工作表名称
            bool isColumnName = true;//判断第一行是否为标题列
            IWorkbook workbook;//创建一个工作薄接口
            string fileExt = Path.GetExtension(fileName).ToLower();//获取文件的拓展名
            //创建一个文件流
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else
                {
                    workbook = new HSSFWorkbook(fs);
                }

                //实例化sheet
                ISheet sheet = workbook.GetSheetAt(index);
                if (sheetName != null && sheetName != "")//判断是否存在sheet
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);//从第一个开始读取，0位索引
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                sheet.RemoveColumnBreak(5);
                //获取表头
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                int startRow = 0;//数据的第一行索引


                if (isColumnName)//表示第一行是列名
                {
                    startRow = sheet.FirstRowNum + 1;//数据从第二行开始读

                    //遍历表的第一行，即所有的列名
                    for (int i = header.FirstCellNum; i < header.LastCellNum; i++)
                    {
                        ICell cell = header.GetCell(i);
                        if (cell != null)
                        {
                            //获取列名的值
                            string colName = cell.ToString();

                            if (colName != null)
                            {
                                DataColumn col = new DataColumn(colName);
                                dt.Columns.Add(col);
                                if (colName == "展开层")
                                {
                                    dt.Columns.Add("母件料品");
                                    dt.Columns.Add("母件物料描述");
                                    dt.Columns.Add("母件基本计量单位");
                                    dt.Columns.Add("母件用量");
                                }
                            }
                            else
                            {
                                DataColumn col = new DataColumn();
                                dt.Columns.Add(col);
                            }
                        }
                    }
                    //dt.Columns.Add("母件料品");
                    dt.Columns.Add("料品形态属性");
                    dt.Columns.Add("工艺路线");
                    dt.Columns.Add("备注");
                }


                //读取数据
                for (int i = startRow; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);

                    if (row == null)
                    {
                        continue;
                    }

                    DataRow dr = dt.NewRow();
                    int k = 0;
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {

                        k = j >= 3 ? j + 4 : j;
                        if (row.GetCell(j) != null)
                        {
                            dr[k] = row.GetCell(j).ToString();
                        }

                    }
                    dt.Rows.Add(dr);
                }
            }

            //dt.DefaultView.Sort = "母件料品 asc, 展开层 asc";
            //List<int> levels = new List<int>();
            //foreach (DataRow row1 in dt.Rows)
            //{
            //    int level = Convert.ToInt32(row1["展开层"]);
            //    if (!levels.Contains(level)) levels.Add(level);
            //}
            //int maxlevel = levels.Max();
            foreach (DataRow row1 in dt.Rows)
            {
                int level = Convert.ToInt32(row1["展开层"]);
                //if (level == 1)
                //{
                //    //row1["母件料品"] = row1["WBS"];
                //}
                //else
                //{

                for (int xuhao = Convert.ToInt32(row1["序号"]) - 1; xuhao >= 0; xuhao--)
                {
                    DataRow temprow = dt.Rows[xuhao];
                    int templevel = Convert.ToInt32(temprow["展开层"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(row1["母件料品"]))) continue;
                    if (templevel + 1 == level)
                    {
                        row1["母件料品"] = temprow["物料编码"];
                        row1["母件物料描述"] = temprow["物料描述"];
                        row1["母件基本计量单位"] = temprow["基本计量单位"];
                        row1["母件用量"] = temprow["数量/重量"];
                    }
                }

                int count = dt.Rows.Count;
                if (Convert.ToInt32(row1["序号"]) == count)
                {
                    row1["料品形态属性"] = "采购件";
                }
                else
                {
                    int xh = Convert.ToInt32(row1["序号"]) - 1;//当前行序号
                    DataRow nowrow = dt.Rows[xh];//当前下一行的值
                    int nowzkc = Convert.ToInt32(nowrow["展开层"]);//下一行展开层
                    DataRow nextrow = dt.Rows[xh + 1];//当前下一行的值
                    int nextzkc = Convert.ToInt32(nextrow["展开层"]);//下一行展开层
                                                                  //}
                    if (nextzkc < nowzkc)
                    {
                        row1["料品形态属性"] = "采购件";
                    }
                    else
                    {
                        if (Convert.ToString(row1["物料编码"]).StartsWith("0"))
                        {
                            row1["料品形态属性"] = "采购件";
                        }
                        else
                        {
                            row1["料品形态属性"] = "制造件";
                        }

                    }
                }

            }
            dt = DeleteRow(dt);
            dt.DefaultView.Sort = "母件料品 asc,物料描述 asc";
            return dt;
        }

        /// <summary>
        ///自接数据Excel原始数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataTable ZjExcelToDataTable(int index)
        {
            //实例化DataTable来存放数据
            DataTable dt = new DataTable();

            //增加自定义列
            dt.Columns.Add("序号");
            dt.Columns.Add("物料编码");
            dt.Columns.Add("物料描述");
            dt.Columns.Add("单位");
            dt.Columns.Add("用量");
            dt.Columns.Add("材料");
            dt.Columns.Add("单重");
            dt.Columns.Add("备注");
            //dt.Columns.Add("材料");
            //dt.Columns.Add("单重");
            //dt.Columns.Add("备  注");
            dt.Columns.Add("料品形态属性");
            //dt.Columns.Add("转换率");
            dt.Columns.Add("标准图号");
            dt.Columns.Add("原物料描述");

            IWorkbook workbook;//创建一个工作薄接口
            string fileExt = Path.GetExtension(fileName).ToLower();//获取文件的拓展名
            //创建一个文件流
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else
                {
                    workbook = new HSSFWorkbook(fs);
                }

                int SheetCount = workbook.NumberOfSheets;//获取表的数量

                //遍历每个Sheet,从sheet开始
                for (int p = 0; p <= SheetCount - 1; p++)
                {
                    //实例化sheet
                    ISheet sheet = workbook.GetSheetAt(p);
                    //获取表头
                    //IRow header = sheet.GetRow(sheet.FirstRowNum + 2);
                    int startRow = 4;//数据的第一行索引

                    //读取数据
                    for (int i = startRow; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);

                        if (row == null)
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(row.Cells[8])))
                        {
                            continue;
                        }
                        if (row.Cells[6].ToString().Contains("说明"))
                        {
                            break;
                        }
                        //if (!row.Cells[7].ToString().Contains("-"))
                        //{
                        //    continue;
                        //}

                        DataRow dr = dt.NewRow();
                        for (int j = row.FirstCellNum + 6; j < row.LastCellNum - 21; j++)
                        {
                            //0 row.GetCell(7)   序号
                            //1 row.GetCell(8)   代 号
                            //2 row.GetCell(10)   名 称 及 规 格
                            //3 row.GetCell(12)  数量
                            //4 row.GetCell(13)  材料
                            //5 row.GetCell(14) 单重
                            //6 row.GetCell(16)  备 注

                            //if (j == 6)
                            //{
                            //    if (string.IsNullOrEmpty(Convert.ToString(row.GetCell(j))))
                            //    {
                            //        dr["料品形态属性"] = "虚拟件";
                            //    }
                            //    else
                            //    {
                            //        dr["料品形态属性"] = "制造件";
                            //    }
                            //}
                            dr["料品形态属性"] = "制造件";
                            if (j == 7)
                            {
                                dr["序号"] = row.GetCell(j).ToString();
                            }
                            if (j == 8)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(row.GetCell(j - 1))))
                                {
                                    dr["物料编码"] = row.GetCell(j).ToString();
                                }
                                else
                                {
                                    dr["物料编码"] = row.GetCell(j).ToString() + "(" + row.GetCell(j - 1).ToString() + ")";
                                }
                            }
                            if (j == 10)
                            {
                                string[] beginningDrawNum = beginningDrawNums.Split(',');
                                foreach (var item in beginningDrawNum)
                                {
                                    string daiHao = DataHelper.getStr(row.GetCell(j - 2));//自接代号列
                                    if (daiHao.StartsWith(item))
                                    {
                                        if (row.GetCell(j).ToString().Contains(","))
                                        {
                                            string[] str = row.GetCell(j).ToString().Split(',');
                                            dr["物料描述"] = daiHao + str[0].Trim();
                                            dr["标准图号"] = daiHao;
                                            dr["原物料描述"] = str[0].Trim();
                                        }
                                        else
                                        {
                                            if (row.GetCell(j).ToString().Contains("L"))
                                            {
                                                string[] str = row.GetCell(j).ToString().Split('L');
                                                dr["物料描述"] = daiHao + str[0].Trim();
                                                dr["标准图号"] = daiHao;
                                                dr["原物料描述"] = str[0].Trim();
                                            }
                                            else
                                            {
                                                dr["物料描述"] = daiHao + row.GetCell(j).ToString();
                                                dr["标准图号"] = daiHao;
                                                dr["原物料描述"] = row.GetCell(j).ToString();
                                            }

                                        }
                                        break;
                                    }
                                    else
                                    {
                                        if (row.GetCell(j).ToString().Contains(","))
                                        {
                                            string[] str = row.GetCell(j).ToString().Split(',');
                                            dr["物料描述"] = str[0].Trim();
                                        }
                                        else
                                        {
                                            if (row.GetCell(j).ToString().Contains("L"))
                                            {
                                                string[] str = row.GetCell(j).ToString().Split('L');
                                                dr["物料描述"] = str[0].Trim();
                                            }
                                            else
                                            {
                                                dr["物料描述"] = row.GetCell(j).ToString();
                                            }

                                        }
                                    }
                                }
                            }
                            if (j == 12)
                            {
                                dr["用量"] = row.GetCell(j).ToString();
                            }
                            dr["单位"] = "PCS";
                            if (j == 13)
                            {
                                dr["材料"] = row.GetCell(j).ToString();
                            }
                            if (j == 14)
                            {
                                dr["单重"] = row.GetCell(j).ToString();
                            }
                            if (j == 16)
                            {
                                dr["备注"] = row.GetCell(j).ToString();
                            }
                            //if (j == 18)
                            //{
                            //    if (row.GetCell(j - 8).ToString().Contains(","))
                            //    {
                            //        string[] str = row.GetCell(j - 8).ToString().Split(',');
                            //        if (str[0].Trim().Contains("φ") && str[0].Trim().Contains("管"))
                            //        {
                            //            string[] vs = str[0].Trim().Split('φ');
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (row.GetCell(j - 8).ToString().Contains("L"))
                            //        {
                            //            string[] str = row.GetCell(j - 8).ToString().Split('L');
                            //            str[0].Trim();
                            //        }
                            //        else
                            //        {
                            //            row.GetCell(j - 8).ToString();
                            //        }

                            //    }
                            //    //dr["转换率"] = row.GetCell(j).ToString();
                            //}

                        }
                        dt.Rows.Add(dr);
                    }
                }

            }
            return dt;
        }


        // <summary>
        ///自接数据BOMDatagrid数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataTable ZjExcelToBOMDataTable(DataTable dataTable)
        {
            dataTable.PrimaryKey = new System.Data.DataColumn[] { dataTable.Columns["序号"] };
            //拼接最上层BOM
            string itemCode = dataTable.Rows[0]["物料编码"].ToString();//物料编码
            string itemName = dataTable.Rows[0]["物料描述"].ToString();//物料名称
            string itemCl = dataTable.Rows[0]["材料"].ToString();//物料材料
            int qty = 1;
            List<string> XhList = new List<string>();//序号集合
            foreach (DataRow item in dataTable.Rows)
            {
                //拼接最上层母BOM数据
                //if (Convert.ToString(item["序号"]) == "1")
                //{
                //    string[] items = Convert.ToString(item["物料编码"]).Split('-');
                //    itemCode = items[0];
                //    itemName = items[0];
                //    itemCl = Convert.ToString(item["材料"]);
                //}
                //if (!Convert.ToString(item["序号"]).Contains("-"))
                //{
                //    qty += Convert.ToInt32(item["用量"]);
                //}
                XhList.Add(Convert.ToString(item["序号"]));
            }
            //实例化DataTable来存放数据
            DataTable dt = new DataTable();

            //增加自定义列
            dt.Columns.Add("序号");
            dt.Columns.Add("母件料品");
            dt.Columns.Add("母件物料描述");
            dt.Columns.Add("母件材料");
            dt.Columns.Add("母件基本计量单位");
            dt.Columns.Add("母件用量");
            dt.Columns.Add("物料编码");
            dt.Columns.Add("物料描述");
            dt.Columns.Add("基本计量单位");
            dt.Columns.Add("数量/重量");
            dt.Columns.Add("材料");
            //dt.Columns.Add("单重");

            dt.Columns.Add("制造路线");
            dt.Columns.Add("是否末阶");
            dt.Columns.Add("是否虚拟");
            dt.Columns.Add("wbs");
            //dt.Columns.Add("备  注");
            dt.Columns.Add("料品形态属性");
            dt.Columns.Add("备注");
            //dt.Columns.Add("转换率");
            dt.Columns.Add("标准图号");
            dt.Columns.Add("原物料描述");
            int i = 0;
            int count = 0;//记录maxcode是否是第一次
            string maxcode = string.Empty;
            try
            {
                dataTable.Rows.RemoveAt(0);//移除最上层母件
                foreach (DataRow row in dataTable.Rows)
                {
                    DataRow dr = dt.NewRow();
                    i++;
                    //1 2 3 4 5 6 
                    if (!Convert.ToString(row["序号"]).Contains("-"))
                    {
                        dr["序号"] = row["序号"];
                        dr["母件料品"] = itemCode;
                        dr["母件物料描述"] = itemName;
                        dr["母件材料"] = itemCl;
                        dr["母件基本计量单位"] = "PCS";
                        dr["母件用量"] = qty;
                        dr["物料编码"] =DataHelper.getStr(row["物料编码"]);
                        dr["物料描述"] = row["物料描述"];
                        dr["基本计量单位"] = "PCS";
                        dr["数量/重量"] = row["用量"];
                        dr["材料"] = row["材料"];
                        //dr["单重"] = row["单重"];
                        dr["料品形态属性"] = row["料品形态属性"];
                        dr["备注"] = row["备注"];
                        //dr["转换率"] = row["转换率"];
                        dr["标准图号"] = row["标准图号"];
                        dr["原物料描述"] = row["原物料描述"];
                        dr["wbs"] = itemCode;
                        dt.Rows.Add(dr);

                        if (GetLevel1Count(Convert.ToString(row["序号"]), XhList))
                        {
                            dr["是否末阶"] = "是";

                            if (getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) == 0 || getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) > 1)
                            {
                                count++;
                                if (count == 1)
                                {
                                    maxcode = GetMaxItemCodeOne();
                                }
                                else
                                {
                                    maxcode = GetMaxItemCode(maxcode);
                                }
                                DataRow drr = dt.NewRow();
                                drr["wbs"] = itemCode;
                                drr["序号"] = row["序号"] + "-" + "1";
                                drr["母件料品"] = row["物料编码"];
                                drr["母件物料描述"] = row["物料描述"];
                                drr["母件材料"] = row["材料"];
                                drr["母件基本计量单位"] = "PCS";
                                drr["母件用量"] = row["用量"];
                                //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                drr["物料编码"] = "";
                                drr["物料描述"] = "";
                                drr["基本计量单位"] = "";
                                //drr["数量/重量"] = row["用量"];
                                drr["材料"] = "";
                                //drr["单重"] = "0";
                                drr["是否虚拟"] = "是";
                                drr["料品形态属性"] = "采购件";
                                drr["备注"] = row["备注"];
                                //drr["转换率"] = row["转换率"];
                                //drr["标准图号"] = row["标准图号"];
                                //drr["原物料描述"] = row["原物料描述"];
                                dt.Rows.Add(drr);
                            }
                            else
                            {
                                DataTable dataTable1 = getItemMasters(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"]));
                                DataRow drr = dt.NewRow();
                                drr["wbs"] = itemCode;
                                drr["序号"] = row["序号"] + "-" + "1";
                                drr["母件料品"] = DataHelper.getStr(row["物料编码"]);
                                drr["母件物料描述"] = row["物料描述"];
                                drr["母件材料"] = row["材料"];
                                drr["母件基本计量单位"] = "PCS";
                                drr["母件用量"] = row["用量"];
                                //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                drr["物料编码"] = dataTable1.Rows[0]["料号"];
                                drr["物料描述"] = dataTable1.Rows[0]["品名"];
                                drr["基本计量单位"] = "PCS";
                                drr["数量/重量"] = row["用量"];
                                drr["材料"] = dataTable1.Rows[0]["材料"];
                                //drr["单重"] = 0;
                                drr["是否虚拟"] = "是";
                                drr["料品形态属性"] = "采购件";
                                drr["备注"] = row["备注"];
                                //drr["转换率"] = row["转换率"];
                                drr["标准图号"] = row["标准图号"];
                                drr["原物料描述"] = row["原物料描述"];
                                dt.Rows.Add(drr);
                            }
                        }
                    }
                    else
                    {
                        //1-1 1-2 2-1  2-2  3-1  3-2 
                        if (!Convert.ToString(row["序号"]).Contains("/"))
                        {
                            string[] str = Convert.ToString(row["序号"]).Split('-');
                            if (str.Count() == 2)
                            {
                                DataRow row1 = dataTable.Rows.Find(str[0]);
                                dr["wbs"] = itemCode;
                                //add by yfj 20220706
                                if (row1 == null) continue;
                                dr["序号"] = row["序号"];
                                dr["母件料品"] = row1["物料编码"];
                                dr["母件物料描述"] = row1["物料描述"];
                                dr["母件材料"] = row1["材料"];
                                dr["母件基本计量单位"] = "PCS";
                                dr["母件用量"] = row1["用量"];
                                dr["物料编码"] = row["物料编码"];
                                dr["物料描述"] = row["物料描述"];
                                dr["基本计量单位"] = "PCS";
                                dr["数量/重量"] = row["用量"];
                                dr["材料"] = row["材料"];
                                //dr["单重"] = row["单重"];
                                dr["料品形态属性"] = row["料品形态属性"];
                                dr["备注"] = row["备注"];
                                //dr["转换率"] = row["转换率"];
                                dr["标准图号"] = row["标准图号"];
                                dr["原物料描述"] = row["原物料描述"];
                                dt.Rows.Add(dr);
                            }
                            if (GetLevelCount(Convert.ToString(row["序号"]), XhList))
                            {
                                dr["是否末阶"] = "是";

                                //if (getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) == 0 || getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) > 1)
                                //{
                                //    count++;
                                //    if (count == 1)
                                //    {
                                //        maxcode = GetMaxItemCodeOne();
                                //    }
                                //    else
                                //    {
                                //        maxcode = GetMaxItemCode(maxcode);
                                //    }
                                //    DataRow drr = dt.NewRow();
                                //    drr["wbs"] = itemCode;
                                //    drr["序号"] = row["序号"] + "/" + "1";
                                //    string[] wl = Convert.ToString(row["物料编码"]).Split('-');
                                //    drr["母件料品"] = row["物料编码"];
                                //    drr["母件物料描述"] = row["物料描述"];
                                //    drr["母件材料"] = row["材料"];
                                //    drr["母件基本计量单位"] = "KG";
                                //    drr["母件用量"] = row["用量"];
                                //    //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                //    //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                //    drr["物料编码"] = "";
                                //    drr["物料描述"] = "";
                                //    drr["基本计量单位"] = "";
                                //    //drr["数量/重量"] = row["用量"];
                                //    drr["材料"] = "";
                                //    //drr["单重"] = "0";
                                //    drr["是否虚拟"] = "是";
                                //    drr["料品形态属性"] = "采购件";
                                //    drr["备注"] = row["备注"];
                                //    //drr["转换率"] = row["转换率"];
                                //    //drr["标准图号"] = row["标准图号"];
                                //    //drr["原物料描述"] = row["原物料描述"];
                                //    dt.Rows.Add(drr);
                                //}
                                //else
                                //{
                                //DataTable dataTable2 = getItemMasters(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"]));

                                DataTable dataTable2 = getSingleItemMaster(DataHelper.getStr(row["物料编码"]), Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"]));
                                DataRow drr = dt.NewRow();
                                drr["wbs"] = itemCode;
                                drr["序号"] = row["序号"] + "/" + "1";
                                drr["母件料品"] = DataHelper.getStr(row["物料编码"]);
                                drr["母件物料描述"] = row["物料描述"];
                                drr["母件材料"] = row["材料"];
                                drr["母件基本计量单位"] = "PCS";
                                drr["母件用量"] = row["用量"];
                                //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                if (dataTable2 != null && dataTable2.Rows.Count == 1)
                                {
                                    drr["物料编码"] = dataTable2.Rows[0]["料号"];
                                    drr["物料描述"] = dataTable2.Rows[0]["品名"];
                                    drr["基本计量单位"] = dataTable2.Rows[0]["单位"];
                                    //drr["数量/重量"] = row["用量"];
                                    drr["材料"] = dataTable2.Rows[0]["材料"];
                                }
                                //drr["单重"] = 0;
                                drr["是否虚拟"] = "是";
                                drr["料品形态属性"] = "采购件";
                                drr["备注"] = row["备注"];
                                //drr["转换率"] = row["转换率"];
                                drr["标准图号"] = row["标准图号"];
                                drr["原物料描述"] = row["原物料描述"];
                                dt.Rows.Add(drr);
                                //}
                            }
                        }
                        else
                        {
                            string[] str1 = Convert.ToString(row["序号"]).Split('-');
                            if (str1.Count() == 2)
                            {
                                string[] str2 = Convert.ToString(row["序号"]).Split('/');
                                DataRow row2 = dataTable.Rows.Find(str2[0]);
                                dr["wbs"] = itemCode;
                                dr["序号"] = row["序号"];
                                dr["母件料品"] = DataHelper.getStr(row2["物料编码"]);
                                dr["母件物料描述"] = row2["物料描述"];
                                dr["母件材料"] = row2["材料"];
                                dr["母件基本计量单位"] = "PCS";
                                dr["母件用量"] = row2["用量"];
                                dr["物料编码"] = row["物料编码"];
                                dr["物料描述"] = row["物料描述"];
                                dr["基本计量单位"] = "PCS";
                                dr["数量/重量"] = row["用量"];
                                //dr["单重"] = row["单重"];
                                dr["材料"] = row["材料"];
                                dr["料品形态属性"] = row["料品形态属性"];
                                dr["备注"] = row["备注"];
                                //dr["转换率"] = row["转换率"];
                                dr["标准图号"] = row["标准图号"];
                                dr["原物料描述"] = row["原物料描述"];
                                dt.Rows.Add(dr);

                                //判断1 - 1 / 1 - 1
                                if (GetLevelCount2(Convert.ToString(row["序号"]), XhList))
                                {
                                    dr["是否末阶"] = "是";
                                    //if (getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) == 0 || getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) > 1)
                                    //{
                                    //    count++;
                                    //    if (count == 1)
                                    //    {
                                    //        maxcode = GetMaxItemCodeOne();
                                    //    }
                                    //    else
                                    //    {
                                    //        maxcode = GetMaxItemCode(maxcode);
                                    //    }
                                    //    DataRow drr = dt.NewRow();
                                    //    drr["wbs"] = itemCode;
                                    //    drr["序号"] = row["序号"] + "-" + "1";
                                    //    string[] wl = Convert.ToString(row["物料编码"]).Split('-');
                                    //    drr["母件料品"] = row["物料编码"];
                                    //    drr["母件物料描述"] = row["物料描述"];
                                    //    drr["母件材料"] = row["材料"];
                                    //    drr["母件基本计量单位"] = "KG";
                                    //    drr["母件用量"] = row["用量"];
                                    //    //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    //    //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    //    drr["物料编码"] = "";
                                    //    drr["物料描述"] = "";
                                    //    drr["基本计量单位"] = "";
                                    //    //drr["数量/重量"] = row["用量"];
                                    //    drr["材料"] = "";
                                    //    //drr["单重"] = "0";
                                    //    drr["是否虚拟"] = "是";
                                    //    //drr["转换率"] = row["转换率"];
                                    //    drr["料品形态属性"] = "采购件";
                                    //    //drr["备注"] = row["备注"];
                                    //    dt.Rows.Add(drr);
                                    //}
                                    //else
                                    //{
                                    //DataTable dataTable2 = getItemMasters(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"]));
                                    DataTable dataTable2 = getSingleItemMaster(DataHelper.getStr(row["物料编码"]), Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"]));
                                    DataRow drr = dt.NewRow();
                                    drr["wbs"] = itemCode;
                                    drr["序号"] = row["序号"] + "-" + "1";
                                    drr["母件料品"] = DataHelper.getStr(row["物料编码"]);
                                    drr["母件物料描述"] = row["物料描述"];
                                    drr["母件材料"] = row["材料"];
                                    drr["母件基本计量单位"] = "PCS";
                                    drr["母件用量"] = row["用量"];
                                    //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    if (dataTable2 != null && dataTable2.Rows.Count == 1)
                                    {
                                        drr["物料编码"] = dataTable2.Rows[0]["料号"];
                                        drr["物料描述"] = dataTable2.Rows[0]["品名"];
                                        drr["基本计量单位"] = dataTable2.Rows[0]["单位"];
                                        //drr["数量/重量"] = row["用量"];
                                        drr["材料"] = dataTable2.Rows[0]["材料"];
                                    }
                                    //drr["单重"] = 0;
                                    drr["是否虚拟"] = "是";
                                    drr["料品形态属性"] = "采购件";
                                    drr["备注"] = row["备注"];
                                    //drr["转换率"] = row["转换率"];
                                    drr["标准图号"] = row["标准图号"];
                                    drr["原物料描述"] = row["原物料描述"];
                                    dt.Rows.Add(drr);
                                    //}
                                }
                            }
                            else
                            {
                                int index = Convert.ToString(row["序号"]).IndexOf("/") + 2;
                                string str3 = Convert.ToString(row["序号"]).Substring(0, index);
                                DataRow row3 = dataTable.Rows.Find(str3);
                                dr["wbs"] = itemCode;
                                dr["序号"] = row["序号"];
                                dr["母件料品"] = row3["物料编码"];
                                dr["母件物料描述"] = row3["物料描述"];
                                dr["母件材料"] = row3["材料"];
                                dr["母件基本计量单位"] = "PCS";
                                dr["母件用量"] = row3["用量"];
                                dr["物料编码"] = row["物料编码"];
                                dr["物料描述"] = row["物料描述"];
                                dr["基本计量单位"] = "PCS";
                                dr["数量/重量"] = row["用量"];
                                dr["材料"] = row["材料"];
                                //dr["单重"] = row["单重"];
                                dr["料品形态属性"] = row["料品形态属性"];
                                dr["备注"] = row["备注"];
                                //dr["转换率"] = row["转换率"];
                                dr["标准图号"] = row["标准图号"];
                                dr["原物料描述"] = row["原物料描述"];
                                dt.Rows.Add(dr);

                                //判断1 - 1 / 1 - 1
                                if (GetLevelCount2(Convert.ToString(row["序号"]), XhList))
                                {
                                    dr["是否末阶"] = "是";

                                    //if (getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) == 0 || getItemMastersCount(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"])) > 1)
                                    //{
                                    //    count++;
                                    //    if (count == 1)
                                    //    {
                                    //        maxcode = GetMaxItemCodeOne();
                                    //    }
                                    //    else
                                    //    {
                                    //        maxcode = GetMaxItemCode(maxcode);
                                    //    }
                                    //    DataRow drr = dt.NewRow();
                                    //    drr["wbs"] = itemCode;
                                    //    drr["序号"] = row["序号"] + "/" + "1";
                                    //    string[] wl = Convert.ToString(row["物料编码"]).Split('-');
                                    //    drr["母件料品"] = row["物料编码"];
                                    //    drr["母件物料描述"] = row["物料描述"];
                                    //    drr["母件材料"] = row["材料"];
                                    //    drr["母件基本计量单位"] = "KG";
                                    //    drr["母件用量"] = row["用量"];
                                    //    //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    //    //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    //    drr["物料编码"] = "";
                                    //    drr["物料描述"] = "";
                                    //    drr["基本计量单位"] = "";
                                    //    //drr["数量/重量"] = row["用量"];
                                    //    drr["材料"] = "";
                                    //    //drr["单重"] = "0";
                                    //    drr["是否虚拟"] = "是";
                                    //    drr["料品形态属性"] = "采购件";
                                    //    drr["备注"] = row["备注"];
                                    //    //drr["标准图号"] = row["标准图号"];
                                    //    //drr["原物料描述"] = row["原物料描述"];
                                    //    dt.Rows.Add(drr);
                                    //}
                                    //else
                                    //{
                                    //DataTable dataTable2 = getItemMasters(Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"]));
                                    DataTable dataTable2 = getSingleItemMaster(DataHelper.getStr(row["物料编码"]), Convert.ToString(row["物料描述"]), Convert.ToString(row["材料"]));
                                    DataRow drr = dt.NewRow();
                                    drr["wbs"] = itemCode;
                                    drr["序号"] = row["序号"] + "/" + "1";
                                    drr["母件料品"] = row["物料编码"];
                                    drr["母件物料描述"] = row["物料描述"];
                                    drr["母件材料"] = row["材料"];
                                    drr["母件基本计量单位"] = "PCS";
                                    drr["母件用量"] = row["用量"];
                                    //dr["物料编码"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    //dr["物料描述"] = wl[0] + "-" + wl[1] + "-" + wl[2] + "-" + "1" + "-" + "0" + "(" + row["序号"] + "/" + "1" + ")";
                                    if (dataTable2 != null && dataTable2.Rows.Count == 1)
                                    {
                                        drr["物料编码"] = dataTable2.Rows[0]["料号"];
                                        drr["物料描述"] = dataTable2.Rows[0]["品名"];
                                        drr["基本计量单位"] = dataTable2.Rows[0]["单位"];
                                        //drr["数量/重量"] = row["用量"];
                                        drr["材料"] = dataTable2.Rows[0]["材料"];
                                    }
                                    //drr["单重"] = 0;
                                    drr["是否虚拟"] = "是";
                                    drr["料品形态属性"] = "采购件";
                                    drr["备注"] = row["备注"];
                                    //drr["转换率"] = row["转换率"];
                                    drr["标准图号"] = row["标准图号"];
                                    drr["原物料描述"] = row["原物料描述"];
                                    dt.Rows.Add(drr);
                                    //}
                                }
                            }
                        }
                        //dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }
            return dt;
        }

        /// <summary>  
        /// 判断DS是否为空  
        /// </summary>  
        /// <param name="ds">需要判断的ds</param>  
        /// <returns>如果ds为空，返回true</returns>  
        private static bool JudgeDs(DataTable dt)
        {
            bool Flag = false;
            if ((dt == null) || dt.Rows.Count == 0)
            {
                Flag = true;
            }
            return Flag;
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
            sql = string.Format(@"select  Code 料号,Name+SPECS 品名,DescFlexField_PrivateDescSeg1 材料 from CBO_ItemMaster where Name='{0}' and DescFlexField_PrivateDescSeg1 = '{1}' 
                                        and SPECS ='{2}' and Effective_IsEffective=1 
                                        ", ItemName.Trim(), itemCz.Trim(), ItemSPECS.Trim());
            dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));

            return dt;
        }
        /// <summary>
        /// 取系统物料
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemCz"></param>
        /// <returns></returns>
        private DataTable getSingleItemMaster(string code, string item, string itemCz)
        {
            string ItemName = string.Empty;
            code = code.Split('(')[0];
            //国标处理
            if (item.StartsWith(code))
            {
                item = item.Replace(code, "");
                ItemName = string.IsNullOrEmpty(KeepChinese(item)) ? Regex.Replace(item, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase).Trim() : KeepChinese(item).Trim();//物料名称
                ItemName = code + ItemName;

            }
            else
            {
                ItemName = string.IsNullOrEmpty(KeepChinese(item)) ? Regex.Replace(item, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase).Trim() : KeepChinese(item).Trim();//物料名称

            }
            string ItemSPECS = Regex.Replace(item, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase).Trim(); //物料规格型号
            DataTable dt = new DataTable();
            string sql = string.Empty;
            sql = $@"select  A.Code 料号,A.Name+A.SPECS 品名,A.DescFlexField_PrivateDescSeg1 材料,A3.Name 单位 
            from CBO_ItemMaster  A
            left join Base_UOM_trl A3  ON A3.ID=A.InventorySecondUOM  and A3.SysMLFlag='zh-CN' 
            where A.Name='{ItemName}' and A.DescFlexField_PrivateDescSeg1 = '{itemCz}' 
            and A.SPECS ='{ItemSPECS}' and A.Effective_IsEffective=1  and A.Org={DataHelper.getStr(Login.u9ContentHt["OrgID"])}";
            dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            if (dt == null || dt.Rows.Count <= 0)
            {

                sql = $@"select  A.Code 料号,A.Name+A.SPECS 品名,A.DescFlexField_PrivateDescSeg1 材料 ,A3.Name 单位 
            from CBO_ItemMaster  A
            left join Base_UOM_trl A3  ON A3.ID=A.InventorySecondUOM and A3.SysMLFlag='zh-CN'  
            where A.DescFlexField_PrivateDescSeg1 = '{itemCz}' 
            and A.SPECS ='{ItemSPECS}' and A.Effective_IsEffective=1  and A.Org={DataHelper.getStr(Login.u9ContentHt["OrgID"])}";
                dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            }
            return dt;
        }


        private int getItemMastersCount(string item, string itemCz)
        {
            int count = 0;
            string ItemName = string.IsNullOrEmpty(KeepChinese(item)) ? Regex.Replace(item, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase) : KeepChinese(item);//物料名称
            string ItemSPECS = Regex.Replace(item, "[\u4e00-\u9fa5]", "", RegexOptions.IgnoreCase); //物料规格型号
            DataTable dt = new DataTable();
            string sql = string.Empty;
            sql = string.Format(@"select count(*) count from CBO_ItemMaster where Name='{0}' and DescFlexField_PrivateDescSeg1 = '{1}' 
                                        and SPECS ='{2}' and Effective_IsEffective=1 
                                        ", ItemName.Trim(), itemCz.Trim(), ItemSPECS.Trim());
            dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            count = Convert.ToInt32(dt.Rows[0]["count"]);
            return count;
        }


        //取最大料号
        private static string GetMaxItemCodeOne()
        {
            string code = string.Empty;
            string sql = string.Empty;
            string EntCode = Login.u9ContentHt["OrgCode"].ToString();

            string getOrgID = string.Format(@"select ID from Base_Organization where Code='{0}'", EntCode);
            DataTable dtt = MiddleDBInterface.getdt(getOrgID, SQLHelper.sqlconn(Login.strConn));

            sql = string.Format(@"select top 1 A.Code from CBO_ItemMaster A where MainItemCategory=1001901100129634 and Org={0} order by A.Code desc", Convert.ToUInt64(dtt.Rows[0]["ID"]));
            DataTable dt = MiddleDBInterface.getdt(sql, SQLHelper.sqlconn(Login.strConn));
            if (JudgeDs(dt))
            {
                code = "S0102010001";
                return code;
            }
            string categoryCode = "S010201";
            string maxcode = dt.Rows[0]["Code"].ToString();
            //流水号
            Int32 liushuiHao = Convert.ToInt32(maxcode.Substring(categoryCode.Length).TrimStart('0')) + 1;
            code = categoryCode + liushuiHao.ToString("0000");
            return code;
        }

        /// <summary>
        /// 第一次获取以后特殊处理
        /// </summary>
        /// <returns></returns>
        private static string GetMaxItemCode(string maxCode)
        {
            string code = string.Empty;
            string categoryCode = "S010201";
            //流水号
            Int32 liushuiHao = Convert.ToInt32(maxCode.Substring(categoryCode.Length).TrimStart('0')) + 1;
            code = categoryCode + liushuiHao.ToString("0000");
            return code;
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
        /// <summary>
        /// 判断当前料号是否是最下级,是返回true,否返回false
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool GetLevelCount(string XH, List<string> list)
        {
            int i = 0;
            foreach (var item in list)
            {
                string[] str = item.Split('/');
                if (str[0].Contains(XH))
                {
                    i++;
                }
            }
            if (i == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool GetLevelCount3(string XH, List<string> list)
        {
            int i = 0;
            foreach (var item in list)
            {
                if (item.Contains(XH))
                {
                    i++;
                }
            }
            if (i == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //1-1/1-1  1-1/1-2   1-1/1-3  1-1/1-1/1
        public bool GetLevelCount2(string XH, List<string> list)
        {
            int i = 0;
            foreach (var item in list)
            {
                if (item.Contains(XH))
                {
                    i++;
                }
            }
            if (i == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断第一层料号是否是最下级,是返回true,否返回false
        /// </summary>
        /// <param name="XH"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool GetLevel1Count(string XH, List<string> list)
        {
            int i = 0;
            foreach (var item in list)
            {
                string[] str = item.Split('/');
                if (str[0].Contains(XH + "-" + "1"))
                {
                    i++;
                }
            }
            if (i == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除展开层是1的行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        protected DataTable DeleteRow(DataTable dataTable)
        {
            DataRow[] foundRow;
            foundRow = dataTable.Select("展开层= '1'");
            foreach (DataRow item in foundRow)
            {
                dataTable.Rows.Remove(item);
            }
            //dataTable.Rows.Remove(foundRow);//注意foundRow 可能为多行，需要循环执行。
            dataTable.AcceptChanges();//对DataTable（全部）操作完之后，一定要执行这一步，否则结果不保存
            return dataTable;
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }

        ///aspose.dll 读取
        public static DataTable GetData(string fileName)
        {
            Workbook workbook = new Workbook(fileName);
            Worksheet sheet = workbook.Worksheets[0]; //工作表 
            Cells cells = sheet.Cells;//单元格 
            DataTable dataTable = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, 9, true);//noneTitle
            return dataTable;
        }
    }
}
