using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif

[InitializeOnLoad]
public class QiyuGradleGeneration
#if UNITY_ANDROID
    : IPostGenerateGradleAndroidProject
#endif
{
    public int callbackOrder { get { return 1; } }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        UnityEngine.Debug.Log("QiyuGradleGeneration triggered.");
        PatchAndroidManifest(path);
    }

    public void PatchAndroidManifest(string path)
    {
        string manifestFolder = Path.Combine(path, "src/main");
        string file = manifestFolder + "/AndroidManifest.xml";   
        QiyuSDKEditor.PatchAndroidManifest(file);
    }

}
