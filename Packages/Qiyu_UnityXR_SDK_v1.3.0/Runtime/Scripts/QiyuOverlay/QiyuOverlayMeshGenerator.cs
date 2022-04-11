using System.Collections.Generic;
using UnityEngine;

namespace Qiyu.Overlay
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteAlways]
    public class QiyuOverlayMeshGenerator : MonoBehaviour
    {
        private Mesh _Mesh;
        private List<Vector3> _Verts = new List<Vector3>();
        private List<Vector2> _UV = new List<Vector2>();
        private List<int> _Tris = new List<int>();
        private QiyuOverlay _Overlay;
        private MeshFilter _MeshFilter;
        private MeshCollider _MeshCollider;
        private MeshRenderer _MeshRenderer;
        private Transform _Transform;

        private OverlayShape _LastShape;
        private Vector3 _LastScale;
        private MeshFace _LastMeshFace;

        public void SetOverlay(QiyuOverlay overlay)
        {
            _MeshFilter = GetComponent<MeshFilter>();
            _MeshCollider = GetComponent<MeshCollider>();
            _MeshRenderer = GetComponent<MeshRenderer>();
            _Overlay = overlay;
            _Transform = transform;
            CheckMeshStatus();
        }

        protected void OnEnable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
#endif
        }

        protected void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
