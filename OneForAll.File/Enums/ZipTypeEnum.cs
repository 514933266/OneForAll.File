using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File.Enums
{
    /// <summary>
    /// 压缩文件类型
    /// </summary>
    public enum ZipTypeEnum
    {
        /// <summary>
        /// ZIP 压缩文件
        /// </summary>
        Zip = FileTypeEnum.Zip,

        /// <summary>
        /// RAR 压缩文件
        /// </summary>
        Rar = FileTypeEnum.Rar
    }
}
