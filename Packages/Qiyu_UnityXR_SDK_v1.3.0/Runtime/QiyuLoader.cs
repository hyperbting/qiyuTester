using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using AOT;
using Unity.XR.Qiyu;
using FunPtr = System.IntPtr;
using DataPtr = System.IntPtr;
#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using Unity.XR.Qiyu.Input;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Unity.XR.Qiyu
{
#if UNITY_INPUT_SYSTEM
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    static class InputLayoutLoader
    {
        static InputLayoutLoader()
        {
            Debug.Log("InputLayoutLoader");
            RegisterInputLayouts();
        }

        public static void RegisterInputLayouts()
        {
            InputSystem.RegisterLayout<Qiyu_HMD>(
               matches: new InputDeviceMatcher()
                   .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                   .WithProduct("^(QiyuHMD)"));
            InputSystem.RegisterLayout<Qiyu_Controller>(
               matches: new InputDeviceMatcher()
                   .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                   .WithProduct(@"^(Qiyu Controller)"));
            //InputSystem.RegisterLayout<QiyuRemote>(
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct(@"Qiyu Remote"));
            //InputSystem.RegisterLayout<QiyuTrackingReference>(
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct(@"((Tracking Reference)|(^(Qiyu Rift [a-zA-Z0-9]* \(Camera)))"));
        }
    }
#endif

    public class QiyuLoader : XRLoaderHelper
#if UNITY_EDITOR
    , IXRLoaderPreInit
#endif
    {
        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();

        public delegate Quaternion ConvertRotationWith2VectorDelegate(Vector3 from, Vector3 to);

        public XRDisplaySubsystem displaySubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRDisplaySubsystem>();
            }
        }

        public XRInputSubsystem inputSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRInputSubsystem>();
            }
        }


        public override bool Initialize()
        {
#if UNITY_INPUT_SYSTEM
            InputLayoutLoader.RegisterInputLayouts();
#endif

            QiyuSettings settings = GetSettings();
            Debug.Log("Initialize++++      settings");
            if (settings != null)
            {
                NativeMethods.UserDefinedSettings userDefinedSettings = new NativeMethods.UserDefinedSettings();
                userDefinedSettings.stereoRenderingMode = (ushort) settings.GetStereoRenderingMode();
                //this value is going to be passed to sxr sdk. not like unity, in sxr linear is 0;
                userDefinedSettings.colorSpace = (ushort) ((QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0);
                if(QualitySettings.vSyncCount == 0)
                    QualitySettings.vSyncCount = 1; 
                userDefinedSettings.TrackPosition = settings.TrackPosition;
                userDefinedSettings.vSyncCount = (int)QualitySettings.vSyncCount;
                userDefinedSettings.cpuPerfLevel = 3;// (int)settings.cpuPerfLevel;
                userDefinedSettings.gpuPerfLevel = 3;// (int)settings.gpuPerfLevel;
                userDefinedSettings.optionFlags = (int)settings.optionFlags;
                userDefinedSettings.interPupilDistance = settings.InterPupilDistance;
                userDefinedSettings.foveationLevel = (int)settings.foveationLevel;
                userDefinedSettings.foveationArea = settings.foveationArea;
                userDefinedSettings.foveationGain = settings.foveationGain;
                userDefinedSettings.foveationMinmum = settings.foveationMinmum;
                userDefinedSettings.isMultiThreadRendering = SystemInfo.graphicsMultiThreaded;
                userDefinedSettings.blitType = settings.blitType;
                
                unsafe{
                    byte* addr = (byte*)(&userDefinedSettings.stereoRenderingMode);
                    Debug.Log("Initialize++++ " + ((byte*)(&userDefinedSettings.stereoRenderingMode) - addr)
                      + " " +( (byte*)&userDefinedSettings.colorSpace - addr)
                      //+ " " + ((byte*)&userDefinedSettings.useDefaultRenderTexture - addr)
                      + " " + ((byte*)&userDefinedSettings.cpuPerfLevel - addr)
                      + " " + ((byte*)&userDefinedSettings.gpuPerfLevel - addr)
                      + " " + ((byte*)&userDefinedSettings.optionFlags - addr)
                      + " " + ((byte*)&userDefinedSettings.interPupilDistance - addr)
                      + " " + ((byte*)&userDefinedSettings.vSyncCount - addr)
                      //+ " " + ((byte*)&userDefinedSettings.TrackEyes - addr)
                      + " " + ((byte*)&userDefinedSettings.TrackPosition - addr)
                      + " " + ((byte*)&userDefinedSettings.isMultiThreadRendering - addr)
                      + " " + ((byte*)&userDefinedSettings.blitType - addr)
                      + " " + sizeof(NativeMethods.UserDefinedSettings));

                    Debug.Log("Initialize++++ " + userDefinedSettings.stereoRenderingMode
                      + " " + userDefinedSettings.colorSpace
                      //+ " " + userDefinedSettings.useDefaultRenderTexture
                      + " " + userDefinedSettings.cpuPerfLevel
                      + " " + userDefinedSettings.gpuPerfLevel
                      + " " + userDefinedSettings.optionFlags
                      + " " + userDefinedSettings.interPupilDistance
                      + " " + userDefinedSettings.vSyncCount
                      //+ " " + userDefinedSettings.TrackEyes
                      + " " + userDefinedSettings.TrackPosition
                      + " " + userDefinedSettings.isMultiThreadRendering
                      + " " + userDefinedSettings.blitType
                      + " " + sizeof(NativeMethods.UserDefinedSettings));
                }
                NativeMethods.SetUserDefinedSettings( ref userDefinedSettings);
            }
            

            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "QiyuVR display");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "QiyuVR input");


            if (displaySubsystem == null || inputSubsystem == null)
            {
                Debug.LogError("Unable to start QIYU XR Plugin.");
            }

            if (displaySubsystem == null)
            {
                Debug.LogError("Failed to load display subsystem.");
            }

            if (inputSubsystem == null)
            {
                Debug.LogError("Failed to load input subsystem.");
            }

 

            return displaySubsystem != null;// && inputSubsystem != null;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallBack(ulong messageId, DataPtr result, int size);
        public static FunPtr RequestProcess_Ptr
        {
            get {
                CallBack callback_delegate = MessageProcess;
                return Marshal.GetFunctionPointerForDelegate(callback_delegate);
            }
        }

        public static FunPtr ListenerProcess_Ptr
        {
            get
            {
                CallBack callback_delegate = ListenerProcess;
                return Marshal.GetFunctionPointerForDelegate(callback_delegate);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(CallBack))]
        public static void MessageProcess(ulong requestId, DataPtr result, int size)
        {
            Debug.Log("MessageProcess");
        }


        [AOT.MonoPInvokeCallback(typeof(CallBack))]
        public static void ListenerProcess(ulong messageCode, DataPtr result, int size)
        {
            Debug.Log("ListenerProcess");
        }

        public override bool Start()
        {
            Debug.Log("Start XRDisplaySubsystem");
            StartSubsystem<XRDisplaySubsystem>();
            Debug.Log("Start XRInputSubsystem");
            StartSubsystem<XRInputSubsystem>();

            Debug.Log("IQIYILoader Register QVRSDKCore");
            Qiyu.QiyuXRCoreRuntime.Instance.Register();

            return true;
        }

        public override bool Stop()
        {
            // Debug.Log("QVR_ControllerEndServer");
            // QVR_ControllerEndServer();
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();
            
            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();

            return true;
        }

