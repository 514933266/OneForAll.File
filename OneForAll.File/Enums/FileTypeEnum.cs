using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File.Enums
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileTypeEnum
    {
        /// <summary>
        /// Excel 97-2003 文件格式
        /// </summary>
        Xls = 2082071,

        /// <summary>
        /// Word 97-2003 文档
        /// </summary>
        Doc,

        /// <summary>
        /// PowerPoint 97-2003 演示文稿
        /// </summary>
        Ppt,

        /// <summary>
        /// Word 2007+ 文档
        /// </summary>
        Docx,

        /// <summary>
        /// ZIP 压缩文件
        /// </summary>
        Zip = 807534,

        /// <summary>
        /// Excel 2007+ 文件
        /// </summary>
        Xlsx = 807534,

        /// <summary>
        /// PowerPoint 2007+ 演示文稿
        /// </summary>
        Pptx,

        /// <summary>
        /// PDF 文档
        /// </summary>
        Pdf = 378068,

        /// <summary>
        /// 纯文本文件
        /// </summary>
        Txt,

        /// <summary>
        /// RAR 压缩文件
        /// </summary>
        Rar = 829711,

        /// <summary>
        /// JPG 图像文件
        /// </summary>
        Jpg = 255216,

        /// <summary>
        /// JPEG 图像文件（同 JPG）
        /// </summary>
        Jpeg,

        /// <summary>
        /// PNG 图像文件
        /// </summary>
        Png = 137807,

        /// <summary>
        /// GIF 图像文件
        /// </summary>
        Gif = 7173,

        /// <summary>
        /// BMP 图像文件
        /// </summary>
        Bmp = 6677,

        /// <summary>
        /// ICO 图标文件
        /// </summary>
        Ico = 10, // 注意：原值 0010 被解析为八进制，应改为十进制 10

        /// <summary>
        /// MP4 视频文件
        /// </summary>
        Mp4 = 32, // 原值 00032 被解析为八进制，应改为十进制 32

        /// <summary>
        /// MP3 音频文件
        /// </summary>
        Mp3,

        /// <summary>
        /// FLV 视频文件
        /// </summary>
        Flv = 707686,

        /// <summary>
        /// WMV 视频文件
        /// </summary>
        Wmv = 483817,

        /// <summary>
        /// AVI 视频文件
        /// </summary>
        Avi = 827370,

        /// <summary>
        /// M3U8 流媒体播放列表
        /// </summary>
        M3u8,

        /// <summary>
        /// RMVB 视频文件
        /// </summary>
        Rmvb
    }
}
