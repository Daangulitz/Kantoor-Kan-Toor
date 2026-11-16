using UnityEngine;

public class FlashMaterial : MonoBehaviour
{
    [SerializeField] Material GlowMaterial;
    [SerializeField] Material Standard;
    [SerializeField] Material HoverMaterial;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Color StartglowColor;
    [SerializeField] Color EndglowColor;
    [SerializeField] private bool Activated;

    private void Awake()
    {

    }

    public void StartFlash()
    {
        Activated = true;
    }

    public void StopFlash()
    {
        Activated = false;
        meshRenderer.material = Standard;
    }

    public void SetHoverMaterial()
    {
        Activated = false;
        meshRenderer.material = HoverMaterial;
    }

    void Start()
    {
        meshRenderer.material = Standard;
    }

    // Update is called once per frame
    void Update()
    {
        if(Activated)
        {
            float Colorratio = Mathf.PingPong(Time.time, 1f);
            Color CurrentColor = Color.Lerp(StartglowColor, EndglowColor, Colorratio);

            meshRenderer.material.color = CurrentColor;
            meshRenderer.material.SetColor("_EmissionColor", CurrentColor);
        }
    }

    public void ActivateHint()
    {
        Material glowCopy = new Material(GlowMaterial); // make a copy to prevent duplicate animations
        meshRenderer.material = glowCopy;
        glowCopy.color = StartglowColor;
        Activated = true;
    }

    public void DeactivateHint()
    {
        meshRenderer.material = Standard;
        Activated = false;
    }
}