#if UNITY_EDITOR && XR_MGMT_GTE_320
        private void RemoveVulkanFromAndroidGraphicsAPIs()
        {
            // don't need to do anything if auto apis is selected
            if (PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android))
                return;

            GraphicsDeviceType[] oldApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            List<GraphicsDeviceType> newApisList = new List<GraphicsDeviceType>();
            bool vulkanRemoved = false;

            // copy all entries except vulkan
            foreach (GraphicsDeviceType dev in oldApis)
            {
                if (dev == GraphicsDeviceType.Vulkan)
                {
                    vulkanRemoved = true;
                    continue;
                }
                
                newApisList.Add(dev);
            }

            // if we didn't remove Vulkan from the list, no need to do any further processing
            if (vulkanRemoved == false)
                return;

            if (newApisList.Count <= 0)
            {
                newApisList.Add(GraphicsDeviceType.OpenGLES3);
                Debug.LogWarning(
                    "Vulkan is currently experimental on Qiyu Quest. It has been removed from your list of Android graphics APIs and replaced with OpenGLES3.\n" +
                    "If you would like to use experimental Quest Vulkan support, you can add it back into the list of graphics APIs in the Player settings.");
            }
            else
            {
                Debug.LogWarning(
                    "Vulkan is currently experimental on Qiyu Quest. It has been removed from your list of Android graphics APIs.\n" +
                    "If you would like to use experimental Quest Vulkan support, you can add it back into the list of graphics APIs in the Player settings.");
            }

            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, newApisList.ToArray());
        }

        public override void WasAssignedToBuildTarget(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup == BuildTargetGroup.Android)
            {
                RemoveVulkanFromAndroidGraphicsAPIs();
            }
        }
#endif


       

        [MonoPInvokeCallback(typeof(ConvertRotationWith2VectorDelegate))]
        static Quaternion ConvertRotationWith2Vector(Vector3 from, Vector3 to)
        {
            return Utils.ConvertQuaternionWith2Vector(from, to);
        }

        public QiyuSettings GetSettings()
        {
            QiyuSettings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<QiyuSettings>("Unity.XR.Qiyu.Settings", out settings);
#else
            settings = QiyuSettings.s_Settings;
#endif
            return settings;
        }

#if UNITY_EDITOR
        public string GetPreInitLibraryName(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup)
        {
            Debug.Log("Ever in here ? GetPreInitLibraryName ...");
            return "UnityQiyuVR";
        }
#endif
    }
}
