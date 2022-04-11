//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace Unity.XR.Qiyu
{
    public class QiyuControllerVisual : QiyuVisualBase
    {
        public static float COLOR_FACTOR_ON = 0.7f;
        public static float GRIP_FORCE_LIMIT_R = -7.0f;
        public static float GRIP_FORCE_LIMIT_L = 7.0f;
        public static float TRIGGER_FORCE_LIMIT = 15;
        public static Vector3 BUTTON_DOWN_POS1 = new Vector3(0, -0.02f, 0);
        public static Vector3 BUTTON_DOWN_POS2 = new Vector3(0, -0.01f, 0);
        public static float JOYSTICK_ANGLE_LIMIT = 20.0f;

        private Transform handleTrans;
        public MeshRenderer bodyMR;

        public Transform homeTrans;
        public MeshRenderer homeMR;

        public Transform aTrans;
        public MeshRenderer aMR;

        public Transform bTrans;
        public MeshRenderer bMR;

        public Transform gripTrans;
        public MeshRenderer gripMR;

        public Transform triggerTrans;
        public MeshRenderer triggerMR;

        public Transform joyStickTrans;
        public MeshRenderer joyStickMR;

        public Material matNormal;

        void Awake()
        {
            handleTrans = this.transform;

            Transform handle = handleTrans.Find("Handle");
            bodyMR = handle.Find("Body").GetComponent<MeshRenderer>();

            homeTrans = handle.Find("BtnHome/BtnHome");
            homeMR = homeTrans.GetComponent<MeshRenderer>();

            aTrans = handle.Find("BtnA/BtnA");
            aMR = aTrans.GetComponent<MeshRenderer>();

            bTrans = handle.Find("BtnB/BtnB");
            bMR = bTrans.GetComponent<MeshRenderer>();

            gripTrans = handle.Find("BtnGrip/BtnGrip");
            gripMR = gripTrans.GetComponent<MeshRenderer>();

            triggerTrans = handle.Find("BtnTrigger/BtnTrigger");
            triggerMR = triggerTrans.GetComponent<MeshRenderer>();

            joyStickTrans = handle.Find("BtnJoyStick/BtnJoyStick");
            joyStickMR = joyStickTrans.GetComponent<MeshRenderer>();

            InitMats();
        }

        private void OnEnable()
        {
            Debug.Log("[qc][Visual] OnEnable");
        }

        private void OnDisable()
        {
            Debug.Log("[qc][Visual] OnDisable");
            ResetButtonOff();
        }

        void ResetButtonOff()
        {
            SetBBtnOff();
            SetABtnOff();
            SetGripOff();
            SetTriggerOff();
            SetHomeBtnOff();
        }

        void InitMats()
        {
            aMR.materials[0] = matNormal;
            bMR.materials[0] = matNormal;
            triggerMR.materials[0] = matNormal;
            joyStickMR.materials[0] = matNormal;
            gripMR.materials[0] = matNormal;
            homeMR.materials[0] = matNormal;
        }

        [ContextMenu("SetABtnOn")]
        public void SetABtnOn()
        {
            Debug.Log("[qc][Visual]SetAppBtnOn");
            aMR.sharedMaterial.SetFloat("_Type", COLOR_FACTOR_ON);
            aTrans.localPosition = BUTTON_DOWN_POS1;
        }

        [ContextMenu("SetABtnOff")]
        public void SetABtnOff()
        {
            Debug.Log("[qc][Visual]SetAppBtnOff");
            aMR.sharedMaterial.SetFloat("_Type", 1);
            aTrans.localPosition = Vector3.zero;
        }

        [ContextMenu("SetBBtnOn")]
        public void SetBBtnOn()
        {
            Debug.Log("[qc][Visual]SetAppBtnOn");
            bMR.sharedMaterial.SetFloat("_Type", COLOR_FACTOR_ON);
            bTrans.localPosition = BUTTON_DOWN_POS1;
        }

        [ContextMenu("SetBBtnOff")]
        public void SetBBtnOff()
        {
            Debug.Log("[qc][Visual]SetAppBtnOff");
            bMR.sharedMaterial.SetFloat("_Type", 1);
            bTrans.localPosition = Vector3.zero;
        }

        [ContextMenu("SetGripOn")]
        public void SetGripOn()
        {
            Debug.Log("[qc][Visual]SetGripOn");
            gripMR.sharedMaterial.SetFloat("_Type", COLOR_FACTOR_ON);
        }

        [ContextMenu("SetGripOff")]
        public void SetGripOff()
        {
            Debug.Log("[qc][Visual]SetGripOff");
            gripMR.sharedMaterial.SetFloat("_Type", 1);
        }

        [ContextMenu("SetHomeBtnOn")]
        public void SetHomeBtnOn()
        {
            Debug.Log("[qc][Visual]SetHomeBtnOn");
            homeMR.sharedMaterial.SetFloat("_Type", COLOR_FACTOR_ON);
            homeTrans.localPosition = BUTTON_DOWN_POS2;
        }

        [ContextMenu("SetHomeBtnOff")]
        public void SetHomeBtnOff()
        {
            Debug.Log("[qc][Visual]SetHomeBtnOff");
            homeMR.sharedMaterial.SetFloat("_Type", 1);
            homeTrans.localPosition = Vector3.zero;
        }

        [ContextMenu("SetTriggerOn")]
        public void SetTriggerOn()
        {
            Debug.Log("[qc][Visual]SetTriggerOn");
            triggerMR.sharedMaterial.SetFloat("_Type", COLOR_FACTOR_ON);
        }

        [ContextMenu("SetTriggerOff")]
        public void SetTriggerOff()
        {
            Debug.Log("[qc][Visual]SetTriggerOff");
            triggerMR.sharedMaterial.SetFloat("_Type", 1);
        }

        [ContextMenu("SetJoyStickOn")]
        public void SetJoyStickOn()
        {
            Debug.Log("[qc][Visual]SetTriggerOn");
            joyStickMR.sharedMaterial.SetFloat("_Type", COLOR_FACTOR_ON);
            joyStickTrans.localPosition = BUTTON_DOWN_POS1;
        }

        [ContextMenu("SetJoyStickOff")]
        public void SetJoyStickOff()
        {
            Debug.Log("[qc][Visual]SetTriggerOff");
            joyStickMR.sharedMaterial.SetFloat("_Type", 1);
            joyStickTrans.localPosition = Vector3.zero;
        }

        public void LateUpdate()
        {
            if (QiyuXRInput.GetButtonAction(CommonUsages.menuButton, out var menuButton, _hand))
            {
                if (menuButton)
                {
                    SetHomeBtnOn();
                }
                else
                {
                    SetHomeBtnOff();
                }
            }
            if (QiyuXRInput.GetButtonAction(CommonUsages.triggerButton, out var triggerButton, _hand))
            {
                if (triggerButton)
                {
                    SetTriggerOn();
                }
                else
                {
                    SetTriggerOff();
                }
            }
            if (QiyuXRInput.GetButtonAction(CommonUsages.gripButton, out var gripButton, _hand))
            {
                if (gripButton)
                {
                    SetGripOn();
                }
                else
                {
                    SetGripOff();
                }
            }
            if (QiyuXRInput.GetButtonAction(CommonUsages.primaryButton, out var primaryButton, _hand))
            {
                if (primaryButton)
                {
                    SetABtnOn();
                }
                else
                {
                    SetABtnOff();
                }
            }
            if (QiyuXRInput.GetButtonAction(CommonUsages.secondaryButton, out var secondaryButton, _hand))
            {
                if (secondaryButton)
                {
                    SetBBtnOn();
                }
                else
                {
                    SetBBtnOff();
                }
            }

            QiyuXRInput.GetButtonValue(CommonUsages.trigger, out var trigger, _hand);
            QiyuXRInput.GetButtonValue(CommonUsages.grip, out var grip, _hand);
            SetTriggerForce(trigger);
            SetGripForce(grip);

            SetJoyStick();
        }

        public void SetTriggerForce(float force)
        {
            float factor = force;
            float value = Mathf.Lerp(0, TRIGGER_FORCE_LIMIT, factor);
            triggerTrans.localEulerAngles = new Vector3(value, 0, 0);
        }

        public void SetGripForce(float force)
        {
            float factor = force;
            float value = Mathf.Lerp(0, _hand == QiyuHand.L ? GRIP_FORCE_LIMIT_L : GRIP_FORCE_LIMIT_R, factor);
            gripTrans.localEulerAngles = new Vector3(0, value, 0);
        }

        public void SetJoyStick()
        {
            if (QiyuXRInput.GetButtonAction(CommonUsages.primary2DAxisClick, out var isDown, _hand))
            {
                if (isDown)
                {
                    joyStickMR.sharedMaterial.SetFloat("_Type", COLOR_FACTOR_ON);
                    joyStickTrans.localPosition = BUTTON_DOWN_POS1;
                }
                else
                {
                    joyStickMR.sharedMaterial.SetFloat("_Type", 1);
                    joyStickTrans.localPosition = Vector3.zero;
                }
            }

            QiyuXRInput.GetButtonValue(CommonUsages.primary2DAxis, out var pos, _hand);
            float factorXAngle = (pos.y + 1.0f) / 2.0f;
            factorXAngle = factorXAngle < 0 ? 0 : factorXAngle;
            float xAnagle = Mathf.Lerp(-JOYSTICK_ANGLE_LIMIT, JOYSTICK_ANGLE_LIMIT, factorXAngle);

            float factorZAngle = (pos.x + 1.0f) / 2.0f;
            factorZAngle = factorZAngle < 0 ? 0 : factorZAngle;
            float zAnagle = Mathf.Lerp(JOYSTICK_ANGLE_LIMIT, -JOYSTICK_ANGLE_LIMIT, factorZAngle);
            joyStickTrans.localEulerAngles = new Vector3(xAnagle, 0, zAnagle);
        }
    }
}