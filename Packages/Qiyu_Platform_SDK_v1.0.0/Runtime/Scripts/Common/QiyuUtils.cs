//=============================================================================
//
//          Copyright (c) 2022 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using Newtonsoft.Json;
using System;

namespace Qiyu.Sdk.Platform
{
    public class QiyuUtils
    {
        public static string ParseToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static long GetTimestamp()
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (DateTime.Now.Ticks - dt1970.Ticks) / 100;
        }

        static long beginTime_;
        public static void BeginTime()
        {
            beginTime_ = GetTimestamp();
        }

        public static long EndTime()
        {
            return GetTimestamp() - beginTime_;
        }
    }
}
