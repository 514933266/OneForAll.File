using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OneForAll.Core.Extension;
using OneForAll.Core.Upload;

namespace OneForAll.File
{
    /// <summary>
    /// 文件上传器
    /// </summary>
    public class Uploader : IUploader
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="path">保存路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="maxSize">最大限制</param>
        public async Task<IUploadResult> WriteAsync(Stream fileStream, string path, string fileName, bool autoName = false, int maxSize = 0)
        {
            if (autoName)
            {
                var extension = Path.GetExtension(fileName);
                var autoFileName = Guid.NewGuid().ToString("N").Append(extension);
                return await WriteAsync(fileStream, path, autoFileName, maxSize);

            }
            else
            {
                return await WriteAsync(fileStream, path, fileName, maxSize);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="path">保存路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="autoName">自动生成的文件名（会覆盖原名）</param>
        /// <param name="maxSize">最大限制</param>
        public async Task<IUploadResult> WriteAsync(Stream fileStream, string path, string fileName, int maxSize = 0)
        {
            return await Task.Run(() =>
            {
                var result = new UploadResult()
                {
                    Title = Path.GetFileNameWithoutExtension(fileName),
                    Original = fileName
                };
                try
                {
                    if (maxSize > 0 && fileStream.Length > maxSize)
                    {
                        result.State = UploadEnum.Overflow;
                    }
                    else
                    {
                        var filePath = Path.Combine(path, fileName);
                        DirectoryHelper.Create(path);
                        FileHelper.Write(filePath, fileStream);
                        result.State = UploadEnum.Success;
                        result.Url = filePath;
                        fileStream.Dispose();
                    }
                }
                catch
                {
                    result.State = UploadEnum.Error;
                }
                return result;
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public async void DeleteAsync(string filePath)
        {
            await Task.Run(() =>
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            });
        }
    }
}
