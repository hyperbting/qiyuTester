using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.XR.Qiyu;

namespace Unity.XR.Qiyu.Editor
{
    [CustomEditor(typeof(QiyuSettings))]
    public class QiyuSettingsEditor : UnityEditor.Editor
    {
        private const string kStereoRenderingModeAndroid = "m_StereoRenderingModeAndroid";
        // private const string kUseDefaultRenderTexture = "UseDefaultRenderTexture";
        //private const string kEyeRenderTextureResolution = "eyeRenderTextureResolution";
        //private const string kCpuPerfLevel = "cpuPerfLevel";
        //private const string kGpuPerfLevel = "gpuPerfLevel";
        // private const string kOptionFlags = "optionFlags";
        // // private const string kTrackEyes = "TrackEyes";
        // private const string kTrackPosition = "TrackPosition";
        // private const string kVSyncCount = "VSyncCount";
        // private const string kInterPupilDistance = "InterPupilDistance";
        // private const string kFoveationLevel = "foveationLevel";
        // private const string kFoveationArea = "foveationArea";
        // private const string kFoveationGain = "foveationGain";
        // private const string kFoveationMinmum = "foveationMinmum";
        // private const string kIsMultiThreadedRendering = "IsMultiThreadedRendering";
        // private const string kAntiAliasing = "AntiAliasing";
        // private const string kRenderTextureDepth = "renderTextureDepth";

        static GUIContent s_StereoRenderingMode = EditorGUIUtility.TrTextContent("Stereo Rendering Mode");
        //static GUIContent s_UseDefaultRenderTexture = EditorGUIUtility.TrTextContent("Use Default Render Texture");
        //static GUIContent s_EyeRenderTextureResolution = EditorGUIUtility.TrTextContent("Render Texture Resolution");
        //static GUIContent s_CpuPerfLevel = EditorGUIUtility.TrTextContent("CPU Performance level");
        //static GUIContent s_GpuPerfLevel = EditorGUIUtility.TrTextContent("GPU Performance level");
        // static GUIContent s_OptionFlags = EditorGUIUtility.TrTextContent("Option Features");
        // //static GUIContent s_TrackEyes = EditorGUIUtility.TrTextContent("Track Eyes");
        // static GUIContent s_TrackPosition = EditorGUIUtility.TrTextContent("Track Position");
        // static GUIContent s_VSyncCount = EditorGUIUtility.TrTextContent("VSync Count");
        // static GUIContent s_InterPupilDistance = EditorGUIUtility.TrTextContent("Inter Pupil Distance");
        // static GUIContent s_FoveationLevel = EditorGUIUtility.TrTextContent("Foveation Level");
        // static GUIContent s_FoveationArea = EditorGUIUtility.TrTextContent("Foveation Area");
        // static GUIContent s_FoveationGain = EditorGUIUtility.TrTextContent("Foveation Gain");
        // static GUIContent s_FoveationMinmum = EditorGUIUtility.TrTextContent("Foveation Minmum");
        // static GUIContent s_kIsMultiThreadedRendering = EditorGUIUtility.TrTextContent("Is Enable MT Rendering");
        // static GUIContent s_AntiAliasing = EditorGUIUtility.TrTextContent("Render Texture Anti-Aliasing");
        // static GUIContent s_RendertTextureDepth = EditorGUIUtility.TrTextContent("Render Textue Bit Depth");


        private SerializedProperty m_StereoRenderingModeAndroid;
        //private SerializedProperty m_UseDefaultRenderTexture;
        //private SerializedProperty m_EyeRenderTextureResolution;
        //private SerializedProperty m_CpuPerfLevel;
        //private SerializedProperty m_GpuPerfLevel;
        // private SerializedProperty m_OptionFlags;
        // //private SerializedProperty m_TrackEyes;
        // private SerializedProperty m_TrackPosition;
        private SerializedProperty m_VSyncCount;
        // private SerializedProperty m_InterPupilDistance;
        // private SerializedProperty m_FoveationLevel;
        // private SerializedProperty m_FoveationArea;
        // private SerializedProperty m_FoveationGain;
        // private SerializedProperty m_FoveationMinmum;
        // private SerializedProperty m_kIsMultiThreadedRendering;
        // private SerializedProperty m_AntiAliasing;
        // private SerializedProperty m_RenderTextureDetph;

