using UnityEngine;

public class PlacingGear : MonoBehaviour
{
    [SerializeField] private HoverMaterial hoverMaterial;
    [SerializeField] private CompletedEvent completedEvent;
    [SerializeField] private GearPlace gearPlace; // which gear belongs in THIS socket

    public enum GearPlace { Gearplace1, Gearplace2, Gearplace3, Gearplace4, Gearplace5, Gearplace6 }

    public bool IsOccupied { get; private set; }

    public Pose SnapPose
    {
        get
        {
            Vector3 pos = transform.position;
            pos.z += 0.05f;
            return new Pose(pos, transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GearIdentity gear = other.GetComponentInParent<GearIdentity>();
        if (gear != null)
            gear.AddSocketInRange(this);
    }

    private void OnTriggerExit(Collider other)
    {
        GearIdentity gear = other.GetComponentInParent<GearIdentity>();
        if (gear != null)
            gear.RemoveSocketInRange(this);
    }

    public void ShowHover()
    {
        if (!IsOccupied && hoverMaterial != null)
            hoverMaterial.SetHoverMaterial();
    }

    public void HideHover()
    {
        if (hoverMaterial != null)
            hoverMaterial.SetTransparentMaterial();
    }

    public void Occupy(GearIdentity gear)
    {
        IsOccupied = true;
        HideHover();
        completedEvent.SetSocketState((int)gearPlace, true, gear.GearId == gearPlace);
    }

    public void Free()
    {
        IsOccupied = false;
        completedEvent.SetSocketState((int)gearPlace, false, false);
    }
}