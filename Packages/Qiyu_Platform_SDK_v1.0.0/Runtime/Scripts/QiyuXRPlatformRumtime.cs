//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Qiyu.Sdk.Platform
{
    public class QiyuXRPlatformRumtime : QiyuSingletion<QiyuXRPlatformRumtime>
    {
        const string TAG = "QiyuXRPlatformRumtime";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInit()
        {
            if (QiyuPlatform.IsAndroid)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                QiyuXRPlatformPlugin.QVR_InitPlatform(activity.GetRawObject(), QiyuMessageManager.RequestProcess_Ptr);
                QiyuXRPlatformRumtime.Instance.Register();
            }
        }

        private void Start()
        {
            if (QiyuPlatform.IsAndroid)
            {
                InvokeRepeating("OnRecordSports", 0, 1);
            }
        }

        private void Update()
        {
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_Update_Platform(Time.unscaledDeltaTime);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            Debug.Log(TAG + " OnApplicationPause:" + pause);

            isPaused = pause;
            if (QiyuPlatform.IsAndroid)
            {
                QiyuXRPlatformPlugin.QVR_OnApplicationPause_Platform(pause);
            }
        }

        bool isPaused = false;
        Vector3 lastHeadPos = Vector3.zero;
        Vector3 lastLeftHandPos = Vector3.zero;
        Vector3 lastRightHandPos = Vector3.zero;
        UnityEngine.XR.InputDevice deviceHead;
        UnityEngine.XR.InputDevice deviceLeftHand;
        UnityEngine.XR.InputDevice deviceRightHand;

        private void CheckDevice()
        {
            if (deviceHead == null || string.IsNullOrEmpty(deviceHead.name))
                deviceHead = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head);

            if (deviceLeftHand == null || string.IsNullOrEmpty(deviceLeftHand.name))
                deviceLeftHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);

            if (deviceRightHand == null || string.IsNullOrEmpty(deviceRightHand.name))
                deviceRightHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
        }

        private void OnRecordSports()
        {
            if (isPaused)
                return;

            if (QiyuPlatform.IsAndroid)
            {
                CheckDevice();

                float headOffset = 0;
                float leftHandOffset = 0;
                float rightHandOffset = 0;
                if (deviceHead.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out var headPos))
                {
                    headOffset = Vector3.Distance(headPos, lastHeadPos);
                    lastHeadPos = headPos;
                }
                if (deviceLeftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out var leftHandPos))
                {
                    leftHandOffset = Vector3.Distance(leftHandPos, lastLeftHandPos);
                    lastLeftHandPos = leftHandPos;
                }
                if (deviceRightHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out var rightHandPos))
                {
                    rightHandOffset = Vector3.Distance(rightHandPos, lastRightHandPos);
                    lastRightHandPos = rightHandPos;
                }

                QiyuXRPlatformPlugin.QVR_RecordSports(headOffset, leftHandOffset, rightHandOffset);
            }
        }
    }
}