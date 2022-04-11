//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunPtr = System.IntPtr;
using DataPtr = System.IntPtr;
using StringPtr = System.IntPtr;
using System;

namespace Qiyu.Sdk.Platform
{
    public static partial class QiyuXRPlatform
    {
        /// <summary>
        /// 初始化平台接口
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="app_id">APPID</param>
        /// <param name="app_secret">开发者秘钥</param>
        public static void InitQiyuSDK(RequestCallback callback, string app_id, string app_secret)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_InitQiyuSDK(QiyuMessageManager.AddRequest(callback), app_id, app_secret);
        }

        /// <summary>
        /// 获取Qiyu账户是否登录
        /// </summary
        public static bool IsQiyuAccountLogin()
        {
            if (!QiyuPlatform.IsAndroid)
                return false;

            return QiyuXRPlatformPlugin.QVR_IsAccountLogin() == 1;
        }
        /// <summary>
        /// 获取Qiyu账户信息
        /// </summary>
        /// <param name="callback">请求的回调函数</param>
        public static void GetQiyuAccountInfo(RequestCallback callback)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_GetQiyuAccountInfo(QiyuMessageManager.AddRequest(callback));
        }

        /// <summary>
        /// 打开其他应用
        /// </summary>
        /// <param name="app_id">应用id</param>
        /// <param name="key">深度连接Key</param>
        /// <param name="value">深度连接value</param>
        public static void LaunchOtherApp(string app_id, string key, string value)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_LaunchOtherApp(app_id, key, value);
        }
        /// <summary>
        /// 打开Home
        /// </summary>
        /// <param name="key">深度连接Key</param>
        /// <param name="value">深度连接value</param>
        public static void LaunchHome(string key, string value)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_LaunchHome(key, value);
        }

        /// <summary>
        /// 获取深度连接信息
        /// </summary>
        /// <param name="callback">回调函数</param>
        public static void GetDeepLink(RequestCallback callback)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_GetDeepLink(QiyuMessageManager.AddRequest(callback));
        }

        /// <summary>
        /// 设置语言接口(后端通信用)，不设置的话，默认中文
        /// </summary>
        /// <param name="lang">英语  en_US
        ///                    中文  zh_CN  (默认)
        /// </param>
        public static void SetLanguage(string lang = "zh_CN")
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_SetLanguage(lang);
        }


    }
}
