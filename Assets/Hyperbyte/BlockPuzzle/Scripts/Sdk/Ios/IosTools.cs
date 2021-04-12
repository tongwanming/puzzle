using System;
using Sdk.Interface;
using UnityEngine;

namespace Sdk.Ios
{
    public class IosTools : ITools
    {
#if UNITY_IOS
        // [DllImport("__Internal")]
        // public static extern string _getLanguage(); //获取语言
#endif
        public string GetGaid()
        {
            return "";
        }

        public string GetUA()
        {
            return "";
        }
    }
}