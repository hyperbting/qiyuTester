//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;
using System.Text;
using UnityEngine;

namespace Unity.XR.Qiyu
{
    public interface IMonitor
    {
        void Update();
        void AppendToStringBuilder(ref StringBuilder sb);
        string GetName();

        bool IsTriggerEvent();

        float GetDurationTime();
    }

    /// <summary>
    /// 变量监听
    /// </summary>
    public class QiyuVariableMonitor<T> : IMonitor where T : IEquatable<T> 
    {
        public delegate T VariableGenerator();
        public delegate void EventPublisher();
        public delegate void ChangePublisher(T val);

        private bool bEventPublish;
        private bool bChangePublish;
        private bool bChangePublishTime;
        private VariableGenerator m_generator;
        private EventPublisher m_eventPublisher;
        private EventPublisher m_eventPublisherTime;
        private ChangePublisher m_changePublisher;

        private string m_name = "";
        private bool m_currentValueChange = false;

        private T m_prevValue = default(T);
        private T m_currentValue = default(T);
        private T m_triggerValue = default(T);

        private float _beginTime;
        private float _time;

        public T CurrentValue
        {
            get
            {
                return m_currentValue;
            }
        }

        public QiyuVariableMonitor(string name, VariableGenerator generator, T initValue = default(T))
        {
            m_name = name;
            m_generator = generator;
            bEventPublish = false;
            bChangePublish = false;
            bChangePublishTime = false;
            m_eventPublisher = null;
            m_changePublisher = null;
            m_triggerValue = default(T);
            m_currentValue = initValue;
            _beginTime = 0;
        }

        public QiyuVariableMonitor<T> AddEventPublisher(T t, EventPublisher epub)
        {
            bEventPublish = true;
            m_triggerValue = t;
            m_eventPublisher = epub;
            return this;
        }

        public QiyuVariableMonitor<T> AddChangePublisher(ChangePublisher cpub)
        {
            bChangePublish = true;
            m_changePublisher = cpub;
            return this;
        }

        public QiyuVariableMonitor<T> AddEventPublisherTime(T t, float time, EventPublisher epub)
        {
            bChangePublishTime = true;
            m_triggerValue = t;
            _time = time;
            m_eventPublisherTime = epub;
            return this;
        }

        public void Update()
        {
            m_prevValue = m_currentValue;
            m_currentValue = m_generator();

            if (!m_currentValue.Equals(m_prevValue))
            {              
                if (bEventPublish && m_currentValue.Equals(m_triggerValue))
                {
                    _beginTime = Time.realtimeSinceStartup;
                    m_currentValueChange = true;
                    m_eventPublisher?.Invoke();
                }

                if (bChangePublish)
                {
                    m_changePublisher?.Invoke(m_currentValue);
                }
            }
            else {
                m_currentValueChange = false;
            }

            if (bChangePublishTime && m_currentValue.Equals(m_triggerValue)
                && (GetDurationTime() > _time))
            {
                _beginTime = Time.realtimeSinceStartup;
                m_eventPublisherTime?.Invoke();
            }

        }

        public void AppendToStringBuilder(ref StringBuilder sb)
        {
            sb.Append(m_name)
                .Append(": ")
                .Append(m_currentValue)
                .Append(m_currentValueChange ? "*" : "")
                .Append("\n");
        }

        public string GetName()
        {
            return m_name;
        }

        public bool IsTriggerEvent()
        {
            return m_currentValueChange;
        }

        public float GetDurationTime()
        {
            return Time.realtimeSinceStartup - _beginTime;
        }
    }
}
