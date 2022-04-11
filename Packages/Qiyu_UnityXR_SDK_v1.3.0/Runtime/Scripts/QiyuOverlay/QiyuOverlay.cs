using System;
using Unity.XR.Qiyu;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Qiyu.Overlay
{
    [RequireComponent(typeof(QiyuOverlayMeshGenerator))]
    public class QiyuOverlay : MonoBehaviour
    {
        protected string TAG = "QiyuOverlay";
        public OverLayType overlayType = OverLayType.Overlay;

        private readonly string overlayID_str = Guid.NewGuid().ToString("N");
        public int layerID;
        protected float[] m_modelScale = new float[3];
        protected float[] m_modelRotation = new float[4];
        protected float[] m_modelTrans = new float[3];
        protected float[] m_vertexs;
        protected int[] m_indices;
        protected float[] m_uv;
        protected IntPtr m_textureID;
        private int texture_format = 0;
        public int texture_width;
        public int texture_height;

        protected float[] m_camerPosition = new float[3];
        protected float[] m_camerRotation = new float[4];

        protected float camera_far = 1000;
        protected float camera_near = 0.01f;

        public bool overrideColorScaleAndOffset = false;

        public Vector4 colorScale = Vector4.one;
        public Vector4 colorOffset = Vector4.zero;

        [HideInInspector]
        public Vector4 colorScale_default = Vector4.one;
        [HideInInspector]
        public Vector4 colorOffset_default = Vector4.zero;

        public float[] colorScale_override = new float[4] { 1, 1, 1, 1 };
        public float[] colorOffset_override = new float[4] { 0, 0, 0, 0 };

        [Tooltip("The Texture to show in the layer.")]
        public Texture texture = null;

        /// <summary>
        /// is android oes texture.
        /// </summary>
        public bool isOESTexture = false;

        [HideInInspector]
        public bool isMeshInit = false;

        public bool isVisable = false;

        [Tooltip("Specify overlay's shape")]
        public OverlayShape currentOverlayShape = OverlayShape.Customize;

        [Tooltip("mesh face direction")]
        public MeshFace meshFace = MeshFace.Front;

        public virtual void Awake()
        {
            QiyuOverlayMeshGenerator qiyuOverlayMeshGenerator = GetComponent<QiyuOverlayMeshGenerator>();
            qiyuOverlayMeshGenerator.SetOverlay(this);
        }
        
        public virtual void Start()
        {
            QiyuOverlayManager.qiyuOverlaysList.Add(this);
            InitMesh();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
#if !UNITY_EDITOR
            if (meshRenderer != null && overlayType == OverLayType.Overlay)
            {
                meshRenderer.enabled = false;
            }
#endif
        }

        public virtual void OnEnable()
        {
            isVisable = true;
            
        }

        public virtual void OnDisable()
        {
            isVisable = false;
        }

        public virtual void InitMesh()
        {
            UpdateModelVertexs();
            UpdateModelIndices();
            UpdateModelUV();
            InitOverlayMesh();
        }

        protected virtual void InitOverlayMesh()
        {
            if (m_vertexs == null || m_indices == null || m_uv == null)
            {
                isMeshInit = false;
            }
            else
            {
                Debug.Log(string.Format(TAG + " ::InitOverlayMesh >> overlay id is {0}, is oes {1},overlay type is {2},vertex num is {3},indices num is {4},uv num is {5},layer id is {6}", overlayID_str, isOESTexture, overlayType, m_vertexs.Length, m_indices.Length, m_uv.Length, layerID));
                NativeMethods.InitOverlayMesh(overlayID_str, layerID, isOESTexture, (int)overlayType, m_vertexs.Length, m_vertexs, m_indices.Length, m_indices, m_uv);
                isMeshInit = true;
            }

        }

        public virtual void UpdateModelInfo()
        {
            if (isMeshInit)
            {
                UpdateModelScale();
                UpdateModelRotation();
                UpdateModelPosition();
                if (texture != null)
                {
                    SetTexture(texture);
                }
                if (GetTextureID().ToInt32() == 0)
                {
                    return;
                }
                DrawLayer();
            }
        }

        public virtual void UpdateModelScale()
        {
            m_modelScale[0] = transform.lossyScale.x;
            m_modelScale[1] = transform.lossyScale.y;
            m_modelScale[2] = transform.lossyScale.z;
        }

        public virtual void SetModelScale(Vector3 scale)
        {
            m_modelScale[0] = scale.x;
            m_modelScale[1] = scale.y;
            m_modelScale[2] = scale.z;
        }

        public virtual void UpdateModelRotation()
        {
            m_modelRotation[0] = transform.rotation.w;
            m_modelRotation[1] = transform.rotation.x;
            m_modelRotation[2] = transform.rotation.y;
            m_modelRotation[3] = transform.rotation.z;
        }

        public virtual void SetModelRotation(Quaternion quaternion)
        {
            m_modelRotation[0] = quaternion.w;
            m_modelRotation[1] = quaternion.x;
            m_modelRotation[2] = quaternion.y;
            m_modelRotation[3] = quaternion.z;
        }

        public virtual void UpdateModelPosition()
        {
            m_modelTrans[0] = transform.position.x;
            m_modelTrans[1] = transform.position.y;
            m_modelTrans[2] = transform.position.z;
        }

        public virtual void SetModelPosition(Vector3 position)
        {
            m_modelTrans[0] = position.x;
            m_modelTrans[1] = position.y;
            m_modelTrans[2] = position.z;
        }

        public virtual void UpdateModelVertexs()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (null != meshFilter && meshFilter.mesh != null)
            {
                m_vertexs = new float[meshFilter.mesh.vertexCount * 3];
                int j = 0;
                for (int i = 0; i < meshFilter.mesh.vertexCount; i++)
                {
                    m_vertexs[j++] = meshFilter.mesh.vertices[i].x;
                    m_vertexs[j++] = meshFilter.mesh.vertices[i].y;
                    m_vertexs[j++] = meshFilter.mesh.vertices[i].z;
                }
            }
        }

        public virtual void SetModelVertexs(float[] vertexs)
        {
            m_vertexs = vertexs;
        }

        public virtual void UpdateModelIndices()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (null != meshFilter && meshFilter.mesh != null)
            {
                m_indices = meshFilter.mesh.GetIndices(0);
            }
        }

        public virtual void SetModelIndices(int[] indices)
        {
            m_indices = indices;
        }

        public virtual void UpdateModelUV()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (null != meshFilter && meshFilter.mesh != null)
            {
                m_uv = new float[meshFilter.mesh.uv.Length * 2];
                int j = 0;
                for (int i = 0; i < meshFilter.mesh.uv.Length; i++)
                {
                    m_uv[j++] = meshFilter.mesh.uv[i].x;
                    m_uv[j++] = meshFilter.mesh.uv[i].y;
                }
            }
        }

        public virtual void SetModelUV(float[] uv)
        {
            m_uv = uv;
        }

        public virtual void SetCameraInfo(Camera camera)
        {
            if (camera == null)
            {
                Debug.LogError(TAG + " Please add QiyuOverlayManager.cs to XR Camera");
                return;
            }
            Vector3 pos;
            Quaternion quaternion;
            pos = camera.transform.position;
            quaternion = camera.transform.rotation;
            m_camerPosition[0] = pos.x;
            m_camerPosition[1] = pos.y;
            m_camerPosition[2] = pos.z;

            m_camerRotation[0] = quaternion.w;
            m_camerRotation[1] = quaternion.x;
            m_camerRotation[2] = quaternion.y;
            m_camerRotation[3] = quaternion.z;

            camera_far = camera.farClipPlane;
            camera_near = camera.nearClipPlane;
        }

        public virtual void SetTexture(Texture texture)
        {
            if (isOESTexture)
                texture = null;
            if (texture != null)
            {
                m_textureID = texture.GetNativeTexturePtr();
                texture_width = texture.width;
                texture_height = texture.height;
                texture_format = (int)texture.graphicsFormat;
                if (texture_format == 0)
                {
                    Debug.LogError(TAG + " Unsupported image compression format,Please modify the image format.");
                }
            }
        }

        public virtual void SetTextureID(IntPtr textureID, GraphicsFormat tex_format)
        {
            m_textureID = textureID;
            texture_format = (int)tex_format;
            if (texture_format == 0)
            {
                Debug.LogError(TAG + " Unsupported image compression format,Please modify the image format.");
            }
        }

        /// <summary>
        /// oes texture only support opengl.if your Graphic API use vulkan,please use unity Texture
        /// </summary>
        /// <param name="oesID"></param>
        public virtual void SetAndroidOESTextureID(int oesID)
        {
            Debug.Log("SetAndroidOESTextureID is "+ oesID);
            m_textureID = new IntPtr(oesID);
        }

        public IntPtr GetTextureID()
        {
            return m_textureID;
        }

        public void SetPerLayerColorScaleAndOffset(Vector4 scale, Vector4 offset)
        {
            colorScale_override[0] = scale.x;
            colorScale_override[1] = scale.y;
            colorScale_override[2] = scale.z;
            colorScale_override[3] = scale.w;
            colorOffset_override[0] = offset.x;
            colorOffset_override[1] = offset.y;
            colorOffset_override[2] = offset.z;
            colorOffset_override[3] = offset.w;
        }

        public void DrawLayer()
        {
            NativeMethods.DrawOverlay(overlayID_str, layerID, m_textureID.ToInt32(), m_textureID, texture_format, texture_width, texture_height, m_modelScale, m_modelRotation, m_modelTrans, m_camerRotation, m_camerPosition, m_camerRotation, m_camerPosition, camera_near, camera_far, colorScale_override, colorOffset_override);
            NativeMethods.SetLayerObjectRender(overlayID_str, isVisable);
        }

        public void RemoveOverlay()
        {
            Debug.Log(string.Format(TAG + "RemoveOverlay {0}", overlayID_str));
            NativeMethods.RemoveOverlay(overlayID_str);
            m_textureID = IntPtr.Zero;
        }

        private void OnDestroy()
        {
            RemoveOverlay();
            QiyuOverlayManager.qiyuOverlaysList.Remove(this);
        }
    }
}



