//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Unity.XR.Qiyu
{
    public class QiyuSortOrder : MonoBehaviour
    {
        public int order;
        public bool isUI = true;
        void Start()
        {
            SetSortOrder();
        }
        void SetSortOrder()
        {
            if (isUI)
            {
                Canvas canvas = GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = gameObject.AddComponent<Canvas>();
                }
                canvas.overrideSorting = true;
                canvas.sortingOrder = order;
            }
            else
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>();

                foreach (Renderer render in renders)
                {
                    render.sortingOrder = order;
                }
            }
        }
    }
}