using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class SplineLine : MonoBehaviour
{
    public SplineContainer container;
    [Min(2)] public int resolution = 200;     // number of points on the line
    public int splineIndex = 0;               // which spline in the container
    public bool updateEveryFrame = false;     // enable if the spline/transform moves at runtime

    LineRenderer lr;

    void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        Spline.Changed += OnSplineChanged;
        Rebuild();
    }

    void OnDisable() => Spline.Changed -= OnSplineChanged;

    void OnValidate() => Rebuild();

    void LateUpdate()
    {
        if (updateEveryFrame) Rebuild();
    }

    void OnSplineChanged(Spline changed, int knotIndex, SplineModification mod)
    {
        if (container != null && splineIndex < container.Splines.Count
            && changed == container.Splines[splineIndex])
            Rebuild();
    }

    public void Rebuild()
    {
        if (container == null || container.Splines.Count == 0) return;
        if (lr == null) lr = GetComponent<LineRenderer>();

        Spline spline = container.Splines[Mathf.Clamp(splineIndex, 0, container.Splines.Count - 1)];
        bool closed = spline.Closed;

        lr.useWorldSpace = false;
        lr.loop = closed;
        lr.positionCount = resolution;

        // Closed: don't repeat the first point at the end (the loop closes it).
        // Open: include t = 1 so the line reaches the end.
        float divisor = closed ? resolution : resolution - 1;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / divisor;
            Vector3 local = (Vector3)spline.EvaluatePosition(t);          // spline space
            Vector3 world = container.transform.TransformPoint(local);    // world space
            lr.SetPosition(i, transform.InverseTransformPoint(world));    // LineRenderer space
        }
    }

    // Handy for the cutting script: the same points as world positions
    public Vector3[] GetWorldPoints()
    {
        var pts = new Vector3[lr.positionCount];
        for (int i = 0; i < pts.Length; i++)
            pts[i] = transform.TransformPoint(lr.GetPosition(i));
        return pts;
    }
}