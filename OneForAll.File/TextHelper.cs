using OneForAll.Core.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OneForAll.File
{
    /// <summary>
    /// 帮助类：文本
    /// </summary>
    public static class TextHelper
   {
        /// <summary>
        /// 创建一个新的txt格式文本,如果文本已经存在则覆盖
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <param name="recover">是否覆盖源文件</param>
        public static void Create(string fileName, bool recover)
        {
            DirectoryHelper.Create(fileName);
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.CreateText(fileName).Close();
            }
            else
            {
                bool b = false;
                while (!b)
                {
                    b = FileHelper.CheckIsWritable(fileName);
                }
                if (b && recover)
                {
                    System.IO.File.WriteAllText(fileName, "");
                }
            }
        }

        /// <summary>
        /// 将文本写入流文件(覆盖旧文件)
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <param name="content">要写入的内容</param>
        public static void Write(string fileName, string content)
        {
            Create(fileName, true);
            using (StreamWriter streamWriter = new StreamWriter(fileName, false))
            {
                foreach (var line in content)
                {
                    streamWriter.Write(line);
                }
            }
        }

        /// <summary>
        /// 将文本写入txt文件(覆盖旧文件)
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <param name="content">要写入的内容</param>
        /// <param name="encoding">编码格式</param>
        public static void Write(string fileName, string content, Encoding encoding)
        {
            Create(fileName, true);
            encoding = encoding ?? Encoding.UTF8;
            using (StreamWriter streamWriter = new StreamWriter(fileName, false, encoding))
            {
                streamWriter.Write(content);
            }
        }

        /// <summary>
        /// 将文本追加写入txt文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        public static void WriteTo(string fileName, string content, Encoding encoding)
        {
            FileStream fs = null;
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(content);
            try
            {
                Create(fileName, false);
                if (FileHelper.CheckIsWritable(fileName))
                {
                    fs = System.IO.File.OpenWrite(fileName);
                    fs.Position = fs.Length;
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch
            {
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// 读取本地文本内容
        /// </summary>
        /// <param name="path">读取路径</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>读取的文本内容</returns>
        public static string Read(string path, Encoding encoding = null)
        {
            string content = string.Empty;
            ReadLine(path, encoding).ForEach(line =>
            {
                content += line.Append("\r\n");
            });
            return content;
        }

        /// <summary>
        /// 读取本地文本内容
        /// </summary>
        /// <param name="path">读取路径</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>读取的文本内容</returns>
        public static string ReadAllText(string path, Encoding encoding=null)
        {
            if (FileHelper.CheckIsExists(path))
            {
                encoding = encoding ?? Encoding.UTF8;
                return System.IO.File.ReadAllText(path, encoding);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 读取本地文本行集合
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>文本集合</returns>
        public static List<string> ReadLine(string path, Encoding encoding)
        {
            string line=string.Empty;
            encoding = encoding ?? Encoding.UTF8;
            List<string> content = new List<string>();
            if (FileHelper.CheckIsExists(path))
            {
                using (StreamReader sr = new StreamReader(path, encoding))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        content.Add(line);
                    }
                }
            }
            return content;
        }
    }
}
