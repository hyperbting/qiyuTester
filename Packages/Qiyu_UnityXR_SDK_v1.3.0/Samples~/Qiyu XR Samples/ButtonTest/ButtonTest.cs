using System.Collections.Generic;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.XR;

public class ButtonTest : MonoBehaviour
{
    InputDevice contrllerR;
    InputDevice headController;
    InputDevice contrllerL;

    public GameObject dbugGameObj;
    Dictionary<string, DebugText> DebugText_Dic = new Dictionary<string, DebugText>();

    public Transform headInfoRoot;
    public Transform leftControllerInfoRoot;
    public Transform rightControllerInfoRoot;

    public void Start()
    {        
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);
        var usage = new List<InputFeatureUsage>();

        foreach (var device in inputDevices)
        {
            Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));

            if (device.role == InputDeviceRole.RightHanded)
            {
                contrllerR = device;
            }
            else if (device.role == InputDeviceRole.Generic)
            {
                headController = device;
            }
            if (device.role == InputDeviceRole.LeftHanded)
            {
                contrllerL = device;
            }
        }
    }

    public void AddDebugText(string name, string info, Transform root)
    {
        if (DebugText_Dic.ContainsKey(name))
        {
            DebugText_Dic[name].SetText(info);
        }
        else
        {
            GameObject obj = Instantiate(dbugGameObj);
            obj.transform.SetParent(root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.EulerRotation(Vector3.zero);
            obj.name = name;

            DebugText debugShow = obj.GetComponent<DebugText>();
            debugShow.SetText(info);
            DebugText_Dic.Add(name, debugShow);
        }
    }

    bool lastState_trigger = false;
    private void Update()
    {
        //down和up触发一次的用例
        if (contrllerR.TryGetFeatureValue(CommonUsages.triggerButton, out var curState_trigger))
        {
            if (lastState_trigger != curState_trigger)
            {
                if (curState_trigger)
                {
                    Debug.Log("trigger down!");
                }
                else
                {
                    Debug.Log("trigger up!");
                }
            }
            lastState_trigger = curState_trigger;
        }

        //头部信息
        ShowHeadInfo(headController, headInfoRoot);
        //手柄信息
        ShowControllerInfo(contrllerL, leftControllerInfoRoot);
        ShowControllerInfo(contrllerR, rightControllerInfoRoot);
        //速度相关包括速度，加速度，角速度等
        ShowSensorState(contrllerL, leftControllerInfoRoot);
        ShowSensorState(contrllerR, rightControllerInfoRoot);
        //柄键值相关
        ShowButtonState(contrllerL, leftControllerInfoRoot);
        ShowButtonState(contrllerR, rightControllerInfoRoot);
    }

    void ShowHeadInfo(InputDevice head, Transform uiRoot)
    {
        string hand_name = "Head";
        AddDebugText($"{hand_name}_isValid", head.isValid.ToString(), uiRoot);
        AddDebugText($"{hand_name}_name", head.name, uiRoot);
        AddDebugText($"{hand_name}_serialNumber", head.serialNumber, uiRoot);
        AddDebugText($"{hand_name}_manufacturer", head.manufacturer, uiRoot);
        AddDebugText($"{hand_name}_characteristics", head.characteristics.ToString(), uiRoot);
        if (head.TryGetFeatureValue(CommonUsages.devicePosition, out var devicePosition))
        {
            AddDebugText($"{hand_name}_devicePosition", devicePosition.ToString(), uiRoot);
        }
        if (head.TryGetFeatureValue(CommonUsages.deviceRotation, out var deviceRotation))
        {
            AddDebugText($"{hand_name}_deviceRotation", deviceRotation.ToString(), uiRoot);
        }
        AddDebugText($"{hand_name}_refreshRate", XRDevice.refreshRate.ToString(), uiRoot);

    }

    void ShowControllerInfo(InputDevice controller, Transform uiRoot)
    {
        string hand_name = controller.role.ToString();
        AddDebugText($"{hand_name}_isValid", controller.isValid.ToString(), uiRoot);
        AddDebugText($"{hand_name}_name", controller.name, uiRoot);
        AddDebugText($"{hand_name}_serialNumber", controller.serialNumber, uiRoot);
        AddDebugText($"{hand_name}_manufacturer", controller.manufacturer, uiRoot);
        AddDebugText($"{hand_name}_characteristics", controller.characteristics.ToString(), uiRoot);
        if (controller.TryGetFeatureValue(CommonUsages.batteryLevel, out var batteryLevel))
        {
            AddDebugText($"{hand_name}_batteryLevel", batteryLevel.ToString(), uiRoot);
        }
    }

    void ShowSensorState(InputDevice controller, Transform uiRoot)
    {
        string hand_name = controller.role.ToString();
        if (controller.TryGetFeatureValue(CommonUsages.devicePosition, out var devicePosition))
        {
            AddDebugText($"{hand_name}_devicePosition", devicePosition.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.deviceRotation, out var deviceRotation))
        {
            AddDebugText($"{hand_name}_deviceRotation", deviceRotation.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.deviceVelocity, out var deviceVelocity))
        {
            AddDebugText($"{hand_name}_deviceVelocity", deviceVelocity.ToString("F2"), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out var deviceAngularVelocity))
        {
            AddDebugText($"{hand_name}_deviceAngularVelocity", deviceAngularVelocity.ToString("F2"), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.deviceAcceleration, out var deviceAcceleration))
        {
            AddDebugText($"{hand_name}_deviceAcceleration", deviceAcceleration.ToString("F2"), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out var deviceAngularAcceleration))
        {
            AddDebugText($"{hand_name}_deviceAngularAcceleration", deviceAngularAcceleration.ToString("F2"), uiRoot);
        }
    }

    void ShowButtonState(InputDevice controller, Transform uiRoot)
    {
        string hand_name = controller.role.ToString();
        if (controller.TryGetFeatureValue(CommonUsages.grip, out var grip))
        {
            AddDebugText($"{hand_name}_grip", grip.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.gripButton, out var gripButton))
        {
            AddDebugText($"{hand_name}_gripButton", gripButton.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out var menuButton))
        {
            AddDebugText($"{hand_name}_menuButton", menuButton.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out var primary2DAxis))
        {
            AddDebugText($"{hand_name}_primary2DAxis", primary2DAxis.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var primary2DAxisClick))
        {
            AddDebugText($"{hand_name}_primary2DAxisClick", primary2DAxisClick.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out var primary2DAxisTouch))
        {
            AddDebugText($"{hand_name}_primary2DAxisTouch", primary2DAxisTouch.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.primaryButton, out var primaryButton))
        {
            AddDebugText($"{hand_name}_primaryButton", primaryButton.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.primaryTouch, out var primaryTouch))
        {
            AddDebugText($"{hand_name}_primaryTouch", primaryTouch.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.secondaryButton, out var secondaryButton))
        {
            AddDebugText($"{hand_name}_secondaryButton", secondaryButton.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.secondaryTouch, out var secondaryTouch))
        {
            AddDebugText($"{hand_name}_secondaryTouch", secondaryTouch.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.trigger, out var trigger))
        {
            AddDebugText($"{hand_name}_trigger", trigger.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerButton))
        {
            AddDebugText($"{hand_name}_triggerButton", triggerButton.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(QiyuUsages.triggerTouch, out var triggerTouch))
        {
            AddDebugText($"{hand_name}_triggerTouch", triggerTouch.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.thumbTouch, out var thumbTouch))
        {
            AddDebugText($"{hand_name}_thumbTouch", thumbTouch.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.thumbrest, out var thumbrest))
        {
            AddDebugText($"{hand_name}_thumbrest", thumbrest.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.trackingState, out var trackingState))
        {
            AddDebugText($"{hand_name}_trackingState", trackingState.ToString(), uiRoot);
        }
        if (controller.TryGetFeatureValue(CommonUsages.isTracked, out var isTracked))
        {
            AddDebugText($"{hand_name}_isTracked", isTracked.ToString(), uiRoot);
        }
    }
}
