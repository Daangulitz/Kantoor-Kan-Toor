using UnityEngine;

[RequireComponent(typeof(SplineLine))]
public class SplineLineCollider : MonoBehaviour
{
    public int segments = 60;          // fewer than the line resolution, enough for a heart
    public float thickness = 0.03f;    // collider thickness (generous for VR)
    public float depth = 0.02f;
    public string layerName = "CutLine";

    SplineLine line;
    Transform holder;

    public class Segment : MonoBehaviour { public int index; }

    void Start() => Build();

    public void Build()
    {
        line = GetComponent<SplineLine>();
        line.Rebuild();

        if (holder != null) Destroy(holder.gameObject);
        holder = new GameObject("LineColliders").transform;
        holder.SetParent(transform, false);

        Vector3[] pts = line.GetWorldPoints();
        int layer = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < segments; i++)
        {
            Vector3 a = pts[i * pts.Length / segments];
            Vector3 b = pts[((i + 1) * pts.Length / segments) % pts.Length];

            var go = new GameObject("Seg" + i);
            go.transform.SetParent(holder, true);
            go.transform.position = (a + b) * 0.5f;

            Vector3 dir = b - a;
            if (dir.sqrMagnitude > 1e-8f)
                go.transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.Cross(transform.forward, dir)) ;
            // local X axis of the box runs along the segment
            go.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up)
                                    * Quaternion.Euler(0, 0, Mathf.Atan2(
                                        Vector3.Dot(dir, transform.up),
                                        Vector3.Dot(dir, transform.right)) * Mathf.Rad2Deg);

            var box = go.AddComponent<BoxCollider>();
            box.size = new Vector3(dir.magnitude + thickness * 0.5f, thickness, depth);

            go.AddComponent<Segment>().index = i;
            if (layer >= 0) go.layer = layer;
        }
    }
}