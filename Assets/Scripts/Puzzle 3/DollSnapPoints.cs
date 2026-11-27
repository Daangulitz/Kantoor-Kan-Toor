using UnityEngine;

public class DollSnapPoints : MonoBehaviour
{

    [SerializeField] private GameObject[] SnapPoints;
    [SerializeField] private GameObject HartSpline;

    private int CounterForSpline;
    private bool SnapPointOpen = false;

    void Start()
    {
        SnapPointOpen = false;
    }

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

        if(CounterForSpline == 2)
        {
            HartSpline.SetActive(true);
        }
        else
        {
            HartSpline.SetActive(false);
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

    public void CounterForSplineAdd()
    {
        CounterForSpline++;
    }

    public void CounterForSplineRemove()
    {
        CounterForSpline--;
    }

}
