using System;
using UnityEngine;

public class PlacingGear : MonoBehaviour
{
    [SerializeField] private HoverMaterial hoverMaterial;
    [SerializeField] private CompletedEvent completedEvent;
    [SerializeField] private GearPlace gearPlace;

    private bool gearPlaced = false;
    private bool isInTrigger = false;
    private GameObject currentGear;
    public enum GearPlace
    {
        Gearplace1,
        Gearplace2,
        Gearplace3,
        Gearplace4,
        Gearplace5,
        Gearplace6
    }


    public void resetGearPlace()
    {
        currentGear.GetComponent<Rigidbody>().isKinematic = false;
        currentGear.GetComponent<Rigidbody>().useGravity = true;
        gearPlaced = false;
        isInTrigger = false;
        currentGear = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            currentGear = other.gameObject;
            if (hoverMaterial != null && !gearPlaced)
            {
                hoverMaterial.SetHoverMaterial();
                isInTrigger = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            currentGear = null;
            if (hoverMaterial != null)
            {
                hoverMaterial.SetTransparentMaterial();
                isInTrigger = false;
                gearPlaced = false;
            }
        }
    }

    public void placeGear()
    {
        if(isInTrigger)
        {
            hoverMaterial.SetTransparentMaterial();
            isInTrigger = false;
            gearPlaced = true;
            completedEvent.AddGearCount();

            placeGearLocation();
        }
    }

    public void placeGearLocation()
    {
        currentGear.GetComponent<Rigidbody>().isKinematic = true;
        currentGear.GetComponent<Rigidbody>().useGravity = false;
        Vector3 gearPosition = this.transform.position;
        gearPosition.z += 0.05f;
        currentGear.transform.position = gearPosition;
        currentGear.transform.rotation = this.transform.rotation;
        currentGear.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void addCorrectGear()
    {
        completedEvent.addCorrectGear((int)gearPlace);
    }
}
