using Qiyu.Sdk.Platform;
using System.Collections.Generic;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QiyuPayTest : MonoBehaviour
{
    const string TAG = "QiyuPayTest";
    public Text textField;
    public Dropdown dropdown_sku;
    public Dropdown dropdown_orderId;

    private void Start()
    {
        //Need init first when use other pay API.
        QiyuPay.InitQiyuPay(QiyuMessage.GetRequestResult<QiyuMessage.SDKInit>((msg) =>
        {
            if (msg.IsSuccess())
            {
                Debug.Log(TAG + $" InitQiyuPay OK!");
                textField.text = $"InitQiyuPay OK!";
            }
            else
            {
                Debug.Log(TAG + $" InitQiyuPay Failed! code:{msg.code}");
                textField.text = $"InitQiyuPay Failed! code:{msg.code}";
            }
        }));

        dropdown_sku.ClearOptions();
        dropdown_sku.options.Add(new Dropdown.OptionData("sku_test"));
        dropdown_sku.RefreshShownValue();

        dropdown_orderId.ClearOptions();
        dropdown_orderId.options.Add(new Dropdown.OptionData("11111111"));
        dropdown_orderId.RefreshShownValue();
    }

    public void OnClickButton(GameObject btn)
    {
        if (btn.name == "BackToSample ")
        {
            SceneManager.LoadScene("PlatformTest");
        }
        else if (btn.name == "GetSkuList")
        {
            string text = "";
            QiyuPay.GetSkuList(QiyuMessage.GetRequestResult<List<QiyuMessage.QiyuPaySkuInfo>>((msg) =>
            {
                if (msg.data != null)
                {
                    dropdown_sku.ClearOptions();
                    foreach (var info in msg.data)
                    {
                        text += info.ToString() + "\n";
                        dropdown_sku.options.Add(new Dropdown.OptionData(info.sku));
                    }
                    dropdown_sku.RefreshShownValue();
                    textField.text = $"GetSkuList->\n{text}";
                    Debug.Log(TAG + " GetSkuList->\n" + text);
                }
                else
                {
                    textField.text = $"GetSkuList Error:{msg.code}-{msg.message}";
                }
            }));
        }
        else if (btn.name == "PlaceOrder")
        {
            QiyuPay.PlaceOrder(QiyuMessage.GetRequestResult<string>((msg) =>
            {
                Debug.Log(TAG + $" PlaceOrder code:{msg.code},orderId:{msg.data}");
                if (msg.data != null)
                {
                    dropdown_orderId.ClearOptions();
                    dropdown_orderId.options.Add(new Dropdown.OptionData(msg.data));
                    dropdown_orderId.RefreshShownValue();

                    textField.text = $"PlaceOrder->\n{msg.data}";
                }
                else
                {
                    textField.text = $"PlaceOrder Error:{msg.code}-{msg.message}";
                }
            }), dropdown_sku.options[dropdown_sku.value].text);
        }
        else if (btn.name == "QueryOrderResult")
        {
            QiyuPay.QueryOrderResult(QiyuMessage.GetRequestResult<QiyuMessage.QiyuPayOrderResult>((msg) =>
            {
                Debug.Log(TAG + " OrderResult " + msg);
                if (msg.data != null)
                {
                    textField.text = $"OrderResult->\n{msg.data}";
                }
                else
                {
                    textField.text = $"OrderResult Error:{msg.code}-{msg.message}";
                }
            }), dropdown_orderId.options[dropdown_orderId.value].text);
        }
        else if (btn.name == "QueryHistoryOrders")
        {
            string text = "";
            QiyuPay.QueryHistoryOrders(QiyuMessage.GetRequestResult<QiyuMessage.QiyuPayHistoryOrders>((msg) =>
            {
                Debug.Log(TAG + $" QueryHistoryOrders code:{msg.code},count:{msg.data?.count}");
                if (msg.data != null)
                {
                    foreach (var order in msg.data.orders)
                    {
                        text += order.ToString() + "\n";
                    }
                    textField.text = $"History->\n{text}";
                    Debug.Log(TAG + $" History->\n{text}");
                }
                else
                {
                    textField.text = $"History Error:{msg.code}-{msg.message}";
                }
            }), dropdown_sku.options[dropdown_sku.value].text);
        }
    }
}
