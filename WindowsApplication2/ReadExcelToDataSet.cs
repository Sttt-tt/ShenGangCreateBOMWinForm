using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.Util;
using System.Text;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
/// <summary>
/// ReadExcelToDataSet 的摘要描述
/// </summary>
public class ReadExcelToDataSet
{
    public ReadExcelToDataSet()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    /// <summary>  
    /// 由Excel导入DataTable   
    /// </summary> 
    /// <param name="excelFileStream">Excel文件流</param>
    /// <param name="sheetName">Excel工作表名称</param>  
    /// <param name="headerRowIndex">Excel表头行索引</param>  
    /// <returns>DataTable</returns>  
    public static DataTable ImportDataTableFromExcel(Stream excelFileStream, string sheetName, int headerRowIndex)
    {
        //HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream); HSSFSheet sheet = workbook.GetSheet(sheetName);
        DataTable table = new DataTable();
        //HSSFRow headerRow = sheet.GetRow(headerRowIndex);
        //int cellCount = headerRow.LastCellNum;
        //for (int i = headerRow.FirstCellNum; i < cellCount; i++)
        //{
        //    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
        //    table.Columns.Add(column);
        //}
        //for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        //{
        //    HSSFRow row = sheet.GetRow(i);
        //    DataRow dataRow = table.NewRow();
        //    for (int j = row.FirstCellNum; j < cellCount; j++)
        //        dataRow[j] = row.GetCell(j).ToString();
        //}
        //excelFileStream.Close(); workbook = null;
        //sheet = null;
        return table;
    }
    /// <summary>  
    /// /// 由Excel导入DataTable  
    /// /// </summary>    
    /// <param name="excelFilePath">Excel文件路径，为物理路径。</param> 
    /// /// <param name="sheetName">Excel工作表名称</param>   
    /// /// <param name="headerRowIndex">Excel表头行索引</param>  
    /// /// <returns>DataTable</returns>   
    public static DataTable ImportDataTableFromExcel(string excelFilePath, string sheetName, int headerRowIndex)
    {
        using (FileStream stream = System.IO.File.OpenRead(excelFilePath))
        {
            return ImportDataTableFromExcel(stream, sheetName, headerRowIndex);
        }
    }
    /// <summary>  
    /// 由Excel导入DataTable  
    /// </summary>   
    /// <param name="excelFileStream">Excel文件流</param>  
    /// <param name="sheetName">Excel工作表索引</param>  
    /// <param name="headerRowIndex">Excel表头行索引</param> 
    /// <param name="sFormat">true bom导入，false 工艺导入</param> 
    /// <returns>DataTable</returns>  
    public static DataTable ImportDataTableFromExcel(Stream excelFileStream, int sheetIndex, int headerRowIndex, bool sFormat)
    {
        //HSSFWorkbook hssfworkbook = new HSSFWorkbook(excelFileStream);
        //HSSFSheet sheetAt = hssfworkbook.GetSheetAt(sheetIndex);
        DataTable dataTable = new DataTable();
        //HSSFRow row = sheetAt.GetRow(headerRowIndex);
        //int num = row.LastCellNum;
        //for (int i = row.FirstCellNum; i < num; i++)
        //{
        //    if ((row.GetCell(i) == null || row.GetCell(i).ToString().Trim() == "") && i > 10)
        //    {
        //        num = i + 1;
        //        break;
        //    }
        //    DataColumn column;
        //    if (row.GetCell(i) == null)
        //    {
        //        column = new DataColumn("序号");
        //    }
        //    else
        //    {
        //        column = new DataColumn(row.GetCell(i).ToString().Trim());
        //    }
        //    dataTable.Columns.Add(column);
        //}
        //for (int i = headerRowIndex + 1; i <= sheetAt.LastRowNum - 8; i++)
        //{
        //    HSSFRow row2 = sheetAt.GetRow(i);
        //    if (row2 == null || row2.GetCell(0) == null || row2.GetCell(0).ToString().Trim() == "")
        //    {
        //        break;
        //    }
        //    DataRow dataRow = dataTable.NewRow();
        //    if (!sFormat)
        //    {
        //        if (row2.GetCell(6).ToString().Trim().Length != 3)
        //        {
        //            MessageBox.Show(string.Concat(new object[]
        //            {
        //                "第",
        //                Convert.ToInt32(i + 1),
        //                "行的工序行号",
        //                row2.GetCell(6).StringCellValue,
        //                "不是3位，请更改源Excel"
        //            }));
        //            return null;
        //        }
        //    }
        //    for (int j = row2.FirstCellNum; j < num; j++)
        //    {
        //        if (j < dataTable.Columns.Count)
        //        {
        //            if (sFormat)
        //            {
        //                if (j == 6)
        //                {
        //                    dataRow[j] = (string.IsNullOrEmpty(row2.GetCell(j).StringCellValue) ? "X0" : row2.GetCell(j).StringCellValue);
        //                }
        //                else
        //                {
        //                    dataRow[j] = ((row2.GetCell(j) == null) ? "" : row2.GetCell(j).ToString().Trim());
        //                }
        //            }
        //            else if (j == 5)
        //            {
        //                dataRow[j] = (string.IsNullOrEmpty(row2.GetCell(j).StringCellValue) ? "X0" : row2.GetCell(j).StringCellValue);
        //            }
        //            else if (j == 15 || j == 16 || j == 14)
        //            {
        //                dataRow[j] = (string.IsNullOrEmpty(row2.GetCell(j).ToString().Trim()) ? 0m : Convert.ToDecimal(row2.GetCell(j).ToString().Trim()));
        //            }
        //            else
        //            {
        //                dataRow[j] = row2.GetCell(j);
        //            }
        //        }
        //    }
        //    dataTable.Rows.Add(dataRow);
        //}
        //excelFileStream.Close();
        return dataTable;
    }

