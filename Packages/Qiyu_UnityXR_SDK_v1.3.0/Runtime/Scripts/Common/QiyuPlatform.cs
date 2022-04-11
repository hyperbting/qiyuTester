//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
namespace Unity.XR.Qiyu
{
    public class QiyuPlatform
    {
        private static bool sIsAndroid;

        static QiyuPlatform()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            sIsAndroid = true;
#else
            sIsAndroid= false;
#endif
        }

        public static bool IsAndroid
        {
            get { return sIsAndroid; }
        }
    }
}