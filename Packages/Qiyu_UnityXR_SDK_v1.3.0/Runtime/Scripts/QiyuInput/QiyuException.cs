//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;

namespace Unity.XR.Qiyu
{
    /// <summary>
    /// Qiyu Exception 类
    /// </summary>
    public class QiyuExceptions
    {
        public static Exception NotImplement = new NotImplementedException("NotImplement!");
        public static Exception NotSupported = new NotSupportedException("NotSupported!");
        public static Exception NullReference = new NullReferenceException("NullReference!");
        public static Exception IndexOutOfRange = new IndexOutOfRangeException("IndexOutOfRange!");
        public static Exception ParamInvalid = new Exception("ParamInvalid!");
    }
}