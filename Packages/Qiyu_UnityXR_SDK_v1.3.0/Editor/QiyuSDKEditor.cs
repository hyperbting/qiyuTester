using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using Unity.XR.Qiyu;

[InitializeOnLoad]
public class Appload
{
    public static string modifyName = "QiyuSDK_ModifySettings";
    private static float delayTime = 0;
    static Appload()
    {
#if UNITY_ANDROID
        EditorApplication.update += Update;
#endif
    }

    static void Update()
    {
        delayTime += Time.deltaTime;
        //Delay processing to prevent editor crash.
        if (delayTime > 3)
        {
            delayTime = 0;
            int settingChange = PlayerPrefs.GetInt(modifyName, 0);
            if (settingChange == 0)
            {
                QiyuSDKEditor.ModifySettings();
            }
            else
            {
                EditorApplication.update -= Update;
            }
        }
    }
}

[InitializeOnLoad]
public class QiyuSDKEditor
{
    static QiyuSDKEditor()
    {
        ObjectFactory.componentWasAdded += ComponentWasAdded;
    }

    static void ComponentWasAdded(Component com)
    {
        if (com.name == "XR Rig")
        {
            var qiyuManager = GameObject.FindObjectOfType<QiyuManager>();
            if (qiyuManager == null)
            {
                string[] paths = AssetDatabase.FindAssets("QiyuManager");
                if (paths.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(paths[0]);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    PrefabUtility.InstantiatePrefab(prefab);
                    Debug.Log("QiyuManager Created.");
                }
            }
        }
    }

    [MenuItem("QIYU/Scene Quick Preview", false, 999)]
    public static void SceneQuickPreview()
    {
#if UNITY_EDITOR_WIN && UNITY_ANDROID
        QiyuSceneQuickPreviewWindow.Init();
#else
         Debug.LogError("Please open build settings and switch to Android platform");
#endif
    }

