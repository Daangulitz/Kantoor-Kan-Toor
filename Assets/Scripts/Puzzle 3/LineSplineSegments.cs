using UnityEngine;

[RequireComponent(typeof(SplineLine))]
[RequireComponent(typeof(LineRenderer))]
public class SplineLineSegments : MonoBehaviour
{
    public int segments = 60;          // must match SplineLineCollider.segments
    public bool hideOriginal = true;   // hide the full line, only show the pieces

    LineRenderer source;
    LineRenderer[] parts;

    void Start() => Build();

    public void Build()
    {
        var line = GetComponent<SplineLine>();
        source = GetComponent<LineRenderer>();
        line.Rebuild();

        Vector3[] world = line.GetWorldPoints();
        int n = world.Length;
        parts = new LineRenderer[segments];

        var holder = new GameObject("LineParts").transform;
        holder.SetParent(transform, false);

        for (int i = 0; i < segments; i++)
        {
            int start = i * n / segments;
            int end = (i + 1) * n / segments;   // same ranges as SplineLineCollider
            int count = end - start + 1;

            var go = new GameObject("Part" + i);
            go.transform.SetParent(holder, false);

            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.positionCount = count;
            for (int k = 0; k < count; k++)
                lr.SetPosition(k, transform.InverseTransformPoint(world[(start + k) % n]));

            // copy the look of the original line
            lr.sharedMaterial = source.sharedMaterial;
            lr.widthCurve = source.widthCurve;
            lr.widthMultiplier = source.widthMultiplier;
            lr.colorGradient = source.colorGradient;
            lr.alignment = source.alignment;
            lr.textureMode = source.textureMode;
            lr.numCapVertices = source.numCapVertices;
            lr.numCornerVertices = source.numCornerVertices;

            parts[i] = lr;
        }

        if (hideOriginal) source.enabled = false;
    }

    public void Cut(int index)
    {
        if (parts == null || index < 0 || index >= parts.Length) return;
        parts[index].gameObject.SetActive(false);
    }
}