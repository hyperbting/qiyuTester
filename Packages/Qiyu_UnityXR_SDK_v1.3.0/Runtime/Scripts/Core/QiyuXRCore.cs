//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using static Unity.XR.Qiyu.QiyuXRCorePlugin;
using DataPtr = System.IntPtr;
using FunPtr = System.IntPtr;

namespace Unity.XR.Qiyu
{
    public static partial class QiyuXRCore
    {
        public static void Update(float deltaTime)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRCorePlugin.QVR_Update(deltaTime);
        }

        public static void OnApplicationPause(bool b)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRCorePlugin.QVR_OnApplicationPause(b);
        }

        /// <summary>
        /// 获取地面高度
        /// </summary>
        public static float GetFloorLevel()
        {
            if (!QiyuPlatform.IsAndroid)
                return 0;

            return QiyuXRCorePlugin.QVR_GetFloorLevel();
        }

        /// <summary>
        /// 获取用户瞳距设置
        /// </summary>
        public static float GetUserIPD()
        {
            if (!QiyuPlatform.IsAndroid)
                return 0;

            return QiyuXRCorePlugin.QVR_GetUserIPD() * 1.0f / 1000;
        }

        /// <summary>
        /// 设置3dof模式，1:3dof,0:6dof
        /// </summary>
        /// <returns></returns>
        public static void SetAppTrackingMode(int mode)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRCorePlugin.QVR_SetAppTrackingMode(mode);
        }

        //public static void SetDisplayRefreshRate(float rate)
        //{
        //    if (!QiyuPlatform.IsAndroid)
        //        return;

        //    QiyuXRCorePlugin.QVR_SetDisplayRefreshRate(rate);
        //}

        //public static float GetDisplayRefreshRate()
        //{
        //    if (!QiyuPlatform.IsAndroid)
        //        return 72;

        //    return QiyuXRCorePlugin.QVR_GetDisplayRefreshRate();
        //}
    }
}
