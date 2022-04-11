using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using UnityEditor.Build.Reporting;

public class SceneInfo
{
    public string scenePath;
    public string sceneName;

    public SceneInfo(string path, string name)
    {
        scenePath = path;
        sceneName = name;
    }
}

public class QiyuSceneQuickPreviewManager
{
    #if UNITY_EDITOR_WIN && UNITY_ANDROID
    private readonly string QiyuLoaderSceneName = "QiyuPreviewSceneLoader.unity";
    private readonly string BUNDLE_MANAGER_OUTPUT_PATH = "QiyuAssetBundles";
    private readonly string BUNDLE_MANAGER_MASTER_BUNDLE = "QiyuLoaderBundle";
    private readonly string SCENE_LOADER_LIST = "SceneList.txt";

    private AndroidArchitecture targetArchitecture;
    private ScriptingImplementation scriptBackend;
    private ManagedStrippingLevel managedStrippingLevel;
    private bool stripEngineCode;

    //app data cache dir.
    public string deviceCachePath
    {
        get=> "/sdcard/Android/data/" + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) + "/cache/scenes";
    }
    public string tempApkName{get => "qiyu_scene_loader.apk"; }
    public string resource_bundle_name { get => "asset_resources"; }
    public int bundle_chunk_size { get => 40; }
    public string qiyuLoaderScenePath { get => Path.Combine(Application.dataPath, QiyuLoaderSceneName); }

    string TAG = "QiyuSceneQuickPreviewManager";


    List<string> buildSceneList = new List<string>();
    List<SceneInfo> sceneInfoList = new List<SceneInfo>();

    private static QiyuSceneQuickPreviewManager _me;
    private QiyuSceneQuickPreviewManager()
    {
    }
    public static QiyuSceneQuickPreviewManager GetMe()
    {
        if (_me == null)
        {
            _me = new QiyuSceneQuickPreviewManager();
        }
        return _me;
    }

    /// <summary>
    /// Build Scene and copy to Device
    /// </summary>
    public void BuildScene(bool isDebug)
    {
        if (!QiyuADBTools.GetMe().IsAdbAvailable())
        {          
            return;
        }
        InitSceneList();
        Dictionary<string, string> assetInSceneBundle = new Dictionary<string, string>();
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        Dictionary<string, List<string>> extToAssetList = new Dictionary<string, List<string>>();

        string[] resDirectories = Directory.GetDirectories("Assets", "Resources", SearchOption.AllDirectories).ToArray();

        if (resDirectories.Length > 0)
        {
            string[] resAssetPaths = AssetDatabase.FindAssets("", resDirectories).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();
            ProcessAssets(resAssetPaths, "resources", ref assetInSceneBundle, ref extToAssetList);

            AssetBundleBuild resBundle = new AssetBundleBuild();
            resBundle.assetNames = assetInSceneBundle.Keys.ToArray();
            resBundle.assetBundleName = resource_bundle_name;
            assetBundleBuilds.Add(resBundle);
        }

        foreach (var scene in sceneInfoList)
        {
            string[] assetDependencies = AssetDatabase.GetDependencies(scene.scenePath);
            ProcessAssets(assetDependencies, scene.sceneName, ref assetInSceneBundle, ref extToAssetList);

            string[] sceneAsset = new string[1] { scene.scenePath };
            AssetBundleBuild sceneBuild = new AssetBundleBuild();
            sceneBuild.assetBundleName = "scene_" + scene.sceneName;
            sceneBuild.assetNames = sceneAsset;
            assetBundleBuilds.Add(sceneBuild);
        }

        foreach (string ext in extToAssetList.Keys)
        {
            int assetCount = extToAssetList[ext].Count;
            int numChunks = (assetCount + bundle_chunk_size - 1) / bundle_chunk_size;
            for (int i = 0; i < numChunks; i++)
            {
                List<string> assetChunkList;
                if (i == numChunks - 1)
                {
                    int size = bundle_chunk_size - (numChunks * bundle_chunk_size - assetCount);
                    assetChunkList = extToAssetList[ext].GetRange(i * bundle_chunk_size, size);
                }
                else
                {
                    assetChunkList = extToAssetList[ext].GetRange(i * bundle_chunk_size, bundle_chunk_size);
                }
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = "asset_" + ext + i;
                build.assetNames = assetChunkList.ToArray();
                assetBundleBuilds.Add(build);
            }
        }
        // Create scene bundle output directory
        string sceneBundleDirectory = Path.Combine(BUNDLE_MANAGER_OUTPUT_PATH, BUNDLE_MANAGER_MASTER_BUNDLE);
        if (!Directory.Exists(sceneBundleDirectory))
        {
            Directory.CreateDirectory(sceneBundleDirectory);
        }
        
        // Build asset bundles
        BuildPipeline.BuildAssetBundles(sceneBundleDirectory, assetBundleBuilds.ToArray(),
                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);

        QiyuSceneQuickPreviewWindow.PrintLog("Building scene bundles . . . ", QiyuSceneQuickPreviewWindow.LogType.Normal);

        string tempDirectory = Path.Combine(BUNDLE_MANAGER_OUTPUT_PATH, "Temp");
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }
        string absoluteTempPath = Path.Combine(Path.Combine(Application.dataPath, ".."), tempDirectory);

        if (!PullSceneBundles(absoluteTempPath, deviceCachePath))
        {
            return;
        }

        string sceneLoadDataPath = Path.Combine(tempDirectory, SCENE_LOADER_LIST);
        if (File.Exists(sceneLoadDataPath))
        {
            File.Delete(sceneLoadDataPath);
        }

        StreamWriter writer = new StreamWriter(sceneLoadDataPath, true);

        long unixTime = (int)(DateTimeOffset.UtcNow.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        writer.WriteLine(unixTime.ToString());
        for (int i = 0; i < sceneInfoList.Count; i++)
        {
            writer.WriteLine(Path.GetFileNameWithoutExtension(sceneInfoList[i].scenePath));
        }
        writer.Close();

        string absoluteSceneLoadDataPath = Path.Combine(absoluteTempPath, SCENE_LOADER_LIST);
        string[] pushCommand = { "-d push", "\"" + absoluteSceneLoadDataPath + "\"", "\"" + deviceCachePath + "\"" };
        string output, error;
        if (QiyuADBTools.GetMe().RunCommand(pushCommand, null, out output, out error) != 0)
        {
            QiyuSceneQuickPreviewWindow.PrintLog(string.IsNullOrEmpty(error) ? output : error,QiyuSceneQuickPreviewWindow.LogType.Error);
            return;
        }

        if (!IsInstalledAPP())
        {
            BuildApk(isDebug); 
        }
        QiyuSceneQuickPreviewWindow.PrintLog("Build Scenes finished.", QiyuSceneQuickPreviewWindow.LogType.Success);
    }

    /// <summary>
    ///  Build Scene and Restart app.
    /// </summary>
    public void BuildSceneAndRestart(bool isDebug)
    {
        BuildScene(isDebug);
        RestartApp();
    }

    public bool StartApp()
    {
        if (!QiyuADBTools.GetMe().IsAdbAvailable())
        {
            return false;
        }
        string output, error;
        string[] appStartCommand = { "-d shell", "am start -n", GetPlayerActivityName() };
        if (QiyuADBTools.GetMe().RunCommand(appStartCommand, null, out output, out error) == 0)
        {
            QiyuSceneQuickPreviewWindow.PrintLog("App " + " Start App Success!", QiyuSceneQuickPreviewWindow.LogType.Success);
            return true;
        }
        string completeError = "Failed to start App. Try restarting it manually through the device.\n" + (string.IsNullOrEmpty(error) ? output : error);
        Debug.LogError(completeError);
        return false;
    }

    public bool StopApp()
    {
        if (!QiyuADBTools.GetMe().IsAdbAvailable())
        {
            return false;
        }
        string output, error;
        string[] appStartCommand = { "-d shell", "am force-stop ", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) };
        if (QiyuADBTools.GetMe().RunCommand(appStartCommand, null, out output, out error) == 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Restart app.
    /// </summary>
    public bool RestartApp()
    {
        if (!QiyuADBTools.GetMe().IsAdbAvailable())
        {
            return false;
        }
        StopApp();
        System.Threading.Thread.Sleep(1000);
        return StartApp();
        string output, error;
        string[] appStartCommand = { "-d shell", "am start -a android.intent.action.MAIN -c android.intent.category.LAUNCHER -S -f 0x10200000 -n", GetPlayerActivityName() };
        if (QiyuADBTools.GetMe().RunCommand(appStartCommand, null, out output, out error) == 0)
        {
            QiyuSceneQuickPreviewWindow.PrintLog("App " + " Restart Success!", QiyuSceneQuickPreviewWindow.LogType.Success);
            return true;
        }

        string completeError = "Failed to restart App. Try restarting it manually through the device.\n" + (string.IsNullOrEmpty(error) ? output : error);
        Debug.LogError(completeError);
        return false;
    }

    public  void BuildApk(bool isDebug)
    {
        QiyuSceneQuickPreviewWindow.PrintLog("installing app  . . .", QiyuSceneQuickPreviewWindow.LogType.Normal);

        if (!Directory.Exists(BUNDLE_MANAGER_OUTPUT_PATH))
            Directory.CreateDirectory(BUNDLE_MANAGER_OUTPUT_PATH);

        PrebuildProjectSettingUpdate();

        if (string.IsNullOrEmpty(qiyuLoaderScenePath) || !File.Exists(qiyuLoaderScenePath))
        {
            string[] editorScenePaths = Directory.GetFiles(Path.GetFullPath("Packages/com.unity.xr.qiyu/"), QiyuLoaderSceneName, SearchOption.AllDirectories);

            if (editorScenePaths.Length == 0 || editorScenePaths.Length > 1)
            {
                QiyuSceneQuickPreviewWindow.PrintLog(editorScenePaths.Length + " " + QiyuLoaderSceneName + " can not found, please import qiyu SDK Plugin.", QiyuSceneQuickPreviewWindow.LogType.Error);
                return;
            }
            if (File.Exists(qiyuLoaderScenePath))
            {
                File.Delete(qiyuLoaderScenePath);
            }
            File.Copy(editorScenePaths[0], qiyuLoaderScenePath);
        }

        string[] buildScenes = new string[1] { qiyuLoaderScenePath };
        string apkOutputPath = Path.Combine(BUNDLE_MANAGER_OUTPUT_PATH, tempApkName);

        if (File.Exists(apkOutputPath))
        {
            File.Delete(apkOutputPath);
        }

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = buildScenes,
            locationPathName = apkOutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.AutoRunPlayer | (isDebug ? BuildOptions.Development : BuildOptions.AutoRunPlayer)
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == BuildResult.Succeeded)
        {
            QiyuSceneQuickPreviewWindow.PrintLog("App is installed.", QiyuSceneQuickPreviewWindow.LogType.Success);
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            QiyuSceneQuickPreviewWindow.PrintLog("Failed", QiyuSceneQuickPreviewWindow.LogType.Error);
        }
        PostbuildProjectSettingUpdate();
    }

    /// <summary>
    /// Uninstall app.
    /// </summary>
    public bool UninstallApp()
    {
        QiyuSceneQuickPreviewWindow.PrintLog("Uninstalling Application . . .", QiyuSceneQuickPreviewWindow.LogType.Normal);

        if (!QiyuADBTools.GetMe().IsAdbAvailable())
        {
            return false;
        }

        string output, error;
        string appPackageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
        string[] appStartCommand = { "-d shell", "pm uninstall", appPackageName };
        if (QiyuADBTools.GetMe().RunCommand(appStartCommand, null, out output, out error) == 0)
        {
            QiyuSceneQuickPreviewWindow.PrintLog("package " + appPackageName + " was uninstalled.", QiyuSceneQuickPreviewWindow.LogType.Success);
            return true;
        }

        QiyuSceneQuickPreviewWindow.PrintLog("Failed to uninstall APK.", QiyuSceneQuickPreviewWindow.LogType.Error);
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void CleanBundleCache()
    {
        try
        {
            if (Directory.Exists(BUNDLE_MANAGER_OUTPUT_PATH))
            {
                Directory.Delete(BUNDLE_MANAGER_OUTPUT_PATH, true);
            }
            if (File.Exists(qiyuLoaderScenePath))
            {
                File.Delete(qiyuLoaderScenePath);
            }
        }
        catch (Exception e)
        {
            QiyuSceneQuickPreviewWindow.PrintLog(e.Message, QiyuSceneQuickPreviewWindow.LogType.Error);
        }
        QiyuSceneQuickPreviewWindow.PrintLog("Clean Cache Bundles.", QiyuSceneQuickPreviewWindow.LogType.Success);
    }

    public void InitSceneList()
    {
        buildSceneList.Clear();
        sceneInfoList.Clear();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                if (Path.GetFileName(scene.path) != QiyuLoaderSceneName)
                {
                    buildSceneList.Add(scene.path);
                    SceneInfo sceneInfo = new SceneInfo(scene.path, Path.GetFileNameWithoutExtension(scene.path));
                    sceneInfoList.Add(sceneInfo);
                }
            }
        }
        if (buildSceneList.Count == 0)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneInfo sceneInfo = new SceneInfo(scene.path, Path.GetFileNameWithoutExtension(scene.path));
            sceneInfoList.Add(sceneInfo);
        }
    }

    private  void ProcessAssets(string[] assetPaths,string assetParent,ref Dictionary<string, string> sceneBundle,ref Dictionary<string, List<string>> assetList)
    {
        foreach (string asset in assetPaths)
        {
            string ext = Path.GetExtension(asset);
            if (string.IsNullOrEmpty(ext))
            {
                continue;
            }

            ext = ext.Substring(1);
            if (ext.Equals("cs") || ext.Equals("unity"))
            {
                continue;
            }

            if (sceneBundle.ContainsKey(asset))
            {
                continue;
            }

            var assetObject = AssetDatabase.LoadAssetAtPath(asset, typeof(UnityEngine.Object));
            if (assetObject == null || (assetObject.hideFlags & HideFlags.DontSaveInBuild) == 0)
            {
                sceneBundle[asset] = assetParent;

                if (assetParent != "resources")
                {
                    if (!assetList.ContainsKey(ext))
                    {
                        assetList[ext] = new List<string>();
                    }
                    assetList[ext].Add(asset);
                }
            }
        }
    }

    private bool PullSceneBundles(string absoluteTempPath, string externalSceneCache)
    {
        List<string> bundlesToTransfer = new List<string>();
        string manifestFilePath = externalSceneCache + "/" + BUNDLE_MANAGER_MASTER_BUNDLE;

        string[] pullManifestCommand = { "-d pull", "\"" + manifestFilePath + "\"", "\"" + absoluteTempPath + "\"" };

        string output, error;
        if (QiyuADBTools.GetMe().RunCommand(pullManifestCommand, null, out output, out error) == 0)
        {
            AssetBundle remoteBundle = AssetBundle.LoadFromFile(Path.Combine(absoluteTempPath, BUNDLE_MANAGER_MASTER_BUNDLE));
            if (remoteBundle == null)
            {
                QiyuSceneQuickPreviewWindow.PrintLog("Failed to load remote asset bundle manifest file.", QiyuSceneQuickPreviewWindow.LogType.Error);
                return false;
            }
            AssetBundleManifest remoteManifest = remoteBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            Dictionary<string, Hash128> remoteBundleToHash = new Dictionary<string, Hash128>();
            if (remoteManifest != null)
            {
                string[] assetBundles = remoteManifest.GetAllAssetBundles();
                foreach (string bundleName in assetBundles)
                {
                    remoteBundleToHash[bundleName] = remoteManifest.GetAssetBundleHash(bundleName);
                }
            }
            remoteBundle.Unload(true);

            AssetBundle localBundle = AssetBundle.LoadFromFile(BUNDLE_MANAGER_OUTPUT_PATH + "\\" + BUNDLE_MANAGER_MASTER_BUNDLE
                    + "\\" + BUNDLE_MANAGER_MASTER_BUNDLE);
            if (localBundle == null)
            {
                QiyuSceneQuickPreviewWindow.PrintLog("Failed to load local asset bundle manifest file.", QiyuSceneQuickPreviewWindow.LogType.Error);
                return false;
            }
            AssetBundleManifest localManifest = localBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            if (localManifest != null)
            {
                Hash128 zeroHash = new Hash128(0, 0, 0, 0);

                string relativeSceneBundlesPath = Path.Combine(BUNDLE_MANAGER_OUTPUT_PATH, BUNDLE_MANAGER_MASTER_BUNDLE);
                bundlesToTransfer.Add(Path.Combine(relativeSceneBundlesPath, BUNDLE_MANAGER_MASTER_BUNDLE));
                string[] assetBundles = localManifest.GetAllAssetBundles();
                foreach (string bundleName in assetBundles)
                {
                    if (!remoteBundleToHash.ContainsKey(bundleName))
                    {
                        bundlesToTransfer.Add(Path.Combine(relativeSceneBundlesPath, bundleName));
                    }
                    else
                    {
                        if (remoteBundleToHash[bundleName] != localManifest.GetAssetBundleHash(bundleName))
                        {
                            bundlesToTransfer.Add(Path.Combine(relativeSceneBundlesPath, bundleName));
                        }
                        remoteBundleToHash[bundleName] = zeroHash;
                    }
                }

                QiyuSceneQuickPreviewWindow.PrintLog(bundlesToTransfer.Count + " dirty bundle(s) will be transferred.\n", QiyuSceneQuickPreviewWindow.LogType.Normal);
            }
        }
        else
        {
            if (output.Contains("does not exist") || output.Contains("No such file or directory"))
            {
                QiyuSceneQuickPreviewWindow.PrintLog("Manifest file not found. Transferring all bundles . . . ", QiyuSceneQuickPreviewWindow.LogType.Normal);

                string[] mkdirCommand = { "-d shell", "mkdir -p", "\"" + externalSceneCache + "\"" };
                if (QiyuADBTools.GetMe().RunCommand(mkdirCommand, null, out output, out error) == 0)
                {
                    string absoluteSceneBundlePath = Path.Combine(Path.Combine(Application.dataPath, ".."),
                            Path.Combine(BUNDLE_MANAGER_OUTPUT_PATH, BUNDLE_MANAGER_MASTER_BUNDLE));

                    string[] assetBundlePaths = Directory.GetFiles(absoluteSceneBundlePath);
                    if (assetBundlePaths.Length == 0)
                    {
                        QiyuSceneQuickPreviewWindow.PrintLog("Failed to locate scene bundles to transfer.", QiyuSceneQuickPreviewWindow.LogType.Error);
                        return false;
                    }
                    foreach (string path in assetBundlePaths)
                    {
                        if (!path.Contains(".manifest"))
                        {
                            bundlesToTransfer.Add(path);
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(error) || output.Contains("error"))
        {
            QiyuSceneQuickPreviewWindow.PrintLog(string.IsNullOrEmpty(error) ? output : error, QiyuSceneQuickPreviewWindow.LogType.Error);
            return false;
        }

        foreach (string bundle in bundlesToTransfer)
        {
            string absoluteBundlePath = Path.Combine(Path.Combine(Application.dataPath, ".."), bundle);
            string[] pushBundleCommand = { "-d push", "\"" + absoluteBundlePath + "\"", "\"" + externalSceneCache + "\"" };
            QiyuADBTools.GetMe().RunCommandAsync(pushBundleCommand, null);
        }

        return true;
    }

    public bool IsInstalledAPP()
    {
        if (!QiyuADBTools.GetMe().IsAdbAvailable())
        {
            return false;
        }

        string matchedPackageList, error;
        var packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

        string[] packageCheckCommand = new string[] { "-d shell pm list package", packageName };
        if (QiyuADBTools.GetMe().RunCommand(packageCheckCommand, null, out matchedPackageList, out error) == 0)
        {
            if (string.IsNullOrEmpty(matchedPackageList))
            {
                return false;
            }

            if (!matchedPackageList.Contains("package:" + packageName + "\r\n"))
            {
                return false;
            }

            string[] dumpPackageInfoCommand = new string[] { "-d shell dumpsys package", packageName };
            string packageInfo;
            if (QiyuADBTools.GetMe().RunCommand(dumpPackageInfoCommand, null, out packageInfo, out error) == 0 &&
                    !string.IsNullOrEmpty(packageInfo))
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public void PrebuildProjectSettingUpdate()
    {
        targetArchitecture = PlayerSettings.Android.targetArchitectures;
        scriptBackend = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
        managedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android);
        stripEngineCode = PlayerSettings.stripEngineCode;

        if (targetArchitecture != AndroidArchitecture.ARMv7)
        {
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
        }

        if (scriptBackend != ScriptingImplementation.Mono2x)
        {
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.Mono2x);
        }

        if (managedStrippingLevel != ManagedStrippingLevel.Disabled)
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);
        }

        if (stripEngineCode)
        {
            PlayerSettings.stripEngineCode = false;
        }
    }

    
    private  void PostbuildProjectSettingUpdate()
    {
        if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) != scriptBackend)
        {
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, scriptBackend);
        }

        if (PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android) != managedStrippingLevel)
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, managedStrippingLevel);
        }

        if (PlayerSettings.stripEngineCode != stripEngineCode)
        {
            PlayerSettings.stripEngineCode = stripEngineCode;
        }

        if (PlayerSettings.Android.targetArchitectures != targetArchitecture)
        {
            PlayerSettings.Android.targetArchitectures = targetArchitecture;
        }
    }

    public string GetPlayerActivityName()
    {
        return "\"" + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) + "/com.unity3d.player.UnityPlayerActivity\"";
    }
#endif
}
