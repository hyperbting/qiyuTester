//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using static Unity.XR.Qiyu.QiyuXRCorePlugin;

namespace Unity.XR.Qiyu
{
    public class QiyuBoundary
    {
        /// <summary>
        /// ½Úµã
        /// </summary>
        public enum Node
        {
            HandLeft = QiyuXRCorePlugin.Node.HandLeft,
            HandRight = QiyuXRCorePlugin.Node.HandRight,
            Head = QiyuXRCorePlugin.Node.Head,
        }

        public struct BoundaryTestResult
        {
            public bool IsTriggering;
            public float ClosestDistance;
            public Vector3 ClosestPoint;
            public Vector3 ClosestPointNormal;

            public override string ToString()
            {
                return string.Format("IsTriggering:{0},ClosestDistance:{1},ClosestPoint:{2},ClosestPointNormal:{3}", IsTriggering, ClosestDistance, ClosestPoint.ToString("f3"), ClosestPointNormal.ToString("f3"));
            }
        }

        public static bool GetConfigured()
        {
            if (!QiyuPlatform.IsAndroid)
            {
                return false;
            }

            return QiyuXRCorePlugin.QVR_GetBoundaryConfigured() == Bool.True;
        }

        public static BoundaryTestResult TestNode(Node node)
        {
            Vector3 point = Vector3.zero;
            if (node == Node.Head)
            {
                UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head).TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out point);
            }
            else if (node == Node.HandLeft)
            {
                UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out point);
            }
            else if (node == Node.HandRight)
            {
                UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out point);
            }

            return TestPoint(point);
        }

        public static QiyuBoundary.BoundaryTestResult TestPoint(Vector3 point)
        {
            if (!QiyuPlatform.IsAndroid)
            {
                return new BoundaryTestResult();
            }

            QiyuXRCorePlugin.BoundaryTestResult qvrRes = QiyuXRCorePlugin.QVR_TestBoundaryPoint(point.ToVector3f(), QiyuXRCorePlugin.BoundaryType.OuterBoundary);

            BoundaryTestResult res = new BoundaryTestResult()
            {
                IsTriggering = (qvrRes.IsTriggering == QiyuXRCorePlugin.Bool.True),
                ClosestDistance = qvrRes.ClosestDistance,
                ClosestPoint = qvrRes.ClosestPoint.FromVector3f(),
                ClosestPointNormal = qvrRes.ClosestPointNormal.FromVector3f(),
            };

            return res;
        }

        private static int cachedVector3fSize = Marshal.SizeOf(typeof(QiyuXRCorePlugin.Vector3f));
        private static QiyuXRCorePlugin.NativeBuffer cachedGeometryNativeBuffer = new QiyuXRCorePlugin.NativeBuffer(0);
        private static float[] cachedGeometryManagedBuffer = new float[0];

        public static Vector3[] GetGeometry()
        {
            if (!QiyuPlatform.IsAndroid)
            {
                return new Vector3[0];
            }

            int pointsCount = 0;
            if (QiyuXRCorePlugin.QVR_GetBoundaryGeometry(QiyuXRCorePlugin.BoundaryType.OuterBoundary, IntPtr.Zero, ref pointsCount) == Bool.True)
            {
                if (pointsCount > 0)
                {
                    int requiredNativeBufferCapacity = pointsCount * cachedVector3fSize;
                    if (cachedGeometryNativeBuffer.GetCapacity() < requiredNativeBufferCapacity)
                        cachedGeometryNativeBuffer.Reset(requiredNativeBufferCapacity);

                    int requiredManagedBufferCapacity = pointsCount * 3;
                    if (cachedGeometryManagedBuffer.Length < requiredManagedBufferCapacity)
                        cachedGeometryManagedBuffer = new float[requiredManagedBufferCapacity];

                    if (QiyuXRCorePlugin.QVR_GetBoundaryGeometry(QiyuXRCorePlugin.BoundaryType.OuterBoundary, cachedGeometryNativeBuffer.GetPointer(), ref pointsCount) == Bool.True)
                    {
                        Marshal.Copy(cachedGeometryNativeBuffer.GetPointer(), cachedGeometryManagedBuffer, 0, requiredManagedBufferCapacity);

                        Vector3[] points = new Vector3[pointsCount];
                        for (int i = 0; i < pointsCount; i++)
                        {
                            points[i] = new QiyuXRCorePlugin.Vector3f()
                            {
                                x = cachedGeometryManagedBuffer[3 * i + 0],
                                y = cachedGeometryManagedBuffer[3 * i + 1],
                                z = cachedGeometryManagedBuffer[3 * i + 2],
                            }.FromVector3f();
                        }

                        return points;
                    }
                }
            }
            return new Vector3[0];
        }

        public static Vector3 GetDimensions()
        {
            if (!QiyuPlatform.IsAndroid)
            {
                return Vector3.one;
            }

            return QiyuXRCorePlugin.QVR_GetBoundaryDimensions(QiyuXRCorePlugin.BoundaryType.OuterBoundary).FromVector3f();
        }

        public static bool GetVisible()
        {
            if (!QiyuPlatform.IsAndroid)
            {
                return false;
            }

            return QiyuXRCorePlugin.QVR_GetBoundaryVisible() == Bool.True;
        }

        public static void SetVisible(bool value)
        {
            if (!QiyuPlatform.IsAndroid)
            {
                return;
            }
            QiyuXRCorePlugin.QVR_SetBoundaryVisible(ToBool(value));
        }

        public static float FloorCameraYOffset
        {
            get
            {
                Vector3f v3 = new Vector3f();
                Quatf q4 = new Quatf();
                QiyuXRCorePlugin.QVR_GetOriginAxis(ref v3, ref q4);
                return -QiyuXRCorePlugin.QVR_GetFloorLevel() - v3.y;
            }
        }

        public static bool GetBoundaryDownHeadShow()
        {
            if (!QiyuPlatform.IsAndroid)
                return false;

            return QiyuXRCorePlugin.QVR_GetBoundaryDownHeadShow() == Bool.True;
        }

        public static bool SetBoundaryDownHeadShow(bool value)
        {
            if (!QiyuPlatform.IsAndroid)
                return false;

            return QiyuXRCorePlugin.QVR_SetBoundaryDownHeadShow(ToBool(value)) == Bool.True;
        }
    }
}
