using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Light))]
public class BlacklightFlashlight : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleAction;   // optional: button to turn it on/off
    [SerializeField] private bool startOn = false;

    private bool isGrabbed;

    private Light spot;
    private bool isOn;

    static readonly int PosID      = Shader.PropertyToID("_BlacklightPos");
    static readonly int DirID      = Shader.PropertyToID("_BlacklightDir");
    static readonly int CosOuterID = Shader.PropertyToID("_BlacklightCosOuter");
    static readonly int CosInnerID = Shader.PropertyToID("_BlacklightCosInner");
    static readonly int RangeID    = Shader.PropertyToID("_BlacklightRange");
    static readonly int OnID       = Shader.PropertyToID("_BlacklightOn");

    void Awake()
    {
        spot = GetComponent<Light>();
        SetOn(startOn);
    }

    void OnEnable()  { if (toggleAction != null) toggleAction.action.Enable(); }
    void OnDisable() { if (toggleAction != null) toggleAction.action.Disable(); }

    void Update()
    {
        if (toggleAction != null && toggleAction.action.WasPressedThisFrame() && isGrabbed == true)
            SetOn(!isOn);

        Shader.SetGlobalVector(PosID, transform.position);
        Shader.SetGlobalVector(DirID, transform.forward);
        Shader.SetGlobalFloat(CosOuterID, Mathf.Cos(spot.spotAngle * 0.5f * Mathf.Deg2Rad));
        Shader.SetGlobalFloat(CosInnerID, Mathf.Cos(spot.innerSpotAngle * 0.5f * Mathf.Deg2Rad));
        Shader.SetGlobalFloat(RangeID, spot.range);
    }

    public void SetOn(bool on)
    {
        isOn = on;
        spot.enabled = on;
        Shader.SetGlobalFloat(OnID, on ? 1f : 0f);
    }

    public void Grabbed()
    {
        isGrabbed = true;
    }

    public void Released()
    {
        isGrabbed = false;
        SetOn(isOn);
    }

    void OnDestroy() => Shader.SetGlobalFloat(OnID, 0f);
}