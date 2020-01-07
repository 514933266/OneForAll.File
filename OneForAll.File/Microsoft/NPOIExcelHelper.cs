using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OneForAll.Core.Extension;
using OneForAll.Core.Utility;

namespace OneForAll.File
{
    /// <summary>
    /// NPOI插件操作EXCEL类
    /// </summary>
    public static class NPOIExcelHelper
    {
        #region 导入

        /// <summary>
        /// 导入本地Excel文件并转化为指定对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <returns>第一个Sheet的列表对象</returns>
        public static IEnumerable<T> Import<T>(string filePath) where T : class, new()
        {
            IEnumerable<T> list = null;
            FileType type = FileType.xlsx;
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                if (Path.GetExtension(filePath) == ".xls")
                {
                    type = FileType.xls;
                }
                var dts = Import(stream, type, false);
                list = ReflectionHelper.ToList<T>(dts.First());
            }
            return list;
        }

        /// <summary>
        /// 读取Excel流返回表格集合
        /// </summary>
        /// <param name="stream">读取的Excel流</param>
        /// <param name="type">Excel的类型</param>
        /// <param name="isFirstTitle">第一行是否标题列</param>
        /// <returns>表格集合</returns>
        public static IEnumerable<DataTable> Import(
            Stream stream,
            FileType type = FileType.xlsx,
            bool isFirstTitle = false)
        {
            var dts = new List<DataTable>();
            var workbook = GetWorkbook(stream, type);
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var rowIndex = 0;
                var sheet = workbook.GetSheetAt(i);
                var firstRow = sheet.GetRow(0);
                var dt = new DataTable(sheet.SheetName);
                //标题行
                if (isFirstTitle)
                {
                    rowIndex++;
                    firstRow.Cells.ForEach(c =>
                    {
                        dt.Columns.Add(c.GetCellStringValue());
                    });
                }
                else
                {
                    firstRow.Cells.ForEach(c =>
                    {
                        dt.Columns.Add("列" + c.ColumnIndex);
                    });
                }
                //内容行
                for (int j = rowIndex; j <= sheet.LastRowNum; j++)
                {
                    var row = sheet.GetRow(j);
                    var colIndex = row.Cells.Count < dt.Columns.Count ? dt.Columns.Count : row.Cells.Count;
                    var column = new string[colIndex];
                    row.Cells.ForEach(c =>
                    {
                        if (c.ColumnIndex >= dt.Columns.Count)
                        {
                            var addNum = c.ColumnIndex - dt.Columns.Count + 1;
                            for (var num = 0; num <= addNum; num++)
                            {
                                dt.Columns.Add("列" + (c.ColumnIndex + num));
                            }
                            Array.Resize(ref column, dt.Columns.Count);
                        };
                        column[c.ColumnIndex] = c.GetCellStringValue();
                    });
                    dt.Rows.Add(column);
                }
                dts.Add(dt);
            }
            return dts;
        }

        /// <summary>
        /// 读取Excel流返回对象集合
        /// </summary>
        /// <typeparam name="T">读取的Excel流</typeparam>
        /// <param name="stream">Excel的类型</param>
        /// <param name="type">第一行是否标题列</param>
        /// <param name="isFirstTitle">表格集合</param>
        /// <returns></returns>
        public static IEnumerable<T> Import<T>(
            Stream stream,
            FileType type = FileType.xlsx,
            bool isFirstTitle = true) where T : class, new()
        {
            var dts = Import(stream, type, isFirstTitle);
            if (dts.Count() > 0)
            {
                return ReflectionHelper.ToList<T>(dts.First());
            }
            return null;
        }

        /// <summary>
        /// 读取Excel流返回对象集合
        /// </summary>
        /// <typeparam name="T">读取的Excel流</typeparam>
        /// <param name="stream">Excel的类型</param>
        /// <param name="errors">errors错误消息</param>
        /// <param name="type">第一行是否标题列</param>
        /// <param name="isFirstTitle">表格集合</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> ImportAsync<T>(
            Stream stream,
            FileType type = FileType.xlsx,
            bool isFirstTitle = true) where T : class, new()
        {
            return await Task.Run(() =>
            {
                return Import<T>(stream, type, isFirstTitle);
            });
        }

        #endregion

        #region 导出

        /// <summary>
        /// 导出Excel并保存到本地
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="filePath">文件保存路径</param>
        /// <param name="noWriteColumns">不被写入Excel的列下标</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        public static void Export(List<DataTable> dts, FileType type, string filePath, int[] noWriteColumns = null, bool isWriteColumnHeader = false)
        {
            var workbook = Export(dts, type, noWriteColumns, isWriteColumnHeader);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="noWriteColumns">不被写入Excel的列下标</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// 
        public static IWorkbook Export(List<DataTable> dts, FileType type, int[] noWriteColumns = null, bool isWriteColumnHeader = false)
        {
            var index = 0;
            ISheet sheet = null;
            IWorkbook workbook = GetWorkbook(type);
            dts.ForEach(t =>
            {
                index++;
                var sheetName = t.TableName.IsNullOrEmpty() ? ("Sheet" + index) : t.TableName;
                sheet = workbook.CreateSheet(sheetName);
                if (isWriteColumnHeader)
                {
                    var row = sheet.CreateRow(0);
                    for (int i = 0; i < t.Columns.Count; i++)
                    {
                        row.CreateCell(i, CellType.String).SetCellValue(t.Columns[i].ColumnName);
                        row.Cells[i].SetColumnWidth();
                    }
                    
                }
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    var row = sheet.CreateRow(isWriteColumnHeader ? i + 1 : i);
                    for (int j = 0; j < t.Columns.Count; j++)
                    {
                        if (noWriteColumns != null && j.In(noWriteColumns)) continue;
                        row.CreateCell(j, CellType.String).SetCellValue(t.Rows[i][j].ToString());
                        row.Cells[j].SetColumnWidth();
                    }
                }
            });
            return workbook;
        }

        #endregion 

        #region 获取工作簿

        /// <summary>
        /// 通过数据流读取Excel并返回Excel工作簿对象
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="type">excel类型</param>
        /// <returns>工作簿（整个Excel文件对象）</returns>
        public static IWorkbook GetWorkbook(Stream stream, FileType type)
        {
            IWorkbook workbook = null;
            string sheetName = string.Empty;
            if (type == FileType.xlsx)
            { 
                // 2007版本
                workbook = new XSSFWorkbook(stream);
            }
            else if (type == FileType.xls)
            {
                // 2003版本
                workbook = new HSSFWorkbook(stream);
            }
            return workbook;
        }


        /// <summary>
        /// 获取一个空的Excel工作簿对象
        /// </summary>
        /// <param name="type">Excel的格式类型</param>
        /// <returns>工作簿（整个Excel文件对象）</returns>
        public static IWorkbook GetWorkbook(FileType type)
        {
            IWorkbook workbook = null;
            string sheetName = string.Empty;
            if (type == FileType.xlsx)
            {
                workbook = new XSSFWorkbook();
            }
            else if (type == FileType.xls)
            {
                workbook = new HSSFWorkbook();
            }
            return workbook;
        }
        #endregion

        #region 其他

        /// <summary>
        /// 获取某些支持的单元格样式的字符串值
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns>字符串值</returns>
        public static string GetCellStringValue(this ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Blank: return cell.Hyperlink == null ? "" : cell.Hyperlink.ToString();
                case CellType.Boolean: return cell.BooleanCellValue.ToString();
                case CellType.Error: return cell.ErrorCellValue.ToString();
                case CellType.Formula: return cell.CellFormula;
                case CellType.Numeric:
                    var isdete = DateUtil.IsCellDateFormatted(cell);
                    if (isdete) return cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                    return cell.NumericCellValue.ToString();
                case CellType.String: return cell.StringCellValue.ToString();
                case CellType.Unknown:
                default: return "";
            }
        }

        /// <summary>
        /// 根据列的内容自适应宽度
        /// </summary>
        /// <param name="cell">单元格</param>
        public static void SetColumnWidth(this ICell cell)
        {
            var length = Encoding.GetEncoding("UTF-8").GetBytes(cell.GetCellStringValue()).Length;//获取当前单元格的内容宽度
            var columnWidth = cell.Sheet.GetColumnWidth(cell.ColumnIndex) / 256;//获取当前列宽度  
            if (length > 255)
            {
                cell.Sheet.SetColumnWidth(cell.ColumnIndex, 255 * 256);
            }
            else
            {
                if (columnWidth <= length && length < 255)
                {
                    columnWidth = length + 1;
                }
                cell.Sheet.SetColumnWidth(cell.ColumnIndex, columnWidth * 256);
            }
        }
        #endregion

    }
}
