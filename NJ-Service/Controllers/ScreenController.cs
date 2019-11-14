using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using NJ_Service.Library;
using NJ_Service.Library.Enum;

namespace NJ_Service.Controllers
{
    [Route("api/Screen/{action}")]
    public class ScreenController : ApiController
    {
        /// <summary>
        /// 注册客户端
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseData Regeist(string key)
        {
            ScreenUtil.RegeistScreen(key);
            return new ResponseData { Code = 200, Success = true, Data = "注册成功！"};
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetInfo(string key)
        {
            Task<HttpResponseMessage> task = new Task<HttpResponseMessage>((state) => GetInfo(state, key), null);
            task.Start();
            return task.Result;
        }

        [NonAction]
        public HttpResponseMessage GetInfo(object state, string key)
        {
            var response = HResponse("没有返回内容", HttpStatusCode.NoContent);
            PlayControl play = ScreenUtil.GetControl(key);
            int nowTimes = 0;
            int maxTimes = 100;
            while(++nowTimes <= maxTimes)
            {
                if(!play.PlayLock)
                {
                    play.PlayLock = true;
                    response = HResponse(JsonConvert.SerializeObject(play), HttpStatusCode.OK);
                    break;
                }
                Thread.Sleep(100);
            }
            return response;
        }

//        public async Task GetInfoAsync(string key)
//        {
//            Task<HttpResponseMessage> task = new Task<HttpResponseMessage>(new Func<object, HttpResponseMessage>((state) =>
//            {
//                return GetInfo(state, key);
//            }), null);
//            task.Start();
//            BeginEventHandler
//            return task.Result;
//        }


        
        //[HttpGet]
        //public  HttpResponseMessage GetInfo(string key)
        //{
        //    PlayControl play = ScreenUtil.GetControl(key);
        //    HttpStatusCode code = HttpStatusCode.OK;
        //    var current = HttpContext.Current;
        //    string content = "未收到控制";
        //    //bool send = InfoHandlerDelay(10, play);
        //    bool send = play.PlayLock;

        //    if (!send)
        //    {
        //        play.PlayLock = true;
        //        content = JsonConvert.SerializeObject(play);
        //    }
        //    else
        //    {
        //        code = HttpStatusCode.NoContent;
        //    }
        //    return  HResponse(content, code);
        //}

        //[HttpGet]
        //public void GetInfo(string key)
        //{
        //    Task.Run(()=>
        //    {
        //        PlayControl play = ScreenUtil.GetControl(key);
        //        HttpStatusCode code = HttpStatusCode.OK;
        //        var current = HttpContext.Current;
        //        string content = "未收到控制";
        //        bool send = InfoHandlerDelay(10, play);

        //        if (send)
        //        {
        //            play.PlayLock = true;
        //            content = JsonConvert.SerializeObject(play);
        //        }
        //        else
        //        {
        //            code = HttpStatusCode.NoContent;
        //        }
        //    });
        //}

        public void CResponse(HttpContext context, string content, HttpStatusCode code)
        {
            var response = context.Response;
            response.Clear();
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.StatusCode = (int)code;
            response.Write(content);
            response.End();
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="value"></param>
        [HttpGet]
        public ResponseData SetVideo(string key, string id, string value)
        {
            var task = Task.Run(() =>
            {
                PlayControl play = ScreenUtil.GetControl(key);
                if (play.PlayLock)
                {
                    play.PlayLock = false;
                    play.Id = id;
                    play.Type = "video";
                    play.Value = value;
                    return 200;
                }
                return 204;
            });
            return new ResponseData { Code = task.Result, Success = true, Data = "播放视频成功！" };
        }

        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="value"></param>
        [HttpGet]
        public ResponseData SetImage(string key, string value)
        {
            var task = Task.Run(() =>
            {
                PlayControl play = ScreenUtil.GetControl(key);
                if (play.PlayLock)
                {
                    play.PlayLock = false;
                    play.Type = "img";
                    play.Value = value;
                    return 200;
                }
                return 204;
            });
            return new ResponseData { Code = task.Result, Success = true, Data = "显示图片成功！" };
        }

        /// <summary>
        /// 视频控制
        /// </summary>
        /// <param name="value"></param>
        [HttpGet]
        public ResponseData SetPlay(string key, string id, string value)
        {
            var task = Task.Run(() =>
            {
                PlayControl play = ScreenUtil.GetControl(key);
                if (play.PlayLock)
                {
                    play.PlayLock = false;
                    play.Id = id;
                    play.Type = "play";
                    play.Value = value;
                    return 200;
                }
                return 204;

            });
            return new ResponseData { Code = task.Result, Success = true, Data = "视频控制成功！" };
        }

        /// <summary>
        /// 关机
        /// </summary>
        /// <param name="value"></param>
        [HttpGet]
        public ResponseData SetShowdown(string keys)
        {
            List<PlayControl> decives = new List<PlayControl>();
            if (keys == "all")
            {
                decives = ScreenUtil.GetAll();
            }
            else if(keys != "")
            {
                decives = ScreenUtil.GetDecives(keys.Split(','));
            }

            if(decives.Count > 0)
            {
                foreach(var item in decives)
                {
                    if (item.PlayLock)
                    {
                        item.PlayLock = false;
                        item.Type = "showdown";
                    }
                }
            }
            return new ResponseData { Code = 200, Success = true, Data = "控制关机成功！" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutSecond">延时时间</param>
        /// <param name="play">控制器</param>
        /// <returns></returns>
        public bool InfoHandlerDelay( int timeoutSecond, PlayControl play)
        {
            //循环间隔
            TimeSpan mill = TimeSpan.FromMilliseconds(500);
            //循环次数
            var count = timeoutSecond * 1000 / mill.Milliseconds;
            for (var i = 0; i < count; i++)
            {
                var task = Task.Delay(mill);
                task.Wait();
                if (!play.PlayLock)
                {
                    return true;
                }
            }

            return false;
        }

        public HttpResponseMessage HResponse(string json, HttpStatusCode code)
        {
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = new HttpResponseMessage(code);
            response.Content = content;
            return response;
        }

        

        // GET api/<controller>
        //        public IEnumerable<string> Get()
        //        {
        //            return new string[] { "value1", "value2" };
        //        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}