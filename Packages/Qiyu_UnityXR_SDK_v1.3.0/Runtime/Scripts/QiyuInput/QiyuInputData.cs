//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.Qiyu
{
    #region InputAlgoManager
    public class VirtualButtonWrap
    {
        public const int ControllerOffset = 1000000;
        public const int ButtonOffset = 1000;

        private string _virtualButtonName;
        private int _vbID;
        private ButtonAlgoBase _algo;

        public VirtualButtonWrap(QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction)
        {
            _virtualButtonName = GetEnumNameByKey<QiyuInput.Controller>((int)controller) +
            "_" + GetEnumNameByKey<QiyuInput.VirtualButton>((int)virtualButton) +
            "_" + GetEnumNameByKey<QiyuInput.ButtonAction>((int)buttonAction);
            _vbID = VirtualButtonWrap.GetVirtualButtonKey(controller, virtualButton, buttonAction);

            InputAlgoManager.Instance.AddVirtualButton(this);
        }

        public VirtualButtonWrap(string virtualButtonName, QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction)
        {
            _virtualButtonName = virtualButtonName;
            _vbID = VirtualButtonWrap.GetVirtualButtonKey(controller, virtualButton, buttonAction);
        }

        public VirtualButtonWrap(string virtualButtonName, VirtualButtonWrap vb)
        {
            _virtualButtonName = virtualButtonName;
            _vbID = vb.VBID;
        }

        public int VBID
        {
            get { return _vbID; }
        }

        public ButtonAlgoBase Algo
        {
            get { return _algo; }
            set { _algo = value; }
        }

        public int Controller
        {
            get { return _vbID / ControllerOffset; }
        }

        public int VirtualButton
        {
            get { return (_vbID % ControllerOffset) / ButtonOffset; }
        }

        public int ButtonAction
        {
            get { return (_vbID % (ControllerOffset + ButtonOffset)); }
        }

        public static int GetVirtualButtonKey(QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction)
        {
            return (int)controller * ControllerOffset + (int)virtualButton * ButtonOffset + (int)buttonAction;
        }

        public string GetEnumNameByKey<T>(int key)
        {
            return Enum.GetName(typeof(T), key);
        }

        public void Update()
        {
            _algo.Update();
        }

    }
    public class VirtualButtonWrapHelp
    {
        public static VirtualButtonWrap Constructor_Event<T>(
            QiyuInput.Controller controller,
            QiyuInput.VirtualButton virtualButton,
            QiyuInput.ButtonAction buttonAction,
            T initValue,
            ButtonAlgo_Event<T>.VariableGenerator variableGenerator,
            T treggerValue,
            float time = -1
            ) where T : IEquatable<T>
        {
            VirtualButtonWrap vbw = new VirtualButtonWrap(controller, virtualButton, buttonAction);
            ButtonAlgo_Event<T> algo = new ButtonAlgo_Event<T>(vbw.VBID.ToString(), variableGenerator, initValue);
            algo.AddEventPublisher(treggerValue, () => QiyuEvent.Trigger(QiyuEventType.DEVICE_INPUT_EVENT, controller, virtualButton, buttonAction), time);
            vbw.Algo = algo;
            return vbw;
        }

        public static VirtualButtonWrap Constructor_EventRange<T>(
    QiyuInput.Controller controller,
    QiyuInput.VirtualButton virtualButton,
    QiyuInput.ButtonAction buttonAction,
    ButtonAlgo_Event<T>.VariableGenerator variableGenerator,
    T lowRange,
    T upRange,
    bool lowClose = true,
    bool upClose = true,
    float time = -1
    ) where T : IComparable<T>
        {
            VirtualButtonWrap vbw = new VirtualButtonWrap(controller, virtualButton, buttonAction);
            ButtonAlgo_EventRange<T> algo = new ButtonAlgo_EventRange<T>(vbw.VBID.ToString(), variableGenerator, lowRange, upRange, lowClose, upClose);
            algo.AddEventPublisher(() => QiyuEvent.Trigger(QiyuEventType.DEVICE_INPUT_EVENT, controller, virtualButton, buttonAction), time);
            vbw.Algo = algo;
            return vbw;
        }

        public static VirtualButtonWrap Constructor_Change<T>(
            QiyuInput.Controller controller,
            QiyuInput.VirtualButton virtualButton,
            T initValue,
            ButtonAlgo_Event<T>.VariableGenerator variableGenerator
            ) where T : IEquatable<T>
        {
            VirtualButtonWrap vbw = new VirtualButtonWrap(controller, virtualButton, QiyuInput.ButtonAction.None);
            ButtonAlgo_Event<T> algo = new ButtonAlgo_Event<T>(vbw.VBID.ToString(), variableGenerator, initValue);
            algo.AddChangePublisher((value) => QiyuEvent.Trigger(QiyuEventType.DEVICE_INPUT_EVENT_VALUE, controller, virtualButton, QiyuInput.ButtonAction.None, value));
            vbw.Algo = algo;
            return vbw;
        }

        public static VirtualButtonWrap Constructor_Change<T>(
            QiyuInput.Controller controller,
            QiyuInput.VirtualButton virtualButton,
            QiyuInput.ButtonAction buttonAction,
            T initValue,
            ButtonAlgo_Event<T>.VariableGenerator variableGenerator
            ) where T : IEquatable<T>
        {
            VirtualButtonWrap vbw = new VirtualButtonWrap(controller, virtualButton, buttonAction);
            ButtonAlgo_Event<T> algo = new ButtonAlgo_Event<T>(vbw.VBID.ToString(), variableGenerator, initValue);
            algo.AddChangePublisher((value) => QiyuEvent.Trigger(QiyuEventType.DEVICE_INPUT_EVENT_VALUE, controller, virtualButton, buttonAction, value));
            vbw.Algo = algo;
            return vbw;
        }

        public static VirtualButtonWrap Constructor_RawButton<T>(
            QiyuInput.Controller controller,
            QiyuInput.VirtualButton virtualButton,
            QiyuInput.ButtonAction buttonAction,
            ButtonAlgo_RawButton<T>.VariableGenerator variableGenerator
            ) where T : IEquatable<T>
        {
            VirtualButtonWrap vbw = new VirtualButtonWrap(controller, virtualButton, buttonAction);
            ButtonAlgo_RawButton<T> algo = new ButtonAlgo_RawButton<T>(vbw.VBID.ToString(), variableGenerator);
            vbw.Algo = algo;
            return vbw;
        }

        public static VirtualButtonWrap Constructor_JoyStickSwipe<T>(
            QiyuInput.Controller controller,
            QiyuInput.VirtualButton virtualButton,
            QiyuInput.ButtonAction buttonAction,
            ButtonAlgo_JoyStickSwipe.VariableGenerator variableGenerator
            ) where T : IEquatable<T>
        {
            VirtualButtonWrap vbw = new VirtualButtonWrap(controller, virtualButton, buttonAction);
            ButtonAlgo_JoyStickSwipe algo = new ButtonAlgo_JoyStickSwipe(vbw.VBID.ToString(), variableGenerator);
            algo.AddEventPublisher(() => QiyuEvent.Trigger(QiyuEventType.DEVICE_INPUT_EVENT, controller, virtualButton, buttonAction));
            vbw.Algo = algo;
            return vbw;
        }

        public static VirtualButtonWrap ButtonAlgoCombinationKey(
            QiyuInput.Controller controller,
            QiyuInput.VirtualButton virtualButton,
            QiyuInput.ButtonAction buttonAction,
            float time,
            ButtonAlgo_CombinationKey.CombinationKeyRelation ckr,
            params int[] vbIDList
    )
        {
            VirtualButtonWrap vbw = new VirtualButtonWrap(controller, virtualButton, buttonAction);
            ButtonAlgo_CombinationKey algo = new ButtonAlgo_CombinationKey(vbw.VBID.ToString(), ckr, vbIDList);
            algo.AddEventPublisher(() => QiyuEvent.Trigger(QiyuEventType.DEVICE_INPUT_EVENT, controller, virtualButton, buttonAction), time);
            vbw.Algo = algo;
            return vbw;
        }

    }
    public class ButtonAlgoBase
    {
        protected string _name = "";
        public virtual string GetName() { return _name; }
        public virtual bool IsTriggerEvent() { return false; }
        public virtual float GetDurationTime() { return 0; }
        public virtual float GetTriggerTime() { return -1; }
        public virtual bool IsTriggerState() { return false; }
        public virtual void Update() { }
    }
    public class ButtonAlgo_RawButton<T> : ButtonAlgoBase
    {
        public delegate T VariableGenerator();
        protected VariableGenerator _generator;
        public ButtonAlgo_RawButton(string name, VariableGenerator generator)
        {
            _name = name;
            _generator = generator;
        }
        public T GetVariableGenerator()
        {
            return _generator();
        }
    }
    public class ButtonAlgo_Event<T> : ButtonAlgo_RawButton<T> where T : IEquatable<T>
    {
        public delegate void EventPublisher();
        public delegate void ChangePublisher(T val);

        private bool _triggerState = false;
        private EventPublisher _eventPublisher;
        private ChangePublisher _changePublisher;

        private bool _currentFrameTrigger = false;

        private T _prevValue = default(T);
        private T _currentValue = default(T);
        private T _triggerValue = default(T);

        private float _beginTime;
        private float _time;

        public T CurrentValue
        {
            get
            {
                return _currentValue;
            }
        }

        public ButtonAlgo_Event(string name, VariableGenerator generator, T initValue = default(T)) : base(name, generator)
        {
            _eventPublisher = null;
            _changePublisher = null;
            _triggerState = false;
            _triggerValue = default(T);
            _currentValue = initValue;
            _beginTime = -1;
        }

        public ButtonAlgo_Event<T> AddEventPublisher(T t, EventPublisher epub, float time = -1)
        {
            _triggerValue = t;
            _eventPublisher = epub;
            _time = time;
            return this;
        }

        public ButtonAlgo_Event<T> AddChangePublisher(ChangePublisher cpub)
        {
            _changePublisher = cpub;
            return this;
        }

        public override void Update()
        {
            _prevValue = _currentValue;
            _currentValue = _generator();

            if (!_currentValue.Equals(_prevValue))
            {
                _beginTime = Time.realtimeSinceStartup;
                if (_currentValue.Equals(_triggerValue))
                    _currentFrameTrigger = true;

                if (_eventPublisher != null && _currentValue.Equals(_triggerValue))
                    _eventPublisher.Invoke();

                if (_changePublisher != null)
                    _changePublisher.Invoke(_currentValue);

            }
            else
            {
                _currentFrameTrigger = false;
            }

            if (_eventPublisher != null && _currentValue.Equals(_triggerValue))
                _triggerState = true;
            else
                _triggerState = false;

            if (_time > 0 && _currentValue.Equals(_triggerValue)
                && (GetDurationTime() > _time))
            {
                _beginTime = Time.realtimeSinceStartup;
                _eventPublisher?.Invoke();
            }

        }

        public override bool IsTriggerEvent()
        {
            return _currentFrameTrigger;
        }

        public override bool IsTriggerState()
        {
            return _triggerState;
        }

        public override float GetDurationTime()
        {
            return Time.realtimeSinceStartup - _beginTime;
        }

        public override float GetTriggerTime()
        {
            return _beginTime;
        }
    }
    public class ButtonAlgo_EventRange<T> : ButtonAlgo_RawButton<T> where T : IComparable<T>
    {
        public delegate void EventPublisher();
        public delegate void ChangePublisher(T val);

        private bool _triggerState = false;
        private EventPublisher _eventPublisher;
        private ChangePublisher _changePublisher;

        private bool _currentFrameTrigger = false;

        private T _prevValue = default(T);
        private T _currentValue = default(T);

        private T _lowRange;
        private bool _lowClose;
        private T _upRange;
        private bool _upClose;

        private float _beginTime;
        private float _time;

        public T CurrentValue
        {
            get
            {
                return _currentValue;
            }
        }

        public ButtonAlgo_EventRange(string name, VariableGenerator generator, T lowRange, T upRange, bool lowClose = true, bool upClose = true) : base(name, generator)
        {
            _eventPublisher = null;
            _changePublisher = null;
            _triggerState = false;
            _lowRange = lowRange;
            _upRange = upRange;
            _lowClose = lowClose;
            _upClose = upClose;
            _beginTime = -1;
        }

        public ButtonAlgo_EventRange<T> AddEventPublisher(EventPublisher epub, float time = -1)
        {
            _eventPublisher = epub;
            _time = time;
            return this;
        }

        public ButtonAlgo_EventRange<T> AddChangePublisher(ChangePublisher cpub)
        {
            _changePublisher = cpub;
            return this;
        }

        bool CheckRange(T value)
        {
            if (_lowClose && _upClose)
            {
                return (value.CompareTo(_lowRange) >= 0
                && value.CompareTo(_upRange) <= 0);
            }
            else if (!_lowClose && !_upClose)
            {
                return (value.CompareTo(_lowRange) > 0
                 && value.CompareTo(_upRange) < 0);
            }
            else if (_lowClose && !_upClose)
            {
                return (value.CompareTo(_lowRange) >= 0
                 && value.CompareTo(_upRange) < 0);
            }
            else if (!_lowClose && _upClose)
            {
                return (value.CompareTo(_lowRange) > 0
                 && value.CompareTo(_upRange) <= 0);
            }
            return false;
        }

        public override void Update()
        {
            _prevValue = _currentValue;
            _currentValue = _generator();

            if (CheckRange(_currentValue) != CheckRange(_prevValue))
            {
                _beginTime = Time.realtimeSinceStartup;
                if (CheckRange(_currentValue))
                {
                    _currentFrameTrigger = true;
                    _eventPublisher?.Invoke();
                }
                _changePublisher?.Invoke(_currentValue);
            }
            else
            {
                _currentFrameTrigger = false;
            }

            if (CheckRange(_currentValue))
                _triggerState = true;
            else
                _triggerState = false;

            if (_time > 0
                && CheckRange(_currentValue)
                && (GetDurationTime() > _time))
            {
                _beginTime = Time.realtimeSinceStartup;
                _eventPublisher?.Invoke();
            }

        }

        public override bool IsTriggerEvent()
        {
            return _currentFrameTrigger;
        }

        public override bool IsTriggerState()
        {
            return _triggerState;
        }

        public override float GetDurationTime()
        {
            return Time.realtimeSinceStartup - _beginTime;
        }

        public override float GetTriggerTime()
        {
            return _beginTime;
        }
    }
    public class ButtonAlgo_CombinationKey : ButtonAlgoBase
    {
        public enum CombinationKeyRelation
        {
            And, //并且
            Or,  //或者
            Sequence, //序列
        }

        List<VirtualButtonWrap> vbwList = new List<VirtualButtonWrap>();

        public delegate void EventPublisher();

        private bool _triggerState = false;
        private EventPublisher _eventPublisher;
        private bool _currentFrameTrigger = false;

        private float _beginTime;
        private float _time;
        private CombinationKeyRelation _ckr;

        public ButtonAlgo_CombinationKey(string name, CombinationKeyRelation ckr, params int[] vbIDList)
        {
            _eventPublisher = null;
            _triggerState = false;
            _beginTime = -1;
            _ckr = ckr;

            for (int i = 0; i < vbIDList.Length; i++)
            {
                VirtualButtonWrap vbw = InputAlgoManager.Instance.GetVirtualButtonWrap(vbIDList[i]);
                if (vbw == null)
                {
                    Debug.LogError("ButtonAlgoCombinationKey->vbID Not Found " + vbIDList[i]);
                }
                else
                {
                    vbwList.Add(vbw);
                }
            }
        }

        public ButtonAlgo_CombinationKey AddEventPublisher(EventPublisher epub, float time = -1)
        {
            _time = time;
            _eventPublisher = epub;
            return this;
        }

        public override void Update()
        {
            if (_ckr == CombinationKeyRelation.And)
                CheckAnd();
            else if (_ckr == CombinationKeyRelation.Or)
                CheckOr();
            else if (_ckr == CombinationKeyRelation.Sequence)
                CheckSequence();
        }

        private void CheckAnd()
        {
            _triggerState = true;
            for (int i = 0; i < vbwList.Count; i++)
            {
                if (!vbwList[i].Algo.IsTriggerState())
                {
                    _triggerState = false;
                }
            }
            Trigger();
        }

        private void CheckOr()
        {
            _triggerState = false;
            for (int i = 0; i < vbwList.Count; i++)
            {
                if (vbwList[i].Algo.IsTriggerState())
                {
                    _triggerState = true;
                }
            }
            Trigger();
        }
        private void CheckSequence()
        {
            _triggerState = true;
            float preTime = -1;
            for (int i = 0; i < vbwList.Count; i++)
            {
                if (vbwList[i].Algo.IsTriggerState())
                {
                    if (vbwList[i].Algo.GetTriggerTime() < preTime)
                    {
                        _triggerState = false;
                        break;
                    }
                    preTime = vbwList[i].Algo.GetTriggerTime();
                }
                else
                {
                    _triggerState = false;
                }
            }

            Trigger();
        }

        private void Trigger()
        {
            if (_triggerState)
            {
                if (_currentFrameTrigger == false)
                {
                    _currentFrameTrigger = true;
                    _beginTime = Time.realtimeSinceStartup;
                    if (_time < 0)
                    {
                        _eventPublisher?.Invoke();
                    }
                }
                else
                {
                    _currentFrameTrigger = false;
                }

                if (_time > 0 && (GetDurationTime() > _time))
                {
                    _beginTime = Time.realtimeSinceStartup;
                    _eventPublisher?.Invoke();
                }
            }
            else
            {
                _currentFrameTrigger = false;
            }
        }

        public override float GetDurationTime()
        {
            return Time.realtimeSinceStartup - _beginTime;
        }

        public override bool IsTriggerState()
        {
            return _triggerState;
        }

        public override bool IsTriggerEvent()
        {
            return _currentFrameTrigger;
        }


    }
    public class ButtonAlgo_JoyStickSwipe : ButtonAlgoBase
    {
        public const float SwipeOffset = 0.4f;
        public const float InitOffset = 0.35f;
        private float _beginTime;
        private float _time;

        public delegate void EventPublisher();
        public delegate float VariableGenerator();
        private VariableGenerator _generator;
        private EventPublisher eventPublisher;
        private bool isRun = true;
        private bool _currentFrameTrigger = false;
        private bool _triggerState = false;


        public ButtonAlgo_JoyStickSwipe(string name, VariableGenerator generator)
        {
            _name = name;
            _generator = generator;
            isRun = true;
            _currentFrameTrigger = false;
            _triggerState = false;
            eventPublisher = null;
            _beginTime = 0;
        }

        public ButtonAlgo_JoyStickSwipe AddEventPublisher(EventPublisher epub)
        {
            eventPublisher = epub;
            return this;
        }

        public override void Update()
        {
            float value = _generator();
            if (value < InitOffset)
            {
                isRun = true;
                _currentFrameTrigger = false;
            }


            if (isRun)
            {
                if (value >= SwipeOffset)
                {
                    _currentFrameTrigger = true;
                    eventPublisher?.Invoke();
                    isRun = false;
                    _beginTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                _currentFrameTrigger = false;
            }

            if (value < SwipeOffset)
            {
                _triggerState = false;
                _beginTime = Time.realtimeSinceStartup;
            }
            else
            {
                _triggerState = true;
            }
        }

        public override bool IsTriggerEvent()
        {
            return _currentFrameTrigger;
        }

        public override float GetDurationTime()
        {
            return Time.realtimeSinceStartup - _beginTime;
        }

        public override bool IsTriggerState()
        {
            return _triggerState;
        }
    }
    public class InputAlgoManager
    {
        public static InputAlgoManager Instance = new InputAlgoManager();

        private List<VirtualButtonWrap> virtualButtonList = new List<VirtualButtonWrap>();
        private Dictionary<string, VirtualButtonWrap> virtualButtonDic = new Dictionary<string, VirtualButtonWrap>();

        private List<IMonitor> _monitors = new List<IMonitor>();

        public void Clear()
        {
            virtualButtonList.Clear();
            virtualButtonDic.Clear();
            _monitors.Clear();
        }

        public void RegisterMonitor(IMonitor monitors)
        {

            for (int i = _monitors.Count - 1; i > 0; i--)
            {
                if (monitors.GetName() == _monitors[i].GetName())
                {
                    Debug.LogError("InputAlgoManager Monitor Repetition =" + monitors.GetName());
                    return;
                }
            }
            _monitors.Add(monitors);
        }
        public void UnRegisterMonitor(string id)
        {
            for (int i = _monitors.Count - 1; i > 0; i--)
            {
                if (id == _monitors[i].GetName())
                {
                    _monitors.RemoveAt(i);
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < virtualButtonDic.Count; i++)
            {
                virtualButtonList[i].Update();
            }

            for (int i = 0; i < _monitors.Count; i++)
            {
                _monitors[i].Update();
            }
        }

        public void AddVirtualButton(VirtualButtonWrap vbw)
        {
            virtualButtonList.Add(vbw);
            virtualButtonDic.Add(vbw.VBID.ToString(), vbw);
        }

        public void NotRegisteButton(QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction)
        {
            Debug.LogError("Not Registe Button: " + controller.ToString() + "_" + virtualButton.ToString() + "_" + buttonAction.ToString());
        }

        public VirtualButtonWrap GetVirtualButtonWrap(int vbID)
        {
            string str_vbID = vbID.ToString();
            if (virtualButtonDic.ContainsKey(str_vbID))
            {
                return virtualButtonDic[str_vbID];
            }
            return null;
        }

        /// <summary>
        /// 获取按键的点击事件
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="virtualButton"></param>
        /// <param name="buttonAction"></param>
        /// <returns></returns>
        public bool GetTriggerEvent(QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction)
        {
            string str = VirtualButtonWrap.GetVirtualButtonKey(controller, virtualButton, buttonAction).ToString();
            if (virtualButtonDic.ContainsKey(str))
            {
                return virtualButtonDic[str].Algo.IsTriggerEvent();
            }
            else
            {
                NotRegisteButton(controller, virtualButton, buttonAction);
            }
            return false;
        }
        public float GetDurationTime(QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction)
        {
            string str = VirtualButtonWrap.GetVirtualButtonKey(controller, virtualButton, buttonAction).ToString();
            if (virtualButtonDic.ContainsKey(str))
            {
                return virtualButtonDic[str].Algo.GetDurationTime();
            }
            else
            {
                NotRegisteButton(controller, virtualButton, buttonAction);
            }
            return 0;
        }

        public bool GetLogicTriggerState(QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction)
        {
            string str = VirtualButtonWrap.GetVirtualButtonKey(controller, virtualButton, buttonAction).ToString();
            if (virtualButtonDic.ContainsKey(str))
            {
                return virtualButtonDic[str].Algo.IsTriggerState();
            }
            else
            {
                NotRegisteButton(controller, virtualButton, buttonAction);
            }
            return false;
        }

        /// <summary>
        /// 从原始数据中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="virtualButton"></param>
        /// <param name="buttonAction"></param>
        /// <returns></returns>
        public T GetVirtualButtonRawData<T>(QiyuInput.Controller controller, QiyuInput.VirtualButton virtualButton, QiyuInput.ButtonAction buttonAction) where T : IEquatable<T>
        {
            string str = VirtualButtonWrap.GetVirtualButtonKey(controller, virtualButton, buttonAction).ToString();
            if (virtualButtonDic.ContainsKey(str))
            {
                ButtonAlgo_RawButton<T> algo = virtualButtonDic[str].Algo as ButtonAlgo_RawButton<T>;
                if (algo != null)
                    return algo.GetVariableGenerator();
                else
                {
                    Debug.LogError("Button Event Type Error " + controller.ToString() + "_" + virtualButton.ToString() + "_" + buttonAction.ToString());
                }
            }
            else
            {
                NotRegisteButton(controller, virtualButton, buttonAction);
            }
            return default(T);
        }

    }
    #endregion
    /// <summary>
    /// 手柄
    /// </summary>
    public class InputControllerData
    {
        public static Matrix4x4 Svr2Unitymatrix = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, -1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 0));

        public static float JOYSTICK_MAX = 120.0f;

        static QiyuXRCorePlugin.ControllerData rawControllerDataLeft;
        static QiyuXRCorePlugin.ControllerData rawControllerDataRight;

        public class QVR_Controller_State
        {

            public Controller_State[] _state = new Controller_State[2];

            public struct DebugPoseInfo
            {
                public Vector3 _position;
                public Quaternion _rotation;
            }

            public class Controller_State
            {
                public bool _isConnected;
                public int _button;
                public int _buttonTouch;

                public int _batteryLevel;
                public int _triggerForce;
                public int _gripForce;

                public bool isShow;

                public Vector2 _joyStickPos;

                public Vector3 _position;
                public Quaternion _rotation = Quaternion.identity;
                public Vector3 _positionRaw;
                public Quaternion _rotationRaw = Quaternion.identity;

                public Vector3 _velocity;
                public Vector3 _acceleration;

                public Vector3 _angVelocity;
                public Vector3 _angAcceleration;


                public void DebugShow(bool isLeft = true)
                {
                    string c = isLeft ? "Left=" : "Right=";
                    string str = "ControllerData " + c + " {" +
                        "_isConnected=" + _isConnected +
                        "_button=" + _button +
                        ",_buttonTouch=" + _buttonTouch +
                        ",_batteryLevel=" + _batteryLevel +
                        ",_triggerForce=" + _triggerForce +
                        ",_gripForce=" + _gripForce +
                        ",_joyStickPos=" + _joyStickPos +
                        ",_position=" + _position +
                        ",_rotation=" + _rotation +
                        ",_velocity=" + _velocity +
                        ",_acceleration=" + _acceleration +
                        ",_angularVelocity=" + _angVelocity +
                        ",_angularAcceleration=" + _angAcceleration +
                        "}";

                    Debug.Log(str);
                }

            }

            public void DebugShow()
            {

                _state[0].DebugShow(true);
                _state[1].DebugShow(false);
            }


        }

        static QVR_Controller_State _rawData = new QVR_Controller_State();

        static InputControllerData()
        {
            _rawData._state[0] = new QVR_Controller_State.Controller_State();
            _rawData._state[1] = new QVR_Controller_State.Controller_State();
        }

        public static void Register()
        {
            InputAlgoManager.Instance.RegisterMonitor(
                new QiyuVariableMonitor<bool>("L_Is_Connected", () => InputControllerData.GetIsConnected(DEVICE_ID.HAND_0)).AddChangePublisher((x) => QiyuEvent.Trigger(QiyuEventType.CONNECTION_CHANGE, x, QiyuInput.Controller.LTouch, QiyuHand.L))
                );
            InputAlgoManager.Instance.RegisterMonitor(
                 new QiyuVariableMonitor<bool>("R_Is_Connected", () => InputControllerData.GetIsConnected(DEVICE_ID.HAND_1)).AddChangePublisher((x) => QiyuEvent.Trigger(QiyuEventType.CONNECTION_CHANGE, x, QiyuInput.Controller.RTouch, QiyuHand.R))
                );

            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.A, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.A, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.B, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.B, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.X, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.X, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Home, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.Home), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Home, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.Home), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Grip, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.Grip), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Grip, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.Grip), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.JoyStick), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_0, QiyuInput.Button.JoyStick), true);
            VirtualButtonWrapHelp.Constructor_RawButton(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Pos, () => InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_0));

            //VirtualButtonWrapHelp.Constructor_EventRange(QVRInput.Controller.LTouch, QVRInput.VirtualButton.Trigger, QVRInput.ButtonAction.Force, () => InputControllerData.GetRawTriggerForce(DEVICE_ID.HAND_0), 6,10);
            VirtualButtonWrapHelp.Constructor_RawButton(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Force, () => InputControllerData.GetRawTriggerForce(DEVICE_ID.HAND_0));
            VirtualButtonWrapHelp.Constructor_RawButton(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Grip, QiyuInput.ButtonAction.Force, () => InputControllerData.GetRawGripForce(DEVICE_ID.HAND_0));

            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeLeft, () => { float value = -InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_0).x; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeRight, () => { float value = InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_0).x; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeUp, () => { float value = InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_0).y; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeDown, () => { float value = -InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_0).y; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_Change(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.ControllerConnect, false, () => InputControllerData.GetIsConnected(DEVICE_ID.HAND_0));
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.X, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.X, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.JoyStick), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_0, QiyuInput.Button.JoyStick), true);


            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.X, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.X, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.A, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.A, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.B, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.B, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Home, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.Home), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Home, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.Home), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Grip, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.Grip), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Grip, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.Grip), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Down, false, () => InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.JoyStick), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Up, true, () => !InputControllerData.GetRawDown(DEVICE_ID.HAND_1, QiyuInput.Button.JoyStick), true);
            VirtualButtonWrapHelp.Constructor_RawButton(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Pos, () => InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_1));
            VirtualButtonWrapHelp.Constructor_RawButton(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Force, () => InputControllerData.GetRawTriggerForce(DEVICE_ID.HAND_1));
            VirtualButtonWrapHelp.Constructor_RawButton(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Grip, QiyuInput.ButtonAction.Force, () => InputControllerData.GetRawGripForce(DEVICE_ID.HAND_1));
            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeLeft, () => { float value = -InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_1).x; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeRight, () => { float value = InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_1).x; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeUp, () => { float value = InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_1).y; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_JoyStickSwipe<float>(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.SwipeDown, () => { float value = -InputControllerData.GetRawjoyStickPos(DEVICE_ID.HAND_1).y; if (value > 0) return value; else return 0; });
            VirtualButtonWrapHelp.Constructor_Change(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.ControllerConnect, false, () => InputControllerData.GetIsConnected(DEVICE_ID.HAND_1));
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.A, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.A, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.A), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.B, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.B, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.B), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.Trigger, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.Trigger), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.Touch, false, () => InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.JoyStick), true);
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.RTouch, QiyuInput.VirtualButton.JoyStick, QiyuInput.ButtonAction.UnTouch, true, () => !InputControllerData.GetRawTouch(DEVICE_ID.HAND_1, QiyuInput.Button.JoyStick), true);

            //组合键最后注册
            VirtualButtonWrapHelp.ButtonAlgoCombinationKey(
                QiyuInput.Controller.LTouch,
                QiyuInput.VirtualButton.Test1,
                QiyuInput.ButtonAction.None,
                -1,
                 ButtonAlgo_CombinationKey.CombinationKeyRelation.And,
                 VirtualButtonWrap.GetVirtualButtonKey(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Home, QiyuInput.ButtonAction.Down),
                 VirtualButtonWrap.GetVirtualButtonKey(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Down)
                );

            VirtualButtonWrapHelp.ButtonAlgoCombinationKey(
                QiyuInput.Controller.LTouch,
                QiyuInput.VirtualButton.Test2,
                QiyuInput.ButtonAction.None,
                -1,
                 ButtonAlgo_CombinationKey.CombinationKeyRelation.Or,
                 VirtualButtonWrap.GetVirtualButtonKey(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Home, QiyuInput.ButtonAction.Down),
                 VirtualButtonWrap.GetVirtualButtonKey(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Down)
                );

            VirtualButtonWrapHelp.ButtonAlgoCombinationKey(
                QiyuInput.Controller.LTouch,
                QiyuInput.VirtualButton.Test3,
                QiyuInput.ButtonAction.None,
                -1,
                 ButtonAlgo_CombinationKey.CombinationKeyRelation.Sequence,
                 VirtualButtonWrap.GetVirtualButtonKey(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Home, QiyuInput.ButtonAction.Down),
                 VirtualButtonWrap.GetVirtualButtonKey(QiyuInput.Controller.LTouch, QiyuInput.VirtualButton.Y, QiyuInput.ButtonAction.Down)
                );
        }

        public static void UpdateRawData()
        {
            QiyuXRCorePlugin.QVR_GetControllerData(ref rawControllerDataLeft, ref rawControllerDataRight);
            //左手

            _rawData._state[0]._isConnected = rawControllerDataLeft.isConnect == 1;
            _rawData._state[0]._button = rawControllerDataLeft.button;
            _rawData._state[0]._buttonTouch = rawControllerDataLeft.buttonTouch;
            _rawData._state[0]._batteryLevel = rawControllerDataLeft.batteryLevel;
            _rawData._state[0]._triggerForce = rawControllerDataLeft.triggerForce;
            _rawData._state[0]._gripForce = rawControllerDataLeft.gripForce;
            _rawData._state[0].isShow = rawControllerDataLeft.isShow == 1;

            rawControllerDataLeft.joyStickPos.WirteToVector2(ref _rawData._state[0]._joyStickPos);
            rawControllerDataLeft.position.WirteToVector3(ref _rawData._state[0]._position);
            rawControllerDataLeft.rotation.WriteToQuaternion(ref _rawData._state[0]._rotation);
            rawControllerDataLeft.velocity.WirteToVector3(ref _rawData._state[0]._velocity);
            rawControllerDataLeft.acceleration.WirteToVector3(ref _rawData._state[0]._acceleration);
            rawControllerDataLeft.angVelocity.WirteToVector3(ref _rawData._state[0]._angVelocity);
            rawControllerDataLeft.angAcceleration.WirteToVector3(ref _rawData._state[0]._angAcceleration);

            //右手
            _rawData._state[1]._isConnected = rawControllerDataRight.isConnect == 1;
            _rawData._state[1]._button = rawControllerDataRight.button;
            _rawData._state[1]._buttonTouch = rawControllerDataRight.buttonTouch;
            _rawData._state[1]._batteryLevel = rawControllerDataRight.batteryLevel;
            _rawData._state[1]._triggerForce = rawControllerDataRight.triggerForce;
            _rawData._state[1]._gripForce = rawControllerDataRight.gripForce;
            _rawData._state[1].isShow = rawControllerDataRight.isShow == 1;

            rawControllerDataRight.joyStickPos.WirteToVector2(ref _rawData._state[1]._joyStickPos);
            rawControllerDataRight.position.WirteToVector3(ref _rawData._state[1]._position);
            rawControllerDataRight.rotation.WriteToQuaternion(ref _rawData._state[1]._rotation);
            rawControllerDataRight.velocity.WirteToVector3(ref _rawData._state[1]._velocity);
            rawControllerDataRight.acceleration.WirteToVector3(ref _rawData._state[1]._acceleration);
            rawControllerDataRight.angVelocity.WirteToVector3(ref _rawData._state[1]._angVelocity);
            rawControllerDataRight.angAcceleration.WirteToVector3(ref _rawData._state[1]._angAcceleration);
            //_rawData.DebugShow();
        }

        public static void Update()
        {
#if UNITY_EDITOR
            EditorDebug();
#else
            UpdateRawData();
#endif
        }

        static void EditorDebug()
        {
            if (UnityEngine.Input.GetKey(KeyCode.F1))
            {
                _rawData._state[1]._isConnected = true;
                _rawData._state[1].isShow = true;
            }

            UpdateSimulatedSensor();
        }

        static float mouseX = 0;
        static float mouseY = 0;
        static float mouseZ = 0;
        static void UpdateSimulatedSensor()
        {
            if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
            {
                mouseX += UnityEngine.Input.GetAxis("Mouse X") * 5;
                if (mouseX <= -180)
                {
                    mouseX += 360;
                }
                else if (mouseX > 180)
                {
                    mouseX -= 360;
                }
                mouseY -= UnityEngine.Input.GetAxis("Mouse Y") * 2.4f;
                mouseY = Mathf.Clamp(mouseY, -80, 80);
            }
            else if (UnityEngine.Input.GetKey(KeyCode.RightShift))
            {
                mouseZ += UnityEngine.Input.GetAxis("Mouse X") * 5;
                mouseZ = Mathf.Clamp(mouseZ, -80, 80);
            }
            _rawData._state[1]._rotation = Quaternion.Euler(mouseY, mouseX, mouseZ);
        }

        #region 数据获取
        public static bool GetIsConnected(DEVICE_ID deviceId)
        {
            if (deviceId == DEVICE_ID.HAND_0)
            {
                return _rawData._state[0]._isConnected;
            }
            else
            {
                return _rawData._state[1]._isConnected;
            }
        }

        public static bool GetIsShow(DEVICE_ID deviceId)
        {
            if (deviceId == DEVICE_ID.HAND_0)
            {
                return _rawData._state[0].isShow;
            }
            else
            {
                return _rawData._state[1].isShow;
            }
        }

        //-------------------------原始数据--------------------------

        public static bool GetRawDown(DEVICE_ID deviceId, QiyuInput.Button button)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return false;
            if (button == QiyuInput.Button.Trigger)
            {
                return GetRawTriggerForce(deviceId) >= 2;
            }
            else if (button == QiyuInput.Button.Grip)
            {
                return GetRawGripForce(deviceId) >= 5;
            }
            return (_rawData._state[(int)deviceId - 1]._button & (int)button) != 0;
        }
        public static bool GetRawTouch(DEVICE_ID deviceId, QiyuInput.Button button)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return false;
            return (_rawData._state[(int)deviceId - 1]._buttonTouch & (int)button) != 0;
        }
        public static Vector2 GetRawjoyStickPos(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return Vector2.zero;
            return _rawData._state[(int)deviceId - 1]._joyStickPos;
        }
        public static int GetRawTriggerForce(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return 0;
            return _rawData._state[(int)deviceId - 1]._triggerForce;
        }
        public static int GetRawGripForce(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return 0;
            return _rawData._state[(int)deviceId - 1]._gripForce;
        }
        public static int GetRawBatteryLevel(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return 0;
            return _rawData._state[(int)deviceId - 1]._batteryLevel;
        }
        public static Vector3 GetRawPosition(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return Vector3.zero;
            return _rawData._state[(int)deviceId - 1]._position;
        }
        public static Quaternion GetRawRotation(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return Quaternion.identity;
            return _rawData._state[(int)deviceId - 1]._rotation;
        }

        public static Vector3 GetRawVelocity(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return Vector3.zero;
            return _rawData._state[(int)deviceId - 1]._velocity;
        }

        public static Vector3 GetRawAcc(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return Vector3.zero;
            return _rawData._state[(int)deviceId - 1]._acceleration;
        }
        public static Vector3 GetRawAngVelocity(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return Vector3.zero;
            return _rawData._state[(int)deviceId - 1]._angVelocity;
        }

        public static Vector3 GetRawAngAcc(DEVICE_ID deviceId)
        {
            if (deviceId != DEVICE_ID.HAND_0 && deviceId != DEVICE_ID.HAND_1) return Vector3.zero;
            return _rawData._state[(int)deviceId - 1]._angAcceleration;
        }

        #endregion
    }

    public class HeadDeviceData
    {
        public static bool _gazeDown;
        public static void Register()
        {
            VirtualButtonWrapHelp.Constructor_Event(QiyuInput.Controller.HeadDevice, QiyuInput.VirtualButton.Gaze, QiyuInput.ButtonAction.Down, true, () => { return _gazeDown; }, false);
        }
    }
}