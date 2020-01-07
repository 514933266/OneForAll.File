﻿using OneForAll.File.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OneForAll.File
{
    /// <summary>
    /// 媒体类型验证
    /// </summary>
    public class ValidateMediaType : IValidateMediaType
    {
        // <summary>
        /// 验证（文件名、文件类型）
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件流</param>
        /// <returns>结果</returns>
        public bool Validate(string fileName, Stream file)
        {
            if (ValidateFileName(fileName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证文件类型
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns>结果</returns>
        public bool ValidateFileType(Stream file)
        {
            return true;
        }

        /// <summary>
        /// 验证文件名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>结果</returns>
        public bool ValidateFileName(string fileName)
        {
            return FileHelper.ValidateFileName<MediaType>(fileName);
        }
    }
}
