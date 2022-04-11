#if(UNITY_ANDROID && !UNITY_EDITOR)
#define QiyuPLUGIN_ANDROID_PLATFORM_ONLY
#endif

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.XR.Qiyu
{

    public static partial class NativeMethods
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct UserDefinedSettings
        {
            public ushort stereoRenderingMode;
            public ushort colorSpace;
            public int cpuPerfLevel;
            public int gpuPerfLevel;
            public int optionFlags;
            public float interPupilDistance;
            public int vSyncCount;
            public int foveationLevel;
            public Vector2 foveationGain;
            public float foveationArea;
            public float foveationMinmum;
            // [MarshalAs(UnmanagedType.U1)]
            // public bool TrackEyes;
            [MarshalAs(UnmanagedType.U1)]
            public bool TrackPosition;
            // [MarshalAs(UnmanagedType.U1)]
            // public bool useDefaultRenderTexture;
            [MarshalAs(UnmanagedType.U1)]
            public bool isMultiThreadRendering;

            public int blitType;
        }

        public enum Bool
        {
            False = 0,
            True
        }
        public enum BoundaryType
        {
            OuterBoundary = 1,
            PlayArea = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3f
        {
            public float x;
            public float y;
            public float z;
            public static readonly Vector3f zero = new Vector3f { x = 0.0f, y = 0.0f, z = 0.0f };
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}", x, y, z);
            }
        }

//        internal static int SetCPULevel(int cpuLevel)
//        {
//#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
//            return Internal.SetCPULevel(cpuLevel);
//#else
//            return -1;
//#endif
//        }

//        internal static int SetGPULevel(int gpuLevel)
//        {
//#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
//            return Internal.SetGPULevel(gpuLevel);
//#else
//            return -1;
//#endif
//        }

//        internal static int SetPerformanceLevels(int cpuLevel, int gpuLevel)
//        {
//#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
//            return Internal.SetPerformanceLevels(cpuLevel, gpuLevel);
//#else
//            return -1;
//#endif
//        }

        internal static int SetFoveationLevel(int level)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            return Internal.SetFoveationLevel(level);
#else
            return -1;
#endif
        }

        internal static int GetFoveationLevel()
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Debug.LogError("GetFoveationLevel");
            return Internal.GetFoveationLevel();
#else
            return -1;
#endif
        }

        internal static void SetFoveationParameters(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetFoveationParamets(foveationGainX, foveationGainY, foveationArea, foveationMinimum);
#else

#endif
        }

        internal static void SetUserDefinedSettings(ref UserDefinedSettings settings)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetUserDefinedSettings(ref settings);
#else

#endif
        }

        internal static float UpdateIPD()
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            return Internal.UpdateIPD();
#else
            return -1f;
#endif
        }

        internal static void SetTrackingPosition(bool value)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetTrackingPosition(value);
#else

#endif
        }

        internal static bool GetTrackingPosition()
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            bool result = false;
            Internal.GetTrackingPosition(ref result);
            return result;
#else
            return false;
#endif
        }

        public static void SetFrameOption(uint option)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetFrameOption(option);
#else

#endif
        }

        public static void UnsetFrameOption(uint option)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.UnsetFrameOption(option);
#else

#endif
        }

        public static void SetOverlayCount(uint num)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetOverlayCount(num);
#else

#endif
        }       

        public static void InitOverlayMesh(string overlayID_str, int layerID,bool is_android_oes_texture,int overlayType,
            int vertexsCount,float[] modelVertexs,int indicesCount, int[] modelIndices, float[] modelUV)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.InitLayerObject(overlayID_str,layerID, is_android_oes_texture, overlayType,vertexsCount, modelVertexs, indicesCount, modelIndices, modelUV);
#else

#endif
        }

        public static void DrawOverlay(string overlayID_str, int layerID, int TextureID,IntPtr vkImage,int texture_type,int width,int height,
            float[] modelScale, float[] modelRotation,float[] modelTrans, float[] leftCameraRot, float[] leftCameraPos,
    float[] rightCameraRot, float[] rightCameraPos,float near,float far,float[] colorScale,float[] colorOffset)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetLayerObjectParameter(overlayID_str,layerID, TextureID, vkImage,texture_type,
            width,height, modelScale, modelRotation, modelTrans, leftCameraRot, leftCameraPos, rightCameraRot, 
            rightCameraPos,near,far,colorScale,colorOffset);
#else

#endif
        }

        public static void RemoveOverlay(string overlayID_str)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.RemoveLayerObject(overlayID_str);
#else

#endif
        }

        public static void SetLayerObjectRender(string overlayID_str, bool isVisible)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetLayerObjectRender(overlayID_str, isVisible);
#else

#endif
        }

        public static void SetLayerRenderQuality(float coefficient)
        {
#if QiyuPLUGIN_ANDROID_PLATFORM_ONLY
            Internal.SetLayerRenderQuality(coefficient);
#else

#endif
        }

        private static class Internal
        {
            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern int SetCPULevel(int cpuLevel);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern int SetGPULevel(int gpuLevel);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern int SetPerformanceLevels(int cpuLevel, int gpuLevel);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern int SetFoveationLevel(int level);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern int GetFoveationLevel();

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern int SetFoveationParamets(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern void SetUserDefinedSettings(ref UserDefinedSettings settings);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern float UpdateIPD();
            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern void SetTrackingPosition([MarshalAs(UnmanagedType.U1)] bool value);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern void GetTrackingPosition([MarshalAs(UnmanagedType.U1)]ref bool value);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern void SetFrameOption(uint option);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern void UnsetFrameOption(uint option);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Auto)]
            internal static extern void SetOverlayCount(uint num);

            #region Overlay and Underlay

            [DllImport("UnityQiyuVR", CharSet = CharSet.Ansi)]
            internal static extern void SetLayerObjectParameter(string overlayID_str, int id,
                int texId,IntPtr vkImage,int texture_type, int width, int height,float[] worldScale, float[] worldRot,
                float[] worldTrans, float[] leftCameraRot, float[] leftCameraPos, float[] rightCameraRot,
                float[] rightCameraPos,float near,float far,float[] colorScale,float[] colorOffset);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Ansi)]
            internal static extern void InitLayerObject(string overlayID_str, int id,bool isOES ,int overlayType, int vertexNum, float[] vertices, int indexNum, int[] indices, float[] texUV);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Ansi)]
            internal static extern void RemoveLayerObject(string overlayID_str);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Ansi)]
            internal static extern void SetLayerObjectRender(string overlayID_str, bool isVisible);

            [DllImport("UnityQiyuVR", CharSet = CharSet.Ansi)]
            internal static extern void SetLayerRenderQuality(float coefficient);
            #endregion
        }
    }
}

