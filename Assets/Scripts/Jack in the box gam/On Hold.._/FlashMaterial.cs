using UnityEngine;

public class FlashMaterial : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material hintMaterial;   // Alpha Clipping material: default + blinking
    [SerializeField] private Material hoverMaterial;  // Shown while hovering

    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Blink")]
    [SerializeField] private Color startGlowColor = Color.white;
    [SerializeField] private Color endGlowColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 1f;
    [SerializeField] private bool Activated;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP / HDRP
    private static readonly int ColorId = Shader.PropertyToID("_Color");         // Built-in
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int CutoffId = Shader.PropertyToID("_Cutoff");

    private Material hintInstance;
    private Color defaultColor;

    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        // Private copy so blinking doesn't affect other objects
        hintInstance = new Material(hintMaterial);

        // Keep the material's own color, but always with alpha 1
        defaultColor = GetColor(hintInstance);
        defaultColor.a = 1f;

        // Safety: a cutoff of 1 would clip everything
        if (hintInstance.HasProperty(CutoffId) && hintInstance.GetFloat(CutoffId) >= 1f)
            hintInstance.SetFloat(CutoffId, 0.5f);
    }

    private void Start()
    {
        ShowDefault();
    }

    private void OnDestroy()
    {
        if (hintInstance != null)
            Destroy(hintInstance);
    }

    private void Update()
    {
        if (!Activated) return;

        float ratio = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        Color current = Color.Lerp(startGlowColor, endGlowColor, ratio);
        current.a = 1f; // never let alpha drop below the clip threshold

        SetColor(hintInstance, current);

        if (hintInstance.HasProperty(EmissionColorId))
            hintInstance.SetColor(EmissionColorId, current);
    }

    public void StartFlash()
    {
        Activated = true;
        gameObject.SetActive(true);
        meshRenderer.sharedMaterial = hintInstance;
    }

    public void StopFlash()
    {
        Activated = false;
        ShowDefault();
    }

    public void SetHoverMaterial()
    {
        Activated = false;
        meshRenderer.sharedMaterial = hoverMaterial;
    }

    public void ActivateHint()
    {
        Activated = true;
        meshRenderer.sharedMaterial = hintInstance;
    }

    public void DeactivateHint()
    {
        Activated = false;
        ShowDefault();
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    // --- Helpers ---

    // Default look: hint material, original color, alpha 1, no glow
    private void ShowDefault()
    {
        SetColor(hintInstance, defaultColor);

        if (hintInstance.HasProperty(EmissionColorId))
            hintInstance.SetColor(EmissionColorId, Color.black);

        meshRenderer.sharedMaterial = hintInstance;
    }

    private static Color GetColor(Material mat)
    {
        if (mat.HasProperty(BaseColorId)) return mat.GetColor(BaseColorId);
        if (mat.HasProperty(ColorId)) return mat.GetColor(ColorId);
        return Color.white;
    }

    private static void SetColor(Material mat, Color color)
    {
        if (mat.HasProperty(BaseColorId)) mat.SetColor(BaseColorId, color);
        if (mat.HasProperty(ColorId)) mat.SetColor(ColorId, color);
    }
}