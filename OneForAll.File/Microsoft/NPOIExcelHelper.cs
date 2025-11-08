using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OneForAll.Core;
using OneForAll.Core.Extension;
using OneForAll.Core.Utility;
using OneForAll.File.Enums;

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
            FileTypeEnum type = FileTypeEnum.Xlsx;
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                if (Path.GetExtension(filePath) == ".xls")
                {
                    type = FileTypeEnum.Xls;
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
        public static IEnumerable<DataTable> Import(Stream stream, FileTypeEnum type = FileTypeEnum.Xlsx, bool isFirstTitle = false)
        {
            var dts = new List<DataTable>();
            using (var workbook = GetWorkbook(stream, type))
            {
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
                            }
                            ;
                            column[c.ColumnIndex] = c.GetCellStringValue();
                        });
                        dt.Rows.Add(column);
                    }
                    dts.Add(dt);
                }
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
        public static IEnumerable<T> Import<T>(Stream stream, FileTypeEnum type = FileTypeEnum.Xlsx, bool isFirstTitle = true) where T : class, new()
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
        /// <param name="type">第一行是否标题列</param>
        /// <param name="isFirstTitle">表格集合</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> ImportAsync<T>(Stream stream, FileTypeEnum type = FileTypeEnum.Xlsx, bool isFirstTitle = true) where T : class, new()
        {
            return await Task.Run(() =>
            {
                return Import<T>(stream, type, isFirstTitle);
            });
        }

        #endregion

        #region DataTable导出

        /// <summary>
        /// 导出Excel并保存到本地
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="filePath">文件保存路径</param>
        /// <param name="columns">列下标</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// <param name="isNoWriteMode">是否不写出模式</param>
        public static void Export(
            IEnumerable<DataTable> dts,
            FileTypeEnum type,
            string filePath,
            int[] columns = null,
            bool isWriteColumnHeader = true,
            bool isNoWriteMode = true)
        {
            using (var workbook = Export(dts, type, columns, isWriteColumnHeader, isNoWriteMode))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="type">文件类型</param>
        /// <param name="columns">列下标</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// <param name="isNoWriteMode">是否不写出模式</param>
        /// <returns>Excel</returns>
        public static IWorkbook Export(
            DataTable dt,
            FileTypeEnum type,
            int[] columns = null,
            bool isWriteColumnHeader = true,
            bool isNoWriteMode = true)
        {
            var dts = new List<DataTable>() { dt };
            return Export(dts, type, columns, isWriteColumnHeader, isNoWriteMode);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="columns">列下标</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// <param name="isNoWriteMode">是否不写出模式</param>
        /// <returns>Excel</returns>
        public static IWorkbook Export(
            IEnumerable<DataTable> dts,
            FileTypeEnum type,
            int[] columns = null,
            bool isWriteColumnHeader = true,
            bool isNoWriteMode = true)
        {
            if (columns.IsNull()) return Export(dts, type, isWriteColumnHeader);
            if (isNoWriteMode)
            {
                return IgnorableExport(dts, type, columns, isWriteColumnHeader);
            }
            else
            {
                return SortableExport(dts, type, columns, isWriteColumnHeader);
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// <returns>Excel</returns>
        public static IWorkbook Export(IEnumerable<DataTable> dts, FileTypeEnum type, bool isWriteColumnHeader)
        {
            ISheet sheet = null;
            IWorkbook workbook = GetWorkbook(type);
            dts.ForEach(t =>
            {
                var index = isWriteColumnHeader ? 1 : 0;
                var sheetName = t.TableName.IsNullOrEmpty() ? ("Sheet" + index) : t.TableName;
                sheet = workbook.CreateSheet(sheetName);
                if (isWriteColumnHeader)
                {
                    var row = sheet.CreateRow(0);
                    var columnIndex = 0;
                    for (int i = 0; i < t.Columns.Count; i++)
                    {
                        row.CreateCell(columnIndex, CellType.String).SetCellValue(t.Columns[columnIndex].ColumnName);
                        row.Cells[columnIndex].SetColumnWidth();
                        columnIndex++;
                    }
                }
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    var row = sheet.CreateRow(index);
                    var columnIndex = 0;
                    for (int j = 0; j < t.Columns.Count; j++)
                    {
                        row.CreateCell(columnIndex, CellType.String).SetCellValue(t.Rows[i][columnIndex].ToString());
                        row.Cells[columnIndex].SetColumnWidth();
                        columnIndex++;
                    }
                    index++;
                }
            });
            return workbook;
        }

        /// <summary>
        /// 导出Excel（可排序列）
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="columns">导出的列下标</param>
        /// <param name="isWriteColumnHeader">是否显示列标题</param>
        /// <returns>Excel</returns>
        public static IWorkbook SortableExport(IEnumerable<DataTable> dts, FileTypeEnum type, int[] columns, bool isWriteColumnHeader)
        {
            ISheet sheet = null;
            IWorkbook workbook = GetWorkbook(type);
            dts.ForEach(t =>
            {
                var index = isWriteColumnHeader ? 1 : 0;
                var sheetName = t.TableName.IsNullOrEmpty() ? ("Sheet" + index) : t.TableName;
                sheet = workbook.CreateSheet(sheetName);
                if (isWriteColumnHeader)
                {
                    var columnIndex = 0;
                    var row = sheet.CreateRow(0);
                    columns.ForEach(i =>
                    {
                        if (!t.Columns[i].IsNull())
                        {
                            row.CreateCell(columnIndex, CellType.String).SetCellValue(t.Columns[i].ColumnName);
                            row.Cells[columnIndex].SetColumnWidth();
                            columnIndex++;
                        }
                    });
                }
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    var row = sheet.CreateRow(index);
                    var columnIndex = 0;
                    columns.ForEach(j =>
                    {
                        if (!t.Columns[j].IsNull())
                        {
                            row.CreateCell(columnIndex, CellType.String).SetCellValue(t.Rows[i][j].ToString());
                            row.Cells[columnIndex].SetColumnWidth();
                            columnIndex++;
                        }
                    });
                    index++;
                }
            });
            return workbook;
        }

        /// <summary>
        /// 导出Excel（可忽略列）
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="noWriteColumns">不导出列下标</param>
        /// <param name="isWriteColumnHeader">是否显示列标题</param>
        /// <returns>Excel</returns>
        public static IWorkbook IgnorableExport(IEnumerable<DataTable> dts, FileTypeEnum type, int[] noWriteColumns, bool isWriteColumnHeader)
        {
            ISheet sheet = null;
            IWorkbook workbook = GetWorkbook(type);
            dts.ForEach(t =>
            {
                var index = isWriteColumnHeader ? 1 : 0;
                var sheetName = t.TableName.IsNullOrEmpty() ? ("Sheet" + index) : t.TableName;
                sheet = workbook.CreateSheet(sheetName);
                if (isWriteColumnHeader)
                {
                    var row = sheet.CreateRow(0);
                    var columnIndex = 0;
                    for (int i = 0; i < t.Columns.Count; i++)
                    {
                        if (noWriteColumns.Contains(i)) continue;
                        row.CreateCell(columnIndex, CellType.String).SetCellValue(t.Columns[columnIndex].ColumnName);
                        row.Cells[columnIndex].SetColumnWidth();
                        columnIndex++;
                    }
                }
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    var row = sheet.CreateRow(index);
                    var columnIndex = 0;
                    for (int j = 0; j < t.Columns.Count; j++)
                    {
                        if (noWriteColumns.Contains(j)) continue;
                        row.CreateCell(columnIndex, CellType.String).SetCellValue(t.Rows[i][columnIndex].ToString());
                        row.Cells[columnIndex].SetColumnWidth();
                    }
                    index++;
                }
            });
            return workbook;
        }

        #endregion

        #region 实体导出

        /// <summary>
        /// 导出Excel并保存到本地
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="filePath">文件保存路径</param>
        /// <param name="columns">列下标</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// <param name="isNoWriteMode">是否不写出模式</param>
        public static void EntityExport<T>(
            IEnumerable<T> dts,
            FileTypeEnum type,
            string filePath,
            int[] columns = null,
            bool isWriteColumnHeader = true,
            bool isNoWriteMode = true) where T : class, new()
        {
            using (var workbook = EntityExport(dts, type, columns, isWriteColumnHeader, isNoWriteMode))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="columns">列下标</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// <param name="isNoWriteMode">是否不写出模式</param>
        /// <returns>Excel</returns>
        public static IWorkbook EntityExport<T>(
            IEnumerable<T> dts,
            FileTypeEnum type,
            int[] columns = null,
            bool isWriteColumnHeader = true,
            bool isNoWriteMode = true) where T : class, new()
        {
            if (columns.IsNull()) return EntityExport(dts, type, isWriteColumnHeader);
            if (isNoWriteMode)
            {
                return IgnorableEntityExport(dts, type, columns, isWriteColumnHeader);
            }
            else
            {
                return SortableEntityExport(dts, type, columns, isWriteColumnHeader);
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="isWriteColumnHeader">是否将列标题写入</param>
        /// <returns>Excel</returns>
        public static IWorkbook EntityExport<T>(
            IEnumerable<T> dts,
            FileTypeEnum type,
            bool isWriteColumnHeader) where T : class, new()
        {
            ISheet sheet = null;
            IWorkbook workbook = GetWorkbook(type);

            // 表名
            var sheetName = "Sheet1";
            var obj = dts.FirstOrDefault();
            var objAttr = typeof(T).GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
            if (objAttr != null)
            {
                sheetName = ((DisplayAttribute)objAttr).Name;
            }
            sheet = workbook.CreateSheet(sheetName);

            // 表头
            var props = typeof(T).GetProperties();
            if (isWriteColumnHeader)
            {
                var row = sheet.CreateRow(0);
                var columnIndex = 0;
                for (int j = 0; j < props.Length; j++)
                {
                    var attr = props[j].GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
                    if (attr != null)
                    {
                        var name = ((DisplayAttribute)attr).Name;
                        row.CreateCell(columnIndex, CellType.String).SetCellValue(name);
                        row.Cells[columnIndex].SetColumnWidth();
                    }
                    else
                    {
                        row.CreateCell(columnIndex, CellType.String).SetCellValue("列{0}".Fmt(columnIndex + 1));
                        row.Cells[columnIndex].SetColumnWidth();
                    }
                    columnIndex++;
                }
            }

            // 列表
            var i = isWriteColumnHeader ? 1 : 0;
            dts.ForEach(t =>
            {
                var row = sheet.CreateRow(i);
                var columnIndex = 0;
                for (int j = 0; j < props.Length; j++)
                {
                    row.CreateCell(columnIndex, props[j], t);
                    row.Cells[columnIndex].SetColumnWidth();
                    columnIndex++;
                }
                i++;
            });
            return workbook;
        }

        /// <summary>
        /// 导出Excel（可排序列）
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="columns">导出的列下标</param>
        /// <param name="isWriteColumnHeader">是否显示列标题</param>
        /// <returns>Excel</returns>
        public static IWorkbook SortableEntityExport<T>(
            IEnumerable<T> dts,
            FileTypeEnum type,
            int[] columns,
            bool isWriteColumnHeader) where T : class, new()
        {
            ISheet sheet = null;
            IWorkbook workbook = GetWorkbook(type);

            // 表名
            var sheetName = "Sheet1";
            var obj = dts.FirstOrDefault();
            var objAttr = typeof(T).GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
            if (objAttr != null)
            {
                sheetName = ((DisplayAttribute)objAttr).Name;
            }
            sheet = workbook.CreateSheet(sheetName);

            // 表头
            var props = typeof(T).GetProperties();
            if (isWriteColumnHeader)
            {
                var row = sheet.CreateRow(0);
                var columnIndex = 0;
                columns.ForEach(j =>
                {
                    if (!props[j].IsNull())
                    {
                        var attr = props[j].GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
                        if (attr != null)
                        {
                            var name = ((DisplayAttribute)attr).Name;
                            row.CreateCell(columnIndex, CellType.String).SetCellValue(name);
                            row.Cells[columnIndex].SetColumnWidth();
                        }
                        else
                        {
                            row.CreateCell(columnIndex, CellType.String).SetCellValue("列{0}".Fmt(columnIndex + 1));
                            row.Cells[columnIndex].SetColumnWidth();
                        }
                        columnIndex++;
                    }
                });
            }

            // 列表
            var i = isWriteColumnHeader ? 1 : 0;
            dts.ForEach(t =>
            {
                var row = sheet.CreateRow(i);
                var columnIndex = 0;
                columns.ForEach(j =>
                {
                    if (!props[j].IsNull())
                    {
                        row.CreateCell(columnIndex, props[j], t);
                        row.Cells[columnIndex].SetColumnWidth();
                        columnIndex++;
                    }
                });
                i++;
            });
            return workbook;
        }

        /// <summary>
        /// 导出Excel（可忽略列）
        /// </summary>
        /// <param name="dts">数据表集合</param>
        /// <param name="type">文件类型</param>
        /// <param name="noWriteColumns">不导出列下标</param>
        /// <param name="isWriteColumnHeader">是否显示列标题</param>
        /// <returns>Excel</returns>
        public static IWorkbook IgnorableEntityExport<T>(
            IEnumerable<T> dts,
            FileTypeEnum type,
            int[] noWriteColumns,
            bool isWriteColumnHeader) where T : class, new()
        {
            ISheet sheet = null;
            IWorkbook workbook = GetWorkbook(type);

            // 表名
            var sheetName = "Sheet1";
            var obj = dts.FirstOrDefault();
            var objAttr = typeof(T).GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
            if (objAttr != null)
            {
                sheetName = ((DisplayAttribute)objAttr).Name;
            }
            sheet = workbook.CreateSheet(sheetName);

            // 表头
            var props = typeof(T).GetProperties();
            if (isWriteColumnHeader)
            {
                var row = sheet.CreateRow(0);
                var columnIndex = 0;
                for (int j = 0; j < props.Length; j++)
                {
                    if (noWriteColumns.Contains(j)) continue;
                    var attr = props[j].GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
                    if (attr != null)
                    {
                        var name = ((DisplayAttribute)attr).Name;
                        row.CreateCell(columnIndex, CellType.String).SetCellValue(name);
                        row.Cells[columnIndex].SetColumnWidth();
                    }
                    else
                    {
                        row.CreateCell(columnIndex, CellType.String).SetCellValue("列{0}".Fmt(columnIndex + 1));
                        row.Cells[columnIndex].SetColumnWidth();
                    }
                    columnIndex++;
                }
            }

            // 列表
            var i = isWriteColumnHeader ? 1 : 0;
            dts.ForEach(t =>
            {
                var row = sheet.CreateRow(i);
                var columnIndex = 0;
                for (int j = 0; j < props.Length; j++)
                {
                    if (noWriteColumns.Contains(j)) continue;
                    row.CreateCell(columnIndex, props[j], t);
                    row.Cells[columnIndex].SetColumnWidth();
                    columnIndex++;
                }
                i++;
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
        public static IWorkbook GetWorkbook(Stream stream, FileTypeEnum type)
        {
            IWorkbook workbook = null;
            string sheetName = string.Empty;
            if (type == FileTypeEnum.Xlsx)
            {
                // 2007版本
                workbook = new XSSFWorkbook(stream);
            }
            else if (type == FileTypeEnum.Xls)
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
        public static IWorkbook GetWorkbook(FileTypeEnum type)
        {
            IWorkbook workbook = null;
            string sheetName = string.Empty;
            if (type == FileTypeEnum.Xlsx)
            {
                workbook = new XSSFWorkbook();
            }
            else if (type == FileTypeEnum.Xls)
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
                    if (isdete) return cell.DateCellValue.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    return cell.NumericCellValue.ToString();
                case CellType.String: return cell.StringCellValue.ToString();
                case CellType.Unknown:
                default: return "";
            }
        }

        /// <summary>
        /// 根据属性类型创建对应单元格
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns>字符串值</returns>
        public static void CreateCell(this IRow row, int columnIndex, PropertyInfo property, object instance)
        {
            var val = property.GetValue(instance);
            var cell = row.CreateCell(columnIndex);
            if (property.PropertyType.Equals(typeof(string)))
            {
                cell.SetCellType(CellType.String);
                cell.SetCellValue(val == null ? "" : val.ToString());
            }
            else if (property.PropertyType.Equals(typeof(int)) ||
                property.PropertyType.Equals(typeof(double)) ||
                property.PropertyType.Equals(typeof(decimal)) ||
                property.PropertyType.Equals(typeof(float)))
            {
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue(Convert.ToDouble(val));
                var attr = property.GetCustomAttributes(typeof(DisplayFormatAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    var format = ((DisplayFormatAttribute)attr).DataFormatString ?? "";
                    if (!format.IsNullOrEmpty())
                    {
                        IDataFormat dataFormat = cell.Sheet.Workbook.CreateDataFormat();
                        ICellStyle style = cell.Sheet.Workbook.CreateCellStyle();
                        style.DataFormat = dataFormat.GetFormat(format);
                        cell.CellStyle = style;
                    }
                }
            }
            else if (property.PropertyType.Equals(typeof(DateTime)) || property.PropertyType.Equals(typeof(DateTime?)))
            {
                cell.SetCellType(CellType.Numeric);
                var format = "yyyy-MM-dd hh:mm:ss ";
                var attr = property.GetCustomAttributes(typeof(DisplayFormatAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    format = ((DisplayFormatAttribute)attr).DataFormatString ?? format;
                }
                IDataFormat dataFormat = cell.Sheet.Workbook.CreateDataFormat();
                ICellStyle style = cell.Sheet.Workbook.CreateCellStyle();
                style.DataFormat = dataFormat.GetFormat(format);
                cell.SetCellValue(val.TryDateTime());
                cell.CellStyle = style;
            }
            else if (property.PropertyType.Equals(typeof(bool)))
            {
                cell.SetCellType(CellType.Boolean);
            }
            else if (property.PropertyType.Equals(typeof(string)) && instance != null)
            {
                var value = property.GetValue(instance);
                if (value != null &&
                    value.ToString().ToLower().StartsWith("http") ||
                    value.ToString().ToLower().StartsWith("https"))
                {
                    cell.SetCellType(CellType.Blank);
                    IHyperlink link;
                    if (cell.Sheet.Workbook.GetType() == typeof(XSSFWorkbook))
                    {
                        link = new XSSFHyperlink(HyperlinkType.Url);
                        link.Address = val.ToString();
                    }
                    else
                    {
                        link = new HSSFHyperlink(HyperlinkType.Url);
                        link.Address = val.ToString();
                    }
                    cell.SetCellValue(val.ToString());
                    cell.Hyperlink = link;
                }
                else
                {
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(val == null ? "" : val.ToString());
                }
            }
            else
            {
                cell.SetCellType(CellType.String);
                cell.SetCellValue(val == null ? "" : val.ToString());
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
