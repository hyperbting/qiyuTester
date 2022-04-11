using System;
using System.Collections.Generic;
using Unity.XR.Qiyu;
using UnityEngine;

namespace Qiyu.Overlay
{

    public enum OverLayType
    {
        Overlay=1,
        Underlay=0
    }

    public enum OverlayShape
    {
        Quad,
        Cylinder,
        Sphere,
        Customize
    }

    public enum MeshFace
    {
        Front,
        Back
    }

    //This Component need add to XR camera.
    public class QiyuOverlayManager : MonoBehaviour
    {
        public static  List<QiyuOverlay> qiyuOverlaysList = new List<QiyuOverlay>();
        public static Action onPipelinePostRenderAction;

        private void Awake()
        {
            NativeMethods.SetLayerRenderQuality(1.3f);
        }

        private void OnPostRender()
        {
            OnOverlayPostRender();
        }
        private void OnOverlayPostRender()
        {
            onPipelinePostRenderAction?.Invoke();
            foreach (var qiyuOverlay in qiyuOverlaysList)
            {
                if (qiyuOverlay.gameObject!=null && qiyuOverlay.isMeshInit)
                {
                    qiyuOverlay.SetCameraInfo(GetComponent<Camera>());
                    qiyuOverlay.UpdateModelInfo();
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var overlay in qiyuOverlaysList)
            {
                overlay.RemoveOverlay();
            }
            qiyuOverlaysList.Clear();
        }
    }
}

