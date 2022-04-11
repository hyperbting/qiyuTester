//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using UnityEngine;


namespace Unity.XR.Qiyu
{
    public static class QiyuInputUtil
    {
        public static DEVICE_ID Ctrl2DeviceId(QiyuInput.Controller controllerMask = QiyuInput.Controller.Default)
        {
            if (controllerMask == QiyuInput.Controller.Default)
            {
                //获取主手柄
                if (QiyuInput.GetDominentHand() == QiyuInput.Handedness.RightHanded)
                {
                    controllerMask = QiyuInput.Controller.RTouch;
                }
                else if (QiyuInput.GetDominentHand() == QiyuInput.Handedness.LeftHanded)
                {
                    controllerMask = QiyuInput.Controller.LTouch;
                }
            }

            if (controllerMask == QiyuInput.Controller.LTouch)
                return DEVICE_ID.HAND_0;
            if (controllerMask == QiyuInput.Controller.RTouch)
                return DEVICE_ID.HAND_1;
            return DEVICE_ID.HAND_0;
        }

        public static QiyuInput.Controller GetControllerFromDefault(QiyuInput.Controller controller = QiyuInput.Controller.Default)
        {
            if (controller == QiyuInput.Controller.Default)
            {
                //获取主手柄
                if (QiyuInput.GetDominentHand() == QiyuInput.Handedness.RightHanded)
                {
                    return QiyuInput.Controller.RTouch;
                }
                else if (QiyuInput.GetDominentHand() == QiyuInput.Handedness.LeftHanded)
                {
                    return QiyuInput.Controller.LTouch;
                }
                return QiyuInput.Controller.RTouch;
            }
            return controller;
        }

        public static float ParseTriggerForce(int origin)
        {
            return Mathf.Clamp01(origin / 255.0f);
        }

        public static bool IsFloatEqual(float a, float b)
        {
            return Mathf.Abs(a - b) <= 0.0000001;
        }
    }

    public static class QVRUtils
	{
        

    }

}