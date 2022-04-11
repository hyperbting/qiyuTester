using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.XR.Qiyu;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using Object = UnityEngine.Object;

namespace UnityEditor.XR.Qiyu
{
    public class QiyuBuildProcessor : XRBuildHelper<QiyuSettings>
    {
        public override string BuildSettingsKey { get { return "Unity.XR.Qiyu.Settings"; } }

        private bool IsCurrentBuildTargetVaild(BuildReport report)
        {
            return report.summary.platformGroup == BuildTargetGroup.Android;
        }

        private bool HasLoaderEnabledForTarget(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup != BuildTargetGroup.Android)
                return false;

            XRGeneralSettings settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            if (settings == null)
                return false;

            bool loaderFound = false;
            for (int i = 0; i < settings.Manager.loaders.Count; ++i)
            {
                if (settings.Manager.loaders[i] as QiyuLoader != null)
                {
                    loaderFound = true;
                    break;
                }
            }

            return loaderFound;
        }

        private bool ShouldIncludeRuntimePluginsInBuild(string path, BuildTargetGroup platformGroup)
        {
            return HasLoaderEnabledForTarget(platformGroup);
        }

        private readonly string[] runtimePluginNames = new string[]
        {
            "libUnityQiyuVR.so",
            "UnityQiyuVR.aar",
            "libashreader.so",
            "libQiyuXRCore.so",
            "QiyuXRCore.aar",
            "libsxrapi.so",
            "svrApi-release.aar",
            "libQiyuXRPlatform.so",
            "QiyuXRPlatform.aar"
        };

        public override void OnPreprocessBuild(BuildReport report)
        {
            if (IsCurrentBuildTargetVaild(report) && HasLoaderEnabledForTarget(report.summary.platformGroup))
                base.OnPreprocessBuild(report);

            var allPlugins = PluginImporter.GetAllImporters();
            foreach (var plugin in allPlugins)
            {
                if (plugin.isNativePlugin)
                {
                    foreach (var pluginName in runtimePluginNames)
                    {
                        if (plugin.assetPath.Contains(pluginName))
                        {
                            plugin.SetIncludeInBuildDelegate((path) => { return ShouldIncludeRuntimePluginsInBuild(path, report.summary.platformGroup); });
                            break;
                        }
                    }
                }
            }
        }
    }

    public static class QiyuBuildTools
    {
        public static bool QiyuLoaderPresentInSettingsForBuildTarget(BuildTargetGroup btg)
        {
            var generalSettingsForBuildTarget = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(btg);
            if (!generalSettingsForBuildTarget)
                return false;
            var settings = generalSettingsForBuildTarget.AssignedSettings;
            if (!settings)
                return false;
            List<XRLoader> loaders = settings.loaders;
            return loaders.Exists(loader => loader is QiyuLoader);
        }

        public static QiyuSettings GetSettings()
        {
            QiyuSettings settings = null;
#if UNITY_EDITOR
            //UnityEditor.EditorBuildSettings.TryGetConfigObject<QiyuSettings>("Unity.XR.Qiyu.Settings", out settings);
#else
            settings = QiyuSettings.s_Settings;
#endif
            Debug.Log(settings.m_StereoRenderingModeAndroid);
            return settings;
        }
    }

    [InitializeOnLoad]
    public static class QiyuEnterPlayModeSettingsCheck
    {
        static QiyuEnterPlayModeSettingsCheck()
        {
            EditorApplication.playModeStateChanged += PlaymodeStateChangedEvent;
        }

        private static void PlaymodeStateChangedEvent(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (!QiyuBuildTools.QiyuLoaderPresentInSettingsForBuildTarget(BuildTargetGroup.Standalone))
                {
                    return;
                }

                if (PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows)[0] !=
                    GraphicsDeviceType.Direct3D11)
                {
                    Debug.LogError("D3D11 is currently the only graphics API compatible with the QIYU XR Plugin on desktop platforms. Please change the preferred Graphics API setting in Player Settings.");
                }
            }
        }
    }

    internal class QiyuPrebuildSettings : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!QiyuBuildTools.QiyuLoaderPresentInSettingsForBuildTarget(report.summary.platformGroup))
                return;


            if (report.summary.platformGroup == BuildTargetGroup.Android)
            {
                GraphicsDeviceType firstGfxType = PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0];
                if (firstGfxType != GraphicsDeviceType.OpenGLES3 && firstGfxType != GraphicsDeviceType.Vulkan && firstGfxType != GraphicsDeviceType.OpenGLES2)
                {
                    throw new BuildFailedException("OpenGLES2, OpenGLES3, and Vulkan are currently the only graphics APIs compatible with the QIYU XR Plugin on mobile platforms.");
                }
                if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel25)
                {
                    throw new BuildFailedException("Minimum API must be set to 24 or higher for QIYU XR Plugin.");
                }
                if (QualitySettings.vSyncCount == 0)
                {
                    throw new BuildFailedException("vSyncCount must be Every V Blank or Every Second V Black for QIYU XR Plugin.");
                }
            }
            if (report.summary.platformGroup == BuildTargetGroup.Standalone)
            {
                if (PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0] !=
                    GraphicsDeviceType.Direct3D11)
                {
                    throw new BuildFailedException("D3D11 is currently the only graphics API compatible with the QIYU XR Plugin on desktop platforms. Please change the Graphics API setting in Player Settings.");
                }
            }
        }
    }



