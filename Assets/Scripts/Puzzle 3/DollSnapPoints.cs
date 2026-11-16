using UnityEngine;

public class DollSnapPoints : MonoBehaviour
{
    private bool SnapPointOpen = false;
    [SerializeField]

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SnapPointOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
