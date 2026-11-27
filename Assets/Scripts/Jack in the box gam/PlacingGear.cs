using UnityEngine;

public class PlacingGear : MonoBehaviour
{
    [SerializeField] private HoverMaterial hoverMaterial;
    [SerializeField] private CompletedEvent completedEvent;
    [SerializeField] private GearPlace gearPlace; // which gear belongs in THIS socket

    public enum GearPlace { Gearplace1, Gearplace2, Gearplace3, Gearplace4, Gearplace5, Gearplace6 }

    private GearIdentity occupant;

    public bool IsOccupied { get; private set; }

    public Pose SnapPose
    {
        get
        {
            Quaternion rot = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
            return new Pose(transform.position, rot);
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
        occupant = gear;
        IsOccupied = true;
        HideHover();

        // The gear is already at the snap pose; parenting keeps it exactly there
        gear.transform.SetParent(transform, true);

        completedEvent.SetSocketState((int)gearPlace, true, gear.GearId == gearPlace);
    }

    public void Free()
    {
        // Normally XRI has already unparented the gear when it was grabbed; this is the safety net
        if (occupant != null && occupant.transform.parent == transform)
            occupant.transform.SetParent(occupant.HomeParent, true);

        occupant = null;
        IsOccupied = false;
        completedEvent.SetSocketState((int)gearPlace, false, false);
    }
}