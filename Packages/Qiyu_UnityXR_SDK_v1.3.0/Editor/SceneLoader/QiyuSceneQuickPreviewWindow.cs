using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QiyuSceneQuickPreviewWindow : EditorWindow
{
#if UNITY_EDITOR_WIN && UNITY_ANDROID
    static string log;
    static GUIStyle logStyle;

    bool isDebug;

    public enum LogType
    {
        Normal,
        Success,
        Error
    }

    //Show Scene Quick Preview Window
    public static void Init()
    {
        QiyuSceneQuickPreviewWindow window = (QiyuSceneQuickPreviewWindow)GetWindow(typeof(QiyuSceneQuickPreviewWindow));
        window.minSize = new Vector2(500,300);
        window.maxSize = new Vector2(500, 300);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Scene Settings", EditorStyles.boldLabel);
        GUILayout.Space(5.0f);

        EditorGUILayout.BeginHorizontal();
        {
            string select_scene_txt = "Scene Select";
            var sceneSelectBtn = GUILayoutUtility.GetRect(new GUIContent(select_scene_txt), GUI.skin.button, GUILayout.Width(100));
            if (GUI.Button(sceneSelectBtn, select_scene_txt))
            {
                SceneSelect();
            };

            GUILayout.Space(30.0f);
            string clean_catch_txt = "Clean Cache Bundles";
            var cleanCatchBtn = GUILayoutUtility.GetRect(new GUIContent(clean_catch_txt), GUI.skin.button, GUILayout.Width(150));
            if (GUI.Button(cleanCatchBtn, clean_catch_txt))
            {
                CleanBundleCache();
                GUIUtility.ExitGUI();
            };

            GUILayout.Space(10.0f);
            GUIContent forceRestartLabel = new GUIContent("Debug Mode", "if debug mode is change,please uninstall app first and rebuild.");
            isDebug = GUILayout.Toggle(isDebug, forceRestartLabel, GUILayout.Width(100));
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10.0f);
        GUILayout.Label("Build scene and copy to device.", EditorStyles.boldLabel);
        GUILayout.Space(10.0f);

        EditorGUILayout.BeginHorizontal();
        {
            string build_scene_txt = "Build Scene";
            var buildSceneBtn = GUILayoutUtility.GetRect(new GUIContent(build_scene_txt), GUI.skin.button, GUILayout.Width(150));
            if (GUI.Button(buildSceneBtn, build_scene_txt))
            {
                BuildScene();
                GUIUtility.ExitGUI();
            };

            GUILayout.Space(10.0f);

            string build_scene_and_restart_txt = "Build Scene And Restart";
            var buildSceneReStartBtn = GUILayoutUtility.GetRect(new GUIContent(build_scene_and_restart_txt), GUI.skin.button, GUILayout.Width(150));
            if (GUI.Button(buildSceneReStartBtn, build_scene_and_restart_txt))
            {
                BuildSceneAndRestart();
                GUIUtility.ExitGUI();
            };
            
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5.0f);
        GUILayout.Label("Device Tools", EditorStyles.boldLabel);

        GUILayout.Space(10.0f);

        EditorGUILayout.BeginHorizontal();
        {
            string adb_restart_app_txt = "Start or restart App";
            var reStartBtn = GUILayoutUtility.GetRect(new GUIContent(adb_restart_app_txt), GUI.skin.button, GUILayout.Width(150));
            if (GUI.Button(reStartBtn, adb_restart_app_txt))
            {
                RestartApp();
                GUIUtility.ExitGUI();
            };
            GUILayout.Space(10.0f);
            string uninstall_app = "Uninstall App";
            var uninstallAppBtn = GUILayoutUtility.GetRect(new GUIContent(uninstall_app), GUI.skin.button, GUILayout.Width(100));
            if (GUI.Button(uninstallAppBtn, uninstall_app))
            {
                UninstallApp();
                GUIUtility.ExitGUI();
            };           
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10.0f);
        GUILayout.Label("Log Info", EditorStyles.boldLabel);

        GUILayout.Space(10.0f);
        if (!string.IsNullOrEmpty(log))
        {
            InitLogUI();
            GUILayout.Label(log, logStyle, GUILayout.Height(30.0f));
        }
        GUIUtility.ExitGUI();
    }

    private static void InitLogUI()
    {
        if (logStyle == null)
        {
            logStyle = new GUIStyle();
            logStyle.margin.left = 5;
            logStyle.wordWrap = true;
            logStyle.normal.textColor = logStyle.focused.textColor = EditorStyles.label.normal.textColor;
            logStyle.richText = true;
        }
    }

    public static void PrintLog(string message, LogType type)
    {
        switch (type)
        {
            case LogType.Normal:
                {
                    log = message;
                    break;
                }
            case LogType.Success:
                {
                    log = "<color=green>Success!\n" + message + "</color>\n";
                    break;
                }
            case LogType.Error:
                {
                    log = "<color=red>Failed!\n" + message + "</color>\n";
                    break;
                }
        }
    }

    /// <summary>
    /// from build setting select scene for build.
    /// </summary>
    void SceneSelect()
    {
        GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
    }


    void BuildScene()
    {
        QiyuSceneQuickPreviewManager.GetMe().BuildScene(isDebug);
    }

    void BuildSceneAndRestart()
    {
        QiyuSceneQuickPreviewManager.GetMe().BuildSceneAndRestart(isDebug);
    }

    void RestartApp()
    {
        QiyuSceneQuickPreviewManager.GetMe().RestartApp();
    }

    void UninstallApp()
    {
        QiyuSceneQuickPreviewManager.GetMe().UninstallApp();
    }

    void CleanBundleCache()
    {
        QiyuSceneQuickPreviewManager.GetMe().CleanBundleCache();
    }
#endif
}
