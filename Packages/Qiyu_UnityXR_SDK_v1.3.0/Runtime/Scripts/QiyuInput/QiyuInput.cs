//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;
using UnityEngine;

namespace Unity.XR.Qiyu
{
    /// <summary>
    /// Qiyu统一输入类(旧版)，推荐使用XR的接口获取手柄按键，这个接口将来会废弃。
    /// </summary>
    [Obsolete("QiyuInput is not recommend,please use UnityEngine.XR.InputDevice instead")]
    public class QiyuInput
    {
        /// <summary>
        /// 利手
        /// </summary>
        public enum Handedness
        {
            RightHanded = 0,
            LeftHanded = 1,
            Default = RightHanded,
        };

        /// <summary>
        /// 控制器
        /// </summary>
        [Flags]
        public enum Controller
        {
            None = 0,
            LTouch = 0x01,
            RTouch = 0x02,
            Touch = LTouch | RTouch,
            HeadDevice = 0x04,

            Default = 0x10,//默认使用带射线的主手柄
        }

        /// <summary>
        /// 物理按键
        /// </summary>
        [Flags]
        public enum Button : int
        {
            None = 0,
            Trigger = 0x01,
            Grip = 0x02,
            A = 0x10,
            B = 0x20,
            Home = 0x40,
            JoyStick = 0x80,
        }

        /// <summary>
        /// 虚拟按键
        /// </summary>
        [Flags]
        public enum VirtualButton
        {
            None = 0,
            Trigger,
            Grip,
            Home,
            A,
            B,
            JoyStick,
            X,
            Y,
            ControllerConnect, //控制器是否连接定义成一个虚拟按键
            Gaze,

            Test1, //测试1
            Test2, //测试2
            Test3, //测试3
        }

        /// <summary>
        /// 按键状态
        /// </summary>
        public enum ButtonAction
        {
            None = 0,
            Up,         //按键抬起
            Down,      //按键按下
            LongDown,   //长按
            ShortDown,  //短按
            Force,      //按键力度 trigger和grip
            Pos,        //摇杆位置
            SwipeLeft,  //摇杆左拨动
            SwipeRight, //摇杆右拨动
            SwipeUp,    //摇杆上拨动
            SwipeDown,  //摇杆下拨动
            Touch,      //按钮触摸
            UnTouch,    //按钮未触摸
        }

        /// <summary>
        /// 手势
        /// </summary>
        public enum Gesture
        {
            None = 0,
            FIST,
            GRIP,
            OPEN,
            POINT,
            POINT_THUMB,
            REST,
            THUMB_UP,
        }

        public enum ControllerVibrationLevel
        {
            OneLevel = 0,      //轻微震动
            TwoLevel = 3,      //轻微震动
            ThreeLevel = 7,   //正常震动
            FourLevel = 12,    //强震动
            FiveLevel = 15,    //剧烈震动，用于游戏，给予用户刺激感；
        }

        public static void Start()
        {
            //注册手柄数据
            InputControllerData.Register();
            HeadDeviceData.Register();
        }

        public static void OnDestroy()
        {
            Debug.Log("QVRInput OnDestroy");
            InputAlgoManager.Instance.Clear();
            QiyuEvent.Clear();
        }

        public static void OnApplicationPause(bool pause)
        {

        }
        public static void Update()
        {
            InputControllerData.Update();
            InputAlgoManager.Instance.Update();
        }

        #region Handedness
        /// <summary>
        /// 获取主手
        /// </summary>
        /// <returns></returns>
        public static Handedness GetDominentHand()
        {
            if (QiyuPlatform.IsAndroid)
                return (Handedness)QiyuXRCorePlugin.QVR_GetHand();

            return Handedness.RightHanded;
        }

        /// <summary>
        /// 设置利手
        /// </summary>
        /// <param name="val"></param>
        public static void SetDominentHand(Handedness val)
        {
            if (QiyuPlatform.IsAndroid)
                QiyuXRCorePlugin.QVR_SetHand((int)val);
        }

        #endregion

        #region Get
        /// <summary>
        /// 获取按钮事件(不松手一直触发)
        /// </summary>
        /// <param name="buttonMask">虚拟按键</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>原始的按键状态</returns>
        public static bool GetRawDown(VirtualButton virtualButton, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetVirtualButtonRawData<bool>(QiyuInputUtil.GetControllerFromDefault(controllerMask), virtualButton, ButtonAction.Down);
        }
        /// <summary>
        /// 获取Trigger力度
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Trigger按键力度[0,10]</returns>
        public static int GetTriggerForce(Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetVirtualButtonRawData<int>(QiyuInputUtil.GetControllerFromDefault(controllerMask), VirtualButton.Trigger, ButtonAction.Force);
        }
        /// <summary>
        /// 获取Grip力度
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Grip按键力度[0,10]</returns>
        public static int GetGripForce(Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetVirtualButtonRawData<int>(QiyuInputUtil.GetControllerFromDefault(controllerMask), VirtualButton.Grip, ButtonAction.Force);
        }
        /// <summary>
        /// 获取按键触摸状态
        /// </summary>
        /// <param name="virtualButton">虚拟按键</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>按键触摸状态</returns>
        public static bool GetRawTouch(VirtualButton virtualButton, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetVirtualButtonRawData<bool>(QiyuInputUtil.GetControllerFromDefault(controllerMask), virtualButton, ButtonAction.Touch);
        }

