//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.XR.Qiyu
{
    public static partial class QiyuXRCorePlugin
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Sizei
        {
            public int w;
            public int h;

            public static readonly Sizei zero = new Sizei { w = 0, h = 0 };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Sizef
        {
            public float w;
            public float h;

            public static readonly Sizef zero = new Sizef { w = 0, h = 0 };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector2i
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Recti
        {
            Vector2i Pos;
            Sizei Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rectf
        {
            Vector2f Pos;
            Sizef Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Frustumf
        {
            public float zNear;
            public float zFar;
            public float fovX;
            public float fovY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Frustumf2
        {
            public float zNear;
            public float zFar;
            public Fovf Fov;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Vector2f
        {
            public float x;
            public float y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3f
        {
            public float x;
            public float y;
            public float z;
            public static readonly Vector3f zero = new Vector3f { x = 0.0f, y = 0.0f, z = 0.0f };
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}", x, y, z);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Quatf
        {
            public float x;
            public float y;
            public float z;
            public float w;
            public static readonly Quatf identity = new Quatf { x = 0.0f, y = 0.0f, z = 0.0f, w = 1.0f };
            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Posef
        {
            public Quatf Orientation;
            public Vector3f Position;
            public static readonly Posef identity = new Posef { Orientation = Quatf.identity, Position = Vector3f.zero };
            public override string ToString()
            {
                return string.Format("Position ({0}), Orientation({1})", Position, Orientation);
            }
        }
        public enum Bool
        {
            False = 0,
            True
        }



        public enum Node
        {
            None = -1,
            EyeLeft = 0,
            EyeRight = 1,
            EyeCenter = 2,
            HandLeft = 3,
            HandRight = 4,
            TrackerZero = 5,
            TrackerOne = 6,
            TrackerTwo = 7,
            TrackerThree = 8,
            Head = 9,
            DeviceObjectZero = 10,
            Count,
        }

        public enum BoundaryType
        {
            OuterBoundary = 1,
            PlayArea = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BoundaryTestResult
        {
            public Bool IsTriggering;
            public float ClosestDistance;
            public Vector3f ClosestPoint;
            public Vector3f ClosestPointNormal;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BoundaryGeometry
        {
            public BoundaryType BoundaryType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public Vector3f[] Points;
            public int PointsCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Colorf
        {
            public float r;
            public float g;
            public float b;
            public float a;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Fovf
        {
            public float UpTan;
            public float DownTan;
            public float LeftTan;
            public float RightTan;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ControllerData
        {
            public int isConnect;
            public int button;
            public int buttonTouch;
            public int batteryLevel;
            public int triggerForce;
            public int gripForce;
            public int isShow;
            public Vector2f joyStickPos;

            public Vector3f position;
            public Quatf rotation;

            public Vector3f velocity;
            public Vector3f acceleration;

            public Vector3f angVelocity;
            public Vector3f angAcceleration;
        }

        public static Color FromColorf(this Colorf c)
        {
            return new Color() { r = c.r, g = c.g, b = c.b, a = c.a };
        }

        public static Colorf ToColorf(this Color c)
        {
            return new Colorf() { r = c.r, g = c.g, b = c.b, a = c.a };
        }

        public static Vector3 FromVector3f(this Vector3f v)
        {
            return new Vector3() { x = v.x, y = v.y, z = v.z };
        }

        public static void WirteToVector3(this Vector3f v, ref Vector3 ret)
        {
            ret.x = v.x; ret.y = v.y; ret.z = v.z;
        }

        public static void WirteToVector2(this Vector2f v, ref Vector2 ret)
        {
            ret.x = v.x; ret.y = v.y;
        }

        public static Vector3 FromFlippedZVector3f(this Vector3f v)
        {
            return new Vector3() { x = v.x, y = v.y, z = -v.z };
        }

        public static Vector3f ToVector3f(this Vector3 v)
        {
            return new Vector3f() { x = v.x, y = v.y, z = v.z };
        }

        public static Vector3f ToFlippedZVector3f(this Vector3 v)
        {
            return new Vector3f() { x = v.x, y = v.y, z = -v.z };
        }

        public static Quaternion FromQuatf(this Quatf q)
        {
            return new Quaternion() { x = q.x, y = q.y, z = q.z, w = q.w };
        }

        public static void WriteToQuaternion(this Quatf q, ref Quaternion ret)
        {
            ret.x = q.x; ret.y = q.y; ret.z = q.z; ret.w = q.w;
        }

        public static Quaternion FromFlippedZQuatf(this Quatf q)
        {
            return new Quaternion() { x = -q.x, y = -q.y, z = q.z, w = q.w };
        }

        public static Quatf ToQuatf(this Quaternion q)
        {
            return new Quatf() { x = q.x, y = q.y, z = q.z, w = q.w };
        }

        public static Quatf ToFlippedZQuatf(this Quaternion q)
        {
            return new Quatf() { x = -q.x, y = -q.y, z = q.z, w = q.w };
        }

        public static Bool ToBool(bool b)
        {
            return (b) ? Bool.True : Bool.False;
        }

        public class NativeBuffer : IDisposable
        {
            private bool disposed = false;
            private int m_numBytes = 0;
            private IntPtr m_ptr = IntPtr.Zero;

            public NativeBuffer(int numBytes)
            {
                Reallocate(numBytes);
            }

            ~NativeBuffer()
            {
                Dispose(false);
            }

            public void Reset(int numBytes)
            {
                Reallocate(numBytes);
            }

            public int GetCapacity()
            {
                return m_numBytes;
            }

            public IntPtr GetPointer(int byteOffset = 0)
            {
                if (byteOffset < 0 || byteOffset >= m_numBytes)
                    return IntPtr.Zero;
                return (byteOffset == 0) ? m_ptr : new IntPtr(m_ptr.ToInt64() + byteOffset);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposed)
                    return;

                Release();

                disposed = true;
            }

            private void Reallocate(int numBytes)
            {
                Release();

                if (numBytes > 0)
                {
                    m_ptr = Marshal.AllocHGlobal(numBytes);
                    m_numBytes = numBytes;
                }
                else
                {
                    m_ptr = IntPtr.Zero;
                    m_numBytes = 0;
                }
            }

            private void Release()
            {
                if (m_ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(m_ptr);
                    m_ptr = IntPtr.Zero;
                    m_numBytes = 0;
                }
            }
        }

        
    }
}
