using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File.Enums
{
    /// <summary>
    /// Excel文档类型
    /// </summary>
    public enum ExcelTypeEnum
    {
        /// <summary>
        /// Excel 2007+ 文件
        /// </summary>
        Xlsx = FileTypeEnum.Xlsx,

        /// <summary>
        /// Excel 97-2003 文件
        /// </summary>
        Xls = FileTypeEnum.Xls
    }
}
