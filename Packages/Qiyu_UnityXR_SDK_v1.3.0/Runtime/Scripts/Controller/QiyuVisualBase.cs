//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.Qiyu
{
    public class QiyuVisualBase : MonoBehaviour
    {
        public QiyuHand _hand = QiyuHand.L;

        public InputDevice deviceHand;

        private void Start()
        {
            //Modify Ray Offset
            if (_hand == QiyuHand.L)
            {
                var LeftHandRay = GameObject.Find("[LeftHand Controller] Original Attach");
                if (LeftHandRay)
                {
                    LeftHandRay.transform.localPosition = new Vector3(0, -0.033f, -0.022f);
                    LeftHandRay.transform.localEulerAngles = new Vector3(40, 0, 0);
                }
            }
            else if (_hand == QiyuHand.R)
            {
                var RightHandRay = GameObject.Find("[RightHand Controller] Original Attach");
                if (RightHandRay)
                {
                    RightHandRay.transform.localPosition = new Vector3(0, -0.033f, -0.022f);
                    RightHandRay.transform.localEulerAngles = new Vector3(40, 0, 0);
                }
            }

            if (_hand == QiyuHand.L)
            {
                deviceHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);
            }
            else
            {
                deviceHand = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
            }
        }
    }
}
