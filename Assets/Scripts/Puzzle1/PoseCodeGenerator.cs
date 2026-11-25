using UnityEngine;
using System.Linq;

public class PoseCodeGenerator : MonoBehaviour
{
    [SerializeField] private int[] poseIndex;
    [SerializeField] private bool[] poseMatch;
    [SerializeField] private GameObject[] dolls;


    private void Start()
    {
        GenerateCode(poseIndex);
    }
    private void Update()
    {
        CheckCode(dolls);
    }
    private void GenerateCode(int[] shuffleArray)
    {
        for (int i = 0; i < shuffleArray.Length; i++)
        {
            shuffleArray[i] = Random.RandomRange(0, 4);
            dolls[i].GetComponent<PoseData>().poseIndex = shuffleArray[i];
        }
    }

    private void CheckCode(GameObject[] PoseDollArray)
    {
        for (int i = PoseDollArray.Length - 1; i >= 0; i--)
        {
            if (PoseDollArray[i].GetComponent<PoseData>().poseMatch == true)
            {
                poseMatch[i] = true;
            }
            else
            {
                poseMatch[i] = false;
            }
        }

        if (poseMatch.All(match => match))
        {
            Debug.Log("ello");
        }
    }
}
