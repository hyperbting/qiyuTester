//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Unity.XR.Qiyu
{
    public enum FoveationLevel
    {
        None = -1,
        Low = 0,
        Med = 1,
        High = 2
    }

    /// <summary>
    /// Qiyu XR SDK Manager
    /// </summary>
    public class QiyuManager : MonoBehaviour
    {
        private static QiyuManager instance = null;
        public static QiyuManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<QiyuManager>();
                    if (instance == null)
                    {
                        Debug.LogWarning("QiyuManager is not exist!");
                    }
                }
                return instance;
            }
        }

        public FoveationLevel foveationLevel = FoveationLevel.None;
        public float eyeResolutionScaleFactor = 1.0f;
        //public bool trackingPosition = true;
        [HideInInspector]
        public Transform Head;

        void Awake()
        {
            instance = this;

            Head = Camera.main?.transform;
        }

        private void Start()
        {
            XRSettings.eyeTextureResolutionScale = eyeResolutionScaleFactor;
            Utils.SetFoveationLevel((int)foveationLevel);
        }

        public void Recenter(int tryCount = 1)
        {
            StopCoroutine("TryRecenter");
            StartCoroutine("TryRecenter", tryCount);
        }

        WaitForSecondsRealtime waitForSeconds_0_1 = new WaitForSecondsRealtime(0.1f);
        private IEnumerator TryRecenter(int tryCount)
        {
            int count = 0;
            while (count++ < tryCount)
            {
                bool? result = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).subsystem?.TryRecenter();
                Debug.Log("QiyuManager TryRecenter count:" + count);
                if (result != null && result.Value) break;

                yield return waitForSeconds_0_1;
            }
        }
    }
}
