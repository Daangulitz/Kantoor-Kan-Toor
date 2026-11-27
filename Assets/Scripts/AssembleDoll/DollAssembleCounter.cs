using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static UnityEngine.Rendering.GPUSort;

public class DollAssembleCounter : MonoBehaviour
{
    [SerializeField] private int assembleCount = 0;

    private void Update()
    {
        if (assembleCount == 5)
        {
            gameObject.GetComponent<XRGrabInteractable>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<XRGrabInteractable>().enabled = false;
        }
    }

    public void TriggerAttach(SelectEnterEventArgs args)
    {
        if (args.interactableObject == null)
            return;

        GameObject enteredObject = args.interactableObject.transform.gameObject;

        if (enteredObject.CompareTag("DollPiece"))
        {
            assembleCount++;
        }
    }

    public void TriggerDetach(SelectExitEventArgs args)
    {
        if (args.interactableObject == null)
        {
            return;
        }

        if (args.interactableObject.transform.gameObject.CompareTag("DollPiece"))
        {
            assembleCount = Mathf.Max(0, assembleCount - 1);
        }
    }
}