#if UNITY_ANDROID
    internal class QiyuManifest : IPostGenerateGradleAndroidProject
    {
        static readonly string k_AndroidURI = "http://schemas.android.com/apk/res/android";

        static readonly string k_AndroidManifestPath = "/src/main/AndroidManifest.xml";

        void UpdateOrCreateAttributeInTag(XmlDocument doc, string parentPath, string tag, string name, string value)
        {
            var xmlNode = doc.SelectSingleNode(parentPath + "/" + tag);

            if (xmlNode != null)
            {
                ((XmlElement)xmlNode).SetAttribute(name, k_AndroidURI, value);
            }
        }

        void UpdateOrCreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
            string firstName, string firstValue, string secondName, string secondValue)
        {
            var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

            foreach (XmlNode node in xmlNodeList)
            {
                var attributeList = ((XmlElement)node).Attributes;

                foreach (XmlAttribute attrib in attributeList)
                {
                    if (attrib.Value == firstValue)
                    {
                        XmlAttribute valueAttrib = attributeList[secondName, k_AndroidURI];
                        if (valueAttrib != null)
                        {
                            valueAttrib.Value = secondValue;
                        }
                        else
                        {
                            ((XmlElement)node).SetAttribute(secondName, k_AndroidURI, secondValue);
                        }
                        return;
                    }
                }
            }

            // Didn't find any attributes that matched, create both (or all three)
            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(firstName, k_AndroidURI, firstValue);
            childElement.SetAttribute(secondName, k_AndroidURI, secondValue);

            var xmlParentNode = doc.SelectSingleNode(parentPath);

            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        void CreateNameValueElementInTag(XmlDocument doc, string parentPath, string tag, string name, string value)
        {
            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(name, k_AndroidURI, value);
            var xmlParentNode = doc.SelectSingleNode(parentPath);
            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        // same as above, but don't create if the node already exists
        void CreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
            string firstName, string firstValue, string secondName, string secondValue, string thirdName = null, string thirdValue = null)
        {
            var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

            // don't create if the firstValue matches
            foreach (XmlNode node in xmlNodeList)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    if (attrib.Value == firstValue)
                    {
                        return;
                    }
                }
            }

            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(firstName, k_AndroidURI, firstValue);
            childElement.SetAttribute(secondName, k_AndroidURI, secondValue);

            if (thirdValue != null)
            {
                childElement.SetAttribute(thirdName, k_AndroidURI, thirdValue);
            }

            var xmlParentNode = doc.SelectSingleNode(parentPath);

            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (!QiyuBuildTools.QiyuLoaderPresentInSettingsForBuildTarget(BuildTargetGroup.Android))
                return;

            var manifestPath = path + k_AndroidManifestPath;
            var manifestDoc = new XmlDocument();
            manifestDoc.Load(manifestPath);

            var sdkVersion = (int)PlayerSettings.Android.minSdkVersion;

            //UpdateOrCreateAttributeInTag(manifestDoc, "/", "manifest", "installLocation", "auto");

            var nodePath = "/manifest/application";
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "pvr.app.type", "value", "vr");

            nodePath = "/manifest";
            CreateNameValueElementInTag(manifestDoc, nodePath, "uses-permission", "name", "android.permission.WRITE_EXTERNAL_STORAGE");
            CreateNameValueElementInTag(manifestDoc, nodePath, "uses-permission", "name", "android.permission.READ_EXTERNAL_STORAGE");

            // CreateNameValueElementInTag(manifestDoc, nodePath, "uses-permission", "name", "android.permission.CAMERA","","");
            // nodePath = "/manifest/application";
            //UpdateOrCreateAttributeInTag(manifestDoc, nodePath, "activity", "screenOrientation", "landscape");
            //UpdateOrCreateAttributeInTag(manifestDoc, nodePath, "activity", "theme", "@android:style/Theme.Black.NoTitleBar.Fullscreen");

            //var configChangesValue = "keyboard|keyboardHidden|navigation|orientation|screenLayout|screenSize|uiMode";
            //configChangesValue = ((sdkVersion >= 24) ? configChangesValue + "|density" : configChangesValue);
            //UpdateOrCreateAttributeInTag(manifestDoc, nodePath, "activity", "configChanges", configChangesValue);

            if (sdkVersion >= 24)
            {
                UpdateOrCreateAttributeInTag(manifestDoc, nodePath, "activity", "resizeableActivity", "false");
            }

            //UpdateOrCreateAttributeInTag(manifestDoc, nodePath, "activity", "launchMode", "singleTask");

            //if (!QiyuBuildTools.GetSettings() || QiyuBuildTools.GetSettings().V2Signing)
            //{
            //    nodePath = "/manifest";
            //    CreateNameValueElementsInTag(manifestDoc, nodePath, "uses-feature", "name", "android.hardware.vr.headtracking", "required", "true", "version", "1");
            //}

            manifestDoc.Save(manifestPath);
        }

        public int callbackOrder { get { return 10000; } }

        void DebugPrint(XmlDocument doc)
        {
            var sw = new System.IO.StringWriter();
            var xw = XmlWriter.Create(sw);
            doc.Save(xw);
            Debug.Log(sw);
        }



    }
