//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Management;
using DataPtr = System.IntPtr;
using FunPtr = System.IntPtr;

namespace Qiyu.Sdk.Platform
{
    public class RequestCallback
    {
        public RequestCallback() { }

        public virtual void HandleRequestMessage(DataPtr ret, int size)
        {

        }
    }
    public class RequestCallbackByVoid : RequestCallback
    {
        protected Action callBack;
        public RequestCallbackByVoid(Action callBack)
        {
            this.callBack = callBack;
        }
        public static RequestCallback Create(Action callBack)
        {
            RequestCallback rcb = new RequestCallbackByVoid(callBack);
            return rcb;
        }

        public override void HandleRequestMessage(DataPtr data, int size)
        {
            callBack?.Invoke();
        }
    }
    /// <summary>
    /// 返回Json格式数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestCallbackByJson<T> : RequestCallback
    {
        protected Action<T> callBack;

        public RequestCallbackByJson(Action<T> callBack)
        {
            this.callBack = callBack;
        }

        public static RequestCallback Create(Action<T> callBack)
        {
            RequestCallback rcb = new RequestCallbackByJson<T>(callBack);

            return rcb;
        }

        public override void HandleRequestMessage(DataPtr data, int size)
        {
            byte[] buffer = new byte[size];
            Marshal.Copy(data, buffer, 0, size);
            string msg = Encoding.UTF8.GetString(buffer);
            Debug.Log("RequestCallbackByJson::HandleMessage=" + msg);
            T obj = QiyuUtils.FromJson<T>(msg);
            callBack?.Invoke(obj);

        }
    }

    public interface IRequestCallbackData<T>
    {
        T ParseData(DataPtr ret);
    }

    /// <summary>
    /// 返回二进制数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestCallbackByBit<T> : RequestCallback
    {
        protected Action<T> callBack;

        public RequestCallbackByBit(Action<T> callBack)
        {
            this.callBack = callBack;
        }

        public static RequestCallback Create(Action<T> callBack)
        {
            RequestCallback rcb = new RequestCallbackByBit<T>(callBack);
            return rcb;
        }

        public override void HandleRequestMessage(DataPtr ret, int size)
        {

        }
    }

    public class RequestID
    {
        static ulong id = 0;
        public static ulong ID
        {
            get { return ++id; }
        }
    }

    public class QiyuMessageManager
    {
        static Dictionary<ulong, RequestCallback> requestCallbackList = new Dictionary<ulong, RequestCallback>();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallBack(ulong messageId, DataPtr result, int size);

        public static FunPtr RequestProcess_Ptr
        {
            get
            {
                CallBack callback_delegate = MessageProcess;
                return Marshal.GetFunctionPointerForDelegate(callback_delegate);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(CallBack))]
        public static void MessageProcess(ulong requestId, DataPtr result, int size)
        {
            if (requestCallbackList.ContainsKey(requestId))
            {
                try
                {
                    requestCallbackList[requestId].HandleRequestMessage(result, size);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                finally
                {
                    requestCallbackList.Remove(requestId);
                }
            }
        }

        public static ulong AddRequest(RequestCallback rcb)
        {
            ulong requestID = RequestID.ID;
            requestCallbackList[requestID] = rcb;
            return requestID;
        }
    }
}

