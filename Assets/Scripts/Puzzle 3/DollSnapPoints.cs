using UnityEngine;

public class DollSnapPoints : MonoBehaviour
{
    private bool SnapPointOpen = false;
    [SerializeField] private GameObject[] SnapPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SnapPointOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(SnapPointOpen)
        {
            foreach(GameObject SnapPoint in SnapPoints)
            {
                SnapPoint.SetActive(true);
            }
        } 
        else
        {
            foreach(GameObject SnapPoint in SnapPoints)
            {
                SnapPoint.SetActive(false);
            }
        }
    }

    public void SetSnapPointOpen()
    {
        SnapPointOpen = true;
    }

    public void SetSnapPointClosed()
    {
        SnapPointOpen = false;
    }

}
