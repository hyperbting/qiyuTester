using Qiyu.Sdk.Platform;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlatformTest : MonoBehaviour
{
    const string TAG = "PlatformTest";

    public Text uid;
    public Text nickName;
    public Text pic;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize SDK Just Once
        QiyuXRPlatform.InitQiyuSDK(QiyuMessage.GetRequestResult<QiyuMessage.SDKInit>((msg) =>
        {
            if (msg.IsSuccess())
            {
                Debug.Log(TAG + $" InitQiyuSDK OK!");
            }
            else
            {
                Debug.Log(TAG + $" InitQiyuSDK Failed! code:{msg.code}");
            }
        }),
        "", //AppId 从开发者后台的APP页面获取 例:23068292
        ""); //App秘钥 从开发者后台的APP页面获取 例:d4093dadce6fa082d2727ebd7bb9eab1

        //Get Deeplink Param
        QiyuXRPlatform.GetDeepLink(QiyuMessage.GetRequestResult<QiyuMessage.DeepLinkParam>(OnGetDeepLink));
    }

    void OnGetDeepLink(QiyuMessage.MessageResult<QiyuMessage.DeepLinkParam> msg)
    {
        if (msg.IsSuccess() && msg.data != null)
        {
            Debug.Log(string.Format(TAG + " OnGetDeepLink key is {0},value is {1}", msg.data.key, msg.data.value));
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == false)
        {
            QiyuXRPlatform.GetDeepLink(QiyuMessage.GetRequestResult<QiyuMessage.DeepLinkParam>(OnGetDeepLink));
        }
    }

    public void GoHomeLogin()
    {
        QiyuXRPlatform.LaunchHome("login", "");
    }

    // Get Qiyu Account information
    public void GetQiyuAccountInfo()
    {
        if (QiyuXRPlatform.IsQiyuAccountLogin())
        {
            QiyuXRPlatform.GetQiyuAccountInfo(QiyuMessage.GetRequestResult<QiyuMessage.QiyuAccountInfo>((msg) =>
            {
                if (msg.IsSuccess())
                {
                    uid.text = msg.data.uid;
                    nickName.text = msg.data.name;
                    pic.text = msg.data.icon;
                    Debug.Log(string.Format(TAG + " GetQiyuAccountInfo uid is {0},name is {1}", msg.data.uid, msg.data.name));
                }
            }));
        }
        else
        {
            //跳转到Home进行登录
            QiyuXRPlatform.LaunchHome("login", "");
        }
    }

    public void OpenApp()
    {
        string appID = "";
        if (string.IsNullOrEmpty(appID))
        {
            appID = "65302330";
        }
        QiyuXRPlatform.LaunchOtherApp(appID, "key", "value");
    }

    private void TestQiyuPrefs()
    {
        //save data test
        Debug.Log("QiyuPrefs GetInt:" + QiyuPrefs.GetInt("Int"));
        Debug.Log("QiyuPrefs GetString:" + QiyuPrefs.GetString("String"));
        Debug.Log("QiyuPrefs GetFloat:" + QiyuPrefs.GetFloat("Float"));
        Debug.Log("QiyuPrefs HasKey:" + QiyuPrefs.HasKey("Float"));
        QiyuPrefs.SetInt("Int", 1);
        QiyuPrefs.SetString("String", "12");
        QiyuPrefs.SetFloat("Float", 0.333f);
        Debug.Log("QiyuPrefs GetInt:" + QiyuPrefs.GetInt("Int"));
        Debug.Log("QiyuPrefs GetString:" + QiyuPrefs.GetString("String"));
        Debug.Log("QiyuPrefs GetFloat:" + QiyuPrefs.GetFloat("Float"));
        QiyuPrefs.DeleteKey("Int");
        QiyuPrefs.Save();
    }

    public void OnClickButton(GameObject btn)
    {
        if (btn.name == "GetAccountInfo")
        {
            GetQiyuAccountInfo();
        }
        else if (btn.name == "GoHomeLogin")
        {
            GoHomeLogin();
        }
        else if (btn.name == "OpenApp")
        {
            OpenApp();
        }
        else if (btn.name == "TestQiyuPrefs")
        {
            TestQiyuPrefs();
        }
        else if (btn.name == "GoQiyuPayTest")
        {
            SceneManager.LoadScene("QiyuPayTest");
        }
    }
}
