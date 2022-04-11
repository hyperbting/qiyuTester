//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.Qiyu
{
    /// <summary>
    /// 左右手
    /// </summary>
    public enum QiyuHand
    {
        L,
        R,

        Def = R,//默认手
    };

    public class QiyuXRInput
    {
        static InputDevice deviceHand_L;
        static InputDevice deviceHand_R;

        static Dictionary<QiyuHand, Dictionary<string, bool>> buttonStateCache = new Dictionary<QiyuHand, Dictionary<string, bool>>() {
            { QiyuHand.L, new Dictionary<string, bool>() },
            { QiyuHand.R, new Dictionary<string, bool>() }
        };

        private static void CheckDevice(QiyuHand hand = QiyuHand.Def)
        {
            if (hand == QiyuHand.L)
            {
                if (deviceHand_L == null || string.IsNullOrEmpty(deviceHand_L.name))
                    deviceHand_L = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);
            }
            else
            {
                if (deviceHand_R == null || string.IsNullOrEmpty(deviceHand_R.name))
                    deviceHand_R = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
            }
        }

        public static bool GetButtonAction(InputFeatureUsage<bool> usage, out bool actionVal, QiyuHand hand = QiyuHand.Def)
        {
            string name = usage.name;
            if (!buttonStateCache[hand].ContainsKey(name))
            {
                buttonStateCache[hand][name] = false;
            }

            CheckDevice(hand);
            bool state;
            if (hand == QiyuHand.L)
            {
                deviceHand_L.TryGetFeatureValue(usage, out state);
            }
            else
            {
                deviceHand_R.TryGetFeatureValue(usage, out state);
            }

            if (buttonStateCache[hand][name] != state)
            {
                buttonStateCache[hand][name] = state;
                actionVal = state;
                return true;
            }
            else
            {
                actionVal = false;
                return false;
            }
        }

        public static bool GetButtonValue(InputFeatureUsage<float> usage, out float buttonVal, QiyuHand hand = QiyuHand.Def)
        {
            CheckDevice(hand);
            if (hand == QiyuHand.L)
            {
                return deviceHand_L.TryGetFeatureValue(usage, out buttonVal);
            }
            else
            {
                return deviceHand_R.TryGetFeatureValue(usage, out buttonVal);
            }
        }

        public static bool GetButtonValue(InputFeatureUsage<Vector2> usage, out Vector2 buttonVal, QiyuHand hand = QiyuHand.Def)
        {
            CheckDevice(hand);
            if (hand == QiyuHand.L)
            {
                return deviceHand_L.TryGetFeatureValue(usage, out buttonVal);
            }
            else
            {
                return deviceHand_R.TryGetFeatureValue(usage, out buttonVal);
            }
        }

        public static bool GetButtonValue(InputFeatureUsage<Vector3> usage, out Vector3 buttonVal, QiyuHand hand = QiyuHand.Def)
        {
            CheckDevice(hand);
            if (hand == QiyuHand.L)
            {
                return deviceHand_L.TryGetFeatureValue(usage, out buttonVal);
            }
            else
            {
                return deviceHand_R.TryGetFeatureValue(usage, out buttonVal);
            }
        }

        public static bool GetButtonValue(InputFeatureUsage<Quaternion> usage, out Quaternion buttonVal, QiyuHand hand = QiyuHand.Def)
        {
            CheckDevice(hand);
            if (hand == QiyuHand.L)
            {
                return deviceHand_L.TryGetFeatureValue(usage, out buttonVal);
            }
            else
            {
                return deviceHand_R.TryGetFeatureValue(usage, out buttonVal);
            }
        }
    }
}