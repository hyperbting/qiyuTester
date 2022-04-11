using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFoveation : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btn;
    public Text btnText;
    int foveationLevel = -1;
    void Start()
    {
        btn = GetComponent<Button>();
        btnText = transform.Find("Text").GetComponent<Text>();

        if (QiyuManager.Instance == null)
            return;

        foveationLevel = (int)QiyuManager.Instance.foveationLevel;
        ShowTest();
        btn.onClick.AddListener(() =>
        {
            foveationLevel++;
            if (foveationLevel > 2) foveationLevel = -1;
            ShowTest();
            QiyuManager.Instance.foveationLevel = (FoveationLevel)foveationLevel;
            Utils.SetFoveationLevel(foveationLevel);
            Debug.Log("SetFoveationLevel:" + foveationLevel);
        });
    }
    void ShowTest()
    {
        if (foveationLevel == -1)
        {
            btnText.text = "FoveationLevel=None";
        }
        else if (foveationLevel == 0)
        {
            btnText.text = "FoveationLevel=Low";
        }
        else if (foveationLevel == 1)
        {
            btnText.text = "FoveationLevel=Med";
        }
        else if (foveationLevel == 2)
        {
            btnText.text = "FoveationLevel=High";
        }
    }

}
