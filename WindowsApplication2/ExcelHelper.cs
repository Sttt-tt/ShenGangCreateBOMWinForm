using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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




        /// <summary>
        /// 拼接上过datagridview数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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

            dt.DefaultView.Sort = "母件料品 asc, 展开层 asc";
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
                        row1["料品形态属性"] = "制造件";
                    }
                }

            }
            dt = DeleteRow(dt);
            dt.DefaultView.Sort = "母件料品 asc";
            return dt;
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

    }
}
