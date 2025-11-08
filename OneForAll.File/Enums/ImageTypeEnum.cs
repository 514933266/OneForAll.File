using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File.Enums
{
    /// <summary>
    /// 图像类型
    /// </summary>
    public enum ImageTypeEnum
    {
        /// <summary>
        /// JPG 图像
        /// </summary>
        Jpg = FileTypeEnum.Jpg,

        /// <summary>
        /// JPEG 图像（同 JPG）
        /// </summary>
        Jpeg = FileTypeEnum.Jpeg,

        /// <summary>
        /// PNG 图像
        /// </summary>
        Png = FileTypeEnum.Png,

        /// <summary>
        /// GIF 图像
        /// </summary>
        Gif = FileTypeEnum.Gif,

        /// <summary>
        /// BMP 图像
        /// </summary>
        Bmp = FileTypeEnum.Bmp,

        /// <summary>
        /// ICO 图标
        /// </summary>
        Ico = FileTypeEnum.Ico,
    }
}