        /// <summary>
        /// 获取按压时间
        /// </summary>
        /// <param name="buttonMask">虚拟按键</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>按键按下时间</returns>
        public static float GetPressTime(VirtualButton virtualButton, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetDurationTime(QiyuInputUtil.GetControllerFromDefault(controllerMask), virtualButton, ButtonAction.Down);
        }

        /// <summary>
        /// 获取按钮Event事件
        /// </summary>
        /// <param name="buttonMask">虚拟按键</param>
        /// <param name="buttonAction">按键触发动作</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>状态值（在当前帧有效）</returns>
        public static bool Get(VirtualButton virtualButton, ButtonAction buttonAction, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetTriggerEvent(QiyuInputUtil.GetControllerFromDefault(controllerMask), virtualButton, buttonAction);
        }

        /// <summary>
        /// 获取按钮Event按下事件
        /// </summary>
        /// <param name="buttonMask">虚拟按键</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>状态值（在当前帧有效）</returns>
        public static bool GetDown(VirtualButton virtualButton, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetTriggerEvent(QiyuInputUtil.GetControllerFromDefault(controllerMask), virtualButton, ButtonAction.Down);
        }

        /// <summary>
        /// 获取按钮Event抬起事件
        /// </summary>
        /// <param name="buttonMask">虚拟按键</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>状态值（在当前帧有效）</returns>
        public static bool GetUp(VirtualButton virtualButton, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetTriggerEvent(QiyuInputUtil.GetControllerFromDefault(controllerMask), virtualButton, ButtonAction.Up);
        }

        /// <summary>
        /// 获取按钮触摸Event状态（仅有支持触摸的按钮才返回）
        /// </summary>
        /// <param name="buttonMask">虚拟按键</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>状态值（在当前帧有效）</returns>
        public static bool GetButtonTouch(VirtualButton virtualButton, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetTriggerEvent(QiyuInputUtil.GetControllerFromDefault(controllerMask), virtualButton, ButtonAction.Touch);
        }

        /// <summary>
        /// 获取摇杆的位置
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Vector2 x[-1, 1], y[-1,1]</returns>
        public static Vector2 GetJoyStickPosition(Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetVirtualButtonRawData<Vector2>(QiyuInputUtil.GetControllerFromDefault(controllerMask), VirtualButton.JoyStick, ButtonAction.Pos);
        }
        /// <summary>
        /// 获取摇杆拨动事件
        /// </summary>
        /// <param name="buttonAction">动作</param>
        /// <param name="controllerMask">控制器</param>
        /// <returns>摇杆拨动事件（当前帧有效）</returns>
        public static bool GetJoyStickSwipeEvent(ButtonAction buttonAction, Controller controllerMask = Controller.Default)
        {
            return InputAlgoManager.Instance.GetTriggerEvent(QiyuInputUtil.GetControllerFromDefault(controllerMask), VirtualButton.JoyStick, buttonAction);
        }

        /// <summary>
        /// 控制器位置
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Vector3 控制器世界坐标 </returns>
        public static Vector3 GetLocalControllerPosition(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetRawPosition(deviceId);
        }

        /// <summary>
        /// 控制器旋转
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns> 控制器四元数</returns>
        public static Quaternion GetLocalControllerRotation(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetRawRotation(deviceId);
        }

        /// <summary>
        /// 控制器线速度
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Vector3 控制器线速度</returns>
        public static Vector3 GetLocalControllerVelocity(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetRawVelocity(deviceId);
        }

        /// <summary>
        /// 控制器线加速度
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Vector3 控制器线加速度</returns>
        public static Vector3 GetLocalControllerAcceleration(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetRawAcc(deviceId);
        }

        /// <summary>
        /// 控制器角速度
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Vector3 控制器角速度</returns>
        public static Vector3 GetLocalControllerAngularVelocity(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetRawAngVelocity(deviceId);
        }

        /// <summary>
        /// 控制器角加速度
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>Vector3 控制器角加速度</returns>
        public static Vector3 GetLocalControllerAngularAcceleration(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetRawAngAcc(deviceId);
        }

        /// <summary>
        /// 获取电量
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>电量百分比</returns>
        public static int GetLocalControllerBatteryPercent(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetRawBatteryLevel(deviceId);
        }

        /// <summary>
        /// 控制器是否连接
        /// </summary>
        /// <param name="controllerMask">控制器</param>
        /// <returns>连接状态</returns>
        public static bool IsControllerConnected(Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            return InputControllerData.GetIsConnected(deviceId);
        }

        /// <summary>
        /// 震动手柄
        /// </summary>
        /// <param name="amplitude">震动振幅 范围为[1-15]</param>
        /// <param name="duration">震动时长，单位为毫秒[1,4095]（ms）</param>
        /// <param name="controllerMask">控制器</param>
        public static void StartVibration(int amplitude, int duration, Controller controllerMask = Controller.Default)
        {
            DEVICE_ID deviceId = QiyuInputUtil.Ctrl2DeviceId(controllerMask);
            QiyuXRCorePlugin.QVR_ControllerStartVibration((int)deviceId, amplitude, duration);
        }

        class ControllerParameter
        {
            public string _key;
            public string _data;
            public ControllerParameter(string key, string data)
            {
                _key = key;
                _data = data;
            }
        }

        public static bool GetButtonOk()
        {
            return false;
        }
        #endregion
    }
}