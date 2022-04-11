//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using FunPtr = System.IntPtr;
using DataPtr = System.IntPtr;
using StringPtr = System.IntPtr;

namespace Qiyu.Sdk.Platform
{
    public class QiyuXRPlatformPlugin
    {
        private const string pluginName = "QiyuXRPlatform";

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_InitPlatform(IntPtr activity, FunPtr funPtr_request);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_InitQiyuSDK(ulong requestID, string app_id, string app_secret);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Update_Platform(float deltaTime);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_OnApplicationPause_Platform(bool pause);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_GetAppUpdateInfo(ulong requestID, string app_id, string curVersion);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QVR_IsAccountLogin();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_GetQiyuAccountInfo(ulong requestID);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_LaunchOtherApp(string app_id, string key, string value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_LaunchHome(string key, string value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_GetDeepLink(ulong requestID);

        //QiyuPrefs
        //[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern bool QVR_Prefs_Init();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float QVR_Prefs_GetFloat(string key, float defValue);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QVR_Prefs_GetInt(string key, int defValue);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string QVR_Prefs_GetString(string key, string defValue);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool QVR_Prefs_HasKey(string key);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Prefs_Save();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Prefs_DeleteAll();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Prefs_DeleteKey(string key);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Prefs_SetFloat(string key, float value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Prefs_SetInt(string key, int value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Prefs_SetString(string key, string value);

        //QiyuPay
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_SetLanguage(string lang);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_InitQiyuPay(ulong requestID);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_GetSkuList(ulong requestID, string skuList);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_PlaceOrder(ulong requestID, string sku);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_QueryOrderResult(ulong requestID, string orderId);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_QueryHistoryOrders(ulong requestID, string sku, int curPage, int pageSize);

        //Other
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_RecordSports(float headOffset, float leftHandOffset, float rightHandOffset);
    }
}
