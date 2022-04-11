#if XR_MGMT_GTE_320

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;

namespace Unity.XR.Qiyu.Editor
{
    internal class QiyuMetadata : IXRPackage
    {
        private class QiyuPackageMetadata : IXRPackageMetadata
        {
            public string packageName => "QIYU XR Plugin";
            public string packageId => "com.unity.xr.qiyu";
            public string settingsType => "Unity.XR.Qiyu.QiyuSettings";
            public List<IXRLoaderMetadata> loaderMetadata => s_LoaderMetadata;

            private readonly static List<IXRLoaderMetadata> s_LoaderMetadata = new List<IXRLoaderMetadata>() { new QiyuLoaderMetadata() };
        }

        private class QiyuLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName => "QIYU";
            public string loaderType => "Unity.XR.Qiyu.QiyuLoader";
            public List<BuildTargetGroup> supportedBuildTargets => s_SupportedBuildTargets;

            private readonly static List<BuildTargetGroup> s_SupportedBuildTargets = new List<BuildTargetGroup>()
            {
                //BuildTargetGroup.Standalone,
                BuildTargetGroup.Android
            };
        }

        private static IXRPackageMetadata s_Metadata = new QiyuPackageMetadata();
        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            var settings = obj as QiyuSettings;
            if (settings != null)
            {
                settings.m_StereoRenderingModeAndroid = QiyuSettings.StereoRenderingModeAndroid.MultiPass;
                settings.cpuPerfLevel = QiyuSettings.PerfLevel.Medium;
                settings.gpuPerfLevel = QiyuSettings.PerfLevel.Medium;
                settings.optionFlags = 0;
                settings.TrackPosition = true;
                settings.VSyncCount = QiyuSettings.EVSyncCount.k1;
                settings.foveationLevel = QiyuSettings.EFoveationLevel.None;
                settings.foveationArea = 0;
                settings.foveationGain = Vector2.zero;
                settings.foveationMinmum = 0;
                settings.IsMultiThreadedRendering = PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android);
                settings.blitType = (int)PlayerSettings.Android.blitType;
                return true;
            }

            return false;
        }
    }
}

#endif // XR_MGMT_GTE_320
