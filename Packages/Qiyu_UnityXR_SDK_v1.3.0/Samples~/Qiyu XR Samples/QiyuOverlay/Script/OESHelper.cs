using Qiyu.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.Qiyu;
using UnityEngine;

enum RenderEventType
{
    InitTexture,
    UpdateTexture,
};


public class OESHelper : MonoBehaviour
{
#if (UNITY_IOS || UNITY_TVOS || UNITY_WEBGL) && !UNITY_EDITOR
    private const string pluginName = "__Internal";
#elif UNITY_ANDROID
    private const string pluginName = "OESHelper";
#endif

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetRenderEventFunc();

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int GetOESTexture();

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void PlayVideo(string url);

    [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void StopVideo();

    public string playUrl = "";
    public string TAG = "OESHelperTest";

    bool isPlaying = false;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log(TAG + " Start");
        if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
        {
            Debug.Log(TAG + "Graphics Api is not Opengl ES3.");
            yield break;
        }
        if (QiyuPlatform.IsAndroid)
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventType.InitTexture);

        yield return new WaitForSeconds(1);
        Debug.Log(TAG + " PlayVideo");
        if (QiyuPlatform.IsAndroid)
        {
            PlayVideo(string.IsNullOrEmpty(playUrl) ? "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4" : playUrl);
            GetComponent<QiyuOverlay>().SetAndroidOESTextureID(GetOESTexture());
            isPlaying = true;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying && QiyuPlatform.IsAndroid)
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventType.UpdateTexture);
    }

    private void OnDestroy()
    {
        if (QiyuPlatform.IsAndroid)
            StopVideo();
    }
}
