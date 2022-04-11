using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class QiyuXRController : XRController
{
    string headControllerName;
    public GameObject Qiyu3_HandControllerPrefab;
    public GameObject QiyuDream_HandControllerPrefab;

    private XRInteractorLineVisual xRInteractorLine;

    protected override void Awake()
    {
        base.Awake();
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (device.isValid && device.name.Contains("QiyuHMD-3"))
        {
            if (Qiyu3_HandControllerPrefab != null)
                modelPrefab = Qiyu3_HandControllerPrefab.transform;
        }
        else
        {
            if (QiyuDream_HandControllerPrefab != null)
                modelPrefab = QiyuDream_HandControllerPrefab.transform;
        }
        xRInteractorLine = GetComponent<XRInteractorLineVisual>();
    }

    protected override void UpdateController()
    {
        base.UpdateController();
        if (inputDevice.isValid && hideControllerModel)
        {
            hideControllerModel = false;
        }

        if (!inputDevice.isValid && !hideControllerModel)
        {
            hideControllerModel = true;           
        }
        if (xRInteractorLine != null && (xRInteractorLine.enabled^!hideControllerModel))
            xRInteractorLine.enabled = !hideControllerModel;

    }
}
