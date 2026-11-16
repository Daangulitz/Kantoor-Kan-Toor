using System;
using UnityEngine;

public class PlacingGear : MonoBehaviour
{
    [SerializeField] FlashMaterial flashMaterial;
    private bool isInTrigger = false;

    [SerializeField] private CompletedEvent completedEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gear"))
        {
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
        }
    }
}
