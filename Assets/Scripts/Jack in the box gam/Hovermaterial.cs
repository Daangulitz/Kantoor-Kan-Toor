using UnityEngine;

public class HoverMaterial : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material transparentMaterial;

    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;

    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        SetTransparentMaterial();
    }

    public void SetHoverMaterial()
    {
        meshRenderer.sharedMaterial = hoverMaterial;
    }

    public void SetTransparentMaterial()
    {
        meshRenderer.sharedMaterial = transparentMaterial;
    }
}