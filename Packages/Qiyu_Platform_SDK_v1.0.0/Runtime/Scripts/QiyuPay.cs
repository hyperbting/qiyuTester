//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qiyu.Sdk.Platform
{
    public static class QiyuPay
    {
        /// <summary>
        /// 初始化奇遇支付接口
        /// </summary>
        public static void InitQiyuPay(RequestCallback callback)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_InitQiyuPay(QiyuMessageManager.AddRequest(callback));
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="skuList">参数格式:"sku1,sku2,sku3",参数为空时获取全部</param>
        public static void GetSkuList(RequestCallback callback, string skuList = "")
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_GetSkuList(QiyuMessageManager.AddRequest(callback), skuList);
        }

        /// <summary>
        /// 下单购买
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="sku">要购买商品的sku</param>
        public static void PlaceOrder(RequestCallback callback, string sku)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_PlaceOrder(QiyuMessageManager.AddRequest(callback), sku);
        }

        /// <summary>
        /// 查询订单结果
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="orderId">PlaceOrder返回的订单号</param>
        public static void QueryOrderResult(RequestCallback callback, string orderId)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_QueryOrderResult(QiyuMessageManager.AddRequest(callback), orderId);
        }

        /// <summary>
        /// 查询历史订单信息
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="sku">要查询商品的sku</param>
        /// <param name="curPage">分页查询的页号，默认查询第1页</param>
        /// <param name="pageSize">每页指定订单数量，默认每页显示10个</param>
        public static void QueryHistoryOrders(RequestCallback callback, string sku, int curPage = 1, int pageSize = 10)
        {
            if (!QiyuPlatform.IsAndroid)
                return;

            QiyuXRPlatformPlugin.QVR_QueryHistoryOrders(QiyuMessageManager.AddRequest(callback), sku, curPage, pageSize);
        }
    }
}
