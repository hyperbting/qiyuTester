using UnityEngine;

    namespace Unity.XR.Qiyu
{
    public static partial class Utils
    {
        public static Quaternion ConvertQuaternionWith2Vector(Vector3 formVec, Vector3 toVec)
        {
            Quaternion res = Quaternion.FromToRotation(formVec, toVec);
            return res;
        }
    }
}