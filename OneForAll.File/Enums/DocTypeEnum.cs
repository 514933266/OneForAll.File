using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File.Enums
{
    /// <summary>
    /// 文档类型
    /// </summary>
    public enum DocTypeEnum
    {
        /// <summary>
        /// Excel 2007+ 文档
        /// </summary>
        Xlsx = FileTypeEnum.Xlsx,

        /// <summary>
        /// Excel 97-2003 文档
        /// </summary>
        Xls = FileTypeEnum.Xls,

        /// <summary>
        /// Word 97-2003 文档
        /// </summary>
        Doc = FileTypeEnum.Doc,

        /// <summary>
        /// Word 2007+ 文档
        /// </summary>
        Docx = FileTypeEnum.Docx,

        /// <summary>
        /// 纯文本文件
        /// </summary>
        Txt = FileTypeEnum.Txt,

        /// <summary>
        /// PowerPoint 97-2003 演示文稿
        /// </summary>
        Ppt = FileTypeEnum.Ppt,

        /// <summary>
        /// PowerPoint 2007+ 演示文稿
        /// </summary>
        Pptx = FileTypeEnum.Pptx,

        /// <summary>
        /// PDF 文档
        /// </summary>
        Pdf = FileTypeEnum.Pdf
    }
}
