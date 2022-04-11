//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using UnityEngine;

namespace Qiyu.Sdk.Platform
{
    public static class QiyuPrefs
    {
        ///// <summary>
        ///// 初始化，会弹存储权限弹窗，在使用存档API前调用
        ///// </summary>
        //public static bool Init()
        //{
        //    if (QiyuPlatform.IsAndroid)
        //    {
        //        return QiyuXRPlatformPlugin.QVR_Prefs_Init();
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        /// <summary>
        /// 获取Float类型存档
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">默认值</param>
        /// <returns>返回值</returns>
        public static float GetFloat(string key, float defValue = 0)
        {
            if (QiyuPlatform.IsAndroid)
            {
                return QiyuXRPlatformPlugin.QVR_Prefs_GetFloat(key, defValue);
            }
            else
            {
                return PlayerPrefs.GetFloat(key, defValue);
            }
        }

        /// <summary>
        /// 获取Int类型存档
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">默认值</param>
        /// <returns>返回值</returns>
        public static int GetInt(string key, int defValue = 0)
        {
            if (QiyuPlatform.IsAndroid)
            {
                return QiyuXRPlatformPlugin.QVR_Prefs_GetInt(key, defValue);
            }
            else
            {
                return PlayerPrefs.GetInt(key, defValue);
            }
        }

        /// <summary>
        /// 获取String类型存档
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">默认值</param>
        /// <returns>返回值</returns>
        public static string GetString(string key, string defValue = "")
        {
            if (QiyuPlatform.IsAndroid)
            {
                return QiyuXRPlatformPlugin.QVR_Prefs_GetString(key, defValue);
            }
            else
            {
                return PlayerPrefs.GetString(key, defValue);
            }
        }

        /// <summary>
        /// 设置Float类型存档
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">默认值</param>
        public static void SetFloat(string key, float value)
        {
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_Prefs_SetFloat(key, value);
            }
            else
            {
                PlayerPrefs.SetFloat(key, value);
            }
        }

        /// <summary>
        /// 设置Int类型存档
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">默认值</param>
        public static void SetInt(string key, int value)
        {
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_Prefs_SetInt(key, value);
            }
            else
            {
                PlayerPrefs.SetInt(key, value);
            }
        }

        /// <summary>
        /// 设置String类型存档
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="defValue">默认值</param>
        public static void SetString(string key, string value)
        {
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_Prefs_SetString(key, value);
            }
            else
            {
                PlayerPrefs.SetString(key, value);
            }
        }

        /// <summary>
        /// 删除所有存档
        /// </summary>
        public static void DeleteAll()
        {
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_Prefs_DeleteAll();
            }
            else
            {
                PlayerPrefs.DeleteAll();
            }
        }

        /// <summary>
        /// 删除指定key存档
        /// </summary>
        /// <param name="key">关键字</param>
        public static void DeleteKey(string key)
        {
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_Prefs_DeleteKey(key);
            }
            else
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

        /// <summary>
        /// 指定key存档是否存在
        /// </summary>
        /// <param name="key">关键字</param>
        public static bool HasKey(string key)
        {
            if (QiyuPlatform.IsAndroid)
            {
                return QiyuXRPlatformPlugin.QVR_Prefs_HasKey(key);
            }
            else
            {
                return PlayerPrefs.HasKey(key);
            }
        }

        /// <summary>
        /// 将存档写入磁盘
        /// </summary>
        public static void Save()
        {
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_Prefs_Save();
            }
            else
            {
                PlayerPrefs.Save();
            }
        }
    }
}