using Qiyi.UI.InputField;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.UI;

public class MainTest : MonoBehaviour
{
    void Start()
    {

    }

    public void OnApplicationPause(bool pause)
    {

    }

    int trackingMode = 0;
    public void OnButtonClick(GameObject btn)
    {
        if (btn.name == "TrackingMode")
        {
            if (trackingMode == 0)
            {
                trackingMode = 1;
                QiyuXRCore.SetAppTrackingMode(trackingMode);
                btn.GetComponentInChildren<Text>().text = $"3Dof(On)";
            }
            else
            {
                trackingMode = 0;
                QiyuXRCore.SetAppTrackingMode(trackingMode);
                btn.GetComponentInChildren<Text>().text = $"3Dof(Off)";
            }
        }
        else
        {

        }
    }
}