#endif
        }

        private void Update()
        {
            if (_Overlay)
            {
                CheckMeshStatus();
            }
#if UNITY_EDITOR
            else
            {
                var overlay = GetComponent<QiyuOverlay>();
                if (overlay == null)
                {
                    Debug.LogError("Need add QiyuOverlay");
                }
                else
                {
                    SetOverlay(overlay);
                }
            }
#endif
        }

        private void CheckMeshStatus()
        {
            OverlayShape shape = _Overlay.currentOverlayShape;
            Vector3 scale = _Transform.lossyScale;
            Texture texture = _Overlay.texture;
            MeshFace meshFace = _Overlay.meshFace;
            if (shape == OverlayShape.Customize)
            {
                return;
            }
            if (_Mesh == null ||
                _LastShape != shape||
                _LastMeshFace != meshFace||
                _LastScale != scale)
            {
                UpdateMesh(shape, scale,meshFace);
                _LastShape = shape;
                _LastScale = scale;
                _LastMeshFace = meshFace;
            }
        }

        private void UpdateMesh(OverlayShape shape, Vector3 scale,MeshFace meshFace)
        {
            if (shape == OverlayShape.Customize)
                return;//Use default mesh
            if (_MeshFilter)
            {
                if (_Mesh == null)
                {
                    _Mesh = new Mesh() { name = "OverlayMesh" };
                }
                _Mesh.Clear();
                _Verts.Clear();
                _UV.Clear();
                _Tris.Clear();

                GenerateMesh(_Verts, _UV, _Tris, shape,meshFace,scale);

                _Mesh.SetVertices(_Verts);
                _Mesh.SetUVs(0, _UV);
                _Mesh.SetTriangles(_Tris, 0);
                _Mesh.UploadMeshData(false);

                _MeshFilter.sharedMesh = _Mesh;

                if (_MeshCollider)
                {
                    _MeshCollider.sharedMesh = _Mesh;
                }
            }
        }
        public static void GenerateMesh(List<Vector3> verts, List<Vector2> uvs, List<int> tris, OverlayShape shape,MeshFace meshFace, Vector3 scale)
        {
            switch (shape)
            {
                case OverlayShape.Sphere:
                    BuildSphere(verts, uvs, tris,meshFace);
                    break;
                case OverlayShape.Quad:
                    BuildQuad(verts, uvs, tris, meshFace);
                    break;
                case OverlayShape.Cylinder:
                    BuildHemicylinder(verts, uvs, tris, scale, meshFace);
                    break;
            }
        }

        private static Vector2 GetSphereUV(float theta, float phi, float expand_coef)
        {
            float thetaU = ((theta / (2 * Mathf.PI) - 0.5f) / expand_coef) + 0.5f;
            float phiV = ((phi / Mathf.PI) / expand_coef) + 0.5f;
            return new Vector2(thetaU, phiV);
        }

        private static Vector3 GetSphereVert(float theta, float phi)
        {
            return new Vector3(-Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(phi), -Mathf.Cos(theta) * Mathf.Cos(phi));
        }

        public static void BuildSphere(List<Vector3> verts, List<Vector2> uv, List<int> triangles,MeshFace meshFace,  int latitudes = 20, int longitudes = 20, float expand_coef = 1.0f)
        {
            Rect rect = new Rect(0, 0, 1, 1);
            latitudes = Mathf.CeilToInt(latitudes * rect.height);
            longitudes = Mathf.CeilToInt(longitudes * rect.width);

            float minTheta = Mathf.PI * 2 * (rect.x);
            float minPhi = Mathf.PI * (0.5f - rect.y - rect.height);

            float thetaScale = Mathf.PI * 2 * rect.width / longitudes;
            float phiScale = Mathf.PI * rect.height / latitudes;

            for (int j = 0; j < latitudes + 1; j += 1)
            {
                for (int k = 0; k < longitudes + 1; k++)
                {
                    float theta = minTheta + k * thetaScale;
                    float phi = minPhi + j * phiScale;

                    Vector2 suv = GetSphereUV(theta, phi, expand_coef);
                    uv.Add(new Vector2((suv.x - rect.x) / rect.width, (suv.y - rect.y) / rect.height));
                    Vector3 vert = GetSphereVert(theta, phi)/2.0f;
                    verts.Add(vert);
                }
            }

            for (int j = 0; j < latitudes; j++)
            {
                for (int k = 0; k < longitudes; k++)
                {
                    if (meshFace == MeshFace.Front)
                    {
                        triangles.Add((j * (longitudes + 1)) + k);
                        triangles.Add(((j + 1) * (longitudes + 1)) + k + 1);
                        triangles.Add(((j + 1) * (longitudes + 1)) + k);
                        triangles.Add(((j + 1) * (longitudes + 1)) + k + 1);
                        triangles.Add((j * (longitudes + 1)) + k);
                        triangles.Add((j * (longitudes + 1)) + k + 1);
                    }
                    else
                    {
                        triangles.Add((j * (longitudes + 1)) + k);
                        triangles.Add(((j + 1) * (longitudes + 1)) + k);
                        triangles.Add(((j + 1) * (longitudes + 1)) + k + 1);
                        triangles.Add(((j + 1) * (longitudes + 1)) + k + 1);
                        triangles.Add((j * (longitudes + 1)) + k + 1);
                        triangles.Add((j * (longitudes + 1)) + k);
                    }
                }
            }
        }       

        public static void BuildQuad(List<Vector3> verts, List<Vector2> uv, List<int> triangles,MeshFace meshFace)
        {
            Rect rect = new Rect(0, 0, 1, 1);
            verts.Add(new Vector3(rect.x - 0.5f, (1 - rect.y - rect.height) - 0.5f, 0));
            verts.Add(new Vector3(rect.x - 0.5f, (1 - rect.y) - 0.5f, 0));
            verts.Add(new Vector3(rect.x + rect.width - 0.5f, (1 - rect.y) - 0.5f, 0));
            verts.Add(new Vector3(rect.x + rect.width - 0.5f, (1 - rect.y - rect.height) - 0.5f, 0));

            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(0, 1));
            uv.Add(new Vector2(1, 1));
            uv.Add(new Vector2(1, 0));

            if (meshFace == MeshFace.Front)
            {
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                triangles.Add(2);
                triangles.Add(3);
                triangles.Add(0);
            }
            else
            {
                triangles.Add(0);
                triangles.Add(2);
                triangles.Add(1);  
                triangles.Add(2);
                triangles.Add(0);
                triangles.Add(3);                
            }

            
        }

        public static void BuildHemicylinder(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Vector3 scale, MeshFace meshFace, int longitudes = 32)
        {
            Rect rect = new Rect(0, 0, 1, 1);
            float height = Mathf.Abs(scale.y) * rect.height;
            float radius = scale.z;
            float arcLength = scale.x * rect.width;

            float arcAngle = arcLength / radius;
            float minAngle = scale.x * (-0.5f + rect.x) / radius;

            int columns = Mathf.CeilToInt(longitudes * arcAngle / (2 * Mathf.PI));

            // we don't want super tall skinny triangles because that can lead to artifacting.
            // make triangles no more than 2x taller than wide

            float triangleWidth = arcLength / columns;
            float ratio = height / triangleWidth;

            int rows = Mathf.CeilToInt(ratio / 2);

            for (int j = 0; j < rows + 1; j += 1)
            {
                for (int k = 0; k < columns + 1; k++)
                {
                    uv.Add(new Vector2((k / (float)columns), 1 - (j / (float)rows)));

                    Vector3 vert = Vector3.zero;
                    // because the scale is used to control the parameters, we need
                    // to reverse multiply by scale to appear correctly
                    vert.x = (Mathf.Sin(minAngle + (k * arcAngle / columns)) * radius) / scale.x;

                    vert.y = (0.5f - rect.y - rect.height + rect.height * (1 - j / (float)rows));
                    vert.z = (Mathf.Cos(minAngle + (k * arcAngle / columns)) * radius) / scale.z;
                    verts.Add(vert);
                }
            }

            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < columns; k++)
                {
                    if (meshFace == MeshFace.Front)
                    {
                        triangles.Add((j * (columns + 1)) + k);
                        triangles.Add(((j + 1) * (columns + 1)) + k + 1);
                        triangles.Add(((j + 1) * (columns + 1)) + k);
                        triangles.Add(((j + 1) * (columns + 1)) + k + 1);
                        triangles.Add((j * (columns + 1)) + k);
                        triangles.Add((j * (columns + 1)) + k + 1);
                    }
                    else
                    {
                        triangles.Add((j * (columns + 1)) + k);
                        triangles.Add(((j + 1) * (columns + 1)) + k);
                        triangles.Add(((j + 1) * (columns + 1)) + k + 1);  
                        triangles.Add(((j + 1) * (columns + 1)) + k + 1);
                        triangles.Add((j * (columns + 1)) + k + 1);
                        triangles.Add((j * (columns + 1)) + k);
                        
                    }
                    
                }
            }
        }
    }
}


