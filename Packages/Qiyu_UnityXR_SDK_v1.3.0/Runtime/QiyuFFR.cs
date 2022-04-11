using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.XR.Qiyu
{
    public enum EFoveationLevel
    {
        None = -1,
        Low = 0,
        Med = 1,
        High = 2
    }

    public static partial class Utils
    {
        /// <summary>
        /// 设置注释点渲染等级
        /// </summary>
        /// <param name="level">-1关闭，0-2分别是低中高三个等级</param>
        public static void SetFoveationLevel(int level)
        {
            NativeMethods.SetFoveationLevel(level);
        }

        public static int GetFoveationLevel()
        {
            int ret = NativeMethods.GetFoveationLevel();
            return ret;
        }

        
        /// <summary>
        /// 设置注释点渲染参数
        /// </summary>
        /// <param name="foveationGainX">x轴模糊区域大小</param>
        /// <param name="foveationGainY">y轴模糊区域大小</param>
        /// <param name="foveationArea">焦点区域大小</param>
        /// <param name="foveationMinimum">最低分辨率</param>
        public static void SetFoveationParameters(float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
        {
            NativeMethods.SetFoveationParameters(foveationGainX, foveationGainY, foveationArea, foveationMinimum);
        }

        /// <summary>
        /// Update Inter Pupil Distance from system
        /// </summary>
        public static float TryUpdateIPD()
        {
            return NativeMethods.UpdateIPD();
        }
    }
}
