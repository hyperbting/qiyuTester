using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.UI;

public class ChangeQXRManager : MonoBehaviour
{
    public Text trackingPosition;

    private void Start()
    {
        //trackingPosition.text = string.Format("TrackingPosition({0})", QiyuManager.Instance.trackingPosition);
    }
    public void OnClickButton(GameObject btn)
    {
        if (btn.name == "TrackingPosition")
        {
            //QiyuManager.Instance.trackingPosition = !QiyuManager.Instance.trackingPosition;
            //trackingPosition.text = string.Format("TrackingPosition({0})", QiyuManager.Instance.trackingPosition);
            //Utils.SetTrackingPosition(QiyuManager.Instance.trackingPosition);
            //var drivers = GameObject.FindObjectsOfType<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            //foreach (var driver in drivers)
            //{
            //    driver.trackingType = QiyuManager.Instance.trackingPosition ? UnityEngine.InputSystem.XR.TrackedPoseDriver.TrackingType.RotationAndPosition : UnityEngine.InputSystem.XR.TrackedPoseDriver.TrackingType.RotationOnly;
            //}
        }
    }
}
