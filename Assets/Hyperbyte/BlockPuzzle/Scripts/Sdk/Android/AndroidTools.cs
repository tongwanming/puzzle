using System;
using Hyperbyte.BlockPuzzle.Scripts.Common;
using Sdk.Interface;
using UnityEngine;

namespace Sdk.Android
{
    public class AndroidTools : ITools
    {
        private string toolActivity = "com.unity3d.player.UnityPlayerActivity";
        
        public string GetGaid()
        {
            var value = "";
            try
            {
                AndroidJavaObject javaClass = new AndroidJavaObject(toolActivity);
                value = javaClass.Call<string>("GetGaid");
            }
            catch (Exception e)
            {
                LogUtils.Log("SDK-原生方法", e);
            }

            return value;
        }

        public string GetUA()
        {
            var value = "";
            try
            {
                AndroidJavaObject javaClass = new AndroidJavaObject(toolActivity);
                value = javaClass.Call<string>("GetUA");
            }
            catch (Exception e)
            {
                LogUtils.Log("SDK-原生方法", e);
            }
            return value;
        }
    }
}