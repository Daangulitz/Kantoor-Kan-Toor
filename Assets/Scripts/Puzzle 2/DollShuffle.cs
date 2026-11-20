using UnityEngine;
using System.Linq;

public class DollShuffle : MonoBehaviour
{
    [SerializeField] private DollSpawner dollSpawner;

    [SerializeField] private GameObject[] FOVTrackers;
    [SerializeField] private bool[] FOVTrackersInView;
    void Update()
    {
        FOVTrackers = dollSpawner.spawnedDolls.ToArray();

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        for (int i = FOVTrackers.Length - 1; i >= 0; i--)
        {
            if (!GeometryUtility.TestPlanesAABB(planes, FOVTrackers[i].GetComponent<Collider>().bounds))
            {
                FOVTrackersInView[i] = true;
            }
            else
            {
                FOVTrackersInView[i] = false;
            }
        }

        if (FOVTrackersInView.All(inView => inView))
        {
            dollSpawner.ReplaceDolls();
        }
    }
}
