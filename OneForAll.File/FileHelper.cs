using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OneForAll.Core;
using OneForAll.Core.Extension;
using OneForAll.File.Enums;
using OneForAll.File.Interfaces;

namespace OneForAll.File
{
    /// <summary>
    /// 帮助类：文件操作
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 在指定路径创建一个文件(覆盖旧文件)
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void Write(string filePath, Stream stream)
        {
            if (stream == null)
            {
                Write(filePath, new byte[0]);
            }
            else
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fs);
                }
            }
        }

        /// <summary>
        /// 在指定路径创建一个文件(覆盖旧文件)
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileByte">文件字节流</param>
        public static void Write(string filePath, byte[] fileByte)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(fileByte, 0, fileByte.Length);
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>文件流</returns>
        public static Stream Read(string filename)
        {
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// 读取指定路径文件
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>文件字节流</returns>
        public static byte[] ReadByte(string filename)
        {
            return ReadByte(filename, -1);
        }

        /// <summary>
        /// 读取指定路径文件
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="length">指定读取长度</param>
        /// <returns>文件字节流</returns>
        public static byte[] ReadByte(string filename, int length)
        {
            byte[] arr = null;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var cur = length > -1 ? length : fs.Length;
                arr = new byte[cur];
                fs.Read(arr, 0, (int)cur);
            }
            return arr;
        }

        /// <summary>
        /// 移动指定文件到新目录，并指定新名称（覆盖旧文件）
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        public static void Move(string sourcePath, string targetPath)
        {
            Copy(sourcePath, targetPath, true);
        }

        /// <summary>
        /// 移动距今指定时间差的文件
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="target">目标文件路径</param>
        /// <param name="datePart">时间类型</param>
        /// <param name="timeSpan">时间差</param>
        public static void MoveByCreateTime(string source, string target, DatePart datePart, int timeSpan)
        {
            if (System.IO.File.Exists(source))
            {
                var canMove = false;
                var fileCreateTime = System.IO.File.GetCreationTime(source);
                switch (datePart)
                {
                    case DatePart.Year: if (fileCreateTime.AddYears(timeSpan) < DateTime.Now) canMove = true; break;
                    case DatePart.Month: if (fileCreateTime.AddMonths(timeSpan) < DateTime.Now) canMove = true; break;
                    case DatePart.Week: if (fileCreateTime.AddDays(timeSpan * 7) < DateTime.Now) canMove = true; break;
                    case DatePart.Day: if (fileCreateTime.AddDays(timeSpan) < DateTime.Now) canMove = true; break;
                    case DatePart.Hour: if (fileCreateTime.AddHours(timeSpan) < DateTime.Now) canMove = true; break;
                    case DatePart.Minute: if (fileCreateTime.AddMinutes(timeSpan) < DateTime.Now) canMove = true; break;
                    case DatePart.Second: if (fileCreateTime.AddSeconds(timeSpan) < DateTime.Now) canMove = true; break;
                }
                if (System.IO.File.Exists(target)) System.IO.File.Delete(target);
                if (canMove)
                {
                    System.IO.File.Copy(source, target, true);
                }
            }
        }

        /// <summary>
        /// 复制文件到指定目录
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="target">目标文件路径</param>
        /// <param name="deleteSource">是否删除源文件</param>
        /// <param name="overWrite">是否覆盖</param>
        public static void Copy(string source, string target, bool deleteSource = false, bool overWrite = true)
        {
            if (System.IO.File.Exists(source))
            {
                DirectoryHelper.Create(target);
                System.IO.File.Copy(source, target, overWrite);
                if (deleteSource) System.IO.File.Delete(source);
            }
        }

        /// <summary>
        /// 确定文件是否可以进行读写
        /// </summary>
        /// <param name="filename">文件路径</param>
        public static bool CheckIsWritable(string filename)
        {
            try
            {
                using (FileStream fs = new FileInfo(filename).Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>结果</returns>
        public static bool CheckIsExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        /// <summary>
        /// 验证文件Hex
        /// </summary>
        /// <param name="file">文件流</param>
        /// <returns>结果</returns>
        public static bool ValidateFileType<T>(Stream file, int hexIndex = 4)
        {
            var hex = "";
            var pass = false;
            var header = new byte[hexIndex];
            file.Read(header, 0, hexIndex);
            header.ForEach(e => hex += e);
            file.Seek(0, SeekOrigin.Begin);
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                var value = (int)item;
                if (hex.StartsWith(value.ToString()))
                {
                    pass = true;
                    break;
                }
            }
            return pass;
        }

        /// <summary>
        /// 验证文件Hex
        /// </summary>
        /// <param name="file">文件流</param>
        /// <param name="fileType">文件类型</param>
        /// <returns>结果</returns>
        public static bool ValidateFileType(Stream file, FileTypeEnum fileType, int hexIndex = 4)
        {
            var hex = "";
            var pass = false;
            var header = new byte[hexIndex];
            file.Read(header, 0, hexIndex);
            header.ForEach(e => hex += e);
            file.Seek(0, SeekOrigin.Begin);
            var value = (int)fileType;
            if (hex.StartsWith(value.ToString()))
            {
                pass = true;
            }
            return pass;
        }

        /// <summary>
        /// 验证文件名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>结果</returns>
        public static bool ValidateFileName<T>(string fileName)
        {
            var pass = false;
            var extension = Path.GetExtension(fileName);
            var types = Enum.GetNames(typeof(T));
            for (var i = 0; i < types.Length; i++)
            {
                var enumType = ".".Append(types[i]);
                if (extension == enumType)
                {
                    pass = true;
                    break;
                }
            }
            return pass;
        }

        /// <summary>
        /// 验证文件名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="extensions">扩展名</param>
        /// <returns>结果</returns>
        public static bool ValidateFileName(string fileName, params string[] extensions)
        {
            var pass = false;
            var extension = Path.GetExtension(fileName);
            for (var i = 0; i < extensions.Length; i++)
            {
                if (extension == extensions[i])
                {
                    pass = true;
                    break;
                }
            }
            return pass;
        }
    }
}
