using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OneForAll.File.Interfaces
{
    /// <summary>
    /// 接口：文件类型验证
    /// </summary>
    public interface IValidateFileType
    {
        /// <summary>
        /// 验证（文件名、文件类型）
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件流</param>
        /// <returns>结果</returns>
        bool Validate(string fileName, Stream file);

        /// <summary>
        /// 验证文件Hex
        /// </summary>
        bool ValidateFileType(Stream file);

        /// <summary>
        /// 验证文件名
        /// </summary>
        bool ValidateFileName(string fileName);
    }
}
