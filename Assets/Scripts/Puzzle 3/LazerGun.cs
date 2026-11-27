using System.Collections.Generic;
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

    [Header("Particle effect prefabs (drag prefabs from the Project window)")]
    [SerializeField] private ParticleSystem[] beamPrefabs;         // 1 or more systems that form the beam (e.g. core + glow)
    [SerializeField] private ParticleSystem[] hitPrefabs;          // sparks at the hit point
    [SerializeField] private ParticleSystem[] muzzlePrefabs;       // optional glow at the controller tip

    [Header("Effect positions")]
    [Tooltip("Optional: object the beam is attached to. Empty = the laser origin.")]
    [SerializeField] private Transform beamPoint;
    [Tooltip("Position of the beam relative to the beam point (local space).")]
    [SerializeField] private Vector3 beamPositionOffset = Vector3.zero;
    [Tooltip("Rotation of the beam relative to the beam point, in degrees.")]
    [SerializeField] private Vector3 beamRotationOffset = Vector3.zero;

    [Tooltip("Optional: object the muzzle effect is attached to. Empty = the laser origin.")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private Vector3 muzzlePositionOffset = Vector3.zero;
    [SerializeField] private Vector3 muzzleRotationOffset = Vector3.zero;

    [Tooltip("Distance of the hit effect from the surface, along the surface normal.")]
    [SerializeField] private float hitSurfaceOffset = 0.005f;
    [Tooltip("Extra offset of the hit effect in world space.")]
    [SerializeField] private Vector3 hitPositionOffset = Vector3.zero;

    [Header("Beam settings")]
    [SerializeField] private float beamThickness = 0.01f;
    [SerializeField] private float maxVisualLength = 10f;          // the beam is drawn at most this long
    [SerializeField] private float particlesPerMeter = 80f;        // beam density
    [SerializeField] private Color missColor = Color.green;
    [SerializeField] private Color hitColor = Color.red;

    [Header("Extra's from other scripts")]
    [SerializeField] private LineRenderer SplineLineRenderer;     // original full line (optional)
    [SerializeField] private SplineLineSegments splineSegments;   // the object with SplineLineSegments

    private readonly List<ParticleSystem> beamLayers = new List<ParticleSystem>();
    private readonly List<ParticleSystem> hitEffects = new List<ParticleSystem>();
    private readonly List<ParticleSystem> muzzleEffects = new List<ParticleSystem>();

    private bool isGrabbed = false;
    private SplineLineCollider.Segment lastSegment;
    private DollGameObjects dollGameObjects;
    private int SegmentCount = 0;
    private int maxSegments = 60;   // keep the same as segments in SplineLineCollider / SplineLineSegments
    private bool finished = false;
    private bool wasFiring = false;

    private Transform BeamParent => beamPoint != null ? beamPoint : lazerOrigin.transform;
    private Transform MuzzleParent => muzzlePoint != null ? muzzlePoint : lazerOrigin.transform;

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
        SpawnEffects();
    }

    void Start()
    {
        dollGameObjects = FindAnyObjectByType<DollGameObjects>();
        if (dollGameObjects == null)
            Debug.LogWarning("DollGameObjects script not found in the scene.");
    }

    void OnDestroy()
    {
        // Hit effects live in the world (not under the gun), so clean them up ourselves
        foreach (var ps in hitEffects)
            if (ps != null) Destroy(ps.gameObject);
    }

    void SpawnEffects()
    {
        // Beam layers: children of the beam point
        foreach (var prefab in beamPrefabs)
        {
            if (prefab == null) continue;

            ParticleSystem ps = Instantiate(prefab, BeamParent);

            var main = ps.main;
            main.loop = true;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.startSpeed = 0f;

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            beamLayers.Add(ps);
        }

        // Hit effects: spawned in the world, moved to the hit point while firing
        foreach (var prefab in hitPrefabs)
        {
            if (prefab == null) continue;

            ParticleSystem ps = Instantiate(prefab);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitEffects.Add(ps);
        }

        // Muzzle effects: children of the muzzle point
        foreach (var prefab in muzzlePrefabs)
        {
            if (prefab == null) continue;

            ParticleSystem ps = Instantiate(prefab, MuzzleParent);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleEffects.Add(ps);
        }

        ApplyEffectPositions();
    }

    // Puts the beam and muzzle effects at the positions set in the Inspector
    void ApplyEffectPositions()
    {
        foreach (var ps in beamLayers)
        {
            if (ps == null) continue;
            ps.transform.localPosition = beamPositionOffset;
            ps.transform.localRotation = Quaternion.Euler(beamRotationOffset);
        }

        foreach (var ps in muzzleEffects)
        {
            if (ps == null) continue;
            ps.transform.localPosition = muzzlePositionOffset;
            ps.transform.localRotation = Quaternion.Euler(muzzleRotationOffset);
        }
    }

    void Update()
    {
        if (!isGrabbed) return;

        if (!finished && SegmentCount >= maxSegments)
        {
            finished = true;
            if (SplineLineRenderer != null) SplineLineRenderer.enabled = false;
            dollGameObjects?.ShowObjects();
        }

        bool firing = triggerAction != null && triggerAction.action.IsPressed();

        if (!firing)
        {
            StopEffects();
            lastSegment = null;
            return;
        }

        SetFiring(true);
        ApplyEffectPositions();   // lets you tweak the offsets live in Play mode

        Vector3 origin = lazerOrigin.transform.position;
        Vector3 dir = lazerOrigin.transform.forward;
        float length = lazerSize;
        Color color = missColor;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, lazerSize, cutlineMask))
        {
            length = hit.distance;
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

            SetHitEffects(true, hit.point, hit.normal);
        }
        else
        {
            lastSegment = null;
            SetHitEffects(false, Vector3.zero, Vector3.up);
        }

        UpdateBeam(Mathf.Min(length, maxVisualLength), color);
    }

    public void Grabbed()
    {
        isGrabbed = true;
    }

    public void Released()
    {
        isGrabbed = false;
        StopEffects();       // no beam left behind when you let go while firing
        lastSegment = null;
    }

    void UpdateBeam(float length, Color color)
    {
        foreach (var ps in beamLayers)
        {
            if (ps == null) continue;

            // Stretch the emission box along the beam
            var shape = ps.shape;
            shape.scale = new Vector3(beamThickness, beamThickness, length);
            shape.position = new Vector3(0f, 0f, length * 0.5f);

            // Same density no matter how long the beam is
            var emission = ps.emission;
            emission.rateOverTime = particlesPerMeter * length;

            var main = ps.main;
            main.startColor = color;
        }
    }

    void StopEffects()
    {
        SetFiring(false);
        SetHitEffects(false, Vector3.zero, Vector3.up);
    }

    void SetFiring(bool on)
    {
        if (on == wasFiring) return;
        wasFiring = on;

        foreach (var ps in beamLayers)
        {
            if (ps == null) continue;
            if (on) ps.Play(true);
            else ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        foreach (var ps in muzzleEffects)
        {
            if (ps == null) continue;
            if (on) ps.Play(true);
            else ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    void SetHitEffects(bool on, Vector3 point, Vector3 normal)
    {
        foreach (var ps in hitEffects)
        {
            if (ps == null) continue;

            if (on)
            {
                // Sparks fly away from the surface
                Vector3 pos = point + normal * hitSurfaceOffset + hitPositionOffset;
                ps.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(normal));
                if (!ps.isPlaying) ps.Play(true);
            }
            else if (ps.isPlaying)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}