#endif
    /// <summary>
    /// Class with helper methods for interacting with the build process.
    /// </summary>
    public static class BuildHelper
    {
        /// <summary>
        /// Adds a background shader with the given name to the project as a preloaded asset.
        /// </summary>
        /// <param name="shaderName">The name of a shader to add to the project.</param>
        /// <exception cref="UnityEditor.Build.BuildFailedException">Thrown if a shader with the given name cannot be
        /// found.</exception>
        public static void AddBackgroundShaderToProject(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName))
            {
                Debug.LogWarning("Incompatible render pipeline in GraphicsSettings.currentRenderPipeline. Background "
                                 + "rendering may not operate properly.");
            }
            else
            {
                Shader shader = FindShaderOrFailBuild(shaderName);

                Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

                var shaderAssets = (from preloadedAsset in preloadedAssets
                                    where shader.Equals(preloadedAsset)
                                    select preloadedAsset);
                if ((shaderAssets == null) || !shaderAssets.Any())
                {
                    List<Object> preloadedAssetsList = preloadedAssets.ToList();
                    preloadedAssetsList.Add(shader);
                    PlayerSettings.SetPreloadedAssets(preloadedAssetsList.ToArray());
                }
            }
        }

        /// <summary>
        /// Removes a shader with the given name from the preloaded assets of the project.
        /// </summary>
        /// <param name="shaderName">The name of a shader to remove from the project.</param>
        /// <exception cref="UnityEditor.Build.BuildFailedException">Thrown if a shader with the given name cannot be
        /// found.</exception>
        public static void RemoveShaderFromProject(string shaderName)
        {
            if (!string.IsNullOrEmpty(shaderName))
            {
                Shader shader = FindShaderOrFailBuild(shaderName);

                Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

                var nonShaderAssets = (from preloadedAsset in preloadedAssets
                                       where !shader.Equals(preloadedAsset)
                                       select preloadedAsset);
                PlayerSettings.SetPreloadedAssets(nonShaderAssets.ToArray());
            }
        }

        /// <summary>
        /// Finds a shader with the given name, or fail the build, if no shader is found.
        /// </summary>
        /// <param name="shaderName">The name of a shader to find.</param>
        /// <returns>
        /// The shader with the given name.
        /// </returns>
        /// <exception cref="UnityEditor.Build.BuildFailedException">Thrown if a shader with the given name cannot be
        /// found.</exception>
        static Shader FindShaderOrFailBuild(string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                throw new BuildFailedException($"Cannot find shader '{shaderName}'");
            }

            return shader;
        }
    }
}