        void OnEnable()
        {
            if (m_StereoRenderingModeAndroid == null) m_StereoRenderingModeAndroid = serializedObject.FindProperty(kStereoRenderingModeAndroid);
            //if (m_UseDefaultRenderTexture == null) m_UseDefaultRenderTexture = serializedObject.FindProperty(kUseDefaultRenderTexture);
            //if (m_EyeRenderTextureResolution == null) m_EyeRenderTextureResolution = serializedObject.FindProperty(kEyeRenderTextureResolution);
            // if (m_AntiAliasing == null) m_AntiAliasing = serializedObject.FindProperty(kAntiAliasing);
            // if (m_RenderTextureDetph == null) m_RenderTextureDetph = serializedObject.FindProperty(kRenderTextureDepth);
            //if (m_CpuPerfLevel == null) m_CpuPerfLevel = serializedObject.FindProperty(kCpuPerfLevel);
            //if (m_GpuPerfLevel == null) m_GpuPerfLevel = serializedObject.FindProperty(kGpuPerfLevel);
            // if (m_OptionFlags == null) m_OptionFlags = serializedObject.FindProperty(kOptionFlags);
            // //if (m_TrackEyes == null) m_TrackEyes = serializedObject.FindProperty(kTrackEyes);
            // if (m_TrackPosition == null) m_TrackPosition = serializedObject.FindProperty(kTrackPosition);
            // if (m_VSyncCount == null) m_VSyncCount = serializedObject.FindProperty(kVSyncCount);
            // if (m_InterPupilDistance == null) m_InterPupilDistance = serializedObject.FindProperty(kInterPupilDistance);
            // if (m_FoveationLevel == null) m_FoveationLevel = serializedObject.FindProperty(kFoveationLevel);
            // if (m_FoveationArea == null) m_FoveationArea = serializedObject.FindProperty(kFoveationArea);
            // if (m_FoveationGain == null) m_FoveationGain = serializedObject.FindProperty(kFoveationGain);
            // if (m_FoveationMinmum == null) m_FoveationMinmum = serializedObject.FindProperty(kFoveationMinmum);
            // if(m_kIsMultiThreadedRendering == null) m_kIsMultiThreadedRendering = serializedObject.FindProperty(kIsMultiThreadedRendering);
            
            // switch (QualitySettings.vSyncCount)
            // {
            //     case 1:
            //         ((QiyuSettings)target).VSyncCount = QiyuSettings.EVSyncCount.k1;
            //         break;
            //     case 2:
            //         ((QiyuSettings)target).VSyncCount = QiyuSettings.EVSyncCount.k2;
            //         break;
            //     default:
            //         ((QiyuSettings)target).VSyncCount = QiyuSettings.EVSyncCount.k1;
            //     break;
            // }
            // ((QiyuSettings)target).IsMultiThreadedRendering = PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android);

            //switch (QualitySettings.antiAliasing)
            //{
            //    case 0:
            //        ((QiyuSettings)target).AntiAliasing = QiyuSettings.RenderTextureAntiAliasing.X_1;
            //        break;
            //    case 2:
            //        ((QiyuSettings)target).AntiAliasing = QiyuSettings.RenderTextureAntiAliasing.X_2;
            //        break;
            //    case 4:
            //        ((QiyuSettings)target).AntiAliasing = QiyuSettings.RenderTextureAntiAliasing.X_4;
            //        break;
            //    case 8:
            //        ((QiyuSettings)target).AntiAliasing = QiyuSettings.RenderTextureAntiAliasing.X_8;
            //        break;
            //}
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;

            serializedObject.Update();

            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Qiyu settings cannnot be changed when the editor is in play mode.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (selectedBuildTargetGroup == BuildTargetGroup.Android)
            {

                EditorGUILayout.PropertyField(m_StereoRenderingModeAndroid, s_StereoRenderingMode);
                //EditorGUILayout.PropertyField(m_UseDefaultRenderTexture, s_UseDefaultRenderTexture);
                //if (!((QiyuSettings)target).UseDefaultRenderTexture)
                //{
                //    EditorGUILayout.PropertyField(m_EyeRenderTextureResolution, s_EyeRenderTextureResolution);
                //}
                // EditorGUILayout.PropertyField(m_AntiAliasing, s_AntiAliasing);
                // EditorGUILayout.PropertyField(m_RenderTextureDetph, s_RendertTextureDepth);
                //EditorGUILayout.PropertyField(m_CpuPerfLevel, s_CpuPerfLevel);
                //EditorGUILayout.PropertyField(m_GpuPerfLevel, s_GpuPerfLevel);
                // EditorGUILayout.PropertyField(m_OptionFlags, s_OptionFlags);
                // //EditorGUILayout.PropertyField(m_TrackEyes, s_TrackEyes);
                // EditorGUILayout.PropertyField(m_TrackPosition, s_TrackPosition);
                // EditorGUILayout.PropertyField(m_VSyncCount, s_VSyncCount);
                // EditorGUILayout.PropertyField(m_InterPupilDistance, s_InterPupilDistance);
                // EditorGUILayout.PropertyField(m_FoveationLevel, s_FoveationLevel);
                // EditorGUILayout.PropertyField(m_FoveationArea, s_FoveationArea);
                // EditorGUILayout.PropertyField(m_FoveationGain, s_FoveationGain);
                // EditorGUILayout.PropertyField(m_FoveationMinmum, s_FoveationMinmum);

                // GUI.enabled = false;
                // EditorGUILayout.PropertyField(m_kIsMultiThreadedRendering, s_kIsMultiThreadedRendering );
                // GUI.enabled = true;
                // switch (((QiyuSettings)target).VSyncCount)
                // {
                //     case QiyuSettings.EVSyncCount.k1:
                //         QualitySettings.vSyncCount = 1;
                //         break;
                //     case QiyuSettings.EVSyncCount.k2:
                //         QualitySettings.vSyncCount = 2;
                //         break;
                // }
                //switch (((QiyuSettings)target).AntiAliasing)
                //{
                //    case QiyuSettings.RenderTextureAntiAliasing.X_1:
                //        QualitySettings.antiAliasing = 0;
                //        break;
                //    case QiyuSettings.RenderTextureAntiAliasing.X_2:
                //        QualitySettings.antiAliasing = 2;
                //        break;
                //    case QiyuSettings.RenderTextureAntiAliasing.X_4:
                //        QualitySettings.antiAliasing = 4;
                //        break;
                //    case QiyuSettings.RenderTextureAntiAliasing.X_8:
                //        QualitySettings.antiAliasing = 8;
                //        break;
                //}
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
