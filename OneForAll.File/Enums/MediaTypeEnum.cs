using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneForAll.File.Enums
{
    /// <summary>
    /// 媒体类型
    /// </summary>
    public enum MediaTypeEnum
    {
        /// <summary>
        /// MP4 视频
        /// </summary>
        Mp4 = FileTypeEnum.Mp4,

        /// <summary>
        /// MP3 音频
        /// </summary>
        Mp3 = FileTypeEnum.Mp3,

        /// <summary>
        /// FLV 视频
        /// </summary>
        Flv = FileTypeEnum.Flv,

        /// <summary>
        /// WMV 视频
        /// </summary>
        Wmv = FileTypeEnum.Wmv,

        /// <summary>
        /// AVI 视频
        /// </summary>
        Avi = FileTypeEnum.Avi,

        /// <summary>
        /// M3U8 流媒体播放列表
        /// </summary>
        M3u8 = FileTypeEnum.M3u8,

        /// <summary>
        /// RMVB 视频
        /// </summary>
        Rmvb = FileTypeEnum.Rmvb
    }
}
