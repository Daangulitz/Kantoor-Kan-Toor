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

    [Header("Crank (what the hand turns)")]
    [SerializeField] private XRBaseInteractable handleInteractable; // the grip
    [SerializeField] private Transform crankPivot;                  // empty on the axle, parent of arm + grip
    [SerializeField] private Vector3 crankLocalAxis = Vector3.up;   // axle direction in CrankPivot's local space

    [Header("Main gear")]
    [SerializeField] private Transform gogwheel;
    [SerializeField] private Vector3 gogwheelLocalAxis = Vector3.up; // axle direction in the gogwheel's local space
    [SerializeField] private float gogwheelDirection = 1f;           // set to -1 if it turns the wrong way

    [Header("Gear chain (in order: 6 first, then 5 to 1)")]
    [SerializeField] private CompletedEvent completedEvent;
    [SerializeField] private GearRotationSettings[] gearChain;
    [SerializeField] private Vector3 gearLocalAxis = Vector3.up;     // axle direction in each gear's local space

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
        Vector3 axis = crankPivot.TransformDirection(crankLocalAxis.normalized);

        Vector3 direction = Vector3.ProjectOnPlane(controller.position - crankPivot.position, axis);
        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        float angle = Vector3.SignedAngle(previousDirection, direction, axis);

        // The crank (arm + grip) follows the hand exactly
        crankPivot.Rotate(axis, angle, Space.World);

        // The gears are driven by that same angle, each around its own axle
        DriveGears(angle);

        previousDirection = direction;
    }

    private void DriveGears(float angle)
    {
        gogwheel.Rotate(gogwheelLocalAxis.normalized, angle * gogwheelDirection, Space.Self);

        float direction = -gogwheelDirection; // gear 6 meshes with the main wheel, so it turns the opposite way

        foreach (GearRotationSettings g in gearChain)
        {
            if (g.gear == null || !completedEvent.IsGearPlaced((int)g.place))
                break; // chain is broken here, so nothing after this turns

            g.gear.Rotate(gearLocalAxis.normalized, angle * direction, Space.Self);
            direction = -direction;
        }
    }

    private void LateUpdate()
    {
        // Keep the grip fixed on the arm, whatever XRI does
        Transform handle = handleInteractable.transform;
        handle.localPosition = handleLocalPosition;
        handle.localRotation = handleLocalRotation;
    }

    private void StartTurning(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;

        Transform controller = currentInteractor.GetAttachTransform(handleInteractable);
        Vector3 axis = crankPivot.TransformDirection(crankLocalAxis.normalized);

        previousDirection = Vector3.ProjectOnPlane(controller.position - crankPivot.position, axis).normalized;
    }

    private void StopTurning(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    // Shows the axles in the Scene view when the object is selected
    private void OnDrawGizmosSelected()
    {
        if (crankPivot != null)
        {
            Vector3 a = crankPivot.TransformDirection(crankLocalAxis.normalized);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(crankPivot.position - a * 0.3f, crankPivot.position + a * 0.3f);
        }

        if (gogwheel != null)
        {
            Vector3 a = gogwheel.TransformDirection(gogwheelLocalAxis.normalized);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gogwheel.position - a * 0.3f, gogwheel.position + a * 0.3f);
        }
    }
}