//=============================================================================
//
//          Copyright (c) 2021 Beijing iQIYI Intelligent Technologies Inc.
//                          All Rights Reserved.
//
//=============================================================================

using UnityEngine;
using System.Collections; 

/// <summary>
/// Fades the screen from black after a new scene is loaded. Fade can also be controlled mid-scene using SetUIFade and SetFadeLevel
/// </summary>
public class QiyuScreenFade : MonoBehaviour
{
	[Tooltip("Fade duration")]
	public float fadeTime = 1.0f;

	[Tooltip("Screen color at maximum fade")]
	public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

	public bool fadeOnStart = true;

	/// <summary>
	/// The render queue used by the fade mesh. Reduce this if you need to render on top of it.
	/// </summary>
	public int renderQueue = 5000;

	/// <summary>
	/// Renders the current alpha value being used to fade the screen.
	/// </summary>
	public float currentAlpha { get { return Mathf.Max(explicitFadeAlpha, animatedFadeAlpha, uiFadeAlpha); } }

	private float explicitFadeAlpha = 0.0f;
	private float animatedFadeAlpha = 0.0f;
	private float uiFadeAlpha = 0.0f;

	private MeshRenderer fadeRenderer;
	private MeshFilter fadeMesh;
	private Material fadeMaterial = null;
	private bool isFading = false;

    /// <summary>
    /// Automatically starts a fade in
    /// </summary>
    void Start()
    {
        // create the fade material
        fadeMaterial = new Material(Shader.Find("QVR/Unlit Transparent Color"));
        fadeMesh = gameObject.AddComponent<MeshFilter>();
        fadeRenderer = gameObject.AddComponent<MeshRenderer>();

        var mesh = new Mesh();
        fadeMesh.mesh = mesh;

        Vector3[] vertices = new Vector3[4];

        float width = 2f;
        float height = 2f;
        float depth = 1f;

        vertices[0] = new Vector3(-width, -height, depth);
        vertices[1] = new Vector3(width, -height, depth);
        vertices[2] = new Vector3(-width, height, depth);
        vertices[3] = new Vector3(width, height, depth);

        mesh.vertices = vertices;

        int[] tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        explicitFadeAlpha = 0.0f;
        animatedFadeAlpha = 0.0f;
        uiFadeAlpha = 0.0f;

        if (fadeOnStart)
        {
            ScreenFadeIn();
        }
    }

    public void ScreenFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1.0f, 1.0f, 0.2f));
        StartCoroutine(Fade(1.0f, 0.0f, fadeTime));
    }

    public void ScreenFadeOut()
    {
        StartCoroutine(Fade(0, 1, fadeTime));
    }


    void OnEnable()
    {
        if (!fadeOnStart)
        {
            explicitFadeAlpha = 0.0f;
            animatedFadeAlpha = 0.0f;
            uiFadeAlpha = 0.0f;
        }
    }

    /// <summary>
    /// Cleans up the fade material
    /// </summary>
    void OnDestroy()
    {
        if (fadeRenderer != null)
            Destroy(fadeRenderer);

        if (fadeMaterial != null)
            Destroy(fadeMaterial);

        if (fadeMesh != null)
            Destroy(fadeMesh);
    }


    /// <summary>
    /// Fades alpha from 1.0 to 0.0
    /// </summary>
    IEnumerator Fade(float startAlpha, float endAlpha,float _fadeTime)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;
            animatedFadeAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / _fadeTime));
            SetMaterialAlpha();
            yield return new WaitForEndOfFrame();
        }
        animatedFadeAlpha = endAlpha;
        SetMaterialAlpha();
    }

    /// <summary>
    /// Update material alpha. UI fade and the current fade due to fade in/out animations (or explicit control)
    /// both affect the fade. (The max is taken)
    /// </summary>
    private void SetMaterialAlpha()
    {
        Color color = fadeColor;
        color.a = currentAlpha;
        isFading = color.a > 0;
        if (fadeMaterial != null)
        {
            fadeMaterial.color = color;
            fadeMaterial.renderQueue = renderQueue;
            fadeRenderer.material = fadeMaterial;
            fadeRenderer.enabled = isFading;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            ScreenFadeIn();
        }
    }
}
