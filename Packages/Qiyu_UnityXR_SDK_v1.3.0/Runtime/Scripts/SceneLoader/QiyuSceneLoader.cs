using Qiyu.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QiyuSceneLoader : MonoBehaviour
{
    public const string RESOURCE_BUNDLE_NAME = "asset_resources";
    private const string EXTERNAL_STORAGE_PATH = "/sdcard/Android/data";
    private const string SCENE_LOADER_LIST = "SceneList.txt";
    private const string QIYU_SCENE_LOADER = "QiyuPreviewSceneLoader";
    private const string CACHE_SCENES_PATH = "cache/scenes";

    private struct SceneInfo
    {
        public List<string> scenes;
        public long version;

        public SceneInfo(List<string> sceneList, long currentVersion)
        {
            scenes = sceneList;
            version = currentVersion;
        }
    }

    private string scenePath = "";
    private string sceneLoadDataPath = "";
    private SceneInfo currentSceneInfo;
    static List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        string appPath = Path.Combine(EXTERNAL_STORAGE_PATH, Application.identifier);
        scenePath = Path.Combine(appPath, CACHE_SCENES_PATH);
        sceneLoadDataPath = Path.Combine(scenePath, SCENE_LOADER_LIST);
        currentSceneInfo = GetSceneInfo();
        if (0 != currentSceneInfo.version && !string.IsNullOrEmpty(currentSceneInfo.scenes[0]))
        {
            LoadScene(currentSceneInfo);
        }
    }

    private SceneInfo GetSceneInfo()
    {
        SceneInfo sceneInfo = new SceneInfo();
        try
        {
            StreamReader reader = new StreamReader(sceneLoadDataPath);
            sceneInfo.version = Convert.ToInt64(reader.ReadLine());
            List<string> sceneList = new List<string>();
            while (!reader.EndOfStream)
            {
                sceneList.Add(reader.ReadLine());
            }
            sceneInfo.scenes = sceneList;
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        return sceneInfo;
    }

    private void LoadScene(SceneInfo sceneInfo)
    {
        AssetBundle mainSceneBundle = null;
        string[] bundles = Directory.GetFiles(scenePath, "*_*");
        string mainSceneBundleFileName = "scene_" + sceneInfo.scenes[0].ToLower();
        try
        {
            foreach (string bundle in bundles)
            {
                var assetBundle = AssetBundle.LoadFromFile(bundle);

                if (null != assetBundle)
                {
                    loadedAssetBundles.Add(assetBundle);
                }

                if (assetBundle.name == mainSceneBundleFileName)
                {
                    mainSceneBundle = assetBundle;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        if (null != mainSceneBundle)
        {
            string[] scenePaths = mainSceneBundle.GetAllScenePaths();
            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            loadSceneOperation.completed += LoadSceneCompleted;
        }
    }

    private void LoadSceneCompleted(AsyncOperation obj)
    {
        StartCoroutine(CheckSceneChangeed());
    }

    IEnumerator CheckSceneChangeed()
    {
        var waitTime = new WaitForSeconds(1f);
        SceneInfo newSceneInfo;
        while (true)
        {
            yield return waitTime;
            newSceneInfo = GetSceneInfo();
            if (newSceneInfo.version != currentSceneInfo.version)
            {
                foreach (var assetBundle in loadedAssetBundles)
                {
                    if (null != assetBundle)
                    {
                        assetBundle.Unload(false);
                    }
                }
                loadedAssetBundles.Clear();
                int activeScenes = SceneManager.sceneCount;

                for (int i = 1; i < activeScenes; i++)
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                }
                SceneManager.LoadScene(QIYU_SCENE_LOADER, LoadSceneMode.Single);
                break;
            }          
        }
    }
}
