using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandleRotation : MonoBehaviour
{
    [System.Serializable]
    public class GearRotationSettings
    {
        public Transform gear;
        public PlacingGear.GearPlace place;
    }

    [Header("References")]
    [SerializeField] private XRBaseInteractable handleInteractable;
    [SerializeField] private Transform gogwheel;
    [SerializeField] private CompletedEvent completedEvent;

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Gear chain (in order: 6 first, then 5 to 1)")]
    [SerializeField] private GearRotationSettings[] gearChain;

    private IXRSelectInteractor currentInteractor;
    private Vector3 previousDirection;

    private Vector3 handleLocalPosition;
    private Quaternion handleLocalRotation;

    private void Awake()
    {
        handleLocalPosition = handleInteractable.transform.localPosition;
        handleLocalRotation = handleInteractable.transform.localRotation;
    }

    private void OnEnable()
    {
        handleInteractable.selectEntered.AddListener(StartTurning);
        handleInteractable.selectExited.AddListener(StopTurning);
    }

    private void OnDisable()
    {
        handleInteractable.selectEntered.RemoveListener(StartTurning);
        handleInteractable.selectExited.RemoveListener(StopTurning);
    }

    private void Update()
    {
        if (currentInteractor == null)
            return;

        Transform controller = currentInteractor.GetAttachTransform(handleInteractable);
        Vector3 axis = gogwheel.TransformDirection(rotationAxis.normalized);

        Vector3 direction = Vector3.ProjectOnPlane(
            controller.position - gogwheel.position, axis);

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        float angle = Vector3.SignedAngle(previousDirection, direction, axis);

        // Rotate root gear
        gogwheel.Rotate(axis, angle, Space.World);

        // Rotate the chain
        RotateConnectedGears(axis, angle);

        previousDirection = direction;
    }

    private void RotateConnectedGears(Vector3 axis, float angle)
    {
        float direction = -1f; // gear 6 meshes with the main wheel, so it turns the opposite way

        foreach (GearRotationSettings g in gearChain)
        {
            if (g.gear == null || !completedEvent.IsGearPlaced((int)g.place))
                break; // chain is broken here, so nothing after this turns

            g.gear.Rotate(axis, angle * direction, Space.World);
            direction = -direction; // each next gear alternates direction
        }
    }

    private void LateUpdate()
    {
        Transform handle = handleInteractable.transform;
        handle.localPosition = handleLocalPosition;
        handle.localRotation = handleLocalRotation;
    }

    private void StartTurning(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;

        Transform controller = currentInteractor.GetAttachTransform(handleInteractable);
        Vector3 axis = gogwheel.TransformDirection(rotationAxis.normalized);

        previousDirection = Vector3.ProjectOnPlane(
            controller.position - gogwheel.position, axis).normalized;
    }

    private void StopTurning(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }
}