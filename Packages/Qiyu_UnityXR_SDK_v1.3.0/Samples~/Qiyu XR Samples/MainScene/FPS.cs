using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class FPS : MonoBehaviour
{
    private Text textField;
    //  private float fps = 60;

    private float m_LastUpdateShowTime = 0f;
    private float m_UpdateShowDeltaTime = 0.2f;
    private int m_FrameUpdate = 0;
    private float m_FPS = 0;

    void Awake()
    {
        textField = this.GetComponent<Text>();
        if (textField == null)
        {
            textField = transform.Find("Text").GetComponent<Text>();
        }
    }

    void Start()
    {
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if (QiyuManager.Instance != null)
        {
            transform.forward = QiyuManager.Instance.Head.forward;
            transform.position = QiyuManager.Instance.Head.position + QiyuManager.Instance.Head.forward.normalized;
        }
    }

    void LateUpdate()
    {
        if (QiyuManager.Instance == null)
            return;

        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
        {
            m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
        }

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        QiyuXRInput.GetButtonValue(CommonUsages.devicePosition, out pos);
        QiyuXRInput.GetButtonValue(CommonUsages.deviceRotation, out rot);
        textField.text = string.Format("fps:{0}\nHeadEuler:{1}\nHeadPos:{2}\nConEuler:{3}\nConPos:{4}",
            Mathf.RoundToInt(m_FPS),
            QiyuManager.Instance.Head.eulerAngles.ToString("f3"),
            QiyuManager.Instance.Head.position.ToString("f3"),
            rot.eulerAngles.ToString("f3"),
            pos.ToString("f3"));
    }
}
