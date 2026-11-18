using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandleRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRBaseInteractable HandleInteractable; // works with Grab or Simple
    [SerializeField] private Transform Gogwheel;

    [Header("Rotation")]
    [SerializeField] private Vector3 RotationAxis = Vector3.up;

    private IXRSelectInteractor currentInteractor;
    private Vector3 previousDirection;
    private Vector3 handleLocalPosition;
    private Quaternion handleLocalRotation;

    private void Awake()
    {
        // Remember where the handle sits relative to the wheel
        handleLocalPosition = HandleInteractable.transform.localPosition;
        handleLocalRotation = HandleInteractable.transform.localRotation;
    }

    private void OnEnable()
    {
        HandleInteractable.selectEntered.AddListener(StartTurning);
        HandleInteractable.selectExited.AddListener(StopTurning);
    }

    private void OnDisable()
    {
        HandleInteractable.selectEntered.RemoveListener(StartTurning);
        HandleInteractable.selectExited.RemoveListener(StopTurning);
    }

    private void Update()
    {
        if (currentInteractor == null)
            return;

        Transform controller = currentInteractor.GetAttachTransform(HandleInteractable);
        Vector3 axis = Gogwheel.TransformDirection(RotationAxis.normalized);

        Vector3 direction = Vector3.ProjectOnPlane(controller.position - Gogwheel.position, axis);
        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        float angle = Vector3.SignedAngle(previousDirection, direction, axis);
        Gogwheel.Rotate(axis, angle, Space.World);

        previousDirection = direction;
    }

    // Safety net: re-lock the handle to the wheel after all other updates
    private void LateUpdate()
    {
        Transform h = HandleInteractable.transform;
        h.localPosition = handleLocalPosition;
        h.localRotation = handleLocalRotation;
    }

    private void StartTurning(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;

        Transform controller = currentInteractor.GetAttachTransform(HandleInteractable);
        Vector3 axis = Gogwheel.TransformDirection(RotationAxis.normalized);

        previousDirection = Vector3.ProjectOnPlane(controller.position - Gogwheel.position, axis).normalized;
    }

    private void StopTurning(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }
}