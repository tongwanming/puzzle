using System;
using Common.Utils;
using Hyperbyte.BlockPuzzle.Scripts.Common;
using Hyperbyte.BlockPuzzle.Scripts.Http.Model;

namespace Hyperbyte.BlockPuzzle.Scripts.Http
{
    public class HttpRequest : Singleton<HttpRequest>
    {
        #region HTTP-URL-KEY

        // private const string LOGIN = "login"; //【游戏登录】

        #endregion
        
        #region 基础方法
        
        /// <summary>
        /// 正式/测试 API URL
        /// </summary>
        /// <returns></returns>
        public string GetApiUrl()
        {
            return "http://mcanlabs.com";
        }
        
        public void SendEvent(EventModel req, Action<string> actionResult)
        {
            LogUtils.Log(LogUtils.LogColor.Yellow, req.ToString());
            HttpUtils.Instance.Post(GetApiUrl() + "/api/PostBaseInfo", req.ToString(), actionResult);
        }
        
        #endregion
    }
}