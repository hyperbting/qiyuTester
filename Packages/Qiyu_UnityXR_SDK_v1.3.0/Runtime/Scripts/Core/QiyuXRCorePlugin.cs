//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
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

namespace Unity.XR.Qiyu
{
    public static partial class QiyuXRCorePlugin
    {
        private const string pluginName = "QiyuXRCore";

        //------------------------------------------------------------------------
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_Update(float deltaTime);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_OnApplicationPause(bool pause);

        /// ----------------------------------------------------------------------------

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_GetBoundaryConfigured();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern BoundaryTestResult QVR_TestBoundaryPoint(Vector3f point, BoundaryType boundaryType);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_GetOriginAxis(ref Vector3f pos, ref Quatf rot);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_GetBoundaryGeometry(BoundaryType boundaryType, IntPtr points, ref int pointsCount);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Vector3f QVR_GetBoundaryDimensions(BoundaryType boundaryType);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_GetBoundaryVisible();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_SetBoundaryVisible(Bool value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_GetBoundaryDownHeadShow();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_SetBoundaryDownHeadShow(Bool value);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_RegisterBoundaryChangedCallback(FunPtr callback);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_UnRegisterBoundaryChangedCallback(FunPtr callback);


        #region controller
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_GetControllerData(ref ControllerData left, ref ControllerData right);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_GetControllerIsInit();
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_ControllerBeginServer();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_ControllerEndServer();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_ControllerSendMsg(string cmd);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_ControllerStartVibration(int deviceId, int amplitude, int duration);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_ControllerSetType(int type);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QVR_ControllerGetType();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool QVR_IsGlobal3Dof();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_SetGlobal3Dof(Bool value);

        #endregion

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QVR_GetHand();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QVR_SetHand(int type);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float QVR_GetFloorLevel();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int QVR_GetUserIPD();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_GetEyeBufferResolution(ref Vector2f resolution);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void QVR_SetAppTrackingMode(int TrackingMode);

        //[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern void QVR_SetDisplayRefreshRate(float refreshRate);

        //[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern float QVR_GetDisplayRefreshRate();
    }
}
