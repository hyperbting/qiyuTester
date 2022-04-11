//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System.Collections.Generic;
using UnityEngine;
namespace Unity.XR.Qiyu
{

    /// <summary>
    /// 事件中心
    /// </summary>
    public class QiyuEvent
    {
        private static readonly string TAG = "QiyuSDK:Event";

        public delegate void EventCallBack(params object[] args);

        public static void AddListener(string message, EventCallBack callback)
        {
            EventCallBack current = null;
            listeners.TryGetValue(message, out current);
            if (current != null)
            {
                listeners[message] = current + callback;
            }
            else
            {
                listeners[message] = callback;
            }
        }

        public static void RemoveListener(string message, EventCallBack callback)
        {
            EventCallBack current = null;
            listeners.TryGetValue(message, out current);
            if (current != null)
            {
                listeners[message] = current - callback;
            }
        }

        public static void Trigger(string message, params object[] args)
        {
            EventCallBack current = null;
            listeners.TryGetValue(message, out current);
            if (current != null)
            {
                foreach (var single in current.GetInvocationList())
                {
                    try
                    {
                        ((EventCallBack)single)(args);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(TAG + e.ToString());
                    }
                }
            }
        }

        public static void Clear()
        {
            listeners.Clear();
        }

        private static Dictionary<string, EventCallBack> listeners = new Dictionary<string, EventCallBack>();
    }
}