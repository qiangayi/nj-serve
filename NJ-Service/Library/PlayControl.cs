using NJ_Service.Library.Enum;

namespace NJ_Service.Library
{
    public class PlayControl
    {
        public PlayControl()
        {
            Type = "";
            Value = "";
            Id = "";
            PlayLock = true;
        }

        /// <summary>
        /// 播放锁
        /// </summary>
        public bool PlayLock { get; set; }

        public string key { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// 大屏显示类型，图片，视频，视频控制
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 当前播放的视频
        /// </summary>
        public string Value { get; set; }
    }
}