using UnityEditor;
using UnityEngine;

namespace Qiyu.Overlay
{
    [CustomEditor(typeof(QiyuOverlay))]
    public class QiyuOverlayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            QiyuOverlay overlay = (QiyuOverlay)target;
            if (overlay == null)
            {
                return;
            }

            EditorGUILayout.LabelField("Display Order", EditorStyles.boldLabel);
            overlay.overlayType = (OverLayType)EditorGUILayout.EnumPopup(new GUIContent("Overlay Type", "Whether this overlay should layer behind the scene or in front of it"), overlay.overlayType);
            overlay.layerID = EditorGUILayout.IntField(new GUIContent("Layer ID", "Depth value used to sort layers in the scene, bigger value appears in front"), overlay.layerID);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Overlay Shape", "The shape of this overlay"), EditorStyles.boldLabel);
            overlay.currentOverlayShape = (OverlayShape)EditorGUILayout.EnumPopup(new GUIContent("Overlay Shape", "The shape of this overlay"), overlay.currentOverlayShape);
            overlay.meshFace = (MeshFace)EditorGUILayout.EnumPopup(new GUIContent("Mesh Face", "The mesh direction"), overlay.meshFace);
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);

#if UNITY_ANDROID
            bool lastIsExternalSurface = overlay.isOESTexture;
            overlay.isOESTexture = EditorGUILayout.Toggle(new GUIContent("Is External Surface", "On Android, retrieve an Android Surface object to render to (e.g., video playback)"), overlay.isOESTexture);
            if (lastIsExternalSurface)
            {
                overlay.texture_width = EditorGUILayout.IntField("External Surface Width", overlay.texture_width);
                overlay.texture_height = EditorGUILayout.IntField("External Surface Height", overlay.texture_height);
                overlay.texture = null;
            }
            else
#endif
            {
                if (overlay.texture == null)
                {
                    Texture[] tmp = new Texture[1];
                    overlay.texture = tmp[0];
                }
                var labelControlRect = EditorGUILayout.GetControlRect();
                EditorGUI.LabelField(new Rect(labelControlRect.x, labelControlRect.y, labelControlRect.width / 2, labelControlRect.height), new GUIContent("Texture", "Texture used for the layer"));

                var textureControlRect = EditorGUILayout.GetControlRect(GUILayout.Height(64));

                overlay.texture = (Texture)EditorGUI.ObjectField(new Rect(textureControlRect.x, textureControlRect.y, 64, textureControlRect.height), overlay.texture, typeof(Texture), true);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Color Scale", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            overlay.overrideColorScaleAndOffset = EditorGUILayout.Toggle(new GUIContent("Override Color Scale", "Manually set color scale and offset of this layer"), overlay.overrideColorScaleAndOffset);
            if (overlay.overrideColorScaleAndOffset)
            {
                overlay.colorScale = EditorGUILayout.Vector4Field(new GUIContent("Color Scale", "Scale that the color values for this overlay will be multiplied by."), overlay.colorScale);
                overlay.colorOffset = EditorGUILayout.Vector4Field(new GUIContent("Color Offset", "Offset that the color values for this overlay will be added to."), overlay.colorOffset);
                overlay.SetPerLayerColorScaleAndOffset(overlay.colorScale, overlay.colorOffset);
            }
            else
            {
                overlay.SetPerLayerColorScaleAndOffset(overlay.colorScale_default, overlay.colorOffset_default);
            }

            EditorUtility.SetDirty(overlay);
        }                     
    }
}

