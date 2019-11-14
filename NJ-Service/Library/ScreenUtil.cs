using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NJ_Service.Library
{
    public static class ScreenUtil
    {
        //private static Dictionary<string, PlayControl> _screenControl = new Dictionary<string, PlayControl>();

        //public static Dictionary<string, PlayControl> _screenControl
        //{
        //    get
        //    {
        //        return CacheUtil.Get<Dictionary<string, PlayControl>>("screen");
        //    }
        //}
        //通过id获取屏幕器,没有就新增
        public static PlayControl GetControl(string key)
        {
            var _screenControl = CacheUtil.Get<Dictionary<string, PlayControl>>("screen");
            if(_screenControl == null)
            {
                _screenControl = new Dictionary<string, PlayControl>();
            }
            if (!_screenControl.ContainsKey(key))
            {
                PlayControl play = new PlayControl
                {
                    key = key
                };
                _screenControl.Add(key, play);
                CacheUtil.Set("screen", _screenControl);
                return play;
            }
            else
            {
               return _screenControl[key];
            }
        }

        /// <summary>
        /// 注册屏幕
        /// </summary>
        public static void RegeistScreen(string key)
        {
            var _screenControl = CacheUtil.Get<Dictionary<string, PlayControl>>("screen");
            if (_screenControl.ContainsKey(key))
            {
                _screenControl.Remove(key);
            }
            PlayControl play = new PlayControl
            {
                key = key
            };
            _screenControl.Add(key, play);
            CacheUtil.Set("screen", _screenControl);
        }

        /// <summary>
        /// 获取所有设备
        /// </summary>
        /// <returns></returns>
        public static List<PlayControl> GetAll()
        {
            var _screenControl = CacheUtil.Get<Dictionary<string, PlayControl>>("screen");
            var keys = _screenControl.Keys.ToArray();
            List<PlayControl> list = new List<PlayControl>();
            foreach (var key in keys)
            {
                list.Add(GetControl(key));
            }
            return list;
        }

        /// <summary>
        /// 获取设备集合
        /// </summary>
        /// <returns></returns>
        public static List<PlayControl> GetDecives(string[] keys)
        {
            List<PlayControl> list = new List<PlayControl>();
            foreach (var key in keys)
            {
                list.Add(GetControl(key));
            }
            return list;
        }

        /// <summary>
        /// 移除设备，在关机时使用
        /// </summary>
        /// <param name="key"></param>
        //public static void Remove(string key)
        //{
        //    _screenControl.Remove(key);
        //}

        //判断屏幕是否存在

        //控制屏幕播放
    }
}