//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using UnityEngine;
using System.Collections;
namespace Unity.XR.Qiyu
{
    public class QiyuSingletion<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static string MonoSingletionName = "QiyuSingletionRoot";
        private static GameObject MonoSingletionRoot;
        private static T instance;
        private static bool isQuit = false;

        public void Register()
        {
        }
        public static T Instance
        {
            get
            {
                if (isQuit)
                    return null;//Don't Instances When App is Quit.

                if (MonoSingletionRoot == null)
                {
                    MonoSingletionRoot = GameObject.Find(MonoSingletionName);
                    if (MonoSingletionRoot == null)
                    {
                        MonoSingletionRoot = new GameObject();
                        MonoSingletionRoot.name = MonoSingletionName;
                        DontDestroyOnLoad(MonoSingletionRoot);
                    }
                }
                if (instance == null)
                {
                    instance = MonoSingletionRoot.GetComponent<T>();
                    if (instance == null)
                    {
                        instance = MonoSingletionRoot.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        public virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
        }
        public virtual void OnApplicationQuit()
        {
            instance = null;
            isQuit = true;
        }

    }
}