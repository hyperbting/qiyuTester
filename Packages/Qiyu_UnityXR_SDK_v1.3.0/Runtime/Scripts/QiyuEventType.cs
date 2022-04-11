//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
namespace Unity.XR.Qiyu
{
    public class QiyuEventType
    {
        //设备输入事件
        public static readonly string DEVICE_INPUT_EVENT = "DEVICE_INPUT_EVENT";
        //设备输入事件（带值）
        public static readonly string DEVICE_INPUT_EVENT_VALUE = "DEVICE_INPUT_EVENT_VALUE";


        public static readonly string CONTROLLER_LOADED = "CONTROLLER_LOADED";
        public static readonly string CONNECTION_CHANGE = "CONNECTION_CHANGE";
        public static readonly string CONTROLLER_INSTANCE_CHANGE = "CONTROLLER_INSTANCE_CHANGE";
        public static readonly string RAY_ONENTER = "RAY_ONENTER";
        public static readonly string RAY_ONEXIT = "RAY_ONEXIT";
        public static readonly string GESTURE_CHANGE = "GESTURE_CHANGE";
    }
}