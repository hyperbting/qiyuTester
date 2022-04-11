//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
namespace Qiyu.Sdk.Platform
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