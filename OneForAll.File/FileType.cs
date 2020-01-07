using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        xls = 208207,
        doc = 208207,
        ppt = 208207,
        docx = 807534,
        xlsx = 807534,
        pptx = 807534,
        pdf = 378068,
        txt,
        rar = 829711,
        zip = 807534,
        jpg = 255216,
        jpeg = 255216,
        png = 137807,
        gif = 7173,
        bmp = 6677,
        ico = 0010,
        mp4 = 00032,
        mp3,
        flv = 707686,
        wmv = 483817,
        avi = 827370,
        m3u8,
        rmvb
    }

    /// <summary>
    /// 图像类型
    /// </summary>
    public enum ImageType
    {
        jpg = FileType.jpg,
        jpeg = FileType.jpeg,
        png = FileType.png,
        gif = FileType.gif,
        bmp = FileType.bmp,
        ico = FileType.ico,
    }

    /// <summary>
    /// 文档类型
    /// </summary>
    public enum DocType
    {
        xlsx = FileType.xlsx,
        xls = FileType.xls,
        doc = FileType.doc,
        docx = FileType.docx,
        txt = FileType.txt,
        ppt = FileType.ppt,
        pptx = FileType.pptx,
        pdf = FileType.pdf
    }

    /// <summary>
    /// 媒体类型
    /// </summary>
    public enum MediaType
    {
        mp4 = FileType.mp4,
        mp3 = FileType.mp3,
        flv = FileType.flv,
        wmv = FileType.wmv,
        avi = FileType.avi,
        m3u8 = FileType.m3u8,
        rmvb = FileType.rmvb
    }

    /// <summary>
    /// 压缩文件类型
    /// </summary>
    public enum ZipType
    {
        zip = FileType.zip,
        rar = FileType.rar
    }
}
