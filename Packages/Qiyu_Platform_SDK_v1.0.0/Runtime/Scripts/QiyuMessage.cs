//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Qiyu.Sdk.Platform
{
    public static class QiyuMessage
    {
        [Preserve]
        public class MessageResult<T>
        {
            public T data;
            public string code;  //返回码
            public string message; //错误信息
            public bool IsSuccess()
            {
                return code.Equals("S0000");
            }

            public string ToJson()
            {
                return QiyuUtils.ParseToJson(this);
            }
        }

        public static RequestCallback GetRequestResult<T>(System.Action<MessageResult<T>> callback)
        {
            return RequestCallbackByJson<MessageResult<T>>.Create((MessageResult<T> msg1) =>
            {
                callback?.Invoke(msg1);
            });
        }

        [Preserve]
        public class SDKInit
        {

        }

        [Preserve]
        public class QiyuAccountInfo
        {
            public string uid;  //用户uid
            public string name; //用户名
            public string icon; //头像地址
        }

        [Preserve]
        public class DeepLinkParam
        {
            public string appID;
            public string key;
            public string value;

            public string ToJson()
            {
                return QiyuUtils.ParseToJson(this);
            }
        }

        [Preserve]
        public class QiyuPaySkuInfo
        {
            //1:持久型,2:消耗型
            public const int TYPE_NON_CONSUMABLE = 1;
            public const int TYPE_CONSUMABLE = 2;
            //10:预发布,20:正常,-1:无效
            public const int STATUS_INVALID = -1;
            public const int STATUS_PRE_RELEASE = 10;
            public const int STATUS_RELEASE = 20;

            public string sku;
            public string productName;
            public int itemType;
            public float originalPrice;//单位元
            public float salePrice;//单位元
            public int status;

            public override string ToString()
            {
                return string.Format("sku:{0},productName:{1},itemType:{2},originalPrice:{3},salePrice:{4},status:{5}", sku, productName, itemType, originalPrice, salePrice, status);
            }
        }

        [Preserve]
        public class QiyuPayOrderResult
        {
            public const int STATUS_SUCCESS = 40;

            public long orderId;
            public string sku;
            public int orderStatus;
            public float paidAmount;
            public string payDate;

            public override string ToString()
            {
                return string.Format("orderId:{0},sku:{1},orderStatus:{2},paidAmount:{3},payDate:{4}", orderId, sku, orderStatus, paidAmount, payDate);
            }
        }

        [Preserve]
        public class QiyuPayHistoryOrders
        {
            public int count;
            public List<QiyuPayOrderResult> orders;
        }
    }
}