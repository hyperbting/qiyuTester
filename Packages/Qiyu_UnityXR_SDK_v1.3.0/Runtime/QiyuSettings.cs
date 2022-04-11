using System;

using UnityEngine;
using UnityEngine.XR.Management;
using Unity.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.XR.Qiyu
{
    [System.Serializable]
    [XRConfigurationData("QIYU", "Unity.XR.Qiyu.Settings")]
    public class QiyuSettings : ScriptableObject
    {
        public enum StereoRenderingModeAndroid
        {
            /// <summary>
            /// Unity makes two passes across the scene graph, each one entirely indepedent of the other. 
            /// Each pass has its own eye matrices and render target. Unity draws everything twice, which includes setting the graphics state for each pass. 
            /// This is a slow and simple rendering method which doesn't require any special modification to shaders.
            /// </summary>
            MultiPass = 0,
             /// <summary>
            /// Unity uses a single texture array with two elements. 
            /// Multiview is very similar to Single Pass Instanced; however, the graphics driver converts each call into an instanced draw call so it requires less work on Unity's side. 
            /// As with Single Pass Instanced, shaders need to be aware of the Multiview setting. Unity's shader macros handle the situation.
            /// </summary>
            Multiview = 1
        }

    
        public enum RenderTextureDepthType
        {
            BD_0 = 0,
            BD_16 = 16,
            BD_24 = 24
        }

        public enum RenderTextureAntiAliasing
        {
            X_1 = 1,
            X_2 = 2,
            X_4 = 4,
            X_8 = 8,
        }

         public enum PerfLevel
        {
            Minimum = 1,
            Medium,
            Maximum
        }
        public enum OptionFlags
        {
            ProtectedContent = (1 << 0),
            MotionAwareFrames = (1 << 1),
            FoveationSubsampled = (1 << 2),
            EnableCameraLayer = (1 << 3),
            Enable3drOcclusion = (1 << 4)
        }

        public enum EVSyncCount
        {
            k1 = 1,
            k2 = 2,
        }

        [Flags] public enum EOptionFlags
        {
            ProtectedContent = (1 << 0),
            MotionAwareFrames = (1 << 1),
            FoveationSubsampled = (1 << 2),
            EnableCameraLayer = (1 << 3),
            Enable3drOcclusion = (1 << 4),
        }

        public enum EFoveationLevel
        {
            None = -1,
            Low = 0,
            Med = 1,
            High = 2
        }



        /// <summary>
        /// Enable or disable support for using a shared depth buffer. This allows Unity and Qiyu to use a common depth buffer which enables Qiyu to composite the Qiyu Dash and other utilities over the Unity application.
        /// </summary>
        //[SerializeField, Tooltip("Enable a shared depth buffer")]
        //public bool SharedDepthBuffer = true;

        /// <summary>
        /// Enable or disable Dash support. This inintializes the Qiyu Plugin with Dash support which enables the Qiyu Dash to composite over the Unity application.
        /// </summary>
        //[SerializeField, Tooltip("Enable Qiyu Dash Support")]
        //public bool DashSupport = true;

        /// <summary>
        /// The current stereo rendering mode selected for Android-based Qiyu platforms
        /// </summary>
        [SerializeField, Tooltip("Set the Stereo Rendering Method")]
        public StereoRenderingModeAndroid m_StereoRenderingModeAndroid;

        [SerializeField, Tooltip("Set whether use the default render texture")]
        public bool UseDefaultRenderTexture = true;

        // [SerializeField, Tooltip("Set render texture antiAliasing type")]
        // public RenderTextureAntiAliasing AntiAliasing = RenderTextureAntiAliasing.X_1;

        //[SerializeField, Tooltip("Set the Resolution of eyes")]
        //public Vector2 eyeRenderTextureResolution = new Vector2(2048, 2048);

        // [SerializeField, Tooltip("Set the depth type of render texture")]
        // public RenderTextureDepthType renderTextureDepth = RenderTextureDepthType.BD_24;
        [SerializeField, Tooltip("Set CPU Performance level")]
        public PerfLevel cpuPerfLevel = PerfLevel.Medium;
        [SerializeField, Tooltip("Set GPU Performance level")]
        public PerfLevel gpuPerfLevel = PerfLevel.Medium;

        [SerializeField, Tooltip("Set Option Features")]
        public EOptionFlags optionFlags = 0;

        //[SerializeField, Tooltip("Use eye tracking (if available)")]
        //public bool TrackEyes = false;

        [SerializeField, Tooltip("Use position tracking (if available)")]
        public bool TrackPosition = true;

        // public float TrackPositionScale = 1;
        [SerializeField, Tooltip("Limit refresh rate")]
        public EVSyncCount VSyncCount = EVSyncCount.k1;

        [SerializeField, Tooltip("Distance between the eyes")]
        public float InterPupilDistance = 0.064f;

        [SerializeField, Tooltip("Foveation Level")]
        public EFoveationLevel foveationLevel;

        [SerializeField, Tooltip("Foveation Gain")]
        public Vector2 foveationGain;
        [SerializeField, Tooltip("Foveation Area")]
        public float foveationArea;
        [SerializeField, Tooltip("Foveation Minmum")]
        public float foveationMinmum;

        public bool IsMultiThreadedRendering;

        public int blitType;


        public ushort GetStereoRenderingMode()
        {
            return (ushort)m_StereoRenderingModeAndroid;
        }
#if !UNITY_EDITOR
		public static QiyuSettings s_Settings;

		public void Awake()
		{
            Debug.Log("QiyuSettings awake....");
			s_Settings = this;
            //Debug.Log("QiyuSettings awake...." + this.eyeRenderTextureResolution);
		}
#endif
    }
}