    [MenuItem("QIYU/Modify Player Settings", false, 1000)]
    public static void ModifySettings()
    {
        try
        {
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            GraphicsDeviceType[] graphicsDeviceType = new GraphicsDeviceType[1] { GraphicsDeviceType.OpenGLES3 };
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, graphicsDeviceType);

            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;

            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

            QualitySettings.vSyncCount = 1;

            PlayerPrefs.SetInt(Appload.modifyName, 1);

            Debug.Log("QiyuSDKEditor Modify player settings  success.");
        }
        catch (System.Exception e)
        {
            PlayerPrefs.SetInt(Appload.modifyName, 0);
            Debug.Log("QiyuSDKEditor Modify player settings  error." + e.StackTrace);
        }
    }

    [MenuItem("QIYU/Tools/Create AndroidManifast", false, 10)]
    public static void CreateAndroidManifast()
    {
        string assetPath = GetAssetFolder("QiyuSDKEditor");
        string srcFile = assetPath + "/AndroidManifest.default.xml";
        Debug.Log("Default android manifest file path is " + srcFile);

        if (!File.Exists(srcFile))
        {
            Debug.LogError("Cannot find default Android manifest template file" +
                " Please delete the QVR folder and reimport the QVRSDK.");
            return;
        }

        string manifestFolder = Application.dataPath + "/Plugins/Android";

        if (!Directory.Exists(manifestFolder))
            Directory.CreateDirectory(manifestFolder);

        string dstFile = manifestFolder + "/AndroidManifest.xml";

        if (File.Exists(dstFile))
        {
            if (!EditorUtility.DisplayDialog("AndroidManifest.xml Already Exists!", "Would you like to replace the existing manifest with a new one? All modifications will be lost.", "Replace", "Cancel"))
            {
                return;
            }
        }

        PatchAndroidManifest(srcFile, dstFile, false);

        AssetDatabase.Refresh();
    }
    [MenuItem("QIYU/Tools/Delete AndroidManifast", false, 10)]
    public static void DeleteAndroidManifast()
    {
        AssetDatabase.DeleteAsset("Assets/Plugins/Android/AndroidManifest.xml");
        AssetDatabase.Refresh();
    }

    static string GetAssetFolder(string _scriptName, string ext = ".cs")
    {
        string[] path = UnityEditor.AssetDatabase.FindAssets(_scriptName);
        if (path.Length > 1)
        {
            return null;
        }
        string _path = AssetDatabase.GUIDToAssetPath(path[0]).Replace((@"/" + _scriptName + ext), "");
        return _path;
    }

    static string GetPath(string _scriptName)
    {
        string[] path = UnityEditor.AssetDatabase.FindAssets(_scriptName);
        if (path.Length > 1)
        {
            return null;
        }
        string _path = AssetDatabase.GUIDToAssetPath(path[0]).Replace((@"/" + _scriptName + ".cs"), "");
        return _path;
    }

    public static void PatchAndroidManifest(string sourceFile, string destinationFile = null, bool skipExistingAttributes = true, bool enableSecurity = false)
    {
        if (destinationFile == null)
        {
            destinationFile = sourceFile;
        }

        bool modifyIfFound = !skipExistingAttributes;

        try
        {
            // Load android manfiest file
            XmlDocument doc = new XmlDocument();
            doc.Load(sourceFile);

            string androidNamepsaceURI;
            XmlElement element = (XmlElement)doc.SelectSingleNode("/manifest");
            if (element == null)
            {
                UnityEngine.Debug.LogError("Could not find manifest tag in android manifest.");
                return;
            }

            // Get android namespace URI from the manifest
            androidNamepsaceURI = element.GetAttribute("xmlns:android");
            if (string.IsNullOrEmpty(androidNamepsaceURI))
            {
                UnityEngine.Debug.LogError("Could not find Android Namespace in manifest.");
                return;
            }

            AddOrRemoveTag(doc,
                androidNamepsaceURI,
                "/manifest/application/activity/intent-filter",
                "category",
                "android.intent.category.LEANBACK_LAUNCHER",
                required: false,
                modifyIfFound: true); // always remove leanback launcher

            AddOrRemoveTag(doc,
                androidNamepsaceURI,
                "/manifest/application/activity/intent-filter",
                "action",
                "android.intent.action.MAIN",
                required: true,
                modifyIfFound: false); //always add  <action android:name="android.intent.action.MAIN" />

            AddOrRemoveTag(doc,
               androidNamepsaceURI,
               "/manifest/application/activity/intent-filter",
               "category",
               "android.intent.category.LAUNCHER",
               required: true,
               modifyIfFound: false); //always add  <category android:name="android.intent.category.LAUNCHER" />

            // make sure android label and icon are set in the manifest
            AddOrRemoveTag(doc,
                androidNamepsaceURI,
                "/manifest",
                "application",
                null,
                true,
                modifyIfFound,
                "label", "@string/app_name",
                "icon", "@mipmap/app_icon"
                );

            doc.Save(destinationFile);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }

    private static void AddOrRemoveTag(XmlDocument doc, string @namespace, string path, string elementName, string name, bool required, bool modifyIfFound, params string[] attrs) // name, value pairs
    {
        var nodes = doc.SelectNodes(path + "/" + elementName);
        XmlElement element = null;
        foreach (XmlElement e in nodes)
        {
            if (name == null || name == e.GetAttribute("name", @namespace))
            {
                element = e;
                break;
            }
        }

        if (required)
        {
            if (element == null)
            {
                var parent = doc.SelectSingleNode(path);
                element = doc.CreateElement(elementName);
                element.SetAttribute("name", @namespace, name);
                parent.AppendChild(element);
            }

            for (int i = 0; i < attrs.Length; i += 2)
            {
                if (modifyIfFound || string.IsNullOrEmpty(element.GetAttribute(attrs[i], @namespace)))
                {
                    if (attrs[i + 1] != null)
                    {
                        element.SetAttribute(attrs[i], @namespace, attrs[i + 1]);
                    }
                    else
                    {
                        element.RemoveAttribute(attrs[i], @namespace);
                    }
                }
            }
        }
        else
        {
            if (element != null && modifyIfFound)
            {
                element.ParentNode.RemoveChild(element);
            }
        }
    }
}