    /// <summary>  
    /// /// 由Excel导入DataTable 
    /// /// </summary>   
    /// /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>  
    /// /// <param name="sheetName">Excel工作表索引</param>  
    /// /// <param name="headerRowIndex">Excel表头行索引</param>  
    /// /// <returns>DataTable</returns>  
    public static DataTable ImportDataTableFromExcel(string excelFilePath, int sheetIndex, int headerRowIndex, bool sFormat)
    {
        using (FileStream stream = System.IO.File.OpenRead(excelFilePath))
        {
            return ImportDataTableFromExcel(stream, sheetIndex, headerRowIndex, sFormat);
        }
    }
    /// <summary> 
    /// /// 由Excel导入DataSet，如果有多个工作表，则导入多个DataTable  
    /// /// </summary> 
    /// /// <param name="excelFileStream">Excel文件流</param>  
    /// /// <param name="headerRowIndex">Excel表头行索引</param> 
    /// /// <returns>DataSet</returns>   
    public static DataSet ImportDataSetFromExcel(Stream excelFileStream, int headerRowIndex)
    {
        DataSet ds = new DataSet();
        //HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
        //for (int a = 0, b = workbook.NumberOfSheets; a < b; a++)
        //{
        //    HSSFSheet sheet = workbook.GetSheetAt(a);
        //    DataTable table = new DataTable();
        //    HSSFRow headerRow = sheet.GetRow(headerRowIndex);
        //    int cellCount = headerRow.LastCellNum;
        //    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
        //    {
        //        if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
        //        {
        //            // 如果遇到第一个空列，则不再继续向后读取        
        //            cellCount = i + 1;
        //            break;
        //        }
        //        DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
        //        table.Columns.Add(column);
        //    }
        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        //    {
        //        HSSFRow row = sheet.GetRow(i);
        //        if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "")
        //        {
        //            // 如果遇到第一个空行，则不再继续向后读取             
        //            break;
        //        }
        //        DataRow dataRow = table.NewRow();
        //        for (int j = row.FirstCellNum; j < cellCount; j++)
        //        {
        //            if (row.GetCell(j) != null)
        //            {
        //                dataRow[j] = row.GetCell(j).ToString();
        //            }
        //        }
        //        table.Rows.Add(dataRow);
        //    }
        //    ds.Tables.Add(table);
        //}
        //excelFileStream.Close();
        //workbook = null;
        return ds;
    }
    /// <summary>  
    /// /// 由Excel导入DataSet，如果有多个工作表，则导入多个DataTable
    /// /// </summary> 
    /// /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>  
    /// /// <param name="headerRowIndex">Excel表头行索引</param>  
    /// /// <returns>DataSet</returns>   
    public static DataSet ImportDataSetFromExcel(string excelFilePath, int headerRowIndex)
    {
        using (FileStream stream = System.IO.File.OpenRead(excelFilePath))
        {
            return ImportDataSetFromExcel(stream, headerRowIndex);
        }
    }

    /// <summary>  
    /// 将Excel的列索引转换为列名，列索引从0开始，列名从A开始。如第0列为A，第1列为B...  
    /// </summary>   
    /// <param name="index">列索引</param>   
    /// <returns>列名，如第0列为A，第1列为B...</returns> 
    public static string ConvertColumnIndexToColumnName(int index)
    {
        index = index + 1; int system = 26;
        char[] digArray = new char[100]; int i = 0;
        while (index > 0)
        {
            int mod = index % system;
            if (mod == 0) mod = system;
            digArray[i++] = (char)(mod - 1 + 'A');
            index = (index - 1) / 26;
        }
        StringBuilder sb = new StringBuilder(i);
        for (int j = i - 1; j >= 0; j--)
        {
            sb.Append(digArray[j]);
        }
        return sb.ToString();
    }
    /// <summary>  
    /// /// 转化日期    
    /// </summary>    
    /// <param name="date">日期</param> 
    /// /// <returns></returns>  
    public static DateTime ConvertDate(string date)
    {
        DateTime dt = new DateTime();
        string[] time = date.Split('-');
        int year = Convert.ToInt32(time[2]);
        int month = Convert.ToInt32(time[0]);
        int day = Convert.ToInt32(time[1]);
        string years = Convert.ToString(year);
        string months = Convert.ToString(month);
        string days = Convert.ToString(day);
        if (months.Length == 4)
        {
            dt = Convert.ToDateTime(date);
        }
        else
        {
            string rq = "";
            if (years.Length == 1)
            {
                years = "0" + years;
            }
            if (months.Length == 1)
            {
                months = "0" + months;
            }
            if (days.Length == 1)
            {
                days = "0" + days;
            }
            rq = "20" + years + "-" + months + "-" + days;
            dt = Convert.ToDateTime(rq);
        }
        return dt;
    }
}
