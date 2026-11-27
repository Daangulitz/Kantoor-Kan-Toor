using UnityEngine;

public class PoseData : MonoBehaviour
{
    public int poseIndex;
    public bool poseMatch;
    private void Update()
    { 
        if (GetComponentInParent<PoseInteract>().poseCount == poseIndex)
        {
            poseMatch = true;
        }
    }
}
