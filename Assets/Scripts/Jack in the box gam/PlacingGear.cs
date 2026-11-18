using System;
using UnityEngine;

public class PlacingGear : MonoBehaviour
{
    [SerializeField] private FlashMaterial flashMaterial;
    [SerializeField] private CompletedEvent completedEvent;
    [SerializeField] private GearPlace gearPlace;
    public enum GearPlace
    {
        Gearplace1,
        Gearplace2,
        Gearplace3,
        Gearplace4,
        Gearplace5,
        Gearplace6
    }

    private bool isInTrigger = false;
    private GameObject currentGear;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            currentGear = other.gameObject;
            if (flashMaterial != null)
            {
                flashMaterial.SetHoverMaterial();
                isInTrigger = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
            currentGear = null;
            if (flashMaterial != null)
            {
                flashMaterial.StartFlash();
                isInTrigger = false;
            }
        }
    }

    public void placeGear()
    {
        if(isInTrigger)
        {
            flashMaterial.StopFlash();
            isInTrigger = false;
            completedEvent.AddGearCount();

            flashMaterial.disableGameObject();

            placeGearLocation();
        }
    }

    public void placeGearLocation()
    {
        Vector3 gearPosition = this.transform.position;
        gearPosition.z += 0.05f;
        currentGear.transform.position = gearPosition;
        currentGear.transform.rotation = this.transform.rotation;
        currentGear.GetComponent<Rigidbody>().isKinematic = true;
    }
}
