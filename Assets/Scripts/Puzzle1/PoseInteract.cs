using UnityEngine;
using UnityEngine.InputSystem;

public class PoseInteract : MonoBehaviour
{
    [SerializeField] private GameObject[] dolls;
    [SerializeField] public int poseCount = 0;
    [SerializeField]private int maxPoseCount;

    private void Start()
    {
        maxPoseCount = dolls.Length;
        dolls[poseCount].SetActive(true);
    }

    public void PoseDollInteract()
    {
        dolls[poseCount].SetActive(false);
        if (poseCount >= maxPoseCount - 1)
        {
            poseCount = 0;
        } else
        {
            poseCount++;
        }
        dolls[poseCount].SetActive(true);
    }
}
