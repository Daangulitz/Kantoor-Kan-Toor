using UnityEngine;

public class DollGameObjects : MonoBehaviour
{
    [SerializeField] private GameObject HeartObject;
    [SerializeField] private GameObject CogWheelObject ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public void ShowObjects()
    {
        Instantiate(HeartObject);
        Instantiate(CogWheelObject);
    }
}
