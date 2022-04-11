//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using UnityEngine;
using UnityEngine.XR.Management;

namespace Unity.XR.Qiyu
{
    public class QiyuXRCoreRuntime : QiyuSingletion<QiyuXRCoreRuntime>
    {
        const string TAG = "QiyuXRCoreRuntime";
        bool running = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInit()
        {
            if (QiyuPlatform.IsAndroid)
            {
                XRManagerSettings manager = XRGeneralSettings.Instance.Manager;
                if (manager.activeLoader is QiyuLoader)
                {
                    QiyuXRCoreRuntime.Instance.Register();
                }
            }
            else
            {
                //TODO:QiyuInput删除后，此行也会删除
                QiyuXRCoreRuntime.Instance.Register();
            }
        }

        public override void Awake()
        {
            base.Awake();
            Debug.Log(TAG + " Awake");
            running = true;
        }

        public void OnDestroy()
        {
            Debug.Log(TAG + " OnDestroy");
            QiyuInput.OnDestroy();
        }

        public void Start()
        {
            Debug.Log(TAG + " Start");
            QiyuInput.Start();
        }

        public void Update()
        {
            if (running == false) return;

            QiyuXRCore.Update(Time.unscaledDeltaTime);
            QiyuInput.Update();
        }

        public void OnApplicationPause(bool pause)
        {
            Debug.Log(TAG + " OnApplicationPause:" + pause);

            if (running == false) return;

            QiyuXRCore.OnApplicationPause(pause);
            QiyuInput.OnApplicationPause(pause);
        }
    }
}
