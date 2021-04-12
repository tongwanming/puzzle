﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
 using Hyperbyte;
 using Hyperbyte.BlockPuzzle.Scripts.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Common.Utils
{
    /// <summary>
    /// ClassName: HttpUtils
    /// Version:1.0
    /// Date:2020-9-23
    /// Author:JiaChunzhen
    /// </summary>
    /// <remarks>
    /// Http请求工具类，需调用挂载到场景中，具体业务接口写到业务层，不要在此脚本添加
    /// </remarks>>
    public class HttpUtils : Singleton<HttpUtils>
    {
        //默认请求超时时间
        private int TIMEOUT = 15;

        /// <summary>
        /// 检测网络环境
        /// </summary>
        /// <returns>true有网 false无网</returns>
        public bool CheckNetwork()
        {
            bool network = !(Application.internetReachability == NetworkReachability.NotReachable);

            if (!network)
            {
                LogUtils.Log(LogUtils.LogColor.Red, "No Network!");
            }

            return network;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="actionResult">请求回调</param>
        public void Get(string url, Action<string> actionResult)
        {

            Get(url, null, actionResult);
        }

        public void Get(string url, Dictionary<string, string> argsValue, Action<string> actionResult)
        {
            //检测网络
            if (!CheckNetwork())
            {
                actionResult?.Invoke(null);
                return;
            }
            
            StartCoroutine(IGet(url, argsValue, actionResult));
        }


        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="argsValue">请求参数</param>
        /// <param name="actionResult">请求回调</param>
        private IEnumerator IGet(string url, Dictionary<string, string> argsValue, Action<string> actionResult)
        {
            UnityWebRequest.ClearCookieCache();

            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (argsValue != null && argsValue.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in argsValue)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }

            LogUtils.Log(LogUtils.LogColor.Yellow, url, builder.ToString());

            using (UnityWebRequest uwr = UnityWebRequest.Get(builder.ToString()))
            {
                UnityWebRequest.ClearCookieCache();
                uwr.SetRequestHeader("Cookie", "Cookie");
                // uwr.SetRequestHeader("Content-Type", "text/plain;charset=utf-8");

                yield return uwr.SendWebRequest();
            
                if (uwr.isNetworkError)
                {
                    actionResult?.Invoke(null);
                    uwr.Dispose();
                }
                else if (uwr.isDone)
                {
                    actionResult?.Invoke(uwr.downloadHandler.text);
                }
                else
                {
                    actionResult?.Invoke(null);
                    uwr.Dispose();
                }
            }
        }


        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="formData">Form表单数据</param>
        /// <param name="actionResult">请求回调</param>
        public void Post(string url, string argsValue, Action<string> actionResult)
        {
            //检测网络
            if (!CheckNetwork())
            {
                actionResult?.Invoke(null);
                return;
            }

            StartCoroutine(IPost(url, argsValue, actionResult));
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="argsValue"></param>
        /// <param name="actionResult">请求回调</param>
        /// <returns></returns>
        private IEnumerator IPost(string url, string argsValue, Action<string> actionResult)
        {
            UnityWebRequest uwr = new UnityWebRequest(url, "POST");
            uwr.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(argsValue));
            uwr.downloadHandler = new DownloadHandlerBuffer();
            
            LogUtils.Log(LogUtils.LogColor.Yellow, uwr.url);

            UnityWebRequest.ClearCookieCache();
            uwr.SetRequestHeader("Cookie", "Cookie");
            uwr.SetRequestHeader("Content-Type", "text/plain;charset=utf-8");
            
            uwr.timeout = TIMEOUT;

            yield return uwr.SendWebRequest();

            LogUtils.Log(LogUtils.LogColor.Yellow, uwr.downloadHandler.text);

            if (uwr.isNetworkError)
            {
                actionResult?.Invoke(null);
                uwr.Dispose();
            }
            else if (uwr.isDone)
            {
                actionResult?.Invoke(uwr.downloadHandler.text);
            }
            else
            {
                actionResult?.Invoke(null);
                uwr.Dispose();
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="downloadFilePathAndName">储存文件的路径和文件名 like 'Application.persistentDataPath+"/unity3d.html"'</param>
        /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求对象</param>
        /// <returns></returns>
        public void DownloadFile(string url, string downloadFilePathAndName, Action<UnityWebRequest> actionResult)
        {
            //检测网络
            if (!CheckNetwork())
            {
                actionResult?.Invoke(null);
                return;
            }

            StartCoroutine(IDownloadFile(url, downloadFilePathAndName, actionResult));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="downloadFilePathAndName">储存文件的路径和文件名 like 'Application.persistentDataPath+"/unity3d.html"'</param>
        /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求对象</param>
        /// <returns></returns>
        private IEnumerator IDownloadFile(string url, string downloadFilePathAndName,
            Action<UnityWebRequest> actionResult)
        {
            var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            uwr.downloadHandler = new DownloadHandlerFile(downloadFilePathAndName);
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                actionResult?.Invoke(null);
                uwr.Dispose();
            }
            else if (uwr.isDone)
            {
                actionResult?.Invoke(uwr);
            }
            else
            {
                actionResult?.Invoke(null);
                uwr.Dispose();
            }
        }

        /// <summary>
        /// 请求图片
        /// </summary>
        /// <param name="url">图片地址,like 'http://www.my-server.com/image.png '</param>
        /// <param name="action">请求发起后处理回调结果的委托,处理请求结果的图片</param>
        /// <returns></returns>
        public void GetTexture(string url, Action<Texture2D> actionResult)
        {
            StartCoroutine(IGetTexture(url, actionResult));
        }

        /// <summary>
        /// 请求图片
        /// </summary>
        /// <param name="url">图片地址,like 'http://www.my-server.com/image.png '</param>
        /// <param name="action">请求发起后处理回调结果的委托,处理请求结果的图片</param>
        /// <returns></returns>
        private IEnumerator IGetTexture(string url, Action<Texture2D> actionResult)
        {
            UnityWebRequest uwr = new UnityWebRequest(url);
            DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
            uwr.downloadHandler = downloadTexture;
            yield return uwr.SendWebRequest();
            Texture2D t = null;
            if (!(uwr.isNetworkError || uwr.isHttpError))
            {
                t = downloadTexture.texture;
            }

            actionResult?.Invoke(t);
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="url">AssetBundle地址,like 'http://www.my-server.com/myData.unity3d'</param>
        /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AssetBundle</param>
        /// <returns></returns>
        public void GetAssetBundle(string url, Action<AssetBundle> actionResult)
        {
            StartCoroutine(IGetAssetBundle(url, actionResult));
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="url">AssetBundle地址,like 'http://www.my-server.com/myData.unity3d'</param>
        /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AssetBundle</param>
        /// <returns></returns>
        private IEnumerator IGetAssetBundle(string url, Action<AssetBundle> actionResult)
        {
            UnityWebRequest www = new UnityWebRequest(url);
            DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(www.url, uint.MaxValue);
            www.downloadHandler = handler;
            yield return www.SendWebRequest();
            AssetBundle bundle = null;
            if (!(www.isNetworkError || www.isHttpError))
            {
                bundle = handler.assetBundle;
            }

            actionResult?.Invoke(bundle);
        }

        /// <summary>
        /// 请求服务器地址上的音效
        /// </summary>
        /// <param name="url">没有音效地址,like 'http://myserver.com/mysound.wav'</param>
        /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AudioClip</param>
        /// <param name="audioType">音效类型</param>
        /// <returns></returns>
        public void GetAudioClip(string url, Action<AudioClip> actionResult, AudioType audioType = AudioType.WAV)
        {
            StartCoroutine(IGetAudioClip(url, actionResult, audioType));
        }

        /// <summary>
        /// 请求服务器地址上的音效
        /// </summary>
        /// <param name="url">没有音效地址,like 'http://myserver.com/mysound.wav'</param>
        /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AudioClip</param>
        /// <param name="audioType">音效类型</param>
        /// <returns></returns>
        private IEnumerator IGetAudioClip(string url, Action<AudioClip> actionResult,
            AudioType audioType = AudioType.WAV)
        {
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                yield return uwr.SendWebRequest();
                if (!(uwr.isNetworkError || uwr.isHttpError))
                {
                    actionResult?.Invoke(DownloadHandlerAudioClip.GetContent(uwr));
                }
            }
        }
    }
}