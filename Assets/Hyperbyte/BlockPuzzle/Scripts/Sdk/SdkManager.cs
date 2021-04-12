using Hyperbyte;
using Hyperbyte.BlockPuzzle.Scripts.Http;
using Hyperbyte.BlockPuzzle.Scripts.Http.Model;
using Sdk.Android;
using Sdk.Interface;
using Sdk.Ios;
using UnityEngine;

namespace Sdk
{
    public class SdkManager : Singleton<SdkManager>
    {
        /// <summary>
        /// 初始化方法
        /// </summary>
        public void Init()
        {
            // 初始化Bugly
            InitBugly();

            // 初始化Facebook
            InitFacebook();

            // 初始化TGA
            InitTGA();

            // 初始化Firebase
            InitFirebase();

            // 初始化广告
            InitAds();
        }

        #region 第三方SDK

        /// <summary>
        /// 初始化Bugly
        /// </summary>
        public void InitBugly()
        {

        }

        /// <summary>
        /// 初始化ThinkingGame
        /// </summary>
        public void InitTGA()
        {
//            _tga = transform.Find("ThinkingGame").GetComponent<ThinkingAnalytics.ThinkingAnalyticsAPI>();
//            _tga.enableLog = ConfigManager.Instance._openDevelopMode;
//            _tga.serverUrl = _tgaDotURL;
//            _tga.tokens = _tokens;
        }

        /// <summary>
        /// 初始化广告
        /// </summary>
        public void InitAds()
        {
        }

        /// <summary>
        /// 初始化Facebook
        /// </summary>
        public void InitFacebook()
        {
            // FacebookUtils.Instance.initFacebookSDK();
        }

        /// <summary>
        /// 初始化Firebase
        /// </summary>
        public void InitFirebase()
        {
        }

        #endregion

        #region 原生交互

#if UNITY_EDITOR || UNITY_STANDALONE
        ITools tools = new DefaultTools();
#elif UNITY_ANDROID
        ITools tools = new AndroidTools();
#elif UNITY_IOS
        ITools tools = new IosTools();
#endif

        //--------------------调取原生
        public string GetGaid()
        {
            return tools.GetGaid();
        }

        public string GetUA()
        {
            return tools.GetUA();
        }

        //--------------------原生回调
        public void OnPackageRemoved(string packageName)
        {
            EventModel req = new EventModel();
            req.distinct_id = SystemInfo.deviceUniqueIdentifier;
            req.time = TimeUtils.GetCurrentTimeUnix();
            req.type = "track";
            req.@event = "PackageRemoved";
            HttpRequest.Instance.SendEvent(req, result =>
            {
                    
            });
        } 
        #endregion
    }
}