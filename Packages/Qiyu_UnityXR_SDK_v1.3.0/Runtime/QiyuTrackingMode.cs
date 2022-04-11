using UnityEngine;
using static Unity.XR.Qiyu.NativeMethods;

namespace Unity.XR.Qiyu
{
    public static partial class Utils
    {
        /// <summary>
        /// enable/disable tracking position
        /// </summary>
        public static void SetTrackingPosition(bool value)
        {
            NativeMethods.SetTrackingPosition(value);
        }

        /// <summary>
        /// Whether tracking position is enable
        /// </summary>
        /// <returns>Whether tracking position is enable</returns>
        public static bool GetTrackingPosition()
        {
            return NativeMethods.GetTrackingPosition();
        }
    }
}