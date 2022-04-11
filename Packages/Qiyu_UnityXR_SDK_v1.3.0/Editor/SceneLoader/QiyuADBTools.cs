using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class QiyuADBTools
{
    private static QiyuADBTools _me;

    private static bool isAdbAvailable;
    public string androidSdkRoot;
    public string androidPlatformToolsPath;
    public string adbPath;

    public string TAG = "QiyuADBTools::";
    private StringBuilder outputStringBuilder = null;
    private StringBuilder errorStringBuilder = null;

    public static QiyuADBTools GetMe()
    {
        if (_me == null)
            _me = new QiyuADBTools();
        return _me;
    }

    private QiyuADBTools()
    {
        androidSdkRoot = GetAndroidSDKPath();
        if (androidSdkRoot.EndsWith("\\") || androidSdkRoot.EndsWith("/"))
        {
            androidSdkRoot = androidSdkRoot.Remove(androidSdkRoot.Length - 1);
        }
        androidPlatformToolsPath = Path.Combine(androidSdkRoot, "platform-tools");
        adbPath = Path.Combine(androidPlatformToolsPath, "adb.exe");
        isAdbAvailable = File.Exists(adbPath);
    }

    public string GetAndroidSDKPath()
    {
        string androidSDKPath = "";

#if UNITY_2019_1_OR_NEWER
        bool useEmbedded = EditorPrefs.GetBool("SdkUseEmbedded") || string.IsNullOrEmpty(EditorPrefs.GetString("AndroidSdkRoot"));
        if (useEmbedded)
        {
            androidSDKPath = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "SDK");
        }
        else
#endif
        {
            androidSDKPath = EditorPrefs.GetString("AndroidSdkRoot");
        }

        androidSDKPath = androidSDKPath.Replace("/", "\\");

        if (!Directory.Exists(androidSDKPath))
        {
            androidSDKPath = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            if (!string.IsNullOrEmpty(androidSDKPath))
            {
                return androidSDKPath;
            }
            EditorUtility.DisplayDialog("Android SDK not Found", "Please ensure that the path is set correctly in External Tools.", "Ok");
            return string.Empty;
        }

        return androidSDKPath;
    }

    public bool IsAdbAvailable()
    {
        if(!isAdbAvailable)
            Debug.LogError(string.Format("{0}", ":", "Can not find adb tools.", TAG));
        return isAdbAvailable;
    }

    public int RunCommand(string[] arguments,Action callback, out string outputString, out string errorString)
    {
        int exitCode = -1;

        if (!isAdbAvailable)
        {
            Debug.LogWarning(TAG+"adb not ready");
            outputString = string.Empty;
            errorString = TAG + "adb  not ready";
            return exitCode;
        }

        string args = string.Join(" ", arguments);

        ProcessStartInfo startInfo = new ProcessStartInfo(adbPath, args);
        startInfo.WorkingDirectory = androidSdkRoot;
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        outputStringBuilder = new StringBuilder("");
        errorStringBuilder = new StringBuilder("");

        Process process = Process.Start(startInfo);
        process.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceivedHandler);
        process.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataReceivedHandler);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            do
            {
                if (callback != null)
                {
                    callback();
                }
            } while (!process.WaitForExit(100));

            process.WaitForExit();
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat(TAG+ "RunCommand exception {0}", e.Message);
        }

        exitCode = process.ExitCode;

        process.Close();

        outputString = outputStringBuilder.ToString();
        errorString = errorStringBuilder.ToString();

        outputStringBuilder = null;
        errorStringBuilder = null;

        if (!string.IsNullOrEmpty(errorString))
        {
            if (errorString.Contains("Warning"))
            {
                Debug.LogWarning(TAG + errorString);
            }
            else
            {
                Debug.LogError(TAG + errorString);
            }
        }

        return exitCode;
    }

    public Process RunCommandAsync(string[] arguments, DataReceivedEventHandler outputDataRecievedHandler)
    {
        if (!isAdbAvailable)
        {
            Debug.LogWarning("adb not ready");
            return null;
        }
        string args = string.Join(" ", arguments);

        ProcessStartInfo startInfo = new ProcessStartInfo(adbPath, args);
        startInfo.WorkingDirectory = androidSdkRoot;
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        Process process = Process.Start(startInfo);
        if (outputDataRecievedHandler != null)
        {
            process.OutputDataReceived += new DataReceivedEventHandler(outputDataRecievedHandler);
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return process;
    }

    private void OutputDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            outputStringBuilder.Append(args.Data);
            outputStringBuilder.Append(Environment.NewLine);
        }
    }

    private void ErrorDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            errorStringBuilder.Append(args.Data);
            errorStringBuilder.Append(Environment.NewLine);
        }
    }
}
