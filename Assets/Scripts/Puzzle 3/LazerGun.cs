using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class LazerGun : MonoBehaviour
{
    [Header("Laser")]
    [SerializeField] private GameObject lazerOrigin;
    [SerializeField] private LayerMask cutlineMask;
    [SerializeField] private float lazerSize = 100f;

    [Header("Input")]
    [SerializeField] private InputActionReference triggerAction;   // trigger of the VR controller

    [Header("Visuals")]
    [SerializeField] private LineRenderer beam;
    [SerializeField] private GameObject hitMarker;                // optional: small sphere at the hit point
    [SerializeField] private Color missColor = Color.green;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float beamWidth = 0.01f;

    [Header("Extra's from other scripts")]
    [SerializeField] private LineRenderer SplineLineRenderer;     // original full line (optional)
    [SerializeField] private SplineLineSegments splineSegments;   // the object with SplineLineSegments


    private bool isGrabbed = false;
    private SplineLineCollider.Segment lastSegment;
    private DollGameObjects dollGameObjects;
    private int SegmentCount = 0;
    private int maxSegments = 60;   // keep the same as segments in SplineLineCollider / SplineLineSegments
    private bool finished = false;

    private void OnEnable()
    {
        if (triggerAction != null) triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        if (triggerAction != null) triggerAction.action.Disable();
    }

    void Awake()
    {
        if (beam == null) beam = GetComponent<LineRenderer>();

        beam.useWorldSpace = true;
        beam.positionCount = 2;
        beam.startWidth = beam.endWidth = beamWidth;
        beam.enabled = false;

        if (hitMarker != null) hitMarker.SetActive(false);
    }

    void Start()
    {
        dollGameObjects = FindAnyObjectByType<DollGameObjects>();
        if (dollGameObjects == null)
            Debug.LogWarning("DollGameObjects script not found in the scene.");
    }

    void Update()
    {
        if(!isGrabbed) return;

        if (!finished && SegmentCount >= maxSegments)
        {
            finished = true;
            if (SplineLineRenderer != null) SplineLineRenderer.enabled = false;
            dollGameObjects?.ShowObjects(); 
        }

        bool firing = triggerAction != null && triggerAction.action.IsPressed();

        if (!firing)
        {
            beam.enabled = false;
            if (hitMarker != null) hitMarker.SetActive(false);
            lastSegment = null;
            return;
        }

        beam.enabled = true;

        Vector3 origin = lazerOrigin.transform.position;
        Vector3 dir = lazerOrigin.transform.forward;
        Vector3 end = origin + dir * lazerSize;
        Color color = missColor;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, lazerSize, cutlineMask))
        {
            end = hit.point;
            color = hitColor;

            var seg = hit.collider.GetComponent<SplineLineCollider.Segment>();
            if (seg != null && seg != lastSegment)
            {
                Debug.Log("Hit segment: " + seg.name);   // only logs when the segment changes
                lastSegment = seg;

                if (splineSegments != null)
                    splineSegments.Cut(seg.index);        // remove the visual part of the line

                hit.collider.gameObject.SetActive(false); // remove the collider
                SegmentCount++;
            }

            if (hitMarker != null)
            {
                hitMarker.SetActive(true);
                hitMarker.transform.position = hit.point;
            }
        }
        else
        {
            lastSegment = null;
            if (hitMarker != null) hitMarker.SetActive(false);
        }

        beam.SetPosition(0, origin);
        beam.SetPosition(1, end);
        beam.startColor = beam.endColor = color;
    }

    public void Grabbed()
    {
        isGrabbed = true;
    }

    public void Released()
    {
        isGrabbed = false;
    }
}