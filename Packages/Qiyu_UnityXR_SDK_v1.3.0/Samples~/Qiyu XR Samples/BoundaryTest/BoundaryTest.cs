using System.Collections.Generic;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoundaryTest : MonoBehaviour
{
    const string TAG = "BoundaryTest";

    public GameObject primitivePrefab;
    public Text textField;

    QiyuBoundary.BoundaryTestResult result_play;
    List<GameObject> points = new List<GameObject>();

    private void Start()
    {
        if (QiyuPlatform.IsAndroid)
        {
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).subsystem.boundaryChanged += Subsystem_boundaryChanged;
        }
    }

    private void OnDestroy()
    {
        if (QiyuPlatform.IsAndroid)
        {
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).subsystem.boundaryChanged -= Subsystem_boundaryChanged;
        }
    }

    private void ClearPoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            DestroyImmediate(points[i]);
        }
        points.Clear();
        Debug.Log(TAG + " ClearPoints!");
    }

    private void Subsystem_boundaryChanged(UnityEngine.XR.XRInputSubsystem obj)
    {
        Debug.Log(TAG + " XR boundaryChanged!");
        //XR Method Get BoundaryPoints
        List<Vector3> list = new List<Vector3>();
        UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).subsystem.TryGetBoundaryPoints(list);
        CreateGeometry(list.ToArray());
    }

    public void OnClickButton(GameObject btn)
    {
        if (btn.name == "BackToSample ")
        {
            SceneManager.LoadScene("MainScene");
        }
        else if (btn.name == "Visible")
        {
            QiyuBoundary.SetVisible(true);
        }
        else if (btn.name == "InVisible")
        {
            QiyuBoundary.SetVisible(false);
        }
        else if (btn.name == "CreateGeometry")
        {
            CreateGeometry(QiyuBoundary.GetGeometry());
        }
    }

    void CreateGeometry(Vector3[] geometryList)
    {
        ClearPoints();
        float xrRigOffset = 0;
        //update offset
        UnityEngine.XR.Interaction.Toolkit.XRRig rig = FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XRRig>();
        if (rig != null)
        {
            if (rig.trackingOriginMode == UnityEngine.XR.TrackingOriginModeFlags.Device)
            {
                xrRigOffset = rig.cameraYOffset;
            }
            else if (rig.trackingOriginMode == UnityEngine.XR.TrackingOriginModeFlags.Floor)
            {
                xrRigOffset = QiyuBoundary.FloorCameraYOffset;
            }
            Debug.Log(TAG + " update xrRigOffset:" + xrRigOffset);
        }

        for (int i = 0; i < geometryList.Length; i++)
        {
            Vector3 pos = geometryList[i];
            pos.y += xrRigOffset;
            var obj = GameObject.Instantiate(primitivePrefab);
            obj.transform.position = pos;
            obj.transform.localScale = Vector3.one * 0.05f;
            points.Add(obj);
            //Debug.Log(TAG + " GetGeometry--> pos:" + pos.ToString("f4"));
        }
        Debug.Log(TAG + " CreateGeometry:" + geometryList.Length);
    }

    void LateUpdate()
    {
        result_play = QiyuBoundary.TestNode(QiyuBoundary.Node.HandRight);
        textField.text = string.Format("RightHand({8})\n-->IsTriggering:{0}\nClosestDistance:{1}\nClosestPoint:{2}\nClosestPointNormal:{3}\nGetConfigured:{4}\nGetVisible:{5}\nGetDimensions:{6}\nGetFloorLevel:{7}",
            result_play.IsTriggering,
            result_play.ClosestDistance,
            result_play.ClosestPoint.ToString("f3"),
            result_play.ClosestPointNormal.ToString("f3"),
            QiyuBoundary.GetConfigured(),
            QiyuBoundary.GetVisible(),
            QiyuBoundary.GetDimensions().ToString("f3"),
            QiyuXRCore.GetFloorLevel(),
            QiyuInput.GetLocalControllerPosition(QiyuInput.Controller.RTouch).ToString("f2"));

        if (QiyuManager.Instance?.Head != null)
        {
            textField.transform.forward = QiyuManager.Instance.Head.forward;
            textField.transform.position = QiyuManager.Instance.Head.position + QiyuManager.Instance.Head.forward.normalized;
        }

        //Vector3 pos = new Vector3();
        //Quaternion rot = new Quaternion();
        //boundary.GetOriginAixs(ref pos, ref rot);
        //this.aixs.position = pos;
        //this.aixs.rotation = rot;
    }
    //public Transform aixs;
}